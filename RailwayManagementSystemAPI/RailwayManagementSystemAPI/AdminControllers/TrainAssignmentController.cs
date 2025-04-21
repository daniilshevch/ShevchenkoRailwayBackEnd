using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.AdminServices;
using RailwayManagementSystemAPI.API_DTO;
using System.Net;
namespace RailwayManagementSystemAPI.AdminControllers
{
    [ApiController]
    [Route("Admin-API/[controller]")]
    public class TrainAssignmentController: ControllerBase
    {
        private readonly ApiTrainAssignmentService api_train_assignment_service;
        public TrainAssignmentController(ApiTrainAssignmentService api_train_assignment_service)
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
        public async Task<IActionResult> CopyTrainRouteOnDateWithSchedule(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            await api_train_assignment_service.CopyTrainRouteOnDateWithSchedule(train_route_id, prototype_date, new_date, creation_option);
            return Ok();
        }
        [HttpPost("Copy-Train-With-Squad")]
        public async Task<IActionResult> CopyTrainRouteOnDateWithSquad(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            await api_train_assignment_service.CopyTrainRouteOnDateWithSquad(train_route_id, prototype_date, new_date, creation_option);
            return Ok();
        }
        [HttpPost("Copy-Train-With-Schedule-And-Squad")]
        public async Task<IActionResult> CopyTrainRouteOnDateWithScheduleAndSquad(string train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            await api_train_assignment_service.CopyTrainRouteOnDateWithScheduleAndSquad(train_route_id, prototype_date, new_date, creation_option);
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
