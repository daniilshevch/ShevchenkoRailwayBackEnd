using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using Swashbuckle.AspNetCore.Annotations;

namespace RailwayManagementSystemAPI.ApiControllers.ModelRepositoryControllers
{
    [ApiController]
    [Route("Admin-API")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    [SwaggerTag("Контролер для призначення пасажирських вагонів до поїздів")]
    
    public class CarriageAssignmentRepositoryController: ControllerBase
    {
        private readonly CarriageAssignmentRepositoryService carriage_assignment_repository_service;
        public CarriageAssignmentRepositoryController(CarriageAssignmentRepositoryService carriage_assignment_repository_service)
        {
            this.carriage_assignment_repository_service = carriage_assignment_repository_service;
        }
        [HttpPost("assign-carriage-to-train-race-squad")]
        public async Task<ActionResult<PassengerCarriageOnTrainRouteOnDateDto>> CreatePassengerCarriageOnTrainRouteOnDate([FromBody] PassengerCarriageOnTrainRouteOnDateDto input)
        {
            QueryResult<PassengerCarriageOnTrainRouteOnDateDto> carriage_assignment_creation_result = 
                await carriage_assignment_repository_service.CreatePassengerCarriageOnTrainRouteOnDate(input);
            if(carriage_assignment_creation_result.Fail)
            {
                return carriage_assignment_creation_result
                    .GetErrorFromQueryResult<PassengerCarriageOnTrainRouteOnDateDto, PassengerCarriageOnTrainRouteOnDateDto>();
            }
            return Ok(carriage_assignment_creation_result.Value);
        }
        [HttpGet("get-carriage-assignments-for-train-race/{train_route_on_date_id}")]
        public async Task<ActionResult<List<PassengerCarriageOnTrainRouteOnDateDto>>> GetPassengerCarriagesForTrainRouteOnDate([FromRoute] string train_route_on_date_id)
        {
            QueryResult<List<PassengerCarriageOnTrainRouteOnDateDto>> carriage_assignments_get_result =
                await carriage_assignment_repository_service.GetPassengerCarriagesForTrainRouteOnDate(train_route_on_date_id);
            if(carriage_assignments_get_result.Fail)
            {
                return carriage_assignments_get_result.GetErrorFromQueryResult<List<PassengerCarriageOnTrainRouteOnDateDto>, List<PassengerCarriageOnTrainRouteOnDateDto>>();
            }
            return Ok(carriage_assignments_get_result.Value);

            
        }
        [HttpPut("update-carriage-assignment/{train_route_on_date_id}/{passenger_carriage_id}")]
        public async Task<ActionResult<PassengerCarriageOnTrainRouteOnDateDto>> UpdatePassengerCarriageOnTrainRouteOnDate(
    [FromRoute] string train_route_on_date_id, [FromRoute] string passenger_carriage_id, [FromBody] ExternalCarriageAssignmentUpdateDto input)
        {
            QueryResult<PassengerCarriageOnTrainRouteOnDateDto> carriage_assignment_update_result =
                await carriage_assignment_repository_service
                .UpdatePassengerCarriageOnTrainRouteOnDate(passenger_carriage_id, train_route_on_date_id, input);
            if (carriage_assignment_update_result.Fail)
            {
                return carriage_assignment_update_result
                    .GetErrorFromQueryResult<PassengerCarriageOnTrainRouteOnDateDto, PassengerCarriageOnTrainRouteOnDateDto>();
            }
            return Ok(carriage_assignment_update_result.Value);

        }
        [HttpDelete("delete-carriage-assignment/{train_route_on_date_id}/{passenger_carriage_id}")]
        public async Task<ActionResult> DeletePassengerCarriageOnTrainRouteOnDate([FromRoute] string passenger_carriage_id, [FromRoute] string train_route_on_date_id)
        {
            bool carriage_assignment_delete_result = await 
                carriage_assignment_repository_service.DeletePassengerCarriageOnTrainRouteOnDate(passenger_carriage_id, train_route_on_date_id);
            if(carriage_assignment_delete_result == false)
            {
                return NotFound("Can't find this train assignment");
            }
            return NoContent();

        }
    }
}
