using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Implementations;
using RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Interfaces;

namespace RailwayCore.InternalServices.CoreServices
{
    public class FullTrainAssignementService
    {
        private readonly AppDbContext context;
        private readonly ITrainRouteOnDateRepository train_route_on_date_service;
        private readonly ITrainRouteOnDateOnStationRepository train_route_on_date_on_station_service;
        private readonly IPassengerCarriageOnTrainRouteOnDateRepository passenger_carriage_on_train_route_on_date_service;
        private readonly IStationRepository station_service;
        private readonly IPassengerCarriageRepository passenger_carriage_service;
        private readonly ITrainSquadCopyService train_squad_copy_service;
        private readonly ITrainScheduleCopyService train_schedule_copy_service;
        public FullTrainAssignementService(AppDbContext context, ITrainRouteOnDateRepository train_route_on_date_service,
            ITrainRouteOnDateOnStationRepository train_route_on_date_on_station_service,
            IPassengerCarriageOnTrainRouteOnDateRepository passenger_carriage_on_train_route_on_date_service,
            IStationRepository station_service, IPassengerCarriageRepository passenger_carriage_service,
            ITrainSquadCopyService train_squad_copy_service, ITrainScheduleCopyService train_schedule_copy_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.train_route_on_date_on_station_service = train_route_on_date_on_station_service;
            this.passenger_carriage_on_train_route_on_date_service = passenger_carriage_on_train_route_on_date_service;
            this.station_service = station_service;
            this.passenger_carriage_service = passenger_carriage_service;
            this.train_squad_copy_service = train_squad_copy_service;
            this.train_schedule_copy_service = train_schedule_copy_service;
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
                    .GetTrainRouteOnDateById(train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date));
            }
            if (train_route_on_date == null)
            {
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
                Station? current_station = await station_service.GetStationByTitle(current_station_dto.Station_Title);
                if (current_station == null)
                {
                    return;
                }
                if (next_station_dto != null)
                {
                    Station? next_station = await station_service.GetStationByTitle(next_station_dto.Station_Title);
                    if (next_station == null)
                    {
                        return;
                    }
                    if (next_station_dto.Arrival_Time < current_station_dto.Departure_Time)
                    {
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
                    .GetTrainRouteOnDateById(train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date));
            }
            if (train_route_on_date == null)
            {
                return;
            }
            foreach (CarriageAssignementWithoutRouteDTO carriage_assignement in carriage_assignements)
            {
                PassengerCarriage? passenger_carriage = await passenger_carriage_service
                    .GetPassengerCarriageById(carriage_assignement.Passenger_Carriage_Id);
                if (passenger_carriage == null)
                {
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


        public async Task<QueryResult> CopyTrainRouteOnDateWithSchedule(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            return await train_schedule_copy_service.CopyTrainRouteOnDateWithSchedule(prototype_train_route_id, new_train_route_id, prototype_date, new_date, creation_option);
        }
        public async Task<QueryResult> CopyTrainRouteOnDateWithSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            return await train_squad_copy_service.CopyTrainRouteOnDateWithSquad(prototype_train_route_id, new_train_route_id, prototype_date, new_date, creation_option);
        }
        public async Task<QueryResult> CopyTrainRouteOnDateWithScheduleAndSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            if (creation_option)
            {
                QueryResult schedule_copy_result = await CopyTrainRouteOnDateWithSchedule(prototype_train_route_id, new_train_route_id, prototype_date, new_date, true);
                QueryResult squad_copy_result = await CopyTrainRouteOnDateWithSquad(prototype_train_route_id, new_train_route_id, prototype_date, new_date, false);
                if(schedule_copy_result.Fail || squad_copy_result.Fail)
                {
                    return new FailQuery(schedule_copy_result.Error); //  Доробити
                }
                return new SuccessQuery();
            }
            else
            {
                QueryResult schedule_copy_result = await CopyTrainRouteOnDateWithSchedule(prototype_train_route_id, new_train_route_id, prototype_date, new_date, false);
                QueryResult squad_copy_result =  await CopyTrainRouteOnDateWithSquad(prototype_train_route_id, new_train_route_id, prototype_date, new_date, false);
                if (schedule_copy_result.Fail || squad_copy_result.Fail)
                {
                    return new FailQuery(schedule_copy_result.Error); //  Доробити
                }
                return new SuccessQuery();
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
