using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces
{
    public interface ITrainRaceRepositoryService
    {
        Task<ExternalSimpleTrainRaceDto?> ChangeTrainRaceCoefficientForTrainRouteOnDate(string train_route_on_date_id, double train_race_coefficient);
        Task<ExternalSimpleTrainRaceDto?> CreateTrainRouteOnDate(TrainRouteOnDateDto input);
        Task<bool> DeleteTrainRouteOnDate(string train_route_on_date_id);
        Task<List<ExternalSimpleTrainRaceDto>> GetTrainRoutesOnDateForTrainRoute(string train_route_id);
    }
}