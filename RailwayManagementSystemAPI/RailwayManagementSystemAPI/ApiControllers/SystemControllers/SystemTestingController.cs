using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayCore.InternalServices.CoreServices;
using RailwayManagementSystemAPI.ApiControllers.ClientControllers;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.SystemAuthenticationServices;

public class SystemUserDemonstrationDto
{
    public int Id { get; set; }
    public string User_Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }

}
namespace RailwayManagementSystemAPI.ApiControllers.SystemControllers
{
    [ApiExplorerSettings(GroupName = "System Controllers")]
    public class SystemTestingController : ControllerBase
    {
        private readonly SystemAuthenticationService system_authentication_service;
        public SystemTestingController(SystemAuthenticationService system_authentication_service)
        {
            this.system_authentication_service = system_authentication_service;
        }
        [HttpGet("/get-authenticated-user")]
        public async Task<ActionResult<SystemUserDemonstrationDto>> GetAuthenticatedUser()
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return user_authentication_result.GetErrorFromQueryResult<User, SystemUserDemonstrationDto>();
            }
            User user = user_authentication_result.Value;
            HttpRequest request = HttpContext.Request;
            IHeaderDictionary headers = request.Headers;
            string? jwt_token = headers.Authorization;
            return new SystemUserDemonstrationDto
            {
                Id = user.Id,
                User_Name = user.Name,
                Email = user.Email,
                Token = jwt_token
            };
        }
    }
}
