using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces
{
    public interface IEmailTicketSender
    {
        public Task<QueryResult> SendTicketToEmailAsync(string user_email, TicketBooking ticket_booking_info);
    }
}
