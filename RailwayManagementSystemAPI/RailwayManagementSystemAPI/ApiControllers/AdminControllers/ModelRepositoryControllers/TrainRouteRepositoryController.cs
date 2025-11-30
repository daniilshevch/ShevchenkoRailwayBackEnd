using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TrainRouteDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;

namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.ModelRepositoryControllers
{
    [ApiController]
    [Route("Admin-API")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    public class TrainRouteRepositoryController: ControllerBase
    {
        private readonly ITrainRouteRepositoryService train_route_repository_service;
        public TrainRouteRepositoryController(ITrainRouteRepositoryService train_route_repository_service)
        {
            this.train_route_repository_service = train_route_repository_service;
        }
        [HttpPost("add-train-route/{id}")]
        public async Task<ActionResult<ExternalTrainRouteDto>> CreateTrainRoute([FromRoute] string id, [FromBody] ExternalTrainRouteCreateAndUpdateDto input)
        {
            QueryResult<ExternalTrainRouteDto> train_route_creation_result = await train_route_repository_service.CreateTrainRoute(id, input);
            if(train_route_creation_result.Fail)
            {
                return train_route_creation_result.GetErrorFromQueryResult<ExternalTrainRouteDto,ExternalTrainRouteDto>();
            }
            return Ok(train_route_creation_result.Value);

        }
        [HttpGet("get-train-routes")]
        public async Task<ActionResult<List<ExternalTrainRouteDto>>> GetTrainRoutes()
        {
            return await train_route_repository_service.GetTrainRoutes();
        }
        [HttpGet("get-train-route/{id}")]
        public async Task<ActionResult<ExternalTrainRouteDto>> GetTrainRouteById([FromRoute] string id)
        {
            ExternalTrainRouteDto? train_route = await train_route_repository_service.GetTrainRouteById(id);
            if(train_route == null)
            {
                return NotFound($"Can't find train route with id: {id}");
            }
            return Ok(train_route);
        }
        [HttpPut("update-train-route/{id}")]
        public async Task<ActionResult<ExternalTrainRouteDto>> UpdateTrainRoute([FromRoute] string id, [FromBody] ExternalTrainRouteCreateAndUpdateDto input)
        {
            QueryResult<ExternalTrainRouteDto> train_route_update_result = await train_route_repository_service.UpdateTrainRoute(id, input);
            if (train_route_update_result.Fail)
            {
                return train_route_update_result.GetErrorFromQueryResult<ExternalTrainRouteDto, ExternalTrainRouteDto>();
            }
            return Ok(train_route_update_result.Value);
        }
        [HttpDelete("delete-train-route/{id}")]
        public async Task<ActionResult> DeleteTrainRouteById(string id)
        {
            bool train_route_delete_result = await train_route_repository_service.DeleteTrainRouteById(id);
            if(train_route_delete_result == false)
            {
                return NotFound($"Can't find train route with id: {id}"); 
            }
            return NoContent();
        }
    }
}
