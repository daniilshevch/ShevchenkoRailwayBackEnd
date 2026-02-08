using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface IUserTicketManagementService
    {
        Task<QueryResult<List<ExternalProfileTicketBookingDto>>> GetAllBookedTicketsForCurrentUser();
        Task<QueryResult<ExternalProfileTicketBookingDto>> ReturnTicketBookingForCurrentUserById(string ticket_id);
        Task<QueryResult<List<ExternalTicketBookingGroupDto>>> GetAllBookedTicketsInGroupsForCurrentUser(TicketGroupSearchOptions search_options);
    }
}