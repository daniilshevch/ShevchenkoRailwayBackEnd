using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayCore.InternalDTO.ModelDTO;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    /// <summary>
    /// Даний сервіс відповідає за аутентифікацію через сервіси гугл. В своєму функціоналі спирається на сервіс UserAccountAuthenticationService, який
    /// проводить внутрішню аутентифікацію в межах системи(не задією зовнішні сервіси, використовує JWT-токени)
    /// </summary>
    public class UserGoogleAccountAuthenticationService : IUserGoogleAccountAuthenticationService
    {
        private readonly IUserAccountAuthenticationService user_account_authentication_service;
        private readonly IUserRepository user_repository;
        private readonly IImageRepository image_repository;
        private readonly PasswordHasher<ExternalInputRegisterUserDto> password_hasher = new PasswordHasher<ExternalInputRegisterUserDto>();
        private readonly string service_name = "UserGoogleAuthenticationService";
        public UserGoogleAccountAuthenticationService(IUserRepository user_repository, IImageRepository image_repository,
            IUserAccountAuthenticationService user_account_authentication_service)
        {
            this.user_repository = user_repository;
            this.image_repository = image_repository;
            this.user_account_authentication_service = user_account_authentication_service;
        }
        /// <summary>
        /// Даний метод проводить логін через сервіси гугл. Використовує сервіси гугл для верифікації пошти, отримує користувача за поштою, а далі розпочинають процес 
        /// аутентифікації через JWT-токени
        /// </summary>
        /// <param name="google_user"></param>
        /// <returns></returns>
        public async Task<QueryResult<ExternalOutputLoginUserDto>> LoginWithGoogle(ClaimsPrincipal google_user)
        {
            //отримуємо з об'єкту ClaimsPrincipal пошту користувача(вона надходить з сервісів Google)
            string? email = google_user.FindFirst(ClaimTypes.Email)?.Value;
            if (email is null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, $"Fail while getting email from google services", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            //Отримуємо користувача з бази за поштою, яку надав Google
            User? user = await user_repository.GetUserByEmail(email);

            if (user is null) //Якщо користувача з такою поштою нема, то необхідно провести його реєстрацію
            {
                ExternalInputRegisterUserDto new_user_info = new ExternalInputRegisterUserDto()
                {
                    User_Name = email, //В якості username беремо пошту(далі користувач може її поміняти)
                    Email = email,
                    Name = google_user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty, //Ім'я надає гугл
                    Surname = google_user.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty, //Прізвише надає гугл
                };
                string random_password = Guid.NewGuid().ToString("N").Substring(0, 15) + "Q1"; //Пароль в акаунт вписуємо випадковий
                new_user_info.Password = random_password;
                QueryResult<User> user_register_result = await user_account_authentication_service.Register(new_user_info); //Пррводимо реєстрацію користувача через сервіс внутрішньої аутентифікації
                if (user_register_result.Fail)
                {
                    return new FailQuery<ExternalOutputLoginUserDto>(user_register_result.Error);
                }
                user = user_register_result.Value; //Якщо користувач не був зареєстрований до того і ми його не знайшли в базі, присвоємо змінній User новоствореного користувача
            }
            string? profile_picture_url = google_user.FindFirst("urn:google:picture")?.Value; //Отримуємо з сервісів гугл фото профілю
            //Записуємо в профіль користувача посилання на зображення його профілю в сервісах гугл
            if (profile_picture_url != null)
            {
                user.User_Profile.Profile_Picture_Url = profile_picture_url.Replace("=s96-c", "=s400-c");
                await user_repository.UpdateUser(user);
            }
            //Генеруємо токен для користувача
            string token = user_account_authentication_service._GenerateJwtToken(user);
            return new SuccessQuery<ExternalOutputLoginUserDto>(new ExternalOutputLoginUserDto
            {
                User_Id = user.Id,
                Token = token,
            }, new SuccessMessage($"User {ConsoleLogService.PrintUser(user)} successfully logged in using Google services", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }
    }
}
