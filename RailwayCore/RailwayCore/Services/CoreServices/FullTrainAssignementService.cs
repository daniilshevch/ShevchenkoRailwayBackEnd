using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.DTO;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Services
{
    public class FullTrainAssignementService
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateService train_route_on_date_service;
        private readonly TrainRouteOnDateOnStationService train_route_on_date_on_station_service;
        private readonly PassengerCarriageOnTrainRouteOnDateService passenger_carriage_on_train_route_on_date_service;
        private readonly StationService station_service;
        private readonly PassengerCarriageService passenger_carriage_service;
        private static TextService text_service = new TextService("FullTrainAssignementService");
        public FullTrainAssignementService(AppDbContext context, TrainRouteOnDateService train_route_on_date_service,
            TrainRouteOnDateOnStationService train_route_on_date_on_station_service,
            PassengerCarriageOnTrainRouteOnDateService passenger_carriage_on_train_route_on_date_service,
            StationService station_service, PassengerCarriageService passenger_carriage_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.train_route_on_date_on_station_service = train_route_on_date_on_station_service;
            this.passenger_carriage_on_train_route_on_date_service = passenger_carriage_on_train_route_on_date_service;
            this.station_service = station_service;
            this.passenger_carriage_service = passenger_carriage_service;
        }
        public async Task CreateTrainRouteOnDateWithSchedule(string train_route_id, DateOnly departure_date,
     List<TrainStopWithoutRouteDto> train_stops, bool creation_option = true)
        {
            TrainRouteOnDate? train_route_on_date;
            if (creation_option)
            {
                train_route_on_date = await train_route_on_date_service.CreateTrainRouteOnDate(train_route_id, departure_date);
            }
            else
            {
                train_route_on_date = await train_route_on_date_service
                    .FindTrainRouteOnDateById(train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date));
            }
            if (train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return;
            }
            for (int i = 0; i < train_stops.Count(); i++)
            {
                TrainStopWithoutRouteDto current_station_dto = train_stops[i];
                TrainStopWithoutRouteDto? next_station_dto;
                if (i < train_stops.Count() - 1)
                {
                    next_station_dto = train_stops[i + 1];
                }
                else
                {
                    next_station_dto = null;
                }
                Station? current_station = await station_service.FindStationByTitle(current_station_dto.Station_Title);
                if (current_station == null)
                {
                    text_service.FailPostInform("Fail in StationService");
                    return;
                }
                if (next_station_dto != null)
                {
                    Station? next_station = await station_service.FindStationByTitle(next_station_dto.Station_Title);
                    if (next_station == null)
                    {
                        text_service.FailPostInform("Fail in StationService");
                        return;
                    }
                    if (next_station_dto.Arrival_Time < current_station_dto.Departure_Time)
                    {
                        text_service.FailPostInform("Incorrect time for stations");
                    }
                }
                TrainRouteOnDateOnStation train_stop = new TrainRouteOnDateOnStation
                {
                    Train_Route_On_Date = train_route_on_date,
                    Station = current_station,
                    Arrival_Time = current_station_dto.Arrival_Time,
                    Departure_Time = current_station_dto.Departure_Time,
                    Stop_Type = current_station_dto.Stop_Type,
                    Distance_From_Starting_Station = current_station_dto.Distance_From_Starting_Station,
                    Speed_On_Section = FindSpeedOnSection(current_station_dto.Distance_From_Starting_Station,
                    next_station_dto?.Distance_From_Starting_Station, current_station_dto.Departure_Time,
                    next_station_dto?.Arrival_Time)
                };
                await train_route_on_date_on_station_service.CreateTrainStop(train_stop);
            }
            await context.SaveChangesAsync();
        }
        public async Task CreateTrainRouteOnDateWithSquad(string train_route_id, DateOnly departure_date,
            List<CarriageAssignementWithoutRouteDTO> carriage_assignements, bool creation_option = true)
        {
            TrainRouteOnDate? train_route_on_date;
            if (creation_option)
            {
                train_route_on_date = await train_route_on_date_service.CreateTrainRouteOnDate(train_route_id, departure_date);
            }
            else
            {
                train_route_on_date = await train_route_on_date_service
                    .FindTrainRouteOnDateById(train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date));
            }
            if (train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return;
            }
            foreach (CarriageAssignementWithoutRouteDTO carriage_assignement in carriage_assignements)
            {
                PassengerCarriage? passenger_carriage = await passenger_carriage_service
                    .FindPassengerCarriageById(carriage_assignement.Passenger_Carriage_Id);
                if (passenger_carriage == null)
                {
                    text_service.FailPostInform("Fail in PassengerCarriageService");
                    return;
                }
                PassengerCarriageOnTrainRouteOnDate final_carriage_assignement = new PassengerCarriageOnTrainRouteOnDate
                {
                    Train_Route_On_Date = train_route_on_date,
                    Passenger_Carriage = passenger_carriage,
                    Position_In_Squad = carriage_assignement.Position_In_Squad,
                    Is_For_Children = carriage_assignement.Is_For_Children,
                    Is_For_Woman = carriage_assignement.Is_For_Woman,
                    Factual_Air_Conditioning = carriage_assignement.Factual_Air_Conditioning,
                    Factual_Shower_Availability = carriage_assignement.Factual_Shower_Availability,
                    Factual_Is_Inclusive = carriage_assignement.Factual_Is_Inclusive,
                    Food_Availability = carriage_assignement.Food_Availability
                };
                await passenger_carriage_on_train_route_on_date_service.CreatePassengerCarriageOnTrainRouteOnDate(final_carriage_assignement);
            }
            await context.SaveChangesAsync();
        }
        public async Task CreateTrainRouteOnDateWithScheduleAndSquad(string train_route_id, DateOnly departure_date,
             List<TrainStopWithoutRouteDto> train_stops, List<CarriageAssignementWithoutRouteDTO> carriage_assignements,
             bool creation_option = true)

        {
            if (creation_option)
            {
                await CreateTrainRouteOnDateWithSchedule(train_route_id, departure_date, train_stops, true);
                await CreateTrainRouteOnDateWithSquad(train_route_id, departure_date, carriage_assignements, false);
            }
            else
            {
                await CreateTrainRouteOnDateWithSchedule(train_route_id, departure_date, train_stops, false);
                await CreateTrainRouteOnDateWithSquad(train_route_id, departure_date, carriage_assignements, false);
            }
        }


        public async Task CopyTrainRouteOnDateWithSchedule(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            string prototype_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, prototype_date);
            TrainRouteOnDate? prototype_train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(prototype_train_route_on_date_id);
            if (prototype_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return;
            }
            TrainRouteOnDate? new_train_route_on_date = null;
            if (creation_option)
            {
                new_train_route_on_date = await train_route_on_date_service.CreateTrainRouteOnDate(train_route_id, new_date);
                if (new_train_route_on_date == null)
                {
                    text_service.FailPostInform("Fail in TrainRouteOnDateService");
                    return;
                }
            }
            else
            {
                new_train_route_on_date = await train_route_on_date_service
                    .FindTrainRouteOnDateById(train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, new_date));
                if (new_train_route_on_date == null)
                {
                    text_service.FailPostInform("Fail in TrainRouteOnDateService");
                    return;
                }
            }
            int days_difference = new_date.DayNumber - prototype_date.DayNumber;
            List<TrainRouteOnDateOnStation> prototype_train_route_on_date_stations = context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Include(train_stop => train_stop.Station)
                .Where(train_stop => train_stop.Train_Route_On_Date_Id == prototype_train_route_on_date_id).ToList(); //REDUNDANT
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
        }
        public async Task CopyTrainRouteOnDateWithSquad(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            string prototype_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, prototype_date);
            TrainRouteOnDate? prototype_train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(prototype_train_route_on_date_id);
            if (prototype_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return;
            }
            TrainRouteOnDate? new_train_route_on_date = null;
            if (creation_option)
            {
                new_train_route_on_date = await train_route_on_date_service.CreateTrainRouteOnDate(train_route_id, new_date);
                if (new_train_route_on_date == null)
                {
                    text_service.FailPostInform("Fail in TrainRouteOnDateService");
                    return;
                }
            }
            else
            {
                new_train_route_on_date = await train_route_on_date_service
                    .FindTrainRouteOnDateById(train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, new_date));
                if (new_train_route_on_date == null)
                {
                    text_service.FailPostInform("Fail in TrainRouteOnDateService");
                    return;
                }
            }
            List<PassengerCarriageOnTrainRouteOnDate> passenger_carriages_on_prototype_train_route_on_date_on_station =
                context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignement => carriage_assignement.Train_Route_On_Date)
                .Include(carriage_assignement => carriage_assignement.Passenger_Carriage)
                .Where(carriage_assignement =>
                carriage_assignement.Train_Route_On_Date_Id == prototype_train_route_on_date_id).ToList();
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignement in passenger_carriages_on_prototype_train_route_on_date_on_station)
            {
                PassengerCarriageOnTrainRouteOnDate new_carriage_assignement = new PassengerCarriageOnTrainRouteOnDate
                {
                    Train_Route_On_Date = new_train_route_on_date,
                    Passenger_Carriage = carriage_assignement.Passenger_Carriage,
                    Position_In_Squad = carriage_assignement.Position_In_Squad,
                    Is_For_Children = carriage_assignement.Is_For_Children,
                    Is_For_Woman = carriage_assignement.Is_For_Woman,
                    Factual_Air_Conditioning = carriage_assignement.Factual_Air_Conditioning,
                    Factual_Shower_Availability = carriage_assignement.Factual_Shower_Availability,
                    Factual_Is_Inclusive = carriage_assignement.Factual_Is_Inclusive,
                    Food_Availability = carriage_assignement.Food_Availability
                };
                await passenger_carriage_on_train_route_on_date_service.CreatePassengerCarriageOnTrainRouteOnDate(new_carriage_assignement);
            }
            await context.SaveChangesAsync();
        }
        public async Task CopyTrainRouteOnDateWithScheduleAndSquad(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            if (creation_option)
            {
                await CopyTrainRouteOnDateWithSchedule(train_route_id, prototype_date, new_date, true);
                await CopyTrainRouteOnDateWithSquad(train_route_id, prototype_date, new_date, false);
            }
            else
            {
                await CopyTrainRouteOnDateWithSchedule(train_route_id, prototype_date, new_date, false);
                await CopyTrainRouteOnDateWithSquad(train_route_id, prototype_date, new_date, false);
            }

        }

        public async Task ChangeTrainRouteOnDateSchedule(string train_route_id, DateOnly departure_date, List<TrainStopWithoutRouteDto> train_stops, bool deletion_option = true)
        {
            string train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date);
            if (deletion_option)
            {
                List<TrainRouteOnDateOnStation> current_train_stops = await context.Train_Routes_On_Date_On_Stations
                    .Where(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_id).ToListAsync();
                foreach (TrainRouteOnDateOnStation train_stop in current_train_stops)
                {
                    context.Train_Routes_On_Date_On_Stations.Remove(train_stop);
                }
            }
            await CreateTrainRouteOnDateWithSchedule(train_route_id, departure_date, train_stops, creation_option: false);
        }
        public async Task ChangeTrainRouteOnDateSquad(string train_route_id, DateOnly departure_date, List<CarriageAssignementWithoutRouteDTO> carriage_assignments, bool deletion_option = true)
        {
            string train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date);
            if (deletion_option)
            {
                List<PassengerCarriageOnTrainRouteOnDate> current_passenger_carriages = await context.Passenger_Carriages_On_Train_Routes_On_Date
                    .Where(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id == train_route_on_date_id).ToListAsync();
                foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in current_passenger_carriages)
                {
                    context.Passenger_Carriages_On_Train_Routes_On_Date.Remove(carriage_assignment);
                }
                await CreateTrainRouteOnDateWithSquad(train_route_id, departure_date, carriage_assignments, creation_option: false);
            }
        }
        public static double? FindSpeedOnSection(double? _starting_point, double? _ending_point, DateTime? _starting_time, DateTime? _ending_time)
        {
            if (_starting_point is double starting_point && _ending_point is double ending_point
                && _starting_time is DateTime starting_time && _ending_time is DateTime ending_time)
            {
                double distance = ending_point - starting_point;
                TimeSpan time_on_sector = ending_time - starting_time;
                double hours = time_on_sector.TotalHours;
                return distance / hours;
            }
            return null;
        }


    }
}
