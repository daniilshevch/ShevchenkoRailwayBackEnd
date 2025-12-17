using Microsoft.Extensions.Primitives;
using RailwayCore.InternalServices.CoreServices.Implementations;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using System.Diagnostics;
using System.Net;
using RailwayCore.InternalServices.CoreServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.SystemAuthenticationServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;
namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    /// <summary>
    /// Допоміжний клас, який дозволяє згрупувати квитки в групи за спільністю рейсу поїзда та поїздки(початкової та кінцевої станції)
    /// </summary>
    public class TicketBookingGroupHeader: IEquatable<TicketBookingGroupHeader>
    {
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public int Starting_Station_Id { get; set; }
        public int Ending_Station_Id { get; set; }
        public TicketBookingGroupHeader(string train_route_on_date_id, int starting_station_id, int ending_station_id)
        {
            Train_Route_On_Date_Id = train_route_on_date_id;
            Starting_Station_Id = starting_station_id;
            Ending_Station_Id = ending_station_id;
        }
        public bool Equals(TicketBookingGroupHeader? other)
        {
            if (other is null)
            {
                return false;
            }
            return Train_Route_On_Date_Id == other.Train_Route_On_Date_Id
                && Starting_Station_Id == other.Starting_Station_Id
                && Ending_Station_Id == other.Ending_Station_Id;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as TicketBookingGroupHeader);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Train_Route_On_Date_Id, Starting_Station_Id, Ending_Station_Id);
        }

    }
    /// <summary>
    /// Даний сервіс містить в собі команди, які відповідають за функціонал можливих дій користувача зі своїми квитками, а саме перегляд 
    /// вже придбаних квитків, квитків, які тимчасово зарезервовані на користувача та квитків, які були повернуті, 
    /// а також функіонал повернення користувачем вже придбаних квитків(саме куплених, а не скасування попередньо броні)
    /// </summary>
    [ClientApiService]
    public class UserTicketManagementService : IUserTicketManagementService
    {
        private readonly string service_name = "UserTicketManagementService";
        private readonly SystemAuthenticationService system_authentication_service;
        private readonly IFullTrainRouteSearchService full_train_route_search_service;
        private readonly IFullTicketManagementService full_ticket_management_service;
        private readonly IQRCodeGeneratorService qr_code_generator_service;
        public UserTicketManagementService(SystemAuthenticationService system_authentication_service,
            IFullTrainRouteSearchService full_train_route_search_service, IFullTicketManagementService full_ticket_management_service,
            IQRCodeGeneratorService qr_code_generator_service)
        {
            this.system_authentication_service = system_authentication_service;
            this.full_train_route_search_service = full_train_route_search_service;
            this.full_ticket_management_service = full_ticket_management_service;
            this.qr_code_generator_service = qr_code_generator_service;
        }
        /// <summary>
        /// Даний метод вертає список всіх квитків, які належать користувачу
        /// </summary>
        /// <returns></returns>
        public async Task<QueryResult<List<ExternalProfileTicketBookingDto>>> GetAllBookedTicketsForCurrentUser()
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<List<ExternalProfileTicketBookingDto>>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            List<TicketBooking> ticket_bookings_for_user = (await full_ticket_management_service.GetAllTicketBookingsForUser(user.Id, only_active: true)).ToList();
            List<ExternalProfileTicketBookingDto> output_tickets = new List<ExternalProfileTicketBookingDto>();
            foreach (TicketBooking ticket_booking in ticket_bookings_for_user)
            {
                ExternalProfileTicketBookingDto output_ticket = await CreateProfileDtoForTicketBooking(ticket_booking);
                output_tickets.Add(output_ticket);
            }
            output_tickets = output_tickets.OrderBy(ticket => ticket.Ticket_Status).ThenBy(ticket => ticket.Departure_Time_From_Trip_Starting_Station).ToList();
            return new SuccessQuery<List<ExternalProfileTicketBookingDto>>(output_tickets);
        }
        /// <summary>
        /// Даний метод вертає список всіх квитків, що належать користувачу, причому групує квитки за поїздкою(тобто якщо декілька квитків пасажир
        /// має на один рейс поїзда, але на різні місця(+можливо, різні вагони), то ці квитки будуть зібрані в одну групу
        /// </summary>
        /// <returns></returns>
        public async Task<QueryResult<List<ExternalTicketBookingGroupDto>>> GetAllBookedTicketsInGroupsForCurrentUser()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------USER TICKET MANAGEMENT-------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
            //Отримуємо користувача, який аутентифікований в системі
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<List<ExternalTicketBookingGroupDto>>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            //Отримуємо з внутрішнього сервіса список всіх квитків для користувача
            List<TicketBooking> ticket_bookings_for_user = (await full_ticket_management_service.GetAllTicketBookingsForUser(user.Id)).ToList();


            //Організовуємо квитки в групи за спільністю рейса поїзда та початкової і кінцевої станцій поїздки в квитку
            IEnumerable<IGrouping<TicketBookingGroupHeader, TicketBooking>> ticket_groups =
                ticket_bookings_for_user.GroupBy(ticket => new TicketBookingGroupHeader(ticket.Train_Route_On_Date_Id, ticket.Starting_Station_Id, ticket.Ending_Station_Id));


            //Перетворюємо внутрішні допоміжні групові об'єкти в зовнішні трансферні об'єкти для представлення груп взаємопов'язаних квитків
            List<ExternalTicketBookingGroupDto> output_ticket_booking_groups = new List<ExternalTicketBookingGroupDto>();
            foreach (IGrouping<TicketBookingGroupHeader, TicketBooking> ticket_group in ticket_groups)
            {
                TicketBookingGroupHeader ticket_group_header = ticket_group.Key;
                string train_route_on_date_id = ticket_group_header.Train_Route_On_Date_Id;
                int starting_station_id = ticket_group_header.Starting_Station_Id;
                int ending_station_id = ticket_group_header.Ending_Station_Id;
                //Знаходимо початкову станцію для всього рейсу поїзда
                string full_route_starting_station_title = (await full_train_route_search_service
                    .GetStartingTrainStopForTrainRouteOnDate(train_route_on_date_id))!.Station.Title;
                //Знаходимо кінцеву станцію для всього рейсу поїзда
                string full_route_ending_station_title = (await full_train_route_search_service
                    .GetEndingTrainStopForTrainRouteOnDate(train_route_on_date_id))!.Station.Title;
                //Отримуємо початкову зупинку поїздки пасажира з квитку
                TrainRouteOnDateOnStation? trip_starting_station = await full_train_route_search_service
                    .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(train_route_on_date_id, starting_station_id);
                //Отримуємо кінцеву зупинку поїздки пасажира з квитку
                TrainRouteOnDateOnStation? trip_ending_station = await full_train_route_search_service
                    .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(train_route_on_date_id, ending_station_id);
                string? trip_starting_station_title = trip_starting_station?.Station.Title;
                string? trip_ending_station_title = trip_ending_station?.Station.Title;
                DateTime? departure_time_from_trip_starting_station = trip_starting_station?.Departure_Time;
                DateTime? arrival_time_to_trip_ending_station = trip_ending_station?.Arrival_Time;
                TimeSpan? trip_duration = arrival_time_to_trip_ending_station - departure_time_from_trip_starting_station;
                string train_route_id = trip_starting_station!.Train_Route_On_Date.Train_Route_Id;
                string train_route_class = TextEnumConvertationService.GetTrainQualityClassIntoString(trip_starting_station!.Train_Route_On_Date.Train_Route.Quality_Class)!;
                string train_branded_name = trip_starting_station!.Train_Route_On_Date.Train_Route.Branded_Name!;
                ExternalTicketBookingGroupDto output_group = new ExternalTicketBookingGroupDto()
                {
                    Train_Route_On_Date_Id = ticket_group_header.Train_Route_On_Date_Id,
                    Train_Route_Id = train_route_id,
                    Train_Route_Class = train_route_class,
                    Train_Route_Branded_Name = train_branded_name,
                    Full_Route_Starting_Station_Title = full_route_starting_station_title,
                    Full_Route_Ending_Station_Title = full_route_ending_station_title,
                    Trip_Starting_Station_Title = trip_starting_station_title!,
                    Trip_Ending_Station_Title = trip_ending_station_title!,
                    Departure_Time_From_Trip_Starting_Station = departure_time_from_trip_starting_station,
                    Arrival_Time_To_Trip_Ending_Station = arrival_time_to_trip_ending_station,
                    Trip_Duration = trip_duration,
                    Ticket_Bookings_List = ticket_group.Select(ticket_booking => new ExternalProfileTicketBookingDto
                    {
                        Full_Ticket_Id = ticket_booking.Full_Ticket_Id,
                        Ticket_Status = TextEnumConvertationService.GetTicketBookingStatusIntoString(ticket_booking.Ticket_Status),
                        Train_Route_On_Date_Id = ticket_booking.Train_Route_On_Date_Id,
                        Train_Route_Id = ticket_booking.Train_Route_On_Date.Train_Route_Id,
                        Passenger_Carriage_Position_In_Squad = ticket_booking.Passenger_Carriage_Position_In_Squad,
                        Place_In_Carriage = ticket_booking.Place_In_Carriage,
                        Carriage_Type = TextEnumConvertationService.GetCarriageTypeIntoString(ticket_booking.Passenger_Carriage.Type_Of),
                        Carriage_Quality_Class = TextEnumConvertationService.GetCarriageQualityClassIntoString(ticket_booking.Passenger_Carriage.Quality_Class),
                        Full_Route_Starting_Station_Title = full_route_starting_station_title,
                        Full_Route_Ending_Station_Title = full_route_ending_station_title,
                        Trip_Starting_Station_Title = ticket_booking.Starting_Station.Title,
                        Trip_Ending_Station_Title = ticket_booking.Ending_Station.Title,
                        Departure_Time_From_Trip_Starting_Station = departure_time_from_trip_starting_station,
                        Arrival_Time_To_Trip_Ending_Station = arrival_time_to_trip_ending_station,
                        Trip_Duration = trip_duration,
                        Passenger_Name = ticket_booking.Passenger_Name,
                        Passenger_Surname = ticket_booking.Passenger_Surname,
                        Qr_Code = qr_code_generator_service.GenerateQrCodeBase64(ticket_booking.Full_Ticket_Id.ToString()) //!!!!Вирішити
                    }).ToList()
                };
                output_ticket_booking_groups.Add(output_group);
            }

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"User tickets search time: ");
            Console.ResetColor();
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            return new SuccessQuery<List<ExternalTicketBookingGroupDto>>(output_ticket_booking_groups, new SuccessMessage($"Successfully got " +
                $"grouped ticket bookings for user {ConsoleLogService.PrintUser(user)}", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }
        /// <summary>
        /// Даний метод вертає вже попередньо придбаний квиток користувача. Важливо, що мова йде саме про куплений квиток(і оплачений), а не
        /// про тимчасово зарезервований за користувачем квиток під час процесу купівлі(таким поверненням 
        /// займається CompleteTicketBookingProcessingService)
        /// </summary>
        /// <param name="ticket_id"></param>
        /// <returns></returns>
        public async Task<QueryResult<ExternalProfileTicketBookingDto>> ReturnTicketBookingForCurrentUserById(string ticket_id)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------USER TICKET MANAGEMENT-------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
            //Отримуємо аутентифікованого користувача
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(user_authentication_result.Error);
            }
            User authenticated_user = user_authentication_result.Value;
            //Отримуємо власника квитка
            User? ticket_owner = await full_ticket_management_service.GetTicketOwner(ticket_id);
            if (ticket_owner == null)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(new Error(ErrorType.NotFound, $"Can't find ticket with id: {ticket_id} or ticket owner not found(internal error)", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            //Якщо айді аутентифікованого користувача не збігається з айді власника квитка, то операція заборонена
            if (authenticated_user.Id != ticket_owner.Id)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(new Error(ErrorType.Forbidden, $"Authenticated user doesn't own this ticket", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            //Проводимо повернення квитка(переведення квитка в статус Returned)
            QueryResult<TicketBooking> ticket_booking_return_result = await full_ticket_management_service.ReturnTicketBookingById(ticket_id);
            if (ticket_booking_return_result.Fail)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(ticket_booking_return_result.Error);
            }
            TicketBooking? returned_ticket_booking = ticket_booking_return_result.Value;
            if (returned_ticket_booking is null)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(new Error(ErrorType.NotFound, $"Can't find ticket with id: {ticket_id}", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            ExternalProfileTicketBookingDto output_ticket = await CreateProfileDtoForTicketBooking(returned_ticket_booking);
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Ticket return time: ");
            Console.ResetColor();
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            return new SuccessQuery<ExternalProfileTicketBookingDto>(output_ticket, new SuccessMessage($"Ticket with ID: {returned_ticket_booking.Full_Ticket_Id} " +
                $"has been successfully returned by user {ConsoleLogService.PrintUser(returned_ticket_booking.User)}", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }
        public async Task<ExternalProfileTicketBookingDto> CreateProfileDtoForTicketBooking(TicketBooking ticket_booking)
        {
            string ticket_status = TextEnumConvertationService.GetTicketBookingStatusIntoString(ticket_booking.Ticket_Status);
            string carriage_type = TextEnumConvertationService.GetCarriageTypeIntoString(ticket_booking.Passenger_Carriage.Type_Of);
            string? carriage_quality_class = TextEnumConvertationService.GetCarriageQualityClassIntoString(ticket_booking.Passenger_Carriage.Quality_Class);
            string full_route_starting_station_title = (await full_train_route_search_service
                .GetStartingTrainStopForTrainRouteOnDate(ticket_booking.Train_Route_On_Date_Id))!.Station.Title;
            string full_route_ending_station_title = (await full_train_route_search_service
                .GetEndingTrainStopForTrainRouteOnDate(ticket_booking.Train_Route_On_Date_Id))!.Station.Title;
            DateTime? departure_time_from_trip_starting_station = (await full_train_route_search_service
                .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(ticket_booking.Train_Route_On_Date_Id, ticket_booking.Starting_Station_Id))!.Departure_Time;
            DateTime? arrival_time_to_trip_ending_station = (await full_train_route_search_service
                .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(ticket_booking.Train_Route_On_Date_Id, ticket_booking.Ending_Station_Id))!.Arrival_Time;
            TimeSpan? trip_duration = arrival_time_to_trip_ending_station - departure_time_from_trip_starting_station;
            string qr_code_base_64 = qr_code_generator_service.GenerateQrCodeBase64(ticket_booking.Full_Ticket_Id.ToString()); //!!!!Вирішити
            ExternalProfileTicketBookingDto output_ticket = new ExternalProfileTicketBookingDto()
            {
                Full_Ticket_Id = ticket_booking.Full_Ticket_Id,
                Ticket_Status = ticket_status,
                Train_Route_On_Date_Id = ticket_booking.Train_Route_On_Date_Id,
                Train_Route_Id = ticket_booking.Train_Route_On_Date.Train_Route_Id,
                Passenger_Carriage_Position_In_Squad = ticket_booking.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = ticket_booking.Place_In_Carriage,
                Carriage_Type = carriage_type,
                Carriage_Quality_Class = carriage_quality_class,
                Full_Route_Starting_Station_Title = full_route_starting_station_title,
                Full_Route_Ending_Station_Title = full_route_ending_station_title,
                Trip_Starting_Station_Title = ticket_booking.Starting_Station.Title,
                Trip_Ending_Station_Title = ticket_booking.Ending_Station.Title,
                Departure_Time_From_Trip_Starting_Station = departure_time_from_trip_starting_station,
                Arrival_Time_To_Trip_Ending_Station = arrival_time_to_trip_ending_station,
                Trip_Duration = trip_duration,
                Passenger_Name = ticket_booking.Passenger_Name,
                Passenger_Surname = ticket_booking.Passenger_Surname,
                Qr_Code = qr_code_base_64
            };
            return output_ticket;
        }
    }
}
