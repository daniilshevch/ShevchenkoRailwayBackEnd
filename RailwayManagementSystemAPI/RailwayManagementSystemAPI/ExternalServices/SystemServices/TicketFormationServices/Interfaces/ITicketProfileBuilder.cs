using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces
{
    public interface ITicketProfileBuilder
    {
        Task<ExternalProfileTicketBookingDto> CreateProfileDtoForTicketBooking(TicketBooking ticket_booking);
    }
}