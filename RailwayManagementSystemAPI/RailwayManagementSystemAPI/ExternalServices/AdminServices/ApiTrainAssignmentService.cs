using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using RailwayCore.InternalServices.CoreServices;
using RailwayManagementSystemAPI.API_DTO;
namespace RailwayManagementSystemAPI.ExternalServices.AdminServices
{
    public class ApiTrainAssignmentService
    {
        private readonly FullTrainAssignementService full_train_assignment_service;
        public ApiTrainAssignmentService(FullTrainAssignementService full_train_assignment_service)
        {
            this.full_train_assignment_service = full_train_assignment_service;
        }
        public async Task AssignTrainRouteOnDateWithSchedule(TrainRouteWithScheduleAssignmentDto input)
        {
            string train_route_id = input.Train_Route_Id;
            DateOnly departure_date = input.Departure_Date;
            bool creation_option = input.Creation_Option;
            List<TrainStopWithArrivalAndDepartureTimeDto> train_stops = input.Train_Stops;

            List<TrainStopWithoutRouteDto> train_stops_list = new List<TrainStopWithoutRouteDto>();
            foreach (TrainStopWithArrivalAndDepartureTimeDto train_stop in train_stops)
            {
                train_stops_list.Add(new TrainStopWithoutRouteDto
                {
                    Station_Title = train_stop.Station_Title,
                    Arrival_Time = train_stop.Arrival_Time,
                    Departure_Time = train_stop.Departure_Time,
                    Distance_From_Starting_Station = train_stop.Distance_From_Starting_Station,
                    Stop_Type = GetStopTypeIntoEnum(train_stop.Stop_Type)
                });
            }
            await full_train_assignment_service.CreateTrainRouteOnDateWithSchedule(train_route_id, departure_date, train_stops_list, creation_option);
        }
        public async Task AssignTrainRouteOnDateWithSquad(TrainRouteWithSquadAssignmentDto input)
        {
            string train_route_id = input.Train_Route_Id;
            DateOnly departure_date = input.Departure_Date;
            bool creation_option = input.Creation_Option;
            List<CarriageAssignementWithoutRouteDTO> carriage_assignments = input.Carriage_Assignments;
            await full_train_assignment_service.CreateTrainRouteOnDateWithSquad(train_route_id, departure_date, carriage_assignments, creation_option);
        }
        public async Task AssignTrainRouteOnDateWithScheduleAndSquad(TrainRouteWithScheduleAndSquadAssignmentDto input)
        {
            string train_route_id = input.Train_Route_Id;
            DateOnly departure_date = input.Departure_Date;
            bool creation_option = input.Creation_Option;
            List<TrainStopWithArrivalAndDepartureTimeDto> train_stops = input.Train_Stops;

            List<TrainStopWithoutRouteDto> train_stops_list = new List<TrainStopWithoutRouteDto>();
            foreach (TrainStopWithArrivalAndDepartureTimeDto train_stop in train_stops)
            {
                train_stops_list.Add(new TrainStopWithoutRouteDto
                {
                    Station_Title = train_stop.Station_Title,
                    Arrival_Time = train_stop.Arrival_Time,
                    Departure_Time = train_stop.Departure_Time,
                    Distance_From_Starting_Station = train_stop.Distance_From_Starting_Station,
                    Stop_Type = GetStopTypeIntoEnum(train_stop.Stop_Type)
                });
            }
            List<CarriageAssignementWithoutRouteDTO> carriage_assignments = input.Carriage_Assignments;
            await full_train_assignment_service.CreateTrainRouteOnDateWithScheduleAndSquad(train_route_id, departure_date, train_stops_list, carriage_assignments, creation_option);
        }
        public async Task CopyTrainRouteOnDateWithSchedule(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            await full_train_assignment_service.CopyTrainRouteOnDateWithSchedule(train_route_id, prototype_date, new_date, creation_option);
        }
        public async Task CopyTrainRouteOnDateWithSquad(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            await full_train_assignment_service.CopyTrainRouteOnDateWithSquad(train_route_id, prototype_date, new_date, creation_option);
        }
        public async Task CopyTrainRouteOnDateWithScheduleAndSquad(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            await full_train_assignment_service.CopyTrainRouteOnDateWithScheduleAndSquad(train_route_id, prototype_date, new_date, creation_option);
        }
        public async Task ChangeTrainRouteOnDateSchedule(string train_route_id, DateOnly departure_date, List<TrainStopWithArrivalAndDepartureTimeDto> train_stops, bool deletion_option = true)
        {
            List<TrainStopWithoutRouteDto> train_stops_list = new List<TrainStopWithoutRouteDto>();
            foreach (TrainStopWithArrivalAndDepartureTimeDto train_stop in train_stops)
            {
                train_stops_list.Add(new TrainStopWithoutRouteDto
                {
                    Station_Title = train_stop.Station_Title,
                    Arrival_Time = train_stop.Arrival_Time,
                    Departure_Time = train_stop.Departure_Time,
                    Distance_From_Starting_Station = train_stop.Distance_From_Starting_Station,
                    Stop_Type = GetStopTypeIntoEnum(train_stop.Stop_Type)
                });
            }
            await full_train_assignment_service.ChangeTrainRouteOnDateSchedule(train_route_id, departure_date, train_stops_list, deletion_option);
        }
        public async Task ChangeTrainRouteOnDateSquad(string train_route_id, DateOnly departure_date, List<CarriageAssignementWithoutRouteDTO> carriage_assignments, bool deletion_option = true)
        {
            await full_train_assignment_service.ChangeTrainRouteOnDateSquad(train_route_id, departure_date, carriage_assignments, deletion_option);
        }
        public StopType GetStopTypeIntoEnum(string stop_type)
        {
            switch (stop_type)
            {
                case "Boarding":
                    return StopType.Boarding;
                case "Technical":
                    return StopType.Technical;
                default:
                    return StopType.Boarding;
            }
        }
    }
}
