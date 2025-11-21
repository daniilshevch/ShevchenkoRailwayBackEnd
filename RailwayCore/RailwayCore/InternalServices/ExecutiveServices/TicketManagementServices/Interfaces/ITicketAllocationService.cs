using RailwayCore.InternalServices.ExecutiveServices.ExecutiveDTO.TicketManagementDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces
{
    public interface ITicketAllocationService
    {
        Task<QueryResult<TicketBooking>> CreateTicketBooking(InternalTicketBookingDto input);
        Task<QueryResult<TicketBooking>> CreateTicketBookingWithCarriagePositionInSquad(InternalTicketBookingWithCarriagePositionDto input);
    }
}