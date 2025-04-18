using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.Models;
using RailwayCore.DTO;
using System;
namespace RailwayCore.Services
{
    public class TrainRouteOnDateOnStationService
    {
        private AppDbContext context;
        private TrainRouteOnDateService train_route_on_date_service;
        private StationService station_service;
        public TrainRouteOnDateOnStationService(AppDbContext context, TrainRouteOnDateService train_route_on_date_service,
            StationService station_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.station_service = station_service;
        }

        private static TextService text_service = new TextService("TrainRouteOnDateOnStationService");
        public async Task<TrainRouteOnDateOnStation?> CreateTrainStop(TrainRouteOnDateOnStationDto input)
        {
            TrainRouteOnDateOnStation? already_in_memory = await context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date).Include(train_stop => train_stop.Station)
                .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && train_stop.Station.Title == input.Station_Title);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform("Train stop with these parameters already exists");
                return already_in_memory;
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if (train_route_on_date == null)
            {
                text_service.FailPostInform($"Fail in TrainRouteOnDateService");
                return null;
            }
            Station? station = await station_service.FindStationByTitle(input.Station_Title);
            if (station == null)
            {
                text_service.FailPostInform($"Fail in StationService");
                return null;
            }
            if (input.Arrival_Time is not null && input.Departure_Time is not null && input.Arrival_Time > input.Departure_Time)
            {
                text_service.FailPostInform($"Arrival time({input.Arrival_Time}) for station {input.Station_Title} is later" +
                   $" than departure time({input.Departure_Time})");
                return null;
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
            text_service.SuccessPostInform($"Train Stop" +
                $" {input.Station_Title}({input.Arrival_Time} - {input.Departure_Time}) for train {train_route_on_date.Id}" +
                $" has been successfully added");
            return train_stop;
        }
        public async Task<TrainRouteOnDateOnStation?> CreateTrainStop(TrainRouteOnDateOnStation input)
        {
            TrainRouteOnDateOnStation? already_in_memory = await context.Train_Routes_On_Date_On_Stations
           .Include(train_stop => train_stop.Train_Route_On_Date).Include(train_stop => train_stop.Station)
           .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == input.Train_Route_On_Date.Id
           && train_stop.Station_Id == input.Station.Id);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform("Train stop with these parameters already exists");
                return already_in_memory;
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date.Id);
            if (train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            Station? station = await station_service.FindStationById(input.Station.Id);
            if (station == null)
            {
                text_service.FailPostInform("Fail in StationService");
                return null;
            }
            context.Train_Routes_On_Date_On_Stations.Add(input);
            await context.SaveChangesAsync();
            text_service.SuccessPostInform($"Train Stop" +
                $" {input.Station.Title}({input.Arrival_Time} - {input.Departure_Time}) for train {input.Train_Route_On_Date_Id}" +
                $" has been successfully added");
            return input;
        }
        public async Task<TrainRouteOnDateOnStation?> FindTrainStop(string train_route_on_date_id, int station_id)
        {
            TrainRouteOnDateOnStation? train_route_on_date_on_station = await context.Train_Routes_On_Date_On_Stations
                .FirstOrDefaultAsync(train_stop => train_stop.Station_Id == station_id &&
                train_stop.Train_Route_On_Date_Id == train_route_on_date_id);
            if (train_route_on_date_on_station == null)
            {
                text_service.FailGetInform("Can't find train stop with these parameters");
                return null;
            }
            return train_route_on_date_on_station;
        }

    }
}
