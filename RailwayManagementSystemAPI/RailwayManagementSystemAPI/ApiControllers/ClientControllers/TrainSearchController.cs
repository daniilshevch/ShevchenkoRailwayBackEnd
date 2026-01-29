using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayCore.InternalServices.CoreServices;
using RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO;
namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Client Controllers")]
    [Route("Client-API/[controller]")]
    public class TrainSearchController : ControllerBase
    {
        private readonly ITrainRouteWithBookingsSearchService train_route_with_booking_search_service;
        public TrainSearchController(ITrainRouteWithBookingsSearchService train_route_with_booking_search_service)
        {
            this.train_route_with_booking_search_service = train_route_with_booking_search_service;
        }
        [HttpGet("Search-Train-Routes-Between-Stations-With-Bookings/{starting_station}/{ending_station}")] //January
        public async Task<ActionResult<List<ExternalTrainRaceWithBookingsInfoDto>>> SearchTrainRoutesBetweenStationsWithBookingsInfo([FromRoute] string starting_station, [FromRoute] string ending_station, [FromQuery] DateOnly departure_date)
        {
            QueryResult<List<ExternalTrainRaceWithBookingsInfoDto>>? train_routes_with_bookings_info_result =
                await train_route_with_booking_search_service.SearchTrainRoutesBetweenStationsWithBookingsInfo(starting_station, ending_station, departure_date, admin_mode: true);
            if (train_routes_with_bookings_info_result is FailQuery<List<ExternalTrainRaceWithBookingsInfoDto>>)
            {
                return NotFound(train_routes_with_bookings_info_result.Error!.Message);
            }
            return Ok(train_routes_with_bookings_info_result.Value);
        }
        [HttpGet("Search-Train-Routes-Between-Stations-With-Bookings-Without-Places/{starting_station}/{ending_station}")] //January
        public async Task<ActionResult<List<ExternalTrainRaceWithBookingsInfoDto>>> SearchTrainRoutesBetweenStationsWithBookingsInfoWithoutPlaces([FromRoute] string starting_station, [FromRoute] string ending_station, [FromQuery] DateOnly departure_date)
        {
            QueryResult<List<ExternalTrainRaceWithBookingsInfoDto>>? train_routes_with_bookings_info_result =
                await train_route_with_booking_search_service.SearchTrainRoutesBetweenStationsWithBookingsInfo(starting_station, ending_station, departure_date, admin_mode: false, places_availability_info: false);
            if (train_routes_with_bookings_info_result is FailQuery<List<ExternalTrainRaceWithBookingsInfoDto>>)
            {
                return NotFound(train_routes_with_bookings_info_result.Error!.Message);
            }
            return Ok(train_routes_with_bookings_info_result.Value);
        }
        [HttpGet("Search-Train-Routes-Through-Station/{station_title}")]
        public async Task<ActionResult<List<ExternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStation([FromRoute] string station_title,
            [FromQuery] DateTime time, [FromQuery] double? left_interval = null, [FromQuery] double? right_interval = null)
        {
            TimeSpan? left = left_interval.HasValue ? TimeSpan.FromHours(left_interval.Value) : null;
            TimeSpan? right = right_interval.HasValue ? TimeSpan.FromHours(right_interval.Value) : null;
            QueryResult<List<ExternalTrainRaceThroughStationDto>> train_races_result = 
                await train_route_with_booking_search_service.SearchTrainRoutesThroughStation(station_title, time, left, right);
            if(train_races_result.Fail)
            {
                return NotFound();
            }
            return Ok(train_races_result.Value);
        }
        [HttpGet("get-full-train-race-info-with-bookings/{train_race_id}/{starting_station}/{ending_station}")]
        public async Task<ActionResult<ExternalTrainRaceWithBookingsInfoDto>> GetCompleteInfoWithBookingsForTrainRaceBetweenStations([FromRoute] string train_race_id, 
            [FromRoute] string starting_station, [FromRoute] string ending_station)
        {
            QueryResult<ExternalTrainRaceWithBookingsInfoDto> train_race_info_get_result =
                await train_route_with_booking_search_service.GetCompleteInfoWithBookingsForTrainRaceOnDateBetweenStations(train_race_id, starting_station, ending_station, admin_mode: true);
            if (train_race_info_get_result.Fail)
            {
                return train_race_info_get_result.GetErrorFromQueryResult<ExternalTrainRaceWithBookingsInfoDto, ExternalTrainRaceWithBookingsInfoDto>();
            }
            return Ok(train_race_info_get_result.Value);
        }
        [HttpGet("get-train-schedule-for-train-race/{train_race_id}")]
        public async Task<ActionResult<List<ExternalSingleTrainStopDto>>> GetScheduleForTrainRace([FromRoute] string train_race_id,
            [FromQuery] string starting_station_title, [FromQuery] string ending_station_title)
        {
            QueryResult<List<ExternalSingleTrainStopDto>> train_stops_get_result = await train_route_with_booking_search_service.GetScheduleForTrainRace(train_race_id,
                starting_station_title, ending_station_title);
            if(train_stops_get_result.Fail)
            {
                return train_stops_get_result.GetErrorFromQueryResult<List<ExternalSingleTrainStopDto>, List<ExternalSingleTrainStopDto>>();
            }
            return Ok(train_stops_get_result.Value);

        }
    }
}
