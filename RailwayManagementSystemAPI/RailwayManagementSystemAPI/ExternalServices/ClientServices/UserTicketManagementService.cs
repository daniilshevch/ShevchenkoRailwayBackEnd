using RailwayManagementSystemAPI.ExternalDTO;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using RailwayCore.InternalServices.CoreServices;
using System.Net;
using Microsoft.Extensions.Primitives;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
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
