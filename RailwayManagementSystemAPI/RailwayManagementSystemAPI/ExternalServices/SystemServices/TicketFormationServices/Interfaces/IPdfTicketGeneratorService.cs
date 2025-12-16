using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces
{
    public interface IPdfTicketGeneratorService
    {
        byte[] GenerateTicketPdf(ExternalProfileTicketBookingDto ticket_booking);
        void TranslateTicketIntoUkrainian(ExternalProfileTicketBookingDto ticket_booking_profile_for_pdf);
    }
}