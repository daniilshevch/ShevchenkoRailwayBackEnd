using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainRouteDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.TrainAssignmentServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.TrainAssignmentServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using System.Net;
namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.TrainAssignmentControllers
{
    [ApiController]
    [Route("Admin-API/[controller]")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    public class TrainAssignmentController : ControllerBase
    {
        private readonly IApiTrainAssignmentService api_train_assignment_service;
        public TrainAssignmentController(IApiTrainAssignmentService api_train_assignment_service)
        {
            this.api_train_assignment_service = api_train_assignment_service;
        }
        [HttpPost("Assign-Train-With-Schedule")]
        public async Task<IActionResult> AssignTrainRouteOnDateWithSchedule(TrainRouteWithScheduleAssignmentDto input)
        {
            await api_train_assignment_service.AssignTrainRouteOnDateWithSchedule(input);
            return Ok();
        }
        [HttpPost("Assign-Train-With-Squad")]
        public async Task<IActionResult> AssignTrainRouteOnDateWithSquad(TrainRouteWithSquadAssignmentDto input)
        {
            await api_train_assignment_service.AssignTrainRouteOnDateWithSquad(input);
            return Ok();
        }
        [HttpPost("Assign-Train-With-Schedule-And-Squad")]
        public async Task<IActionResult> AssignTrainRouteOnDateWithScheduleAndSquad(TrainRouteWithScheduleAndSquadAssignmentDto input)
        {
            await api_train_assignment_service.AssignTrainRouteOnDateWithScheduleAndSquad(input);
            return Ok();
        }
        [HttpPost("Copy-Train-With-Schedule")]
        public async Task<IActionResult> CopyTrainRouteOnDateWithSchedule([FromQuery] string prototype_train_route_id, [FromQuery] string new_train_route_id,
            [FromQuery] DateOnly prototype_date, [FromQuery] DateOnly new_date, [FromQuery] bool creation_option = true)
        {
            QueryResult schedule_copy_result = await api_train_assignment_service.CopyTrainRouteOnDateWithSchedule(prototype_train_route_id, new_train_route_id, prototype_date, new_date, creation_option);
            if(schedule_copy_result.Fail)
            {
                return schedule_copy_result.GetErrorFromNonGenericQueryResult();
            }    
            return Ok();
        }
        [HttpPost("Copy-Train-With-Squad")]
        public async Task<IActionResult> CopyTrainRouteOnDateWithSquad([FromQuery] string prototype_train_route_id, [FromQuery] string new_train_route_id,
            [FromQuery] DateOnly prototype_date, [FromQuery] DateOnly new_date, [FromQuery] bool creation_option = true)
        {
            QueryResult squad_copy_result = await api_train_assignment_service.CopyTrainRouteOnDateWithSquad(prototype_train_route_id, new_train_route_id, prototype_date, new_date, creation_option);
            if(squad_copy_result.Fail)
            {
                return squad_copy_result.GetErrorFromNonGenericQueryResult();
            }
            return Ok();
        }
        [HttpPost("Copy-Train-With-Schedule-And-Squad")]
        public async Task<IActionResult> CopyTrainRouteOnDateWithScheduleAndSquad([FromQuery] string prototype_train_route_id, 
            [FromQuery] string new_train_route_id, [FromQuery] DateOnly prototype_date, [FromQuery] DateOnly new_date, [FromQuery] bool creation_option = true)
        {
            QueryResult schedule_and_squad_copy_result =await api_train_assignment_service.CopyTrainRouteOnDateWithScheduleAndSquad(prototype_train_route_id, new_train_route_id, prototype_date, new_date, creation_option);
            if(schedule_and_squad_copy_result.Fail)
            {
                return schedule_and_squad_copy_result.GetErrorFromNonGenericQueryResult();
            }
            return Ok();
        }
        [HttpPut("Change-Train-Schedule")]
        public async Task<IActionResult> ChangeTrainRouteOnDateSchedule(string train_route_id, DateOnly departure_date, List<TrainStopWithArrivalAndDepartureTimeDto> train_stops, bool deletion_option = true)
        {
            await api_train_assignment_service.ChangeTrainRouteOnDateSchedule(train_route_id, departure_date, train_stops, deletion_option);
            return Ok();
        }
        [HttpPut("Change-Train-Squad")]
        public async Task<IActionResult> ChangeTrainRouteOnDateSquad(string train_route_id, DateOnly departure_date, List<CarriageAssignementWithoutRouteDTO> carriage_assignments, bool deletion_option = true)
        {
            await api_train_assignment_service.ChangeTrainRouteOnDateSquad(train_route_id, departure_date, carriage_assignments, deletion_option);
            return Ok();
        }

    }
}
