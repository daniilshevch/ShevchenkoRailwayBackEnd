using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    public class UserGoogleAccountAuthenticationService : IUserGoogleAccountAuthenticationService
    {
        private readonly IHttpContextAccessor http_context_accessor;
        private readonly IUserAccountAuthenticationService user_account_authentication_service;
        private readonly PasswordHasher<ExternalInputRegisterUserDto> password_hasher = new PasswordHasher<ExternalInputRegisterUserDto>();
        private readonly AppDbContext db_context;
        private readonly string service_name = "UserGoogleAuthenticationService";
        public UserGoogleAccountAuthenticationService(IHttpContextAccessor http_context_accessor, AppDbContext db_context,
            IUserAccountAuthenticationService user_account_authentication_service)
        {
            this.http_context_accessor = http_context_accessor;
            this.db_context = db_context;
            this.user_account_authentication_service = user_account_authentication_service;
        }
        public async Task<QueryResult<ExternalOutputLoginUserDto>> LoginWithGoogle()
        {
            HttpContext? http_context = http_context_accessor.HttpContext;
            if (http_context is null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.InternalServerError, "Can't get access to HttpContext", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            ClaimsPrincipal user_principal = http_context.User;
            string? email = user_principal.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, $"User is not authorized", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }

            User? user = await db_context.Users.FirstOrDefaultAsync(user => user.Email == email);
            if (user is null)
            {
                ExternalInputRegisterUserDto new_user_info = new ExternalInputRegisterUserDto()
                {
                    User_Name = email,
                    Email = email,
                    Name = user_principal.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
                    Surname = user_principal.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty,
                };
                string password = password_hasher.HashPassword(new_user_info, Guid.NewGuid().ToString());
                new_user_info.Password = password;
                QueryResult<User> user_register_result = await user_account_authentication_service.Register(new_user_info);
                if (user_register_result.Fail)
                {
                    return new FailQuery<ExternalOutputLoginUserDto>(user_register_result.Error);
                }
                user = user_register_result.Value;
            }
            string token = user_account_authentication_service._GenerateJwtToken(user);
            return new SuccessQuery<ExternalOutputLoginUserDto>(new ExternalOutputLoginUserDto
            {
                User_Id = user.Id,
                Token = token,
            }, new SuccessMessage($"User {ConsoleLogService.PrintUser(user)} successfully logged in"));
        }
    }
}
