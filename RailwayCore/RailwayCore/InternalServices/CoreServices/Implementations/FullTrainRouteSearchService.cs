using RailwayCore.Models;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices.Implementations;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices.Interfaces;
using RailwayCore.InternalServices.CoreServices.Interfaces;

namespace RailwayCore.InternalServices.CoreServices.Implementations
{
    /// <summary>
    /// Цей сервіс є оркестратором всіх функцій, пов'язаних з пошуком рейсів поїздів, їх розкладу руху, а також їх складу
    /// (тобто вагонів), однак він не виконує функцій, пов'язаних з пошуком квитків. Крім того,цей сервіс є надбудовою над трьома
    /// іншими, кожен з яких окремо відповідає за пошук рейсів, визначення розкладу руху поїздів та визначення рухомого складу поїздів
    /// </summary>
    [CoreService]
    public class FullTrainRouteSearchService : IFullTrainRouteSearchService
    {
        private readonly ITrainTripsSearchService train_trips_search_service;
        private readonly ITrainScheduleSearchService train_schedule_search_service;
        private readonly ITrainSquadSearchService train_squad_search_service;
        public FullTrainRouteSearchService(ITrainTripsSearchService train_trips_search_service,
            ITrainScheduleSearchService train_schedule_search_service,
            ITrainSquadSearchService train_squad_search_service)
        {
            this.train_trips_search_service = train_trips_search_service;
            this.train_schedule_search_service = train_schedule_search_service;
            this.train_squad_search_service = train_squad_search_service;
        }

        //TrainTripsSearchService
        [CoreMethod]
        public async Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> SearchTrainRoutesBetweenStationsOnDate(string start_station_title, string end_station_title, DateOnly trip_departure_date)
        {
            return await train_trips_search_service.SearchTrainRoutesBetweenStationsOnDate(start_station_title, end_station_title, trip_departure_date);
        }
        [CoreMethod]
        public async Task<QueryResult<List<InternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStationOnDate(string station_title, DateTime time,
    TimeSpan? left_interval = null, TimeSpan? right_interval = null)
        {
            return await train_trips_search_service.SearchTrainRoutesThroughStationOnDate(station_title, time, left_interval, right_interval);
        }

        //TrainScheduleSearchService
        [CoreMethod]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id, bool order_mode = true)
        {
            return await train_schedule_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode);
        }
        [CoreMethod]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            return await train_schedule_search_service.GetTrainStopsForTrainRouteOnDate(train_route_id, departure_date);
        }
        [CoreMethod]
        public async Task<List<TrainRouteOnDateOnStation>> GetTrainStopsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            return await train_schedule_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids);
        }
        [CoreMethod]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsBetweenTwoStationsForTrainRouteOnDate(string train_route_on_date_id, string first_station_title, string second_station_title)
        {
            return await train_schedule_search_service.GetTrainStopsBetweenTwoStationsForTrainRouteOnDate(train_route_on_date_id, first_station_title, second_station_title);
        }
        [CoreMethod]
        public async Task<TrainRouteOnDateOnStation?> GetStartingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            return await train_schedule_search_service.GetStartingTrainStopForTrainRouteOnDate(train_route_on_date_id);
        }
        [CoreMethod]
        public async Task<TrainRouteOnDateOnStation?> GetEndingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            return await train_schedule_search_service.GetEndingTrainStopForTrainRouteOnDate(train_route_on_date_id);
        }
        [CoreMethod]
        public async Task<TrainRouteOnDateOnStation?> GetTrainStopInfoByTrainRouteOnDateIdAndStationId(string train_route_on_date_id, int station_id)
        {
            return await train_schedule_search_service.GetTrainStopInfoByTrainRouteOnDateIdAndStationId(train_route_on_date_id, station_id);
        }
        //TrainSquadSearchService
        [CoreMethod]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_on_date_id)
        {
            return await train_squad_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_id);
        }
        [CoreMethod]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            return await train_squad_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_id, departure_date);
        }
        [CoreMethod]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>> GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            return await train_squad_search_service.GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(train_route_on_date_ids);
        }
    }
}







