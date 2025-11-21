using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces
{
    public interface ICarriageAssignmentRepositoryService
    {
        Task<QueryResult<ExternalCarriageAssignmentDto>> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input);
        Task<bool> DeletePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id, string train_route_on_date_id);
        Task<QueryResult<List<ExternalCarriageAssignmentDto>>> GetPassengerCarriagesForTrainRouteOnDate(string train_route_on_date_id);
        Task<QueryResult<ExternalCarriageAssignmentDto>> UpdatePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id, string train_route_on_date_id, ExternalCarriageAssignmentUpdateDto input);
    }
}