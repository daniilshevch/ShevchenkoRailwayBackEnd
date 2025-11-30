using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;

namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Client Controllers")]
    [Route("Client-API")]
    public class CompleteAuthenticationController: ControllerBase
    {
        IUserAccountAuthenticationService user_account_authentication_service;
        IUserGoogleAccountAuthenticationService user_google_account_authentication_service;
        public CompleteAuthenticationController(IUserAccountAuthenticationService user_account_authentication_service, IUserGoogleAccountAuthenticationService user_google_account_authentication_service)
        {
            this.user_account_authentication_service = user_account_authentication_service;
            this.user_google_account_authentication_service = user_google_account_authentication_service;
        }
        [HttpPost("register")]
        public async Task<ActionResult<ExternalOutputRegisterUserDto>> Register([FromBody] ExternalInputRegisterUserDto input)
        {
            QueryResult<User> user_registration_result = await user_account_authentication_service.Register(input);
            if (user_registration_result.Fail)
            {
                return user_registration_result.GetErrorFromQueryResult<User, ExternalOutputRegisterUserDto>();
            }
            User user = user_registration_result.Value;
            ExternalOutputRegisterUserDto output_user = (ExternalOutputRegisterUserDto)user;
            return Created($"/users/{output_user.Id}", output_user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<ExternalOutputLoginUserDto>> Login([FromBody] ExternaInputlLoginUserDto input)
        {
            QueryResult<ExternalOutputLoginUserDto> user_login_result = await user_account_authentication_service.Login(input);
            if (user_login_result.Fail)
            {
                return user_login_result.GetErrorFromQueryResult<ExternalOutputLoginUserDto, ExternalOutputLoginUserDto>();
            }
            ExternalOutputLoginUserDto login_user_dto = user_login_result.Value;
            return Ok(login_user_dto);
        }
        [HttpGet("login-with-google")]
        public ActionResult<ExternalOutputLoginUserDto> LoginWithGoogle([FromQuery] string returnUrl)
        {
            string? redirect_url = Url.Action(nameof(GoogleLoginCallback), "CompleteAuthentication", new { returnUrl });
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = redirect_url };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("login/google/callback")]
        public async Task<ActionResult<ExternalOutputLoginUserDto>> GoogleLoginCallback(string returnUrl = "/")
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal is null)
            {
                return Unauthorized("Google authentication failed");
            }
            QueryResult<ExternalOutputLoginUserDto> google_login_result = await user_google_account_authentication_service.LoginWithGoogle(result.Principal);
            if (google_login_result.Fail)
            {
                return google_login_result.GetErrorFromQueryResult<ExternalOutputLoginUserDto, ExternalOutputLoginUserDto>();
            }
            string jwt_token = google_login_result.Value.Token;
            HttpResponse response = Response;
            IResponseCookies cookies = response.Cookies;
            cookies.Append("temp_token", jwt_token, new CookieOptions()
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddSeconds(15)
            });
            return Redirect(returnUrl);
        }
    }
}
