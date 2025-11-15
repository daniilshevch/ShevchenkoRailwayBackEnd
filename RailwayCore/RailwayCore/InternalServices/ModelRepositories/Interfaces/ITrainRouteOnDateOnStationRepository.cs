using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface ITrainRouteOnDateOnStationRepository
    {
        Task<TrainRouteOnDateOnStation?> CreateTrainStop(TrainRouteOnDateOnStation input);
        Task<QueryResult<TrainRouteOnDateOnStation>> CreateTrainStop(TrainRouteOnDateOnStationDto input);
        Task<bool> DeleteTrainStop(string train_route_on_date_id, string station_title);
        Task<TrainRouteOnDateOnStation?> FindTrainStop(string train_route_on_date_id, int station_id);
        Task<QueryResult<List<TrainRouteOnDateOnStation>>> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id);
        Task<QueryResult<TrainRouteOnDateOnStation>> UpdateTrainStop(TrainRouteOnDateOnStationDto input);
    }
}