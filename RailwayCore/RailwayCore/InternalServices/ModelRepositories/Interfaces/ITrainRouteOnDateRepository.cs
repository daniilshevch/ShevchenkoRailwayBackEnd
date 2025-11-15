using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface ITrainRouteOnDateRepository
    {
        string BuildTrainRouteOnDateIdentificator(string train_route_id, DateOnly departure_date);
        Task<TrainRouteOnDate?> ChangeTrainRaceCoefficientForTrainRouteOnDate(string train_route_on_date_id, double train_race_coefficient);
        Task<TrainRouteOnDate?> CreateTrainRouteOnDate(string train_route_id, DateOnly departure_date);
        Task<TrainRouteOnDate?> CreateTrainRouteOnDate(TrainRouteOnDateDto input);
        Task<bool> DeleteTrainRouteOnDate(string train_route_on_date_id);
        (string, DateOnly) GetTrainRouteIdAndDateFromTrainRouteOnDate(string train_route_on_date_id);
        Task<TrainRouteOnDate?> GetTrainRouteOnDateById(string id);
        Task<List<TrainRouteOnDate>> GetTrainRoutesOnDateForTrainRoute(string train_route_id);
    }
}