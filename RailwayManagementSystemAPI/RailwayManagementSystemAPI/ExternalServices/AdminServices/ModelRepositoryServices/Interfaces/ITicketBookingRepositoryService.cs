using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces
{
    public interface ITicketBookingRepositoryService
    {
        Task<QueryResult<List<ExternalTicketGroupByEndingStationDto>>> GetGroupedTicketBookingsForCarriageAssignmentOrderedByEndingStation(string train_route_on_date_id, string passenger_carriage_id);
        Task<QueryResult<List<ExternalTicketGroupByStartingStationDto>>> GetGroupedTicketBookingsForCarriageAssignmentOrderedByStartingStation(string train_route_on_date_id, string passenger_carriage_id);
        Task<QueryResult<List<ExternalTicketGroupByEndingStationDto>>> GetGroupedTicketBookingsForTrainRouteOnDateOrderedByEndingStation(string train_route_on_date_id);
        Task<QueryResult<List<ExternalTicketGroupByStartingStationDto>>> GetGroupedTicketBookingsForTrainRouteOnDateOrderedByStartingStation(string train_route_on_date_id);
        Task<QueryResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForCarriageAssignment(string train_route_on_date_id, string passenger_carriage_id);
        Task<QueryResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForTrainRouteOnDate(string train_route_on_date_id);
    }
}