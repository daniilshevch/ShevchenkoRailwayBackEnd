using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayCore.InternalServices.CoreServices;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Client Controllers")]
    [Route("Client-API")]
    public class UserAccountManagementController : ControllerBase
    {
        private readonly UserAccountAuthenticationService user_account_management_service;
        private readonly UserProfileManagementService user_profile_management_service;
        public UserAccountManagementController(UserAccountAuthenticationService user_account_management_service, 
            UserProfileManagementService user_profile_management_service)
        {
            this.user_account_management_service = user_account_management_service;
            this.user_profile_management_service=  user_profile_management_service;
        }
        [Refactored("v1", "02.05.2025")]
        [HttpPost("register")]
        public async Task<ActionResult<ExternalOutputRegisterUserDto>> Register([FromBody] ExternalInputRegisterUserDto input)
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
        [HttpPost("login")]
        public async Task<ActionResult<ExternalOutputLoginUserDto>> Login([FromBody] ExternaInputlLoginUserDto input)
        {
            QueryResult<ExternalOutputLoginUserDto> user_login_result = await user_account_management_service.Login(input);
            if (user_login_result.Fail)
            {
                return user_login_result.GetErrorFromQueryResult<ExternalOutputLoginUserDto, ExternalOutputLoginUserDto>();
            }
            ExternalOutputLoginUserDto login_user_dto = user_login_result.Value;
            return Ok(login_user_dto);
        }
        [HttpPost("upload-profile-image-for-current-user")]
        public async Task<ActionResult> UploadProfileImage(IFormFile image_file)
        {
            QueryResult image_uploading_result = await user_profile_management_service.UploadProfileImage(image_file);
            if(image_uploading_result.Fail)
            {
                return image_uploading_result.GetErrorFromNonGenericQueryResult();
            }
            return Ok();
        }
        [HttpGet("get-profile-image-for-current-user")]
        public async Task<ActionResult> GetProfileImage()
        {
            QueryResult<Image> image_get_result = await user_profile_management_service.GetProfileImage();
            if(image_get_result.Fail)
            {
                return image_get_result.GetErrorFromQueryResultWithoutOutput();
            }
            return File(image_get_result.Value.Image_Data, "image/jpeg");
        }
    }
}
