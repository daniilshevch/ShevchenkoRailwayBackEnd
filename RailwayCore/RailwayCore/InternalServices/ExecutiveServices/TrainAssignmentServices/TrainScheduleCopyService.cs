using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ExecutiveServices
{
    public class TrainScheduleCopyService
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateRepository train_route_on_date_service;
        private readonly TrainRouteOnDateOnStationRepository train_route_on_date_on_station_service;
        private readonly StationRepository station_service;
        public TrainScheduleCopyService(AppDbContext context, TrainRouteOnDateRepository train_route_on_date_service, TrainRouteOnDateOnStationRepository train_route_on_date_on_station_service, StationRepository station_service)
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
                //??
                /*
                if(await train_route_on_date_on_station_service.FindTrainStop(new_train_route_on_date.Id, old_train_stop.Station.Id) is not null)
                {
                    text_service.DuplicateGetInform("Train stop with these parameters already exists");
                    return;
                }
                //??*/
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
    }
}
