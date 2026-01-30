using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Interfaces;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Implementations
{
    public class TrainScheduleCopyService : ITrainScheduleCopyService
    {
        private readonly AppDbContext context;
        private readonly ITrainRouteOnDateRepository train_route_on_date_service;
        private readonly ITrainRouteOnDateOnStationRepository train_route_on_date_on_station_service;
        private readonly IStationRepository station_service;
        public TrainScheduleCopyService(AppDbContext context,
            ITrainRouteOnDateRepository train_route_on_date_service,
            ITrainRouteOnDateOnStationRepository train_route_on_date_on_station_service,
            IStationRepository station_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.train_route_on_date_on_station_service = train_route_on_date_on_station_service;
            this.station_service = station_service;
        }

        public async Task<QueryResult> CopyTrainRouteOnDateWithSchedule(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            string prototype_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(prototype_train_route_id, prototype_date);
            TrainRouteOnDate? prototype_train_route_on_date = await train_route_on_date_service.GetTrainRouteOnDateById(prototype_train_route_on_date_id);
            if (prototype_train_route_on_date is null)
            {
                return new FailQuery(new Error(ErrorType.NotFound, $"Can't find prototype with ID: {prototype_train_route_on_date_id}"));
            }
            TrainRouteOnDate? new_train_route_on_date = null;
            if (creation_option)
            {
                new_train_route_on_date = await train_route_on_date_service.CreateTrainRouteOnDate(new_train_route_id, new_date);
                if (new_train_route_on_date is null)
                {
                    return new FailQuery(new Error(ErrorType.BadRequest, "Problems while creating train race"));
                }
            }
            else
            {
                string new_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(new_train_route_id, new_date);
                new_train_route_on_date = await train_route_on_date_service
                    .GetTrainRouteOnDateById(new_train_route_on_date_id);

                if (new_train_route_on_date == null)
                {
                    return new FailQuery(new Error(ErrorType.NotFound, $"Can't find train race with ID: {new_train_route_on_date_id}"));
                }
            }

            int days_difference = new_date.DayNumber - prototype_date.DayNumber;
            List<TrainRouteOnDateOnStation> prototype_train_route_on_date_stations = context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Include(train_stop => train_stop.Station)
                .Where(train_stop => train_stop.Train_Route_On_Date_Id == prototype_train_route_on_date_id).ToList();
            List<TrainRouteOnDateOnStation> new_train_route_on_date_stations = new List<TrainRouteOnDateOnStation>();
            foreach (TrainRouteOnDateOnStation old_train_stop in prototype_train_route_on_date_stations)
            {
                DateTime? _old_arrival_time = old_train_stop.Arrival_Time;
                DateTime? _old_departure_time = old_train_stop.Departure_Time;
                DateTime? new_arrival_time;
                DateTime? new_departure_time;
                if (_old_arrival_time is DateTime old_arrival_time)
                {
                    new_arrival_time = old_arrival_time.AddDays(days_difference);
                }
                else
                {
                    new_arrival_time = _old_arrival_time;
                }

                if (_old_departure_time is DateTime old_departure_time)
                {
                    new_departure_time = old_departure_time.AddDays(days_difference);
                }
                else
                {
                    new_departure_time = _old_departure_time;
                }
                TrainRouteOnDateOnStation new_train_stop = new TrainRouteOnDateOnStation
                {
                    Train_Route_On_Date = new_train_route_on_date,
                    Station = old_train_stop.Station,
                    Arrival_Time = new_arrival_time,
                    Departure_Time = new_departure_time,
                    Stop_Type = old_train_stop.Stop_Type,
                    Distance_From_Starting_Station = old_train_stop.Distance_From_Starting_Station,
                    Speed_On_Section = old_train_stop.Speed_On_Section
                };
                await train_route_on_date_on_station_service.CreateTrainStop(new_train_stop);
            }
            await context.SaveChangesAsync();
            return new SuccessQuery();
        }
        public async Task<QueryResult> CopyTrainRouteOnDateWithInvertedSchedule(string prototype_train_route_id, string new_inverted_train_route_id, DateOnly prototype_date, DateTime new_date_and_departure_time,  bool creation_option = true)
        {
            string prototype_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(prototype_train_route_id, prototype_date);
            TrainRouteOnDate? prototype_train_route_on_date = await train_route_on_date_service.GetTrainRouteOnDateById(prototype_train_route_on_date_id);
            if (prototype_train_route_on_date is null)
            {
                return new FailQuery(new Error(ErrorType.NotFound, $"Can't find prototype with ID: {prototype_train_route_on_date_id}"));
            }
            TrainRouteOnDate? new_train_route_on_date = null;
            if (creation_option)
            {
                new_train_route_on_date = await train_route_on_date_service.CreateTrainRouteOnDate(new_inverted_train_route_id, DateOnly.FromDateTime(new_date_and_departure_time));
                if (new_train_route_on_date is null)
                {
                    return new FailQuery(new Error(ErrorType.BadRequest, "Problems while creating train race"));
                }
            }
            else
            {
                string new_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(new_inverted_train_route_id, DateOnly.FromDateTime(new_date_and_departure_time));
                new_train_route_on_date = await train_route_on_date_service
                    .GetTrainRouteOnDateById(new_train_route_on_date_id);
                if (new_train_route_on_date == null)
                {
                    return new FailQuery(new Error(ErrorType.NotFound, $"Can't find train race with ID: {new_train_route_on_date_id}"));
                }
            }
            List<TrainRouteOnDateOnStation> prototype_train_route_on_date_stations = context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Include(train_stop => train_stop.Station)
                .Where(train_stop => train_stop.Train_Route_On_Date_Id == prototype_train_route_on_date_id)
                .OrderBy(train_stop => train_stop.Arrival_Time)
                .ToList();
            int stops_amount = prototype_train_route_on_date_stations.Count;
            int sections_amount = stops_amount - 1;

            List<TrainRouteOnDateOnStation> new_train_route_on_date_stations = new List<TrainRouteOnDateOnStation>();

            List<TimeSpan> time_differences_between_stations = new List<TimeSpan>();
            List<double?> distances_between_stations = new List<double?>();
            for(int old_station_index = 1; old_station_index < stops_amount; old_station_index++)
            {
                TrainRouteOnDateOnStation current_stop = prototype_train_route_on_date_stations[old_station_index];
                TrainRouteOnDateOnStation previous_stop = prototype_train_route_on_date_stations[old_station_index - 1];
                TimeSpan time_difference = (DateTime)current_stop.Arrival_Time! - (DateTime)previous_stop.Departure_Time!;
                double? distance_difference = current_stop.Distance_From_Starting_Station - previous_stop.Distance_From_Starting_Station;
                time_differences_between_stations.Add(time_difference);
                distances_between_stations.Add(distance_difference);
            }

            TrainRouteOnDateOnStation current_prototype_stop = prototype_train_route_on_date_stations[stops_amount - 1];
            DateTime? current_stop_departure_time = new_date_and_departure_time;
            DateTime? current_stop_arrival_time = null;
            double current_stop_distance_from_starting_station = 0;
           
            for(int old_train_stop_index = stops_amount - 1; old_train_stop_index >= 0; old_train_stop_index--)
            {
                TrainRouteOnDateOnStation prototype_train_stop = prototype_train_route_on_date_stations[old_train_stop_index];
                TimeSpan? prototype_train_stop_duration = prototype_train_stop.Departure_Time - prototype_train_stop.Arrival_Time;
                if (old_train_stop_index != stops_amount - 1)
                {
                    current_stop_departure_time = current_stop_arrival_time + prototype_train_stop_duration;
                }
                if(old_train_stop_index == 0)
                {
                    current_stop_departure_time = null;
                }
                double? current_speed_on_section = null;
                if (old_train_stop_index != 0)
                {
                    current_speed_on_section = (double)distances_between_stations[old_train_stop_index - 1]! / time_differences_between_stations[old_train_stop_index - 1].TotalHours;
                }
                TrainRouteOnDateOnStation new_train_stop = new TrainRouteOnDateOnStation
                {
                    Train_Route_On_Date = new_train_route_on_date,
                    Station = prototype_train_stop.Station,
                    Arrival_Time = current_stop_arrival_time,
                    Departure_Time = current_stop_departure_time,
                    Stop_Type = prototype_train_stop.Stop_Type,
                    Distance_From_Starting_Station = current_stop_distance_from_starting_station,
                    Speed_On_Section = current_speed_on_section
                };
                new_train_route_on_date_stations.Add(new_train_stop);
                if (old_train_stop_index != 0)
                {
                    current_stop_arrival_time = current_stop_departure_time + time_differences_between_stations[old_train_stop_index - 1];
                    current_stop_distance_from_starting_station += (double)distances_between_stations[old_train_stop_index - 1]!;
                }
                await train_route_on_date_on_station_service.CreateTrainStop(new_train_stop);
            }
            await context.SaveChangesAsync();
            return new SuccessQuery();
        }
    }
}
