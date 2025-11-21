using RailwayCore.Models;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces
{
    public interface ITicketBookingTimerService
    {
        Task DeleteAllExpiredTickets();
        Task<List<TicketBooking>> GetAllExpiredTicketBookings();
    }
}