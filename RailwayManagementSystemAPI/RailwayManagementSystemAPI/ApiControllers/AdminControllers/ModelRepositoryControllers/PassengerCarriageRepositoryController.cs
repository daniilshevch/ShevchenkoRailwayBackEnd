using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.PassengerCarriageDTO.AdminDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;

namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.ModelRepositoryControllers
{
    [ApiController]
    [Route("Admin-API")]
    [Authorize(Roles = "Administrator")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    public class PassengerCarriageRepositoryController: ControllerBase
    {
        private readonly IPassengerCarriageRepositoryService passenger_carriage_repository_service;
        public PassengerCarriageRepositoryController(IPassengerCarriageRepositoryService passenger_carriage_repository_service)
        {
            this.passenger_carriage_repository_service = passenger_carriage_repository_service;
        }
        [HttpPost("add-passenger-carriage/{passenger_carriage_id}")]
        public async Task<ActionResult<PassengerCarriageDto>> CreatePassengerCarriage([FromRoute] string passenger_carriage_id, 
            [FromBody] ExternalPassengerCarriageCreateAndUpdateDto input)
        {
            QueryResult<PassengerCarriageDto> passenger_carriage_creation_result = await passenger_carriage_repository_service.CreatePassengerCarriage(passenger_carriage_id, input);
            if (passenger_carriage_creation_result.Fail)
            {
                return passenger_carriage_creation_result.GetErrorFromQueryResult<PassengerCarriageDto, PassengerCarriageDto>();
            }
            return Ok(passenger_carriage_creation_result.Value);
        }
        [HttpGet("get-passenger-carriage/{passenger_carriage_id}")]
        public async Task<ActionResult<PassengerCarriageDto>> GetPassengerCarriageById(string passenger_carriage_id)
        {
            PassengerCarriageDto? passenger_carriage = await passenger_carriage_repository_service.GetPassengerCarriageById(passenger_carriage_id);
            if (passenger_carriage is null)
            {
                return NotFound($"Can't find passenger_carriage with ID: {passenger_carriage_id}");
            }
            return Ok(passenger_carriage);
        }
        [HttpGet("get-passenger-carriages")]
        public async Task<ActionResult<List<PassengerCarriageDto>>> GetAllPassengerCarriages()
        {
            List<PassengerCarriageDto> passenger_carriages = await passenger_carriage_repository_service.GetAllPassengerCarriages();
            return Ok(passenger_carriages);
        }
        [HttpPut("update-passenger-carriage/{passenger_carriage_id}")]
        public async Task<ActionResult<PassengerCarriageDto>> UpdatePassengerCarriage(string passenger_carriage_id, ExternalPassengerCarriageCreateAndUpdateDto input)
        {
            QueryResult<PassengerCarriageDto> passenger_carriage_update_result = await passenger_carriage_repository_service.UpdatePassengerCarriage(passenger_carriage_id, input);
            if (passenger_carriage_update_result.Fail)
            {
                return passenger_carriage_update_result.GetErrorFromQueryResult<PassengerCarriageDto, PassengerCarriageDto>();
            }
            return Ok(passenger_carriage_update_result.Value);
        }
        [HttpDelete("delete-passenger-carriage/{passenger_carriage_id}")]
        public async Task<ActionResult> DeletePassengerCarriage(string passenger_carriage_id)
        {
            bool succesful_delete = await passenger_carriage_repository_service.DeletePassengerCarriage(passenger_carriage_id);
            if(succesful_delete == false)
            {
                return NotFound($"Can't find passenger carriage with ID: {passenger_carriage_id}");
            }
            return NoContent();
        }
    }
}
