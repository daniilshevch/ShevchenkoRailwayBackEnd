using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.ModelRepositoryServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;

namespace RailwayManagementSystemAPI.ApiControllers.ModelRepositoryControllers
{
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
            QueryResult<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_creation_result = 
                await carriage_assignment_repository_service.CreatePassengerCarriageOnTrainRouteOnDate(input);
            if(carriage_assignment_creation_result.Fail)
            {
                return carriage_assignment_creation_result
                    .GetErrorFromQueryResult<PassengerCarriageOnTrainRouteOnDate, PassengerCarriageOnTrainRouteOnDateDto>();
            }
            return Ok((PassengerCarriageOnTrainRouteOnDateDto)carriage_assignment_creation_result.Value);
        }
        [HttpPatch("update-carriage-assignment/{train_route_on_date_id}/{passenger_carriage_id}")]
        public async Task<ActionResult<PassengerCarriageOnTrainRouteOnDateDto>> UpdatePassengerCarriageOnTrainRouteOnDate(
            [FromRoute] string train_route_on_date_id, [FromRoute] string passenger_carriage_id, [FromBody] ExternalCarriageAssignmentUpdateDto input)
        {
            QueryResult<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_update_result =
                await carriage_assignment_repository_service
                .UpdatePassengerCarriageOnTrainRouteOnDate(passenger_carriage_id, train_route_on_date_id, input);
            if(carriage_assignment_update_result.Fail)
            {
                return carriage_assignment_update_result
                    .GetErrorFromQueryResult<PassengerCarriageOnTrainRouteOnDate, PassengerCarriageOnTrainRouteOnDateDto>();
            }
            return Ok((PassengerCarriageOnTrainRouteOnDateDto)carriage_assignment_update_result.Value);

        }
           
    }
}
