using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.EmailServices.Interfaces
{
    public interface IEmailTicketSender
    {
        Task<QueryResult> SendMultipleTicketBookingsInGroupsToEmail(string user_email, List<ExternalOutputCompletedTicketBookingDto> ticket_bookings_list);
        Task<QueryResult> SendMultipleTicketsToEmail(string user_email, List<ExternalOutputCompletedTicketBookingDto> ticket_bookings_list);
        Task<QueryResult> SendTicketReturnReceiptToEmail(string user_email, ExternalProfileTicketBookingDto ticket_booking_profile_dto);
        public Task<QueryResult> SendTicketToEmailAsync(string user_email, TicketBooking ticket_booking_info);
    }
}
