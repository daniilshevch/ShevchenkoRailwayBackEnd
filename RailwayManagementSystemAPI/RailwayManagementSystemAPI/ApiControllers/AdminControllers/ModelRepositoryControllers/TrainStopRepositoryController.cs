using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;

namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.ModelRepositoryControllers
{
    [ApiController]
    [Route("Admin-API")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    public class TrainStopRepositoryController: ControllerBase
    {
        private readonly ITrainStopRepositoryService train_stop_repository_service;
        public TrainStopRepositoryController(ITrainStopRepositoryService train_stop_repository_service)
        {
            this.train_stop_repository_service = train_stop_repository_service;
        }

        [HttpPost("add-train-stop-for-train-race")]
        public async Task<ActionResult<TrainRouteOnDateOnStationDto>> CreateTrainStop(TrainRouteOnDateOnStationDto input)
        {
            QueryResult<TrainRouteOnDateOnStationDto> train_stop_creation_result = await train_stop_repository_service.CreateTrainStop(input);
            if(train_stop_creation_result.Fail)
            {
                return train_stop_creation_result.GetErrorFromQueryResult<TrainRouteOnDateOnStationDto, TrainRouteOnDateOnStationDto>();
            }
            return Ok(train_stop_creation_result.Value);
        }
        [HttpGet("get-train-stops-for-train-race/{train_route_on_date_id}")]
        public async Task<ActionResult<List<TrainRouteOnDateOnStationDto>>> GetTrainStopsForTrainRouteOnDate([FromRoute] string train_route_on_date_id)
        {
            QueryResult<List<TrainRouteOnDateOnStationDto>> train_stops_get_result =
                await train_stop_repository_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (train_stops_get_result.Fail)
            {
                return train_stops_get_result.GetErrorFromQueryResult<List<TrainRouteOnDateOnStationDto>, List<TrainRouteOnDateOnStationDto>>();
            }
            return Ok(train_stops_get_result.Value);
        }
        [HttpPut("update-train-stop/{train_route_on_date_id}/{station_title}")]
        public async Task<ActionResult<TrainRouteOnDateOnStationDto>> UpdateTrainStop([FromRoute] string train_route_on_date_id, 
            [FromRoute] string station_title, [FromBody] ExternalTrainStopUpdateDto input)
        {
            QueryResult<TrainRouteOnDateOnStationDto> train_stop_update_result = 
                await train_stop_repository_service.UpdateTrainStop(train_route_on_date_id, station_title, input);
            if(train_stop_update_result.Fail)
            {
                return train_stop_update_result.GetErrorFromQueryResult<TrainRouteOnDateOnStationDto, TrainRouteOnDateOnStationDto>();
            }
            return Ok(train_stop_update_result.Value);
        }
        [HttpDelete("delete-train-stop/{train_route_on_date_id}/{station_title}")]
        public async Task<ActionResult> DeleteTrainStop([FromRoute] string train_route_on_date_id, [FromRoute] string station_title)
        {
            bool train_stop_delete_result = await train_stop_repository_service.DeleteTrainStop(train_route_on_date_id, station_title);
            if(train_stop_delete_result == false)
            {
                return NotFound("Can't find this train stop");
            }
            return NoContent();
        }
    }
}
