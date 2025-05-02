using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayCore.Services;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [Route("Client-API/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManagementService user_management_service;
        public UserManagementController(UserManagementService user_management_service)
        {
            this.user_management_service = user_management_service;
        }
        [HttpPost("/register")]
        public async Task<ActionResult<User>> Register(ExternalInputRegisterUserDto input)
        {
            QueryResult<User> user_result = await user_management_service.Register(input);
            if (user_result.Fail)
            {
                return BadRequest(user_result.Error);
            }
            return Ok(user_result);
        }
        [HttpPost("/login")]
        public async Task<ActionResult<ExternalOutputLoginUserDto>> Login(ExternaInputlLoginUserDto input)
        {
            QueryResult<ExternalOutputLoginUserDto> user_result = await user_management_service.Login(input);
            if (user_result.Fail)
            {
                return BadRequest(user_result?.Error);
            }
            return Ok(user_result.Value);
        }
    }
}
