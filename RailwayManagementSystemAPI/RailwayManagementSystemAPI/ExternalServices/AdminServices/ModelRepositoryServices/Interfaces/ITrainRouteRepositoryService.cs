using RailwayManagementSystemAPI.ExternalDTO.TrainRouteDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces
{
    public interface ITrainRouteRepositoryService
    {
        Task<QueryResult<ExternalTrainRouteDto>> CreateTrainRoute(string id, ExternalTrainRouteCreateAndUpdateDto input);
        Task<bool> DeleteTrainRouteById(string id);
        Task<ExternalTrainRouteDto?> GetTrainRouteById(string id);
        Task<List<ExternalTrainRouteDto>> GetTrainRoutes();
        Task<QueryResult<ExternalTrainRouteDto>> UpdateTrainRoute(string id, ExternalTrainRouteCreateAndUpdateDto input);
    }
}