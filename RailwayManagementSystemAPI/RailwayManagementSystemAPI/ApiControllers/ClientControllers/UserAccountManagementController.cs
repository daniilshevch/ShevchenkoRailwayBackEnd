using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;

namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Client Controllers")]
    [Route("Client-API")]
    public class UserAccountManagementController : ControllerBase
    {
        private readonly IUserProfileManagementService user_profile_management_service;
        public UserAccountManagementController(IUserProfileManagementService user_profile_management_service)
        {
            this.user_profile_management_service = user_profile_management_service;
        }

        [HttpPost("upload-profile-image-for-current-user")]
        public async Task<ActionResult> UploadProfileImage(IFormFile image_file)
        {
            QueryResult image_uploading_result = await user_profile_management_service.UploadProfileImage(image_file);
            if (image_uploading_result.Fail)
            {
                return image_uploading_result.GetErrorFromNonGenericQueryResult();
            }
            return Ok();
        }
        [HttpGet("get-profile-image-for-current-user")]
        public async Task<ActionResult> GetProfileImage()
        {
            QueryResult<Image> image_get_result = await user_profile_management_service.GetProfileImage();
            if (image_get_result.Fail == false)
            {
                return File(image_get_result.Value.Image_Data, "image/jpeg");
            }
            QueryResult<string> google_image_get_result = await user_profile_management_service.GetProfileImageFromGoogleUrl();
            if (google_image_get_result.Fail)
            {
                return google_image_get_result.GetErrorFromQueryResultWithoutOutput();
            }
            return Ok(new { imageUrl = google_image_get_result.Value });
        }
    }
}
