using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RailwayCore.Context;
using RailwayCore.InternalDTO.CoreDTO;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
namespace RailwayCore.Services
{
    [Checked("19.04.2025")]
    public class BookerInfo
    {
        public string Passenger_Name { get; set; } = null!;
        public string Passenger_Surname { get; set; } = null!;
        public User User_Booker { get; set; } = null!;
        public string Starting_Station_Title { get; set; } = null!;
        public string Ending_Station_Title { get; set; } = null!;
    }
    [Checked("19.04.2025")]
    public class CarriageBookedPlace
    {
        public string Carriage_Id { get; set; } = null!;
        public int Place { get; set; }
    }
    [Checked("19.04.2025")]
    public class TrainRouteOnDateCarriageBookedPlace
    {
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public string Carriage_Id { get; set; } = null!;
        public int Place { get; set; }
        public  BookerInfo Booker_Info { get; set; } = null!;
    }
    public class FullTicketBookingService
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateService train_route_on_date_service;
        private readonly StationService station_service;
        private readonly PassengerCarriageService passenger_carriage_service;
        private readonly FullTrainRouteSearchService full_train_route_search_service;
        private readonly TextService text_service = new TextService("TicketBookingService");
        public FullTicketBookingService(AppDbContext context, TrainRouteOnDateService train_route_on_date_service, StationService station_service, PassengerCarriageService passenger_carriage_service, FullTrainRouteSearchService full_train_route_search_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.station_service = station_service;
            this.passenger_carriage_service = passenger_carriage_service;
            this.full_train_route_search_service = full_train_route_search_service;
        }

  

        [Refactored("v1", "19.04.2025")]
        [Crucial]
        public async Task<QueryResult<TicketBooking>> CreateTicketBooking(InternalTicketBookingDto input)
        {
            //Перевірка існування користувача, рейсу поїзда, пасажирського вагона, початкової та кінцевої станції(існування в принципі, а не в контексті конкретного рейсу)
            User? user_from_ticket = await context.Users.FirstOrDefaultAsync(user => user.Id == input.User_Id);
            if(user_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find user with ID: {input.User_Id}"));
            }
            TrainRouteOnDate? train_route_on_date_from_ticket = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if(train_route_on_date_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find train route on date with ID: {input.Train_Route_On_Date_Id}"));
            }
            PassengerCarriage? passenger_carriage_from_ticket = await passenger_carriage_service.FindPassengerCarriageById(input.Passenger_Carriage_Id);
            if(passenger_carriage_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find passenger carriage with ID: {input.Passenger_Carriage_Id}"));
            }
            Station? starting_station_from_ticket = await station_service.FindStationByTitle(input.Starting_Station_Title);
            Station? ending_station_from_ticket = await station_service.FindStationByTitle(input.Ending_Station_Title);
            if(starting_station_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find station with title: {input.Starting_Station_Title}"));
            }
            if(ending_station_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find station with title: {input.Ending_Station_Title}"));
            }

            //Отримуємо всі зупинки для даного рейсу поїзда в порядку слідування
            List<TrainRouteOnDateOnStation>? all_train_stops_for_train_route_on_date = await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(input.Train_Route_On_Date_Id);
            if(all_train_stops_for_train_route_on_date is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Can't get stations list for train route on date"));
            }
            
            //Якщо початкової чи кінцевої зупинки подорожі немає в списку зупинок поїзда, то вертаємо помилку і квиток купити не можливо
            if(!all_train_stops_for_train_route_on_date.Any(train_stop => train_stop.Station_Id == starting_station_from_ticket.Id) || 
            !all_train_stops_for_train_route_on_date.Any(train_stop => train_stop.Station_Id == ending_station_from_ticket.Id))
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"This train route on date doesn't pass through these stations"));
            }
            //Знаходимо номер за рахунком у слідування поїзда через станції
            int starting_stop_from_ticket_index = all_train_stops_for_train_route_on_date.FindIndex(train_stop => train_stop.Station_Id == starting_station_from_ticket.Id);
            int ending_stop_from_ticket_index = all_train_stops_for_train_route_on_date.FindIndex(train_stop => train_stop.Station_Id == ending_station_from_ticket.Id);
            
            //Якщо поїзд проходить між цими станціями, але в іншому порядку, то вертаємо помилку і квиток купити не можливо
            if(ending_stop_from_ticket_index <= starting_stop_from_ticket_index)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"This train doesn't pass through these stations IN THIS ORDER"));
            }

            //Отримуємо інформацію про всі призначення вагонів в склад рейсу поїзду з квитку(отримуємо склад вагонів рейсу, який вказано в квитку)
            List<PassengerCarriageOnTrainRouteOnDate>? all_passenger_carriage_assignments_for_train_route_on_date = await
                full_train_route_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(input.Train_Route_On_Date_Id);
            if(all_passenger_carriage_assignments_for_train_route_on_date is null)
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
            if(desired_carriage_assignment is null) //Дана перевірка є надлишкова
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Train route on date {input.Train_Route_On_Date_Id} doesn't contain carriage {input.Passenger_Carriage_Id} in its squad"));
            }
            int carriage_position_in_squad = desired_carriage_assignment.Position_In_Squad;

            //Перевіряємо, чи даний вагон містить місце з номером, який вказано в квитку
            int capacity = passenger_carriage_from_ticket.Capacity;
            if(input.Place_In_Carriage > capacity || input.Place_In_Carriage <= 0)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Passenger carriage with ID: {input.Passenger_Carriage_Id} doesn't containt place # {input.Place_In_Carriage}"));
            }

            QueryResult<bool> place_availability_result = await CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(input.Train_Route_On_Date_Id, input.Starting_Station_Title, input.Ending_Station_Title,
                input.Passenger_Carriage_Id, input.Place_In_Carriage);
            if(place_availability_result is FailQuery<bool>)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"Fail while checking place availability"));
            }
            bool place_availability = place_availability_result.Value; //Отримуємо інформацію про те, чи вільне місце в вагоні(заброньоване чи ні)
            
            //Якщо місце зайняте, то вертаємо помилку і квиток купити не можливо
            if(place_availability == false)
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
                Ticket_Status = input.Ticket_Status
            };
            await context.Ticket_Bookings.AddAsync(ticket_booking);
            await context.SaveChangesAsync();
            return new SuccessQuery<TicketBooking>(ticket_booking);
        }

        [Refactored("v1", "19.04.2025")]
        public async Task<QueryResult<TicketBooking>> CreateTicketBookingWithCarriagePositionInSquad(InternalTicketBookingDtoWithCarriagePosition input)
        {
            //Перевіряємо, чи містить склад рейсу поїзда з квитку вагон на позиції, яка вказана в квитку
            PassengerCarriageOnTrainRouteOnDate? carriage_assignment_from_ticket = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignment => carriage_assignment.Passenger_Carriage)
                .Include(carriage_assignment => carriage_assignment.Train_Route_On_Date)
                .FirstOrDefaultAsync(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && carriage_assignment.Position_In_Squad == input.Passenger_Carriage_Position_In_Squad);
            if(carriage_assignment_from_ticket is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, $"There is not passenger carriage on position # {input.Passenger_Carriage_Position_In_Squad} in train" +
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
                Ticket_Status = input.Ticket_Status
            };
            QueryResult<TicketBooking> ticket_booking_result = await CreateTicketBooking(ticket_booking_dto);
            if(ticket_booking_result is FailQuery<TicketBooking> fail_result)
            {
                return new FailQuery<TicketBooking>(fail_result.Error!);
            }

            TicketBooking? ticket_booking = ticket_booking_result.Value;
            if(ticket_booking is null) //Перевірка є надлишковою
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.BadRequest, "Fail while booking tickets"));
            }
            return new SuccessQuery<TicketBooking>(ticket_booking);

        }

        [Crucial("Перевірка броні на одне конкретне місце в одному конкретному вагоні в рейсі на машруті між двома станціями")]
        [Refactored("v1", "19.04.2025")]
        public async Task<QueryResult<bool>> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title, 
            string desired_ending_station_title, string passenger_carriage_id, int place_in_carrriage)
        {
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
            if(train_route_on_date is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.NotFound, $"Can't find train route on date with ID: {train_route_on_date_id}"));
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(passenger_carriage_id);
            if(passenger_carriage is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.NotFound, $"Can't find passenger carriage with ID: {passenger_carriage_id}"));
            }
            Station? desired_starting_station = await station_service.FindStationByTitle(desired_starting_station_title);
            Station? desired_ending_station = await station_service.FindStationByTitle(desired_ending_station_title);
            if(desired_starting_station is null || desired_ending_station is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.NotFound, $"Can't find one of the stations"));
            }

            //Знаходимо всі зупинки для поїзда, в якому ми потенційно хочемо купити квиток(в порядку слідування поїзда)
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date = await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode: true);
            if(all_train_stops_of_train_route_on_date is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.BadRequest, $"Fail while searching stations for train route on date"));
            }
            //Ділимо зупинки на 3 секції згідно з алгоритмом
            (List<int> left_part, List<int> central_part, List<int> right_part) = 
                DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, desired_starting_station_title, desired_ending_station_title);

            //Перевірка, чи існує квиток(або квитки), який блокує придбання квитку на це місце пасажиром(фактично перевіряє, чи місце в даному вагоні в даному поїзді між цими станціями вільне)
            bool is_available = !(await context.Ticket_Bookings.AnyAsync(ticket => ticket.Train_Route_On_Date_Id == train_route_on_date_id &&
            ticket.Passenger_Carriage_Id == passenger_carriage_id && ticket.Place_In_Carriage == place_in_carrriage && 
            ((left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id)) ||
            (central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id)))
                && ticket.Ticket_Status != TicketStatus.Returned));

            return new SuccessQuery<bool>(is_available);
        }




        [Executive("Перевірка броні на всі місця в одному конкретному вагоні в рейсі на машруті між двома станціями " +
            "з поверненням об'єтку, що містить призначення вагона і список вільних-зайнятих місць")]
        public async Task<InternalCarriageAssignmentRepresentationDto?> GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(string train_route_on_date_id, int position_in_squad, string starting_station_title, string ending_station_title)
        {
            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments_for_train_route_on_date = await full_train_route_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_id);
            if (carriage_assignments_for_train_route_on_date == null)
            {
                return null;
            }
            PassengerCarriageOnTrainRouteOnDate? desired_carriage_assignment = carriage_assignments_for_train_route_on_date.FirstOrDefault(carriage_assignment => carriage_assignment.Position_In_Squad == position_in_squad);
            if (desired_carriage_assignment == null)
            {
                return null;
            }
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            (List<int> left_part, List<int> central_part, List<int> right_part) =
                DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, starting_station_title, ending_station_title);
            List<int> booked_places_in_carriage = await context.Ticket_Bookings.Where(ticket => ticket.Train_Route_On_Date_Id == train_route_on_date_id
            && ticket.Passenger_Carriage_Id == desired_carriage_assignment.Passenger_Carriage_Id && (
           central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id)
           || left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id)))
                .Select(ticket => ticket.Place_In_Carriage).ToListAsync();
            int total_places_amount = desired_carriage_assignment.Passenger_Carriage.Capacity;
            List<InternalSinglePlaceDto> places_list = new List<InternalSinglePlaceDto>();
            for (int place_number = 1; place_number <= total_places_amount; place_number++)
            {
                if (booked_places_in_carriage.Contains(place_number))
                {
                    places_list.Add(new InternalSinglePlaceDto { Place_In_Carriage = place_number, Is_Free = false });
                }
                else
                {
                    places_list.Add(new InternalSinglePlaceDto { Place_In_Carriage = place_number, Is_Free = true });
                }
            }
            InternalCarriageAssignmentRepresentationDto single_carriage_statistics = new InternalCarriageAssignmentRepresentationDto
            {
                Carriage_Assignment = desired_carriage_assignment,
                Places_Availability = places_list
            };
            return single_carriage_statistics;
        }







        [Crucial]
        [Refactored("v1", "19.04.2025")]
        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate
            (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {
            //Знаходимо список тих поїздів, айді яких надано в списку
            List<TrainRouteOnDate> train_routes_on_date_list = await context.Train_Routes_On_Date.Where(train_route_on_date =>
            train_route_on_date_ids.Contains(train_route_on_date.Id)).ToListAsync();

            //За один запит знаходимо всі зупинки для всіх поїздів зі списку(вони будуть посортовані спочатку за поїздом, а потім за порядком слідування на маршруті,
            //тобто спочатку всі зупинки першого поїзду в порядку слідування, потім всі зупинки другого поїзда в порядку слідування і так далі)
            List<TrainRouteOnDateOnStation> train_stops_from_all_train_routes_on_date = await full_train_route_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Ініціалізуємо список списків, де кожен вкладений список представляє собою посортований список станцій для одного конкретного поїзда в дату(потрібно для алгоритму трьох секцій)
            List<List<TrainRouteOnDateOnStation>> train_stops_for_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Для кожного поїзду зі списку бажаних поїздів знаходимо його посортовані станції
                List<TrainRouteOnDateOnStation> train_stops_for_current_train_route_on_date =
                    train_stops_from_all_train_routes_on_date.Where(train_stop => train_stop.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();
                train_stops_for_train_route_on_date_list.Add(train_stops_for_current_train_route_on_date);
            }
            //Збираємо станції всіх поїздів зі списку і ділимо їх на 3 зони(ліва зона, центральна зона, права зона) - подробиці в Алгоритмі Трьох Секцій
            (List<int> consolidated_left_part, List<int> consolidated_central_part, List<int> consolidated_right_part)
                = DivideSectionIntoThreePartsForSeveralTrains(train_stops_for_train_route_on_date_list, starting_station_title, ending_station_title);

            //Збираємо всі бронювання квитків в усіх вагонах в усіх поїздах зі списку між обраними станціями поїздки
            List<TicketBooking> booked_tickets = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.User)
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Where(ticket_booking =>
            (consolidated_left_part.Contains(ticket_booking.Starting_Station_Id) && consolidated_right_part.Contains(ticket_booking.Ending_Station_Id)
            || (consolidated_central_part.Contains(ticket_booking.Starting_Station_Id) || consolidated_central_part.Contains(ticket_booking.Ending_Station_Id)))
            && train_route_on_date_ids.Contains(ticket_booking.Train_Route_On_Date_Id) && ticket_booking.Ticket_Status != TicketStatus.Returned).ToListAsync();

            //Збираємо з квитків потрібну інформацію про заброньоване місце(поїзд, номер вагона, місце в вагоні)
            List<TrainRouteOnDateCarriageBookedPlace> booked_places_for_carriage_in_train_route_on_date =
                booked_tickets.Select(ticket => new TrainRouteOnDateCarriageBookedPlace
                {
                    Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id,
                    Carriage_Id = ticket.Passenger_Carriage_Id,
                    Place = ticket.Place_In_Carriage,
                }).ToList();

            //Словник статистичних-трансферів для всіх поїздів зі списку, де кожен окремий трансфер містить статистику по одному поїзду, включаючи бронювання в усіх його вагонах
            //ключ - ідентифікатор рейсу поїзда в дату, значення - об'єкт трансферу для конкретного рейсу
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> train_route_on_date_carriage_booking_statistics_dictionary
                = new Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>();

            //За один запит отримуємо інформацію про всі призначення вагонів на всі рейси поїздів(вони будуть посортовані спочатку за поїздом, а потім за номерами,
            //тобто спочатку всі вагони першого поїзду, посортовані за номерами в складі, потім другого і так далі)
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments_for_all_train_routes_on_date =
                await full_train_route_search_service.GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Перебираємо кожен поїзд зі списку і збираємо статистику бронювань для нього
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Ініціалізуємо об'єкт-трансфер, який буде мати інформацію про один конкретний поїзд зі статистикою бронювань по всіх його вагонах
                train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id] =
                    new InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto()
                    {
                        Train_Route_On_Date = train_routes_on_date_list.FirstOrDefault(train_route_on_date => train_route_on_date.Id == current_train_route_on_date_id)!,
                        Total_Place_Highlights = new PlaceAccumulator()
                    };

                //Отримуємо всі вагони, що призначені в склад конкретного поїзда(фільтруємо попередньо отриманий список вагонів зі всіх поїздів)
                List<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_for_current_train_route_on_date =
                    carriage_assignments_for_all_train_routes_on_date.Where(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();

                carriage_assignment_for_current_train_route_on_date = carriage_assignment_for_current_train_route_on_date
                    .OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();  //Загалом-то це сортування надлишкове

                //Перебираємо кожен вагон в складі поїзда для отримання статистики бронювань по ньому
                foreach (PassengerCarriageOnTrainRouteOnDate current_carriage_assignment in carriage_assignment_for_current_train_route_on_date)
                {
                    //Отримуємо всі заброньовані місця в даному конкретному вагоні в даному конкретному рейсі поїзда 
                    //Зберігаються через HashSet, тому не буде дублікатів і буде дуже швидкий пошук
                    HashSet<int> booked_places_in_current_carriage = booked_places_for_carriage_in_train_route_on_date
                        .Where(ticket => ticket.Train_Route_On_Date_Id == current_train_route_on_date_id
                        && ticket.Carriage_Id == current_carriage_assignment.Passenger_Carriage_Id).Select(ticket => ticket.Place).ToHashSet();

                    //Отримуємо докладну інформацію про вагон
                    PassengerCarriage passenger_carriage = current_carriage_assignment.Passenger_Carriage;

                    //Отримуємо загальну кількість місць в вагоні
                    int capacity = passenger_carriage.Capacity;

                    //Ініціалізуємо список, де кожен об'єкт представляє одне місце в вагоні
                    List<InternalSinglePlaceDto> places_availability = new List<InternalSinglePlaceDto>();

                    int booked_places_amount = 0;
                    //Перебираємо всі місця в даному вагоні і, якщо місце є в списку зайнятих, помічаємо його, як зайняте
                    for (int current_place_in_carriage = 1; current_place_in_carriage <= capacity; current_place_in_carriage++)
                    {
                        //Перевірка, чи є дане місце в списку зайнятих для даного вагону
                        if (booked_places_in_current_carriage.Contains(current_place_in_carriage))
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = false
                            });
                            booked_places_amount++;
                        }
                        else
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = true
                            });
                        }
                    }
                    int free_places_amount = capacity - booked_places_amount;

                    //Додаємо в загальну статистику поїзда статистику одного конкретного вагона
                    train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id].Carriage_Statistics_List.Add(new InternalCarriageAssignmentRepresentationDto
                    {
                        Carriage_Assignment = current_carriage_assignment,
                        Free_Places = free_places_amount,
                        Places_Availability = places_availability,
                        Total_Places = capacity,
                    });
                    //Оновлюємо загальну статистику місць в поїзді на основі бронювань  в даному вагоні
                    train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id].Total_Place_Highlights.CountPlaces(passenger_carriage.Type_Of, free_places_amount, capacity);
                }
            }
            return new SuccessQuery<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>(train_route_on_date_carriage_booking_statistics_dictionary);
        }


        [Crucial]
        [Refactored("v1", "19.04.2025")]
        //Метод додатково вертає інформацію про те, хто бронював місце
        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics
    (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {
            //Знаходимо список тих поїздів, айді яких надано в списку
            List<TrainRouteOnDate> train_routes_on_date_list = await context.Train_Routes_On_Date.Where(train_route_on_date =>
            train_route_on_date_ids.Contains(train_route_on_date.Id)).ToListAsync();

            //За один запит знаходимо всі зупинки для всіх поїзів зі списку(вони будуть посортовані спочатку за поїздом, а потім за порядком слідування на маршруті,
            //тобто спочатку всі зупинки першого поїзду в порядку слідування, потім всі зупинки другого поїзда в порядку слідування і так далі)
            List<TrainRouteOnDateOnStation> train_stops_from_all_train_routes_on_date = await full_train_route_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Ініціалізуємо список списків, де кожен вкладений список представляє собою посортований список станцій для одного конкретного поїзда в дату(потрібно для алгоритму трьох секцій)
            List<List<TrainRouteOnDateOnStation>> train_stops_for_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Для кожного поїзду зі списку бажаних поїздів знаходимо його посортовані станції
                List<TrainRouteOnDateOnStation> train_stops_for_current_train_route_on_date =
                    train_stops_from_all_train_routes_on_date.Where(train_stop => train_stop.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();
                train_stops_for_train_route_on_date_list.Add(train_stops_for_current_train_route_on_date);
            }
            //Збираємо станції всіх поїздів зі списку і ділимо їх на 3 зони(ліва зона, центральна зона, права зона) - подробиці в Алгоритмі Трьох Секцій
            (List<int> consolidated_left_part, List<int> consolidated_central_part, List<int> consolidated_right_part)
                = DivideSectionIntoThreePartsForSeveralTrains(train_stops_for_train_route_on_date_list, starting_station_title, ending_station_title);

            //Збираємо всі бронювання квитків в усіх вагонах в усіх поїздах зі списку між обраними станціями поїздки
            List<TicketBooking> booked_tickets = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.User)
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Where(ticket_booking =>
            (consolidated_left_part.Contains(ticket_booking.Starting_Station_Id) && consolidated_right_part.Contains(ticket_booking.Ending_Station_Id)
            || (consolidated_central_part.Contains(ticket_booking.Starting_Station_Id) || consolidated_central_part.Contains(ticket_booking.Ending_Station_Id)))
            && train_route_on_date_ids.Contains(ticket_booking.Train_Route_On_Date_Id) && ticket_booking.Ticket_Status != TicketStatus.Returned).ToListAsync();

            //Збираємо з квитків потрібну інформацію про заброньоване місце(поїзд, номер вагона, місце в вагоні)
            List<TrainRouteOnDateCarriageBookedPlace> booked_places_for_carriage_in_train_route_on_date =
                booked_tickets.Select(ticket => new TrainRouteOnDateCarriageBookedPlace
                {
                    Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id,
                    Carriage_Id = ticket.Passenger_Carriage_Id,
                    Place = ticket.Place_In_Carriage,
                    Booker_Info = new BookerInfo //Інформація про пасажира + початкову та кінцеву станції його поїздки
                    {
                        Passenger_Name = ticket.Passenger_Name,
                        Passenger_Surname = ticket.Passenger_Surname,
                        Starting_Station_Title = ticket.Starting_Station.Title,
                        Ending_Station_Title = ticket.Ending_Station.Title,
                        User_Booker = ticket.User
                    }
                }).ToList();

            //Словник статистичних-трансферів для всіх поїздів зі списку, де кожен окремий трансфер містить статистику по одному поїзду, включаючи бронювання в усіх його вагонах
            //ключ - ідентифікатор рейсу поїзда в дату, значення - об'єкт трансферу для конкретного рейсу
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> train_route_on_date_carriage_booking_statistics_dictionary
                = new Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>();

            //За один запит отримуємо інформацію про всі призначення вагонів на всі рейси поїздів(вони будуть посортовані спочатку за поїздом, а потім за номерами,
            //тобто спочатку всі вагони першого поїзду, посортовані за номерами в складі, потім другого і так далі)
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments_for_all_train_routes_on_date =
                await full_train_route_search_service.GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Перебираємо кожен поїзд зі списку і збираємо статистику бронювань для нього
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Ініціалізуємо об'єкт-трансфер, який буде мати інформацію про один конкретний поїзд зі статистикою бронювань по всіх його вагонах
                train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id] =
                    new InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto()
                    {
                        Train_Route_On_Date = train_routes_on_date_list.FirstOrDefault(train_route_on_date => train_route_on_date.Id == current_train_route_on_date_id)!,
                        Total_Place_Highlights = new PlaceAccumulator()
                    };

                //Отримуємо всі вагони, що призначені в склад конкретного поїзда(фільтруємо попередньо отриманий список вагонів зі всіх поїздів)
                List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignment_for_current_train_route_on_date =
                    carriage_assignments_for_all_train_routes_on_date.Where(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();

                carriage_assignment_for_current_train_route_on_date = carriage_assignment_for_current_train_route_on_date
                    .OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();  //Загалом-то це сортування надлишкове

                //Перебираємо кожен вагон в складі поїзда для отримання статистики бронювань по ньому
                foreach (PassengerCarriageOnTrainRouteOnDate current_carriage_assignment in carriage_assignment_for_current_train_route_on_date)
                {
                    //Отримуємо всі заброньовані місця в даному конкретному вагоні в даному конкретному рейсі поїзда 
                    //Зберігаються через HashSet, тому не буде дублікатів і буде дуже швидкий пошук
                    HashSet<int> booked_places_in_current_carriage = booked_places_for_carriage_in_train_route_on_date
                        .Where(ticket => ticket.Train_Route_On_Date_Id == current_train_route_on_date_id
                        && ticket.Carriage_Id == current_carriage_assignment.Passenger_Carriage_Id).Select(ticket => ticket.Place).ToHashSet();

                    //Отримуємо докладну інформацію про вагон
                    PassengerCarriage passenger_carriage = current_carriage_assignment.Passenger_Carriage;

                    //Отримуємо загальну кількість місць в вагоні
                    int capacity = passenger_carriage.Capacity;

                    //Ініціалізуємо список, де кожен об'єкт представляє одне місце в вагоні
                    List<InternalSinglePlaceDto> places_availability = new List<InternalSinglePlaceDto>();

                    int booked_places_amount = 0;
                    //Перебираємо всі місця в даному вагоні і, якщо місце є в списку зайнятих, помічаємо його, як зайняте
                    for (int current_place_in_carriage = 1; current_place_in_carriage <= capacity; current_place_in_carriage++)
                    {
                        List<BookerInfo> bookers_for_current_place = booked_places_for_carriage_in_train_route_on_date.
                            Where(ticket => ticket.Train_Route_On_Date_Id == current_train_route_on_date_id && ticket.Carriage_Id == current_carriage_assignment.Passenger_Carriage_Id
                            && ticket.Place == current_place_in_carriage).Select(ticket => ticket.Booker_Info).ToList();
                        List<PassengerTripInfoDto> trip_info_for_current_place = bookers_for_current_place.Select(booker => new PassengerTripInfoDto
                        {
                            User_Id = booker.User_Booker.Id,
                            Trip_Starting_Station = booker.Starting_Station_Title,
                            Trip_Ending_Station = booker.Ending_Station_Title,
                            Passenger_Name = booker.Passenger_Name,
                            Passenger_Surname = booker.Passenger_Surname
                        }).ToList();

                        //Перевірка, чи є дане місце в списку зайнятих для даного вагону
                        if (booked_places_in_current_carriage.Contains(current_place_in_carriage))
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = false,
                                Passenger_Trip_Info = trip_info_for_current_place
                            });
                            booked_places_amount++;
                        }
                        else
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = true
                            });
                        }
                    }
                    int free_places_amount = capacity - booked_places_amount;

                    //Додаємо в загальну статистику поїзда статистику одного конкретного вагона
                    train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id].Carriage_Statistics_List.Add(new InternalCarriageAssignmentRepresentationDto
                    {
                        Carriage_Assignment = current_carriage_assignment,
                        Free_Places = free_places_amount,
                        Places_Availability = places_availability,
                        Total_Places = capacity,
                    });
                    //Оновлюємо загальну статистику місць в поїзді на основі бронювань  в даному вагоні
                    train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id].Total_Place_Highlights.CountPlaces(passenger_carriage.Type_Of, free_places_amount, capacity);
                }
            }
            return new SuccessQuery<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>(train_route_on_date_carriage_booking_statistics_dictionary);
        }

        [Crucial]
        [Algorithm("АЛГОРИТМ ТРЬОХ СЕКЦІЙ")]
        [Refactored("v1", "18.04.2025")]
        public static (List<int>, List<int>, List<int>) DivideSectionIntoThreeParts(List<TrainRouteOnDateOnStation> all_sorted_train_stops_of_train_route_on_date,
            string desired_starting_station_title, string desired_ending_station_title)
        {
            //Ініціалізовуємо 3 секції
            List<int> left_part = new List<int>(); //Ліва секція включає всі станції на маршруті поїзда до початкової станції подорожі включно з нею
            List<int> central_part = new List<int>(); //Центральна секція включає всі станції на маршруті поїзда між початковою та кінцевою станцією подорожі, обидві не включно
            List<int> right_part = new List<int>(); //Права секція включає всі станції на маршруті поїзда після кінцевої станції подорожі включно з нею
            int desired_starting_stop_index = all_sorted_train_stops_of_train_route_on_date
                .FindIndex(train_stop => train_stop.Station.Title == desired_starting_station_title); //Знаходимо номер за рахунком початкової станції подорожі 
            int desired_ending_stop_index = all_sorted_train_stops_of_train_route_on_date
                .FindIndex(train_stop => train_stop.Station.Title == desired_ending_station_title); //Знаходимо номер за рахунком кінцевої станції подорожі
            
            for (int stop_index = 0; stop_index <= desired_starting_stop_index; stop_index++) //Включаємо в ліву секцію всі станції до початкової станції подорожі включно з нею
            {
                left_part.Add(all_sorted_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            for (int stop_index = desired_starting_stop_index + 1; stop_index < desired_ending_stop_index; stop_index++) //Включаємо в центральну секцію всі станції між початковою та кінцевою станцією подорожі не включно
            {
                central_part.Add(all_sorted_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            for (int stop_index = desired_ending_stop_index; stop_index < all_sorted_train_stops_of_train_route_on_date.Count; stop_index++) //Включаємо в праву секції всі станції після кінцевої у подорожі включно
            {
                right_part.Add(all_sorted_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            //Індекси в списку станцій можна брати до уваги, бо метод приймає посортований список станцій поїзда(це треба враховувати при виклику методу)
            return (left_part, central_part, right_part);
        }

        [Crucial]
        [Algorithm("МОДИФІКОВАНИЙ АЛГОРИТМ ТРЬОХ СЕКЦІЙ ДЛЯ ДЕКІЛЬКОХ ПОЇЗДІВ")]
        [Refactored("v1", "18.04.2025")]
        public static (List<int>, List<int>, List<int>) DivideSectionIntoThreePartsForSeveralTrains(List<List<TrainRouteOnDateOnStation>> all_sorted_train_stops_of_several_train_routes_on_date,
            string desired_starting_station_title, string desired_ending_station_title)
        {
            //Ініціалізація секцій аналогічно до базового алгоритму
            List<int> consolidated_left_part = new List<int>();
            List<int> consolidated_central_part = new List<int>();
            List<int> consolidated_right_part = new List<int>();
            //Перебираємо список станцій для кожного поїзду окремо
            foreach (List<TrainRouteOnDateOnStation> all_sorted_train_stops_for_current_train_route_on_date in all_sorted_train_stops_of_several_train_routes_on_date)
            {
                //Ділимо станції на 3 секцій для одного конкретного поїзда через базовий алгоритм
                (List<int> left_part_for_current_train, List<int> central_part_for_current_train, List<int> right_part_for_current_train) =
                    DivideSectionIntoThreeParts(all_sorted_train_stops_for_current_train_route_on_date, desired_starting_station_title, desired_ending_station_title);
                //Консолідуємо секцій для всіх поїздів(об'єднуємо ліву, центральну та праву секції між собою поокремо для кожного поїзда, створюючи консолідовані ліву, центральну та праву секції
                consolidated_left_part = consolidated_left_part.Union(left_part_for_current_train).ToList();
                consolidated_central_part = consolidated_central_part.Union(central_part_for_current_train).ToList();
                consolidated_right_part = consolidated_right_part.Union(right_part_for_current_train).ToList();
            }
            return (consolidated_left_part, consolidated_central_part, consolidated_right_part);
        }
        public async Task<TicketBooking?> FindTicketBooking(int user_id, string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title, int place_in_carriage)
        {
            TicketBooking? ticket_booking = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .FirstOrDefaultAsync(ticket_booking =>
            ticket_booking.User_Id == user_id && ticket_booking.Train_Route_On_Date_Id == train_route_on_date_id
            && ticket_booking.Passenger_Carriage_Id == passenger_carriage_id && ticket_booking.Starting_Station.Title == starting_station_title
            && ticket_booking.Ending_Station.Title == ending_station_title && ticket_booking.Place_In_Carriage == place_in_carriage);
            return ticket_booking;
        }
        public async Task<TicketBooking?> FindTicketBookingById(int ticket_booking_id)
        {
            TicketBooking? ticket_booking = await context.Ticket_Bookings.FirstOrDefaultAsync(ticket_booking => ticket_booking.Id == ticket_booking_id);
            return ticket_booking;
        }
        public async Task UpdateTicketBooking(TicketBooking ticket_booking)
        {
            context.Ticket_Bookings.Update(ticket_booking);
            await context.SaveChangesAsync();
        }
        public async Task DeleteTicketBooking(TicketBooking ticket_booking)
        {
            context.Ticket_Bookings.Remove(ticket_booking);
            await context.SaveChangesAsync();
        }

        //Перевантажена версія попереднього методу
        public async Task<InternalCarriageAssignmentRepresentationDto?> GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDate carriage_assignment, string starting_station_title, string ending_station_title)
        {
            TrainRouteOnDate train_route_on_date = carriage_assignment.Train_Route_On_Date;
            return await GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(train_route_on_date.Id, carriage_assignment.Position_In_Squad, starting_station_title, ending_station_title);
        }


















        //НЕ ВИКОРИСТОВУЄТЬСЯ
        [Executive("Перевірка броні на всі місця в усіх вагонах в рейсі на машруті між двома станціями " +
              "з поверненням об'єтку, що містить призначення вагона і список вільних-зайнятих місць")]
        [NotInUse]
        public async Task<List<InternalCarriageAssignmentRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForTrainRouteOnDate(string train_route_on_date_id, string starting_station_title, string ending_station_title)
        {
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            (List<int> left_part, List<int> central_part, List<int> right_part) =
                DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, starting_station_title, ending_station_title);


            List<CarriageBookedPlace> booked_places_in_all_carriages = await context.Ticket_Bookings.Where(ticket => ticket.Train_Route_On_Date_Id == train_route_on_date_id &&
            (
           central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id)
           || left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id)))
                .Select(ticket => new CarriageBookedPlace { Carriage_Id = ticket.Passenger_Carriage_Id, Place = ticket.Place_In_Carriage }).ToListAsync();


            List<InternalCarriageAssignmentRepresentationDto> carriage_statistics_list = new List<InternalCarriageAssignmentRepresentationDto>();

            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments = await full_train_route_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_id);
            if (carriage_assignments == null)
            {
                return null;
            }
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments)
            {
                HashSet<int> booked_places_for_single_carriage = booked_places_in_all_carriages
                    .Where(carriage_place => carriage_place.Carriage_Id == carriage_assignment.Passenger_Carriage_Id)
                    .Select(carriage_place => carriage_place.Place).ToHashSet();

                int total_places_amount = carriage_assignment.Passenger_Carriage.Capacity;
                List<InternalSinglePlaceDto> places_list_for_single_carriage = new List<InternalSinglePlaceDto>();
                for (int place_number = 1; place_number <= total_places_amount; place_number++)
                {
                    if (booked_places_for_single_carriage.Contains(place_number))
                    {
                        places_list_for_single_carriage.Add(new InternalSinglePlaceDto { Place_In_Carriage = place_number, Is_Free = false });
                    }
                    else
                    {
                        places_list_for_single_carriage.Add(new InternalSinglePlaceDto { Place_In_Carriage = place_number, Is_Free = true });
                    }
                }
                carriage_statistics_list.Add(new InternalCarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = carriage_assignment,
                    Places_Availability = places_list_for_single_carriage
                });
            }
            return carriage_statistics_list;

        }

        [Crucial("Перевірка броні на всі місця в усіх вагонах в декількох рейсах на машрутах між двома станціями " +
    "з поверненням об'єтку, що містить призначення вагона і список вільних-зайнятих місць)")]
        [Archieved]
        [NotInUse]
        public async Task<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDateIntoRepresentationDto_OLDVERSION(List<string> train_route_on_date_ids_list, string starting_station_title, string ending_station_title)
        {
            List<TrainRouteOnDate> train_route_on_date_list = context.Train_Routes_On_Date
                .Where(train_route_on_date => train_route_on_date_ids_list.Contains(train_route_on_date.Id)).ToList();
            List<TrainRouteOnDateOnStation>? all_train_stops_of_all_train_routes_on_date = await full_train_route_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
            if (all_train_stops_of_all_train_routes_on_date == null)
            {
                return null;
            }
            List<List<TrainRouteOnDateOnStation>> all_train_stops_of_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach (string train_route_on_date_id in train_route_on_date_ids_list)
            {
                List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date = all_train_stops_of_all_train_routes_on_date
                    .Where(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_id).ToList();
                all_train_stops_of_train_route_on_date_list.Add(all_train_stops_of_train_route_on_date);
            }

            (List<int> left_part, List<int> central_part, List<int> right_part) =
                DivideSectionIntoThreePartsForSeveralTrains(all_train_stops_of_train_route_on_date_list, starting_station_title, ending_station_title);


            List<TrainRouteOnDateCarriageBookedPlace> booked_places_in_all_carriages = await context.Ticket_Bookings.Where(ticket => train_route_on_date_ids_list.Contains(ticket.Train_Route_On_Date_Id) &&
            (
           central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id)
           || left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id)))
                .Select(ticket => new TrainRouteOnDateCarriageBookedPlace { Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id, Carriage_Id = ticket.Passenger_Carriage_Id, Place = ticket.Place_In_Carriage }).ToListAsync();


            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments = await full_train_route_search_service.GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
            if (carriage_assignments == null)
            {
                return null;
            }
            carriage_assignments = carriage_assignments.OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> carriage_statistics_list_for_every_train_route_on_date =
                new Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>();
            foreach (string train_route_on_date_id in train_route_on_date_ids_list)
            {
                carriage_statistics_list_for_every_train_route_on_date[train_route_on_date_id] = new InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto
                {
                    Train_Route_On_Date = train_route_on_date_list.FirstOrDefault(train_route_on_date => train_route_on_date.Id == train_route_on_date_id)!,
                };
            }
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments)
            {
                HashSet<int> booked_places_for_single_carriage = booked_places_in_all_carriages
                    .Where(carriage_place => carriage_place.Carriage_Id == carriage_assignment.Passenger_Carriage_Id)
                    .Select(carriage_place => carriage_place.Place).ToHashSet(); //Цей метод був частково не правильний, адже не перевіряв поїзд, в якому курсує вагон, в який куплено квиток
                                                                                 //якщо складеться така ситуація, що один вагон на одному маршруті в один день буде курсувати в різних поїздах, то станеться аномальна, некоректна ситуація

                int booked_places_amount = 0;
                int total_places_amount = carriage_assignment.Passenger_Carriage.Capacity;
                string current_train_route_on_date_id = carriage_assignment.Train_Route_On_Date_Id;
                List<InternalSinglePlaceDto> places_list_for_single_carriage = new List<InternalSinglePlaceDto>();
                for (int place_number = 1; place_number <= total_places_amount; place_number++)
                {
                    if (booked_places_for_single_carriage.Contains(place_number))
                    {
                        places_list_for_single_carriage.Add(new InternalSinglePlaceDto { Place_In_Carriage = place_number, Is_Free = false });
                        booked_places_amount++;
                    }
                    else
                    {
                        places_list_for_single_carriage.Add(new InternalSinglePlaceDto { Place_In_Carriage = place_number, Is_Free = true });
                    }
                }
                int free_places_amount = total_places_amount - booked_places_amount;
                carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Carriage_Statistics_List.Add(new InternalCarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = carriage_assignment,
                    Places_Availability = places_list_for_single_carriage,
                    Total_Places = total_places_amount,
                    Free_Places = free_places_amount
                });
                switch (carriage_assignment.Passenger_Carriage.Type_Of)
                {
                    /*
                    case PassengerCarriageType.Platskart:
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Platskart_Free += free_places_amount;
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Platskart_Total += total_places_amount;
                        break;
                    case PassengerCarriageType.Coupe:
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Coupe_Free += free_places_amount;
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Coupe_Total += total_places_amount;
                        break;
                    case PassengerCarriageType.SV:
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].SV_Free += free_places_amount;
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].SV_Total += total_places_amount;
                        break;
                    */
                }

            }


            return carriage_statistics_list_for_every_train_route_on_date;
        }

        [Archieved]
        [NotInUse]
        public async Task<bool?> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate_OLDVERSION(string train_route_on_date_id, string desired_starting_station_title, string desired_ending_station_title,
        string passenger_carriage_id, int place_in_carriage)
        {

            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
            if (train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(passenger_carriage_id);
            if (passenger_carriage == null)
            {
                text_service.FailPostInform("Fail in PassengerCarriageService");
                return null;
            }
            Station? desired_starting_station = await station_service.FindStationByTitle(desired_starting_station_title);
            Station? desired_ending_station = await station_service.FindStationByTitle(desired_ending_station_title);
            if (desired_starting_station == null || desired_ending_station == null)
            {
                text_service.FailPostInform("Fail in StationService");
                return null;
            }

            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }

            (List<int> left_part, List<int> central_part, List<int> right_part) =
                DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, desired_starting_station_title, desired_ending_station_title);

            bool is_available = !await context.Ticket_Bookings.AnyAsync(ticket => ticket.Passenger_Carriage_Id == passenger_carriage_id
            && ticket.Train_Route_On_Date_Id == train_route_on_date_id && ticket.Place_In_Carriage == place_in_carriage
            && (central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id)
             || left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id)));

            return is_available;

        }

        [Archieved]
        [NotInUse]
        public async Task<TicketBooking?> CreateTicketBooking_OLDVERSION(InternalTicketBookingDto input)
        {
            User? user_from_ticket = await context.Users.FindAsync(input.User_Id);
            if (user_from_ticket == null)
            {
                return null;
            }
            TrainRouteOnDate? train_route_on_date_from_ticket = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if (train_route_on_date_from_ticket == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            PassengerCarriage? passenger_carriage_from_ticket = await passenger_carriage_service.FindPassengerCarriageById(input.Passenger_Carriage_Id);
            if (passenger_carriage_from_ticket == null)
            {
                text_service.FailPostInform("Fail in PassengerCarriageService");
                return null;
            }
            Station? starting_station_from_ticket = await station_service.FindStationByTitle(input.Starting_Station_Title);
            Station? ending_station_from_ticket = await station_service.FindStationByTitle(input.Ending_Station_Title);
            if (starting_station_from_ticket == null || ending_station_from_ticket == null)
            {
                text_service.FailPostInform("Fail in StationService");
                return null;
            }
            //Check if train route on date passes through the stations from ticket
            List<TrainRouteOnDateOnStation>? train_stops_for_train_route_on_date_from_ticket = await
                full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_from_ticket.Id);
            if (train_stops_for_train_route_on_date_from_ticket == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            if (!train_stops_for_train_route_on_date_from_ticket.Any(train_stop => train_stop.Station_Id == starting_station_from_ticket.Id)
                || !train_stops_for_train_route_on_date_from_ticket.Any(train_stop => train_stop.Station_Id == ending_station_from_ticket.Id))
            {
                text_service.FailPostInform("Train route on date doesn't pass through these stations");
                return null;
            }
            int starting_station_from_ticket_number = train_stops_for_train_route_on_date_from_ticket.FindIndex(train_stop =>
            train_stop.Station_Id == starting_station_from_ticket.Id);
            int ending_station_from_ticket_number = train_stops_for_train_route_on_date_from_ticket.FindIndex(train_stop =>
            train_stop.Station_Id == ending_station_from_ticket.Id);
            if (starting_station_from_ticket_number >= ending_station_from_ticket_number)
            {
                text_service.FailPostInform("Train route on date doesn't pass through these stations(in this order)");
                return null;
            }
            //Check if train route on date contains this carriage in its squad
            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignements_for_train_route_on_date_from_ticket =
                await full_train_route_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_from_ticket.Id);
            if (carriage_assignements_for_train_route_on_date_from_ticket == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            if (!carriage_assignements_for_train_route_on_date_from_ticket
                .Any(carriage_assignement => carriage_assignement.Passenger_Carriage_Id == passenger_carriage_from_ticket.Id))
            {
                text_service.FailPostInform("Train route on date doesn't contains these carriage in its squad");
                return null;
            }
            if (input.Place_In_Carriage > passenger_carriage_from_ticket.Capacity || input.Place_In_Carriage <= 0)
            {
                text_service.FailPostInform($"This carriage doesn't have place with ID: {input.Place_In_Carriage}");
                return null;
            }
            //Check if the place in carriage is available for booking between these stations on train route on date
            /*
            bool? _ticket_booking_availability = await CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_from_ticket.Id,
                starting_station_from_ticket.Title, ending_station_from_ticket.Title, passenger_carriage_from_ticket.Id,
                input.Place_In_Carriage);
            bool ticket_booking_availability;
            if (_ticket_booking_availability == null)
            {
                text_service.FailPostInform("Fail in checking ticket availability");
                return null;
            }
            else
            {
                ticket_booking_availability = (bool)_ticket_booking_availability;
            }
            if (ticket_booking_availability == false)
            {
                text_service.FailPostInform($"The place with ID: {input.Place_In_Carriage} in carriage {passenger_carriage_from_ticket.Id} has already been booked");
                return null;
            }*/
            TicketBooking ticket_booking = new TicketBooking()
            {
                User = user_from_ticket,
                Passenger_Carriage = passenger_carriage_from_ticket,
                Train_Route_On_Date = train_route_on_date_from_ticket,
                Starting_Station = starting_station_from_ticket,
                Ending_Station = ending_station_from_ticket,
                Place_In_Carriage = input.Place_In_Carriage,
                Passenger_Name = input.Passenger_Name,
                Passenger_Surname = input.Passenger_Surname,
                Booking_Time = DateTime.Now,
                Ticket_Status = input.Ticket_Status
            };
            await context.Ticket_Bookings.AddAsync(ticket_booking);
            await context.SaveChangesAsync();
            return ticket_booking;
        }

        [Archieved]
        [NotInUse]
        public async Task<TicketBooking?> CreateTicketBooking_OLDVERSION(InternalTicketBookingDtoWithCarriagePosition input)
        {
            PassengerCarriageOnTrainRouteOnDate? carriage_assignement = await
                context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignement => carriage_assignement.Passenger_Carriage)
                .Include(carriage_assignement => carriage_assignement.Train_Route_On_Date)
                .FirstOrDefaultAsync(carriage_assignement =>
                carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id && carriage_assignement.Position_In_Squad == input.Passenger_Carriage_Position_In_Squad);
            if (carriage_assignement is null)
            {
                text_service.FailPostInform($"There is no carriage with number {input.Passenger_Carriage_Position_In_Squad} in train route on date squad");
                return null;
            }
            PassengerCarriage passenger_carriage = carriage_assignement.Passenger_Carriage;
            InternalTicketBookingDto ticket_booking_dto = new InternalTicketBookingDto()
            {
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Passenger_Carriage_Id = passenger_carriage.Id,
                Starting_Station_Title = input.Starting_Station_Title,
                Ending_Station_Title = input.Ending_Station_Title,
                Place_In_Carriage = input.Place_In_Carriage,
                User_Id = input.User_Id,
                Passenger_Name = input.Passenger_Name,
                Passenger_Surname = input.Passenger_Surname,
                Ticket_Status = input.Ticket_Status

            };
            /*
            TicketBooking? ticket_booking = await CreateTicketBooking(ticket_booking_dto);
            if (ticket_booking == null)
            {
                text_service.FailPostInform("Fail while booking ticket");
                return null;
            }
            return ticket_booking;*/
            return null;
        }
    }
}
    