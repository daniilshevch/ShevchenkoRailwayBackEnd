using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.StationDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;
using System.Net;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;

namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.ModelRepositoryControllers
{
    [ApiController]
    [Route("Admin-API")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    public class StationRepositoryController: ControllerBase
    {
        private readonly IStationRepositoryService station_repository_service;
        public StationRepositoryController(IStationRepositoryService station_repository_service)
        {
            this.station_repository_service = station_repository_service;
        }
        [HttpPost("create-station/{id}")]
        public async Task<ActionResult<StationDto>> CreateStation([FromRoute] int id, [FromBody] ExternalInputStationDto input)
        {
            QueryResult<StationDto> station_creation_result = await station_repository_service.CreateStation(id, input);
            if(station_creation_result.Fail)
            {
                return station_creation_result.GetErrorFromQueryResult<StationDto, StationDto>();
            }
            return Ok(station_creation_result.Value);
        }
        [HttpGet("get-station-by-id/{id}")]
        public async Task<ActionResult<StationDto>> GetStationById([FromRoute] int id)
        {
            StationDto? station = await station_repository_service.GetStationById(id);
            if(station is null)
            {
                return NotFound($"Can't find station with ID: {id}");
            }
            return Ok(station);
        }
        [HttpGet("get-station-by-title/{title}")]
        public async Task<ActionResult<StationDto>> GetStationByTitle([FromRoute] string title)
        {
            StationDto? station = await station_repository_service.GetStationByTitle(title);
            if (station is null)
            {
                return NotFound($"Can't find station with title: {title}");
            }
            return Ok(station);
        }
        [HttpGet("get-stations")]
        public async Task<ActionResult<List<StationDto>>> GetStations()
        {
            List<StationDto> stations = await station_repository_service.GetStations();
            return Ok(stations);
        }
        [HttpPut("update-station/{id}")]
        public async Task<ActionResult<StationDto>> UpdateStation([FromRoute] int id, [FromBody] ExternalInputStationDto input)
        {
            QueryResult<StationDto> station_update_result = await station_repository_service.UpdateStation(id, input);
            if(station_update_result.Fail)
            {
                return station_update_result.GetErrorFromQueryResult<StationDto, StationDto>();
            }
            return Ok(station_update_result.Value);
        }
        [HttpDelete("delete-station/{id}")]
        public async Task<ActionResult> DeleteStation([FromRoute] int id)
        {
            bool successful_delete = await station_repository_service.DeleteStation(id);
            if (successful_delete == false)
            {
                return NotFound($"Can't find station with ID: {id}");
            }
            return NoContent();
        }
    }
}
