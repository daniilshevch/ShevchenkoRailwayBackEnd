using RailwayCore.InternalServices.ExecutiveServices.ExecutiveDTO.TicketManagementDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.CoreServices.Interfaces
{
    public interface IFullTicketManagementService
    {
        Task<QueryResult<bool>> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title, string desired_ending_station_title, string passenger_carriage_id, int place_in_carrriage);
        Task<QueryResult<TicketBooking>> CreateTicketBooking(InternalTicketBookingDto input);
        Task<QueryResult<TicketBooking>> CreateTicketBookingWithCarriagePositionInSquad(InternalTicketBookingWithCarriagePositionDto input);
        Task DeleteAllExpiredTickets();
        Task<QueryResult<TicketBooking>> DeleteTicketBooking(TicketBooking ticket_booking);
        Task<TicketBooking?> FindTicketBooking(int user_id, string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title, int place_in_carriage);
        Task<TicketBooking?> FindTicketBookingById(int ticket_booking_id);
        Task<List<TicketBooking>> GetAllExpiredTicketBookings();
        Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title);
        Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics(List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title);
        Task<List<TicketBooking>> GetAllTicketBookingsForUser(int user_id, bool only_active = true);
        Task<User?> GetTicketOwner(string ticket_id);
        Task<QueryResult<TicketBooking>> ReturnTicketBookingById(string ticket_id);
        Task<QueryResult<TicketBooking>> UpdateTicketBooking(TicketBooking ticket_booking);
    }
}