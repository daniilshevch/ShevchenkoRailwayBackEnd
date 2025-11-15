using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface ITrainRouteRepository
    {
        static abstract int GetTrainRouteNumericNumber(string train_route_id);
        Task<QueryResult<TrainRoute>> CreateTrainRoute(TrainRouteDto input);
        Task<bool> DeleteTrainRouteById(string id);
        Task<TrainRoute?> GetTrainRouteById(string id);
        Task<List<TrainRoute>> GetTrainRoutes();
        Task<QueryResult<TrainRoute>> UpdateTrainRoute(TrainRouteDto input);
    }
}