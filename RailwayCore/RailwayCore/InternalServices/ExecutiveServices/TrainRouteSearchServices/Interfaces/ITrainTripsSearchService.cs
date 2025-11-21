using RailwayCore.Models;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices.Interfaces
{
    public interface ITrainTripsSearchService
    {
        Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> SearchTrainRoutesBetweenStationsOnDate(string start_station_title, string end_station_title, DateOnly trip_departure_date);
        Task<QueryResult<List<InternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStationOnDate(string station_title, DateTime time, TimeSpan? left_interval = null, TimeSpan? right_interval = null);
        Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> _FilterTrainRoutesBetweenStationsOnDateByPassingOrderAndTransformIntoTrainRaceDto(List<TrainRouteOnDate> possible_train_routes_on_date, string start_station_title, string end_station_title, DateOnly trip_departure_date);
        Task<List<TrainRouteOnDate>> _SearchAllTrainRoutesBetweenStationsOnDateWithNoPassingOrderConsideration(string start_station_title, string end_station_title, DateOnly trip_departure_date);
    }
}