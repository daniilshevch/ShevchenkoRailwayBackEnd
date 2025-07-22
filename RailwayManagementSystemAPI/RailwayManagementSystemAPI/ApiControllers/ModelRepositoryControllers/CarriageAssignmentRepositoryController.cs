using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
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
        [HttpPost("assign-carriage")]
        public async Task<ActionResult<PassengerCarriageOnTrainRouteOnDate>> CreatePassengerCarriageOnTrainRouteOnDate([FromBody] PassengerCarriageOnTrainRouteOnDateDto input)
        {
            QueryResult<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_creation_result = 
                await carriage_assignment_repository_service.CreatePassengerCarriageOnTrainRouteOnDate(input);
            if(carriage_assignment_creation_result.Fail)
            {
                return carriage_assignment_creation_result
                    .GetErrorFromQueryResult<PassengerCarriageOnTrainRouteOnDate, PassengerCarriageOnTrainRouteOnDate>
                    ();
            }
            return Ok(carriage_assignment_creation_result.Value);
        }
    }
}
