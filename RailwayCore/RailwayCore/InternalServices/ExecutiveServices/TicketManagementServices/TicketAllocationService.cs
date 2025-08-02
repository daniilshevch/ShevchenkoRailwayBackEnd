using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using RailwayCore.InternalServices.CoreServices;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices;
namespace RailwayCore.InternalServices.ExecutiveServices
{
    public class TicketAllocationService
    {
        private readonly AppDbContext context;
        private readonly StationRepository station_service;
        private readonly TrainRouteOnDateRepository train_route_on_date_service;
        private readonly PassengerCarriageRepository passenger_carriage_service;
        private readonly TrainScheduleSearchService train_schedule_search_service;
        private readonly TrainSquadSearchService train_squad_search_service;
        private readonly TicketAvailabilityCheckService ticket_search_service;
        public TicketAllocationService(AppDbContext context, 
            StationRepository station_service, 
            TrainRouteOnDateRepository train_route_on_date_service, 
            PassengerCarriageRepository passenger_carriage_service, 
            FullTrainRouteSearchService full_train_route_search_service, 
            TicketAvailabilityCheckService ticket_search_service,
            TrainScheduleSearchService train_schedule_search_service,
            TrainSquadSearchService train_squad_search_service)
        {
            this.context = context;
            this.station_service = station_service;
            this.train_route_on_date_service = train_route_on_date_service;
            this.passenger_carriage_service = passenger_carriage_service;
            this.train_schedule_search_service = train_schedule_search_service;
            this.train_squad_search_service = train_squad_search_service;
            this.ticket_search_service = ticket_search_service;
            
        }

        [Refactored("v1", "19.04.2025")]
        [Refactored("v2", "02.05.2025")]
        [Checked("06.07.2025")]
        [Crucial]
        public async Task<QueryResult<TicketBooking>> CreateTicketBooking(InternalTicketBookingDto input)
        {
            //Перевірка існування користувача, рейсу поїзда, пасажирського вагона, початкової та кінцевої станції(існування в принципі, а не в контексті конкретного рейсу)
            User? user_from_ticket = await context.Users.FirstOrDefaultAsync(user => user.Id == input.User_Id);
            if (user_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find user with ID: {input.User_Id}"));
            }
            TrainRouteOnDate? train_route_on_date_from_ticket = await train_route_on_date_service.GetTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if (train_route_on_date_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find train route on date with ID: {input.Train_Route_On_Date_Id}"));
            }
            PassengerCarriage? passenger_carriage_from_ticket = await passenger_carriage_service.GetPassengerCarriageById(input.Passenger_Carriage_Id);
            if (passenger_carriage_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find passenger carriage with ID: {input.Passenger_Carriage_Id}"));
            }
            Station? starting_station_from_ticket = await station_service.FindStationByTitle(input.Starting_Station_Title);
            Station? ending_station_from_ticket = await station_service.FindStationByTitle(input.Ending_Station_Title);
            if (starting_station_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find station with title: {input.Starting_Station_Title}"));
            }
            if (ending_station_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find station with title: {input.Ending_Station_Title}"));
            }

            //Отримуємо всі зупинки для даного рейсу поїзда в порядку слідування
            List<TrainRouteOnDateOnStation>? all_train_stops_for_train_route_on_date = await train_schedule_search_service.GetTrainStopsForTrainRouteOnDate(input.Train_Route_On_Date_Id);
            if (all_train_stops_for_train_route_on_date is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Can't get stations list for train route on date"));
            }

            //Якщо початкової чи кінцевої зупинки подорожі немає в списку зупинок поїзда, то вертаємо помилку і квиток купити не можливо
            if (!all_train_stops_for_train_route_on_date.Any(train_stop => train_stop.Station_Id == starting_station_from_ticket.Id) ||
            !all_train_stops_for_train_route_on_date.Any(train_stop => train_stop.Station_Id == ending_station_from_ticket.Id))
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"This train route on date doesn't pass through these stations"));
            }
            //Знаходимо номер за рахунком у слідування поїзда через станції
            int starting_stop_from_ticket_index = all_train_stops_for_train_route_on_date.FindIndex(train_stop => train_stop.Station_Id == starting_station_from_ticket.Id);
            int ending_stop_from_ticket_index = all_train_stops_for_train_route_on_date.FindIndex(train_stop => train_stop.Station_Id == ending_station_from_ticket.Id);

            //Якщо поїзд проходить між цими станціями, але в іншому порядку, то вертаємо помилку і квиток купити не можливо
            if (ending_stop_from_ticket_index <= starting_stop_from_ticket_index)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"This train doesn't pass through these stations IN THIS ORDER"));
            }

            //Отримуємо інформацію про всі призначення вагонів в склад рейсу поїзду з квитку(отримуємо склад вагонів рейсу, який вказано в квитку)
            List<PassengerCarriageOnTrainRouteOnDate>? all_passenger_carriage_assignments_for_train_route_on_date = await
                 train_squad_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(input.Train_Route_On_Date_Id);
            if (all_passenger_carriage_assignments_for_train_route_on_date is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Can't find carriage squad for train route on date with ID: {input.Train_Route_On_Date_Id}"));
            }
            //Якщо в складі рейсу поїзда немає вагону з квитка, то вертаємо помилку і квиток купити не можливо
            if (!all_passenger_carriage_assignments_for_train_route_on_date.Any(carriage_assignment => carriage_assignment.Passenger_Carriage_Id == passenger_carriage_from_ticket.Id))
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Train route on date {input.Train_Route_On_Date_Id} doesn't contain carriage {input.Passenger_Carriage_Id} in its squad"));
            }

            //Якщо вагон в складі, то знаходимо інформацію про його призначення(в тому числі номер в складі)
            PassengerCarriageOnTrainRouteOnDate? desired_carriage_assignment = all_passenger_carriage_assignments_for_train_route_on_date
    .FirstOrDefault(carriage_assignment => carriage_assignment.Passenger_Carriage_Id == passenger_carriage_from_ticket.Id);
            if (desired_carriage_assignment is null) //Дана перевірка є надлишкова
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Train route on date {input.Train_Route_On_Date_Id} doesn't contain carriage {input.Passenger_Carriage_Id} in its squad"));
            }
            int carriage_position_in_squad = desired_carriage_assignment.Position_In_Squad;

            //Перевіряємо, чи даний вагон містить місце з номером, який вказано в квитку
            int capacity = passenger_carriage_from_ticket.Capacity;
            if (input.Place_In_Carriage > capacity || input.Place_In_Carriage <= 0)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Passenger carriage with ID: {input.Passenger_Carriage_Id} doesn't containt place # {input.Place_In_Carriage}"));
            }

            QueryResult<bool> place_availability_result = await ticket_search_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(input.Train_Route_On_Date_Id, input.Starting_Station_Title, input.Ending_Station_Title,
                input.Passenger_Carriage_Id, input.Place_In_Carriage);
            if (place_availability_result.Fail)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Fail while checking place availability"));
            }
            bool place_availability = place_availability_result.Value; //Отримуємо інформацію про те, чи вільне місце в вагоні(заброньоване чи ні)

            //Якщо місце зайняте, то вертаємо помилку і квиток купити не можливо
            if (place_availability == false)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Place # {input.Place_In_Carriage} in carriage # {input.Passenger_Carriage_Id} in train route on date {input.Train_Route_On_Date_Id} has  " +
                    $"already been booked"));
            }

            //Якщо всі перевірки пройдено, то додаємо квиток в базу
            TicketBooking ticket_booking = new TicketBooking
            {
                User = user_from_ticket,
                Train_Route_On_Date = train_route_on_date_from_ticket,
                Starting_Station = starting_station_from_ticket,
                Ending_Station = ending_station_from_ticket,
                Passenger_Carriage = passenger_carriage_from_ticket,
                Passenger_Carriage_Position_In_Squad = carriage_position_in_squad,
                Place_In_Carriage = input.Place_In_Carriage,
                Passenger_Name = input.Passenger_Name,
                Passenger_Surname = input.Passenger_Surname,
                Booking_Time = DateTime.Now,
                Ticket_Status = input.Ticket_Status,
                Full_Ticket_Id = Guid.NewGuid(),
            };
            await context.Ticket_Bookings.AddAsync(ticket_booking);
            await context.SaveChangesAsync();
            return new SuccessQuery<TicketBooking>(ticket_booking);
        }

        [Refactored("v1", "19.04.2025")]
        [Refactored("v2", "02.05.2025")]
        [Checked("06.07.2025")]
        [Crucial]
        public async Task<QueryResult<TicketBooking>> CreateTicketBookingWithCarriagePositionInSquad(InternalTicketBookingDtoWithCarriagePosition input)
        {
            //Перевіряємо, чи містить склад рейсу поїзда з квитку вагон на позиції, яка вказана в квитку
            PassengerCarriageOnTrainRouteOnDate? carriage_assignment_from_ticket = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignment => carriage_assignment.Passenger_Carriage)
                .Include(carriage_assignment => carriage_assignment.Train_Route_On_Date)
                .FirstOrDefaultAsync(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && carriage_assignment.Position_In_Squad == input.Passenger_Carriage_Position_In_Squad);
            if (carriage_assignment_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"There is no passenger carriage on position # {input.Passenger_Carriage_Position_In_Squad} in train" +
                    $" route on date # {input.Train_Route_On_Date_Id}"));
            }

            //Якщо вагон на цій позиції існує, то отримуємо інформацію про нього
            PassengerCarriage passenger_carriage_from_ticket = carriage_assignment_from_ticket.Passenger_Carriage;

            InternalTicketBookingDto ticket_booking_dto = new InternalTicketBookingDto()
            {
                User_Id = input.User_Id,
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Starting_Station_Title = input.Starting_Station_Title,
                Ending_Station_Title = input.Ending_Station_Title,
                Passenger_Carriage_Id = passenger_carriage_from_ticket.Id,
                Place_In_Carriage = input.Place_In_Carriage,
                Passenger_Name = input.Passenger_Name,
                Passenger_Surname = input.Passenger_Surname,
                Ticket_Status = input.Ticket_Status,
            };
            QueryResult<TicketBooking> ticket_booking_result = await CreateTicketBooking(ticket_booking_dto);
            if (ticket_booking_result.Fail)
            {
                return new FailQuery<TicketBooking>(ticket_booking_result.Error);
            }
            TicketBooking ticket_booking = ticket_booking_result.Value;
            return new SuccessQuery<TicketBooking>(ticket_booking);
        }
    }
}
