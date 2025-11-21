using RailwayCore.Models;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces
{
    public interface ITicketSystemManipulationService
    {
        Task<QueryResult<TicketBooking>> DeleteTicketBooking(TicketBooking ticket_booking);
        Task<TicketBooking?> FindTicketBooking(int user_id, string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title, int place_in_carriage);
        Task<TicketBooking?> FindTicketBookingById(int ticket_booking_id);
        Task<QueryResult<TicketBooking>> UpdateTicketBooking(TicketBooking ticket_booking);
    }
}