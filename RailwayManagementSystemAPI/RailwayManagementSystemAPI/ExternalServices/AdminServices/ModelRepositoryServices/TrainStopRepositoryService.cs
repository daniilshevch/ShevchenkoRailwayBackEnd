using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
{
    public class TrainStopRepositoryService
    {
        private readonly TrainRouteOnDateOnStationRepository train_route_on_date_on_station_repository;
        public TrainStopRepositoryService(TrainRouteOnDateOnStationRepository train_route_on_date_on_station_repository)
        {
            this.train_route_on_date_on_station_repository = train_route_on_date_on_station_repository;
        }
        public async Task<QueryResult<TrainRouteOnDateOnStationDto>> CreateTrainStop(TrainRouteOnDateOnStationDto input)
        {
            QueryResult<TrainRouteOnDateOnStation> train_stop_creation_result = await train_route_on_date_on_station_repository.CreateTrainStop(input);
            if(train_stop_creation_result.Fail)
            {
                return new FailQuery<TrainRouteOnDateOnStationDto>(train_stop_creation_result.Error);
            }
            return new SuccessQuery<TrainRouteOnDateOnStationDto>((TrainRouteOnDateOnStationDto)train_stop_creation_result.Value);
        }
        public async Task<QueryResult<List<TrainRouteOnDateOnStationDto>>> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id)
        {
            QueryResult<List<TrainRouteOnDateOnStation>> train_stops_get_result = 
                await train_route_on_date_on_station_repository.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if(train_stops_get_result.Fail)
            {
                return new FailQuery<List<TrainRouteOnDateOnStationDto>>(train_stops_get_result.Error);
            }
            List<TrainRouteOnDateOnStationDto> train_stops = train_stops_get_result.Value.Select(single_train_stop =>
                (TrainRouteOnDateOnStationDto)single_train_stop).ToList();

            TrainRouteOnDateOnStationDto? previous_stop = null;
            foreach(TrainRouteOnDateOnStationDto current_stop in train_stops)
            {
                double? speed_on_section = null;
                if(previous_stop is not null)
                {
                    double distance_between_stations = (double)(current_stop.Distance_From_Starting_Station - previous_stop.Distance_From_Starting_Station)!;
                    double trip_duration_between_station = ((DateTime)current_stop.Arrival_Time! - (DateTime)previous_stop.Departure_Time!).TotalHours;
                    speed_on_section = distance_between_stations / trip_duration_between_station;
                    previous_stop.Speed_On_Section = speed_on_section;
                }
                previous_stop = current_stop;
            }
            return new SuccessQuery<List<TrainRouteOnDateOnStationDto>>(train_stops);
        }
        public async Task<QueryResult<TrainRouteOnDateOnStationDto>> UpdateTrainStop(string train_route_on_date_id, string station_title, 
            ExternalTrainStopUpdateDto input)
        {
            TrainRouteOnDateOnStationDto update_dto = new TrainRouteOnDateOnStationDto()
            {
                Train_Route_On_Date_Id = train_route_on_date_id,
                Station_Title = station_title,
                Arrival_Time = input.Arrival_Time,
                Departure_Time = input.Departure_Time,
                Distance_From_Starting_Station = input.Distance_From_Starting_Station,
                Speed_On_Section = input.Speed_On_Section,
                Stop_Type = input.Stop_Type
            };
            QueryResult<TrainRouteOnDateOnStation> train_stop_update_result = 
                await train_route_on_date_on_station_repository.UpdateTrainStop(update_dto);
            if(train_stop_update_result.Fail)
            {
                return new FailQuery<TrainRouteOnDateOnStationDto>(train_stop_update_result.Error);
            }
            return new SuccessQuery<TrainRouteOnDateOnStationDto>((TrainRouteOnDateOnStationDto)train_stop_update_result.Value);
        }

        public async Task<bool> DeleteTrainStop(string train_route_on_date_id, string station_title)
        {
            return await train_route_on_date_on_station_repository.DeleteTrainStop(train_route_on_date_id, station_title);
        }

    }
}
