using RailwayCore.Models;

namespace RailwayCore.InternalServices.CoreServices.Interfaces
{
    public interface IFullTrainRouteSearchService
    {
        Task<TrainRouteOnDateOnStation?> GetEndingTrainStopForTrainRouteOnDate(string train_route_on_date_id);
        Task<List<PassengerCarriageOnTrainRouteOnDate>> GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids);
        Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_on_date_id);
        Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_id, DateOnly departure_date);
        Task<TrainRouteOnDateOnStation?> GetStartingTrainStopForTrainRouteOnDate(string train_route_on_date_id);
        Task<TrainRouteOnDateOnStation?> GetTrainStopInfoByTrainRouteOnDateIdAndStationId(string train_route_on_date_id, int station_id);
        Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsBetweenTwoStationsForTrainRouteOnDate(string train_route_on_date_id, string first_station_title, string second_station_title);
        Task<List<TrainRouteOnDateOnStation>> GetTrainStopsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids);
        Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id, bool order_mode = true);
        Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_id, DateOnly departure_date);
        Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> SearchTrainRoutesBetweenStationsOnDate(string start_station_title, string end_station_title, DateOnly trip_departure_date);
        Task<QueryResult<List<InternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStationOnDate(string station_title, DateTime time, TimeSpan? left_interval = null, TimeSpan? right_interval = null);
        Task<QueryResult<InternalTrainRaceBetweenStationsDto>> TransformTrainRouteOnDateIntoTrainRaceDto(string train_route_on_date_id, string start_station_title, string end_station_title);
    }
}