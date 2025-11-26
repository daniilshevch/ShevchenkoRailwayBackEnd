using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces
{
    public interface IEmailTicketSender
    {
        public Task<QueryResult> SendTicketToEmailAsync(string user_email, ExternalOutputCompletedTicketBookingDto ticket_booking_info);
    }
}
