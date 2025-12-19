using RailwayCore.Models;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces
{
    public interface ITicketUserManipulationService
    {
        Task<List<TicketBooking>> GetAllArchievedAndReturnedTicketBookingsForUser(int user_id);
        Task<List<TicketBooking>> GetAllTicketBookingsForUser(int user_id, bool only_active = true);
        Task<List<TicketBooking>> GetAllTicketBookingsInProgressForUser(int user_id);
        Task<User?> GetTicketOwner(string ticket_id);
        Task<QueryResult<TicketBooking>> ReturnTicketBookingById(string ticket_id);
    }
}