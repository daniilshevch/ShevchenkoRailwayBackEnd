using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.Models;
using System;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.SystemServices;
namespace RailwayCore.InternalServices.ModelServices
{
    public class TrainRouteOnDateOnStationRepository
    {
        private AppDbContext context;
        private TrainRouteOnDateRepository train_route_on_date_service;
        private StationRepository station_service;
        public TrainRouteOnDateOnStationRepository(AppDbContext context, TrainRouteOnDateRepository train_route_on_date_service,
            StationRepository station_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.station_service = station_service;
        }
        [Refactored("22.07.2025", "v1")]
        public async Task<QueryResult<TrainRouteOnDateOnStation>> CreateTrainStop(TrainRouteOnDateOnStationDto input)
        {
            TrainRouteOnDateOnStation? already_in_memory = await context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date).Include(train_stop => train_stop.Station)
                .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && train_stop.Station.Title == input.Station_Title);
            if (already_in_memory is not null)
            {
                return new FailQuery<TrainRouteOnDateOnStation>(new Error(ErrorType.BadRequest, $"Station {input.Station_Title} already exists" +
                    $"in schedule of train {input.Train_Route_On_Date_Id}"));
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if (train_route_on_date == null)
            {
                return new FailQuery<TrainRouteOnDateOnStation>(new Error(ErrorType.NotFound, $"Can't find train race with id: {input.Train_Route_On_Date_Id}"));
            }
            Station? station = await station_service.FindStationByTitle(input.Station_Title);
            if (station == null)
            {
                return new FailQuery<TrainRouteOnDateOnStation>(new Error(ErrorType.NotFound, $"Can't find station: {input.Station_Title}"));
            }
            if (input.Arrival_Time is not null && input.Departure_Time is not null && input.Arrival_Time > input.Departure_Time)
            {
                return new FailQuery<TrainRouteOnDateOnStation>(new Error(ErrorType.BadRequest, $"Arrival time can't be after departure time"));
            }
            TrainRouteOnDateOnStation train_stop = new TrainRouteOnDateOnStation()
            {
                Train_Route_On_Date = train_route_on_date,
                Station = station,
                Arrival_Time = input.Arrival_Time,
                Departure_Time = input.Departure_Time,
                Stop_Type = input.Stop_Type,
                Distance_From_Starting_Station = input.Distance_From_Starting_Station,
                Speed_On_Section = input.Speed_On_Section
            };
            context.Train_Routes_On_Date_On_Stations.Add(train_stop);
            await context.SaveChangesAsync();
            return new SuccessQuery<TrainRouteOnDateOnStation>(train_stop);
        }
        [Refactored("22.07.2025", "v1")]
        public async Task<QueryResult<List<TrainRouteOnDateOnStation>>> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Train_Stops)
                .ThenInclude(train_stop => train_stop.Station)
                .Include(train_route_on_date => train_route_on_date.Train_Route)
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == train_route_on_date_id);
            if (train_route_on_date is null)
            {
                return new FailQuery<List<TrainRouteOnDateOnStation>>(new Error(ErrorType.NotFound, $"Can't find train route on date" +
                    $"with id: {train_route_on_date_id}"));
            }
            List<TrainRouteOnDateOnStation> train_stops = train_route_on_date.Train_Stops;
            train_stops = train_stops.OrderBy(train_stop => train_stop.Arrival_Time).ToList();

            return new SuccessQuery<List<TrainRouteOnDateOnStation>>(train_stops);
        }
        [Refactored("22.07.2025", "v1")]
        public async Task<QueryResult<TrainRouteOnDateOnStation>> UpdateTrainStop(TrainRouteOnDateOnStationUpdateDto input)
        {
            TrainRouteOnDateOnStation? existing_train_stop = await context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date).Include(train_stop => train_stop.Station)
                .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && train_stop.Station.Title == input.Station_Title);
            if (existing_train_stop is null)
            {
                return new FailQuery<TrainRouteOnDateOnStation>(new Error(ErrorType.NotFound, $"Can't find this train stop"));
            }
            if(input.Stop_Type is StopType stop_type)
            {
                existing_train_stop.Stop_Type = stop_type;
            }
            if(input.Arrival_Time is not null || (input.Arrival_Time is null && input.Arrival_Time_Change == true))
            {
                existing_train_stop.Arrival_Time = input.Arrival_Time;
            }
            if(input.Departure_Time is not null || (input.Departure_Time is null && input.Departure_Time_Change == true))
            {
                existing_train_stop.Departure_Time = input.Departure_Time;
            }
            if(input.Distance_From_Starting_Station is double distance_from_starting_station)
            {
                existing_train_stop.Distance_From_Starting_Station = distance_from_starting_station;
            }
            if(input.Speed_On_Section is double speed_on_section)
            {
                existing_train_stop.Speed_On_Section = speed_on_section;
            }
            context.Train_Routes_On_Date_On_Stations.Update(existing_train_stop);
            await context.SaveChangesAsync();
            return new SuccessQuery<TrainRouteOnDateOnStation>(existing_train_stop);
        }
        [Refactored("22.07.2025", "v1")]
        public async Task<bool> DeleteTrainStop(string train_route_on_date_id, string station_title)
        {
            TrainRouteOnDateOnStation? existing_train_stop = await context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date).Include(train_stop => train_stop.Station)
                .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_id
                && train_stop.Station.Title == station_title);
            if (existing_train_stop is null)
            {
                return false;
            }
            context.Train_Routes_On_Date_On_Stations.Remove(existing_train_stop);
            await context.SaveChangesAsync();
            return true;    
        }



        public async Task<TrainRouteOnDateOnStation?> CreateTrainStop(TrainRouteOnDateOnStation input)
        {
            TrainRouteOnDateOnStation? already_in_memory = await context.Train_Routes_On_Date_On_Stations
           .Include(train_stop => train_stop.Train_Route_On_Date).Include(train_stop => train_stop.Station)
           .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == input.Train_Route_On_Date.Id
           && train_stop.Station_Id == input.Station.Id);
            if (already_in_memory is not null)
            {
                return already_in_memory;
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date.Id);
            if (train_route_on_date == null)
            {
                return null;
            }
            Station? station = await station_service.FindStationById(input.Station.Id);
            if (station == null)
            {
                return null;
            }
            context.Train_Routes_On_Date_On_Stations.Add(input);
            await context.SaveChangesAsync();
            return input;
        }
        public async Task<TrainRouteOnDateOnStation?> FindTrainStop(string train_route_on_date_id, int station_id)
        {
            TrainRouteOnDateOnStation? train_route_on_date_on_station = await context.Train_Routes_On_Date_On_Stations
                .FirstOrDefaultAsync(train_stop => train_stop.Station_Id == station_id &&
                train_stop.Train_Route_On_Date_Id == train_route_on_date_id);
            if (train_route_on_date_on_station == null)
            {
                return null;
            }
            return train_route_on_date_on_station;
        }

    }
}
