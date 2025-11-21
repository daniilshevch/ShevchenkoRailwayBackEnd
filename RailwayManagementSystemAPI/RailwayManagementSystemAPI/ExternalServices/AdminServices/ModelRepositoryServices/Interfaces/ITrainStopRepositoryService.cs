using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.AdminDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces
{
    public interface ITrainStopRepositoryService
    {
        Task<QueryResult<TrainRouteOnDateOnStationDto>> CreateTrainStop(TrainRouteOnDateOnStationDto input);
        Task<bool> DeleteTrainStop(string train_route_on_date_id, string station_title);
        Task<QueryResult<List<TrainRouteOnDateOnStationDto>>> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id);
        Task<QueryResult<TrainRouteOnDateOnStationDto>> UpdateTrainStop(string train_route_on_date_id, string station_title, ExternalTrainStopUpdateDto input);
    }
}