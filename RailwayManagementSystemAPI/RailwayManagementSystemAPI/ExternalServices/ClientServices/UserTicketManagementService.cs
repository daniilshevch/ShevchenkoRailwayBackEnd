using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using RailwayCore.InternalServices.CoreServices;
using System.Net;
using Microsoft.Extensions.Primitives;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
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

    public class UserTicketManagementService
    {
        private readonly SystemAuthenticationService system_authentication_service;
        private readonly FullTrainRouteSearchService full_train_route_search_service;
        private readonly FullTicketManagementService full_ticket_management_service;
        public UserTicketManagementService(SystemAuthenticationService system_authentication_service,
            FullTrainRouteSearchService full_train_route_search_service, FullTicketManagementService full_ticket_management_service)
        {
            this.system_authentication_service = system_authentication_service;
            this.full_train_route_search_service = full_train_route_search_service;
            this.full_ticket_management_service = full_ticket_management_service;
        }
        public async Task<QueryResult<List<ExternalProfileTicketBookingDto>>> GetAllBookedTicketsForCurrentUser()
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if(user_authentication_result.Fail)
            {
                return new FailQuery<List<ExternalProfileTicketBookingDto>>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            List<TicketBooking> ticket_bookings_for_user = (await full_ticket_management_service.GetAllTicketBookingsForUser(user.Id)).ToList();
            List<ExternalProfileTicketBookingDto> output_tickets = new List<ExternalProfileTicketBookingDto>();
            foreach(TicketBooking ticket_booking in ticket_bookings_for_user)
            {
                ExternalProfileTicketBookingDto output_ticket = await CreateProfileDtoForTicketBooking(ticket_booking);
                output_tickets.Add(output_ticket);
            }
            output_tickets = output_tickets.OrderBy(ticket => ticket.Ticket_Status).ThenBy(ticket => ticket.Departure_Time_From_Trip_Starting_Station).ToList();
            return new SuccessQuery<List<ExternalProfileTicketBookingDto>>(output_tickets);
        }
        public async Task<QueryResult<List<ExternalTicketBookingGroupDto>>> GetAllBookedTicketsInGroupsForCurrentUser()
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<List<ExternalTicketBookingGroupDto>>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            List<TicketBooking> ticket_bookings_for_user = (await full_ticket_management_service.GetAllTicketBookingsForUser(user.Id)).ToList();


            IEnumerable<IGrouping<TicketBookingGroupHeader, TicketBooking>> ticket_groups =
                ticket_bookings_for_user.GroupBy(ticket => new TicketBookingGroupHeader(ticket.Train_Route_On_Date_Id, ticket.Starting_Station_Id, ticket.Ending_Station_Id));


            List<ExternalTicketBookingGroupDto> output_ticket_booking_groups = new List<ExternalTicketBookingGroupDto>();
            foreach(IGrouping<TicketBookingGroupHeader, TicketBooking> ticket_group in ticket_groups)
            {
                TicketBookingGroupHeader ticket_group_header = ticket_group.Key;
                string train_route_on_date_id = ticket_group_header.Train_Route_On_Date_Id;
                int starting_station_id = ticket_group_header.Starting_Station_Id;
                int ending_station_id = ticket_group_header.Ending_Station_Id;
                string full_route_starting_station_title = (await full_train_route_search_service
                    .GetStartingTrainStopForTrainRouteOnDate(train_route_on_date_id))!.Station.Title;
                string full_route_ending_station_title = (await full_train_route_search_service
                    .GetEndingTrainStopForTrainRouteOnDate(train_route_on_date_id))!.Station.Title;
                TrainRouteOnDateOnStation? trip_starting_station = await full_train_route_search_service
                    .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(train_route_on_date_id, starting_station_id);
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
                        Passenger_Surname = ticket_booking.Passenger_Surname
                    }).ToList()
                };
                output_ticket_booking_groups.Add(output_group);
            }
            return new SuccessQuery<List<ExternalTicketBookingGroupDto>>(output_ticket_booking_groups);
        }
        public async Task<QueryResult<ExternalProfileTicketBookingDto>> ReturnTicketBookingForCurrentUserById(string ticket_id)
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if(user_authentication_result.Fail)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(user_authentication_result.Error);
            }
            User authenticated_user = user_authentication_result.Value;
            User? ticket_owner = await full_ticket_management_service.GetTicketOwner(ticket_id);
            if(ticket_owner == null)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(new Error(ErrorType.NotFound, $"Can't find ticket with id: {ticket_id} or ticket owner not found(internal error)"));
            }
            if(authenticated_user.Id != ticket_owner.Id)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(new Error(ErrorType.Forbidden, $"Authenticated user doesn't own this ticket"));
            }
            TicketBooking? returned_ticket_booking = await full_ticket_management_service.ReturnTicketBookingById(ticket_id);
            if(returned_ticket_booking is null)
            {
                return new FailQuery<ExternalProfileTicketBookingDto>(new Error(ErrorType.NotFound, $"Can't find ticket with id: {ticket_id}"));
            }
            ExternalProfileTicketBookingDto output_ticket = await CreateProfileDtoForTicketBooking(returned_ticket_booking);
            return new SuccessQuery<ExternalProfileTicketBookingDto>(output_ticket);
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
                Passenger_Surname = ticket_booking.Passenger_Surname
            };
            return output_ticket;
        }

    }
}
