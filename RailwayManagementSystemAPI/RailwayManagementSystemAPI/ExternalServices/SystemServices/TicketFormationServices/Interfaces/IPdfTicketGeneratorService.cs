using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces
{
    public interface IPdfTicketGeneratorService
    {
        byte[] GenerateTicketPdf(ExternalProfileTicketBookingDto ticket_booking);
    }
}