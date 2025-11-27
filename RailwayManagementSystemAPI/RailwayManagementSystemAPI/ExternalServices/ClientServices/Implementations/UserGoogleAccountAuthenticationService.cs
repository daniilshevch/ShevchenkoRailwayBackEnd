using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    public class UserGoogleAccountAuthenticationService : IUserGoogleAccountAuthenticationService
    {
        private readonly IUserAccountAuthenticationService user_account_authentication_service;
        private readonly IImageRepository image_repository;
        private readonly PasswordHasher<ExternalInputRegisterUserDto> password_hasher = new PasswordHasher<ExternalInputRegisterUserDto>();
        private readonly AppDbContext db_context;
        private readonly string service_name = "UserGoogleAuthenticationService";
        public UserGoogleAccountAuthenticationService(IImageRepository image_repository,
            AppDbContext db_context,
            IUserAccountAuthenticationService user_account_authentication_service)
        {
            this.image_repository = image_repository;
            this.db_context = db_context;
            this.user_account_authentication_service = user_account_authentication_service;
        }
        public async Task<QueryResult<ExternalOutputLoginUserDto>> LoginWithGoogle(ClaimsPrincipal google_user)
        {
            string? email = google_user.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, $"Fail while getting email from google", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }

            User? user = await db_context.Users.Include(user => user.User_Profile).FirstOrDefaultAsync(user => user.Email == email);
            if (user is null)
            {
                ExternalInputRegisterUserDto new_user_info = new ExternalInputRegisterUserDto()
                {
                    User_Name = email,
                    Email = email,
                    Name = google_user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
                    Surname = google_user.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty,
                };
                string random_password = Guid.NewGuid().ToString("N").Substring(0, 15) + "Q1";
                new_user_info.Password = random_password;
                QueryResult<User> user_register_result = await user_account_authentication_service.Register(new_user_info);
                if (user_register_result.Fail)
                {
                    return new FailQuery<ExternalOutputLoginUserDto>(user_register_result.Error);
                }
                user = user_register_result.Value;
                string? profile_picture_url = google_user.FindFirst("urn:google:picture")?.Value;
                if (profile_picture_url != null)
                {
                    user.User_Profile.Profile_Picture_Url = profile_picture_url.Replace("=s96-c", "=s400-c");
                    await db_context.SaveChangesAsync();    
                }
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
