using RailwayManagementSystemAPI.ExternalDTO;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using RailwayCore.Services;
using System.Net;
using Microsoft.Extensions.Primitives;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
    public class UserTicketManagementService
    {
        private readonly UserAccountManagementService user_account_management_service;
        private readonly CompleteTicketBookingService complete_ticket_booking_service;
        private readonly FullTrainRouteSearchService full_train_route_search_service;
        public UserTicketManagementService(UserAccountManagementService user_account_management_service,
            CompleteTicketBookingService complete_ticket_booking_service, FullTrainRouteSearchService full_train_route_search_service)
        {
            this.user_account_management_service = user_account_management_service;
            this.complete_ticket_booking_service = complete_ticket_booking_service;
            this.full_train_route_search_service = full_train_route_search_service;
        }
        public async Task<QueryResult<List<ExternalProfileTicketBookingDto>>> GetAllBookedTicketsForCurrentUser()
        {
            QueryResult<User> user_authentication_result = await user_account_management_service.GetAuthenticatedUser();
            if(user_authentication_result.Fail)
            {
                return new FailQuery<List<ExternalProfileTicketBookingDto>>(user_authentication_result.Error);
            }
            User user = user_authentication_result.Value;
            List<TicketBooking> ticket_bookings_for_user = (await complete_ticket_booking_service.GetAllTicketBookingsForUser(user.Id))
                .OrderBy(ticket_booking => ticket_booking.Ticket_Status).ToList();
            List<ExternalProfileTicketBookingDto> output_tickets = new List<ExternalProfileTicketBookingDto>();
            foreach(TicketBooking ticket_booking in ticket_bookings_for_user)
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
                output_tickets.Add(output_ticket);
            }
            return new SuccessQuery<List<ExternalProfileTicketBookingDto>>(output_tickets);
            
        }
    }
}
