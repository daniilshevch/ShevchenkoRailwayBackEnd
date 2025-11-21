using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface IUserTicketManagementService
    {
        Task<ExternalProfileTicketBookingDto> CreateProfileDtoForTicketBooking(TicketBooking ticket_booking);
        Task<QueryResult<List<ExternalProfileTicketBookingDto>>> GetAllBookedTicketsForCurrentUser();
        Task<QueryResult<List<ExternalTicketBookingGroupDto>>> GetAllBookedTicketsInGroupsForCurrentUser();
        Task<QueryResult<ExternalProfileTicketBookingDto>> ReturnTicketBookingForCurrentUserById(string ticket_id);
    }
}