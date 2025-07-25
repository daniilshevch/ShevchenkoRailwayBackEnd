using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices;

namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.ModelRepositoryControllers
{
    [ApiController]
    [Route("Admin-API")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    public class TrainRaceRepositoryController: ControllerBase
    {
        public TrainRaceRepositoryService train_race_repository_service;
        public TrainRaceRepositoryController(TrainRaceRepositoryService train_race_repository_service)
        {
            this.train_race_repository_service = train_race_repository_service;
        }
        [HttpPost("create-train-race")]
        public async Task<ActionResult<ExternalSimpleTrainRaceDto>> CreateTrainRouteOnDate([FromBody] TrainRouteOnDateDto input)
        {
            ExternalSimpleTrainRaceDto? train_race = await train_race_repository_service.CreateTrainRouteOnDate(input);
            if(train_race is null)
            {
                return NotFound("Can't find train route with this id(or duplicate creation)");
            }
            return Ok(train_race);
        }
        [HttpGet("get-races-for-train-route/{train_route_id}")]
        public async Task<ActionResult<List<ExternalSimpleTrainRaceDto>>> GetTrainRoutesOnDateForTrainRoute([FromRoute] string train_route_id)
        {
            return await train_race_repository_service.GetTrainRoutesOnDateForTrainRoute(train_route_id);
        }
        [HttpPatch("update-train-race-price-coefficient/{train_route_on_date_id}")]
        public async Task<ActionResult<ExternalSimpleTrainRaceDto>> ChangeTrainRaceCoefficientForTrainRouteOnDate([FromRoute] string train_route_on_date_id, [FromQuery] double train_race_coefficient)
        {
            ExternalSimpleTrainRaceDto? updated_train_race = await train_race_repository_service
                .ChangeTrainRaceCoefficientForTrainRouteOnDate(train_route_on_date_id, train_race_coefficient);
            if(updated_train_race is null)
            {
                return NotFound($"Can't find train race with id: {train_route_on_date_id}");
            }
            return Ok(updated_train_race);
        }
        [HttpDelete("delete-train-race/{train_route_on_date_id}")]
        public async Task<ActionResult> DeleteTrainRouteOnDate([FromRoute] string train_route_on_date_id)
        {
            bool successful_delete = await train_race_repository_service.DeleteTrainRouteOnDate(train_route_on_date_id);
            if(successful_delete == false)
            {
                return NotFound($"Can't find train race with id: {train_route_on_date_id}");
            }
            return NoContent();
        }
    }
}
