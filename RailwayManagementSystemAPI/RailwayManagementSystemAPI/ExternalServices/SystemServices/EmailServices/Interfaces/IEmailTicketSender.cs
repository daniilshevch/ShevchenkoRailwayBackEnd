using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces
{
    public interface IEmailTicketSender
    {
        Task<QueryResult> SendMultipleTicketsToEmail(string user_email, List<ExternalOutputCompletedTicketBookingDto> ticket_bookings_list);
        public Task<QueryResult> SendTicketToEmailAsync(string user_email, TicketBooking ticket_booking_info);
    }
}
