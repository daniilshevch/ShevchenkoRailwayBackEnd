using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayCore.Services;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [Route("Client-API/[controller]")]
    public class UserAccountManagementController : ControllerBase
    {
        private readonly UserAccountManagementService user_account_management_service;
        public UserAccountManagementController(UserAccountManagementService user_account_management_service)
        {
            this.user_account_management_service = user_account_management_service;
        }
        [Refactored("v1", "02.05.2025")]
        [HttpPost("/register")]
        public async Task<ActionResult<ExternalOutputRegisterUserDto>> Register(ExternalInputRegisterUserDto input)
        {
            QueryResult<User> user_registration_result = await user_account_management_service.Register(input);
            if (user_registration_result.Fail)
            {
                return user_registration_result.GetErrorFromQueryResult<User,ExternalOutputRegisterUserDto>();
            }
            User user = user_registration_result.Value;
            ExternalOutputRegisterUserDto output_user = (ExternalOutputRegisterUserDto)user;
            return Created($"/users/{output_user.Id}", output_user);
        }
        [HttpPost("/login")]
        public async Task<ActionResult<ExternalOutputLoginUserDto>> Login(ExternaInputlLoginUserDto input)
        {
            QueryResult<ExternalOutputLoginUserDto> user_login_result = await user_account_management_service.Login(input);
            if (user_login_result.Fail)
            {
                return user_login_result.GetErrorFromQueryResult<ExternalOutputLoginUserDto, ExternalOutputLoginUserDto>();
            }
            ExternalOutputLoginUserDto login_user_dto = user_login_result.Value;
            return Ok(login_user_dto);
        }
    }
}
