using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface ITicketBookingRepository
    {
        Task<QueryResult<List<TicketBooking>>> GetTicketBookingsForCarriageAssignment(string train_route_on_date_id, string passenger_carriage_id, bool inverted_sorting = false);
        Task<QueryResult<List<TicketBooking>>> GetTicketBookingsForTrainRouteOnDate(string train_route_on_date_id, bool inverted_sorting = false);
    }
}