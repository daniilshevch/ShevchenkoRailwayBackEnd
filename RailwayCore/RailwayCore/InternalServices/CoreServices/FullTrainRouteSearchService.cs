using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices;


namespace RailwayCore.InternalServices.CoreServices
{
    public class FullTrainRouteSearchService
    {
        private readonly TrainTripsSearchService train_trips_search_service;
        private readonly TrainScheduleSearchService train_schedule_search_service;
        private readonly TrainSquadSearchService train_squad_search_service;
        public FullTrainRouteSearchService(TrainTripsSearchService train_trips_search_service, 
            TrainScheduleSearchService train_schedule_search_service,
            TrainSquadSearchService train_squad_search_service)
        {
            this.train_trips_search_service = train_trips_search_service;
            this.train_schedule_search_service = train_schedule_search_service;
            this.train_squad_search_service= train_squad_search_service;
        }

        public async Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> SearchTrainRoutesBetweenStationsOnDate(string start_station_title, string end_station_title, DateOnly trip_departure_date)
        {
            return await train_trips_search_service.SearchTrainRoutesBetweenStationsOnDate(start_station_title, end_station_title, trip_departure_date);   
        }
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id, bool order_mode = true)
        {
            return await train_schedule_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode);
        }

        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            return await train_schedule_search_service.GetTrainStopsForTrainRouteOnDate(train_route_id, departure_date);
        }

        public async Task<List<TrainRouteOnDateOnStation>> GetTrainStopsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            return await train_schedule_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids);
        }
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsBetweenTwoStationsForTrainRouteOnDate(string train_route_on_date_id, string first_station_title, string second_station_title)
        {
            return await train_schedule_search_service.GetTrainStopsBetweenTwoStationsForTrainRouteOnDate(train_route_on_date_id, first_station_title, second_station_title);
        }


        public async Task<TrainRouteOnDateOnStation?> GetStartingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            return await train_schedule_search_service.GetStartingTrainStopForTrainRouteOnDate(train_route_on_date_id);
        }
        public async Task<TrainRouteOnDateOnStation?> GetEndingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            return await train_schedule_search_service.GetEndingTrainStopForTrainRouteOnDate(train_route_on_date_id);
        }
        public async Task<TrainRouteOnDateOnStation?> GetTrainStopInfoByTrainRouteOnDateIdAndStationId(string train_route_on_date_id, int station_id)
        {
            return await train_schedule_search_service.GetTrainStopInfoByTrainRouteOnDateIdAndStationId(train_route_on_date_id, station_id);
        }
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_on_date_id)
        {
            return await train_squad_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_id);
        }
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            return await train_squad_search_service.GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_id, departure_date);
        }

        public async Task<List<PassengerCarriageOnTrainRouteOnDate>> GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            return await train_squad_search_service.GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(train_route_on_date_ids);
        }

        public async Task<QueryResult<List<InternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStationOnDate(string station_title, DateTime time,
            TimeSpan? left_interval = null, TimeSpan? right_interval = null)
        {
            return await train_trips_search_service.SearchTrainRoutesThroughStationOnDate(station_title, time, left_interval, right_interval);
        }















        //public async Task<List<TrainRouteOnDateArrivalDepartureTimeDto>?> SearchTrainRouteBetweenStationOnDate(string start_station_title, string end_station_title,
        //    DateOnly departure_date)
        //{
        //    Station? start_station = await station_service.FindStationByTitle(start_station_title);
        //    Station? end_station = await station_service.FindStationByTitle(end_station_title);
        //    if (start_station == null || end_station == null)
        //    {
        //        text_service.FailPostInform("Fail in StationService");
        //        return null;
        //    }

        //    List<TrainRouteOnDate> possible_train_routes_on_date = await context.Train_Routes_On_Date
        //        .Include(train_route_on_date => train_route_on_date.Train_Route)
        //        .Include(train_route_on_date => train_route_on_date.Train_Stops)
        //        .ThenInclude(train_stop => train_stop.Station)
        //        .Where(train_route_on_date => train_route_on_date.Train_Stops.Where(train_stop => train_stop.Departure_Time.HasValue)
        //        .Any(train_stop => DateOnly.FromDateTime((DateTime)train_stop.Departure_Time!) == departure_date
        //        && train_stop.Station.Title == start_station_title)
        //        && train_route_on_date.Train_Stops
        //        .Any(train_stop => train_stop.Station.Title == end_station_title)).ToListAsync();

        //    List<TrainRouteOnDateArrivalDepartureTimeDto> final_train_routes_on_date =
        //        new List<TrainRouteOnDateArrivalDepartureTimeDto>();
        //    foreach (TrainRouteOnDate train_route_on_date in possible_train_routes_on_date)
        //    {
        //        List<TrainRouteOnDateOnStation> train_stops_for_train_route_on_date = train_route_on_date
        //            .Train_Stops.OrderBy(train_stop => train_stop.Arrival_Time).ToList();
        //        int full_route_stops_count = train_stops_for_train_route_on_date.Count;
        //        TrainRouteOnDateOnStation? full_route_starting_stop = train_stops_for_train_route_on_date[0];
        //        TrainRouteOnDateOnStation? full_route_ending_stop = train_stops_for_train_route_on_date[full_route_stops_count - 1];

        //        TrainRouteOnDateOnStation? desired_start_station = train_stops_for_train_route_on_date.FirstOrDefault(train_stop =>
        //        train_stop.Station.Title == start_station_title);
        //        TrainRouteOnDateOnStation? desired_end_station = train_stops_for_train_route_on_date.FirstOrDefault(train_stop =>
        //        train_stop.Station.Title == end_station_title);
        //        if (desired_start_station is null || desired_end_station is null)
        //        {
        //            Console.WriteLine("Can't find one of the stations");
        //            return null;
        //        }
        //        DateTime? _desired_start_station_departure_time = desired_start_station.Departure_Time;
        //        DateTime? _desired_end_station_arrival_time = desired_end_station.Arrival_Time;
        //        if (_desired_start_station_departure_time == null || _desired_end_station_arrival_time == null)
        //        {
        //            continue;
        //        }
        //        DateTime desired_start_station_departure_time = (DateTime)_desired_start_station_departure_time;
        //        DateTime desired_end_station_arrival_time = (DateTime)_desired_end_station_arrival_time;

        //        if (desired_start_station.Departure_Time is null || desired_end_station.Arrival_Time is null)
        //        {
        //            continue;
        //        }
        //        if (desired_start_station.Departure_Time < desired_end_station.Arrival_Time)
        //        {
        //            final_train_routes_on_date.Add(new TrainRouteOnDateArrivalDepartureTimeDto
        //            {
        //                Train_Route_On_Date = train_route_on_date,
        //                Departure_Time_From_Desired_Starting_Station = desired_start_station_departure_time,
        //                Arrival_Time_For_Desired_Ending_Station = desired_end_station_arrival_time,
        //                Route_Starting_Stop = full_route_starting_stop,
        //                Route_Ending_Stop = full_route_ending_stop,
        //                Full_Route_Stops_List = train_stops_for_train_route_on_date,
        //                Km_Point_Of_Desired_Starting_Station = desired_start_station.Distance_From_Starting_Station,
        //                Km_Point_Of_Desired_Ending_Station = desired_end_station.Distance_From_Starting_Station

        //            });
        //        }

        //    }
        //    return final_train_routes_on_date.OrderBy(train_route => train_route.Departure_Time_From_Desired_Starting_Station).ToList();
        //}
    }
}







