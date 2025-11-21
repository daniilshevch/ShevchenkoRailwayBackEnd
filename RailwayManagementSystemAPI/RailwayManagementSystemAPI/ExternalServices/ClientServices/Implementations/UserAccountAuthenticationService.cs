using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RailwayCore.Context;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using RailwayCore.Models.ModelEnums.UserEnums;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    /// <summary>
    /// Даний сервіс відповідає за аутентифікаційні операції користувача, такі як реєстрація та логін
    /// </summary>
    [ClientApiService]
    public class UserAccountAuthenticationService : IUserAccountAuthenticationService
    {
        private readonly string service_title = "UserAccountAuthenticationService";
        private readonly AppDbContext db_context;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor http_context_accessor;
        private readonly PasswordHasher<User> password_hasher = new PasswordHasher<User>();
        public UserAccountAuthenticationService(AppDbContext db_context, IConfiguration configuration, IHttpContextAccessor http_context_accessor)
        {
            this.db_context = db_context;
            this.configuration = configuration;
            this.http_context_accessor = http_context_accessor;
        }
        /// <summary>
        /// Проводить реєстрацію користувача
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ClientApiMethod]
        public async Task<QueryResult<User>> Register(ExternalInputRegisterUserDto input)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------USER ACCOUNT AUTHENTICATION-------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
            //Перевіряємо, чи користувач з такою поштою або іменем користувача вже існує в базі
            User? existing_user = await db_context.Users.FirstOrDefaultAsync(user => user.Email == input.Email || user.User_Name == input.User_Name);
            //Якщо такий користувач існує, видаємо помилку
            if (existing_user != null)
            {
                if (existing_user.Email == input.Email)
                {
                    return new FailQuery<User>(new Error(ErrorType.Conflict, $"User with email [{input.Email}] already exists", annotation: service_title, unit: ProgramUnit.ClientAPI));
                }
                else
                {
                    return new FailQuery<User>(new Error(ErrorType.Conflict, $"User with username [{input.User_Name}] already exists", annotation: service_title, unit: ProgramUnit.ClientAPI));
                }
            }
            Sex? sex = TextEnumConvertationService.GetUserSexIntoEnum(input.Sex);
            User new_user = new User
            {
                Email = input.Email,
                Surname = input.Surname,
                User_Name = input.User_Name,
                Name = input.Name,
                Sex = sex,
                Phone_Number = input.Phone_Number,
                Role = Role.General_User
            };
            //Перевіряємо, чи пароль проходить за вимогами
            bool appropriate_password = _VerifyPassword(input.Password);
            if (!appropriate_password)
            {
                return new FailQuery<User>(new Error(ErrorType.BadRequest, "Password must be between 8 and 24 symbols long, contain at least one uppercase letter," +
                    "lowercase letter and number", annotation: service_title, unit: ProgramUnit.ClientAPI));
            }
            //Хешуємо пароль
            new_user.Password = password_hasher.HashPassword(new_user, input.Password);
            await db_context.Users.AddAsync(new_user);
            await db_context.SaveChangesAsync();
            UserProfile user_profile = new UserProfile
            {
                User_Id = new_user.Id,
            };
            await db_context.User_Profiles.AddAsync(user_profile);
            await db_context.SaveChangesAsync();
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"User registration time: ");
            Console.ResetColor();
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            return new SuccessQuery<User>(new_user, new SuccessMessage($"User {ConsoleLogService.PrintUser(new_user)} has been successfully created",
                annotation: service_title, unit: ProgramUnit.ClientAPI));
        }
        /// <summary>
        /// Проводить логін користувача
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ClientApiMethod]
        public async Task<QueryResult<ExternalOutputLoginUserDto>> Login(ExternaInputlLoginUserDto input)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------USER ACCOUNT AUTHENTICATION-------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();
            //Отримуємо користувача за поштою
            User? user = await db_context.Users.FirstOrDefaultAsync(user => user.Email == input.Email);
            if (user == null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, "Invalid email or password", annotation: service_title, unit: ProgramUnit.ClientAPI));
            }
            //Перевіряємо пароль користувача
            PasswordVerificationResult verification_result = password_hasher.VerifyHashedPassword(user, user.Password, input.Password);
            if (verification_result == PasswordVerificationResult.Failed)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, "Invalid email or password", annotation: service_title, unit: ProgramUnit.ClientAPI));
            }
            //Генеруємо jwt-токен для користувача
            string token = _GenerateJwtToken(user);
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"User registration time: ");
            Console.ResetColor();
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            return new SuccessQuery<ExternalOutputLoginUserDto>(new ExternalOutputLoginUserDto
            {
                User_Id = user.Id,
                Token = token,
            }, new SuccessMessage($"User {ConsoleLogService.PrintUser(user)} successfully logged in",
            annotation: service_title, unit: ProgramUnit.ClientAPI));

        }
        /// <summary>
        /// Генерує jwt-токен під час логіну користувача
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [PartialLogicMethod]
        public string _GenerateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.User_Name),
                new Claim(ClaimTypes.Email, user.Email)
            };
            SecurityKey security_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtAuthentication:SecretKey"]!));
            SigningCredentials creds = new SigningCredentials(security_key, SecurityAlgorithms.HmacSha256);
            SecurityToken token = new JwtSecurityToken(
                audience: configuration["JwtAuthentication:Audience"],
                issuer: configuration["JwtAuthentication:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// Перевіряє пароль на відповідність вимогам
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [PartialLogicMethod]
        public static bool _VerifyPassword(string password)
        {
            if (password.Length < 8 || password.Length > 24)
            {
                return false;
            }
            bool has_upper = password.Any(char.IsUpper);
            bool has_lower = password.Any(char.IsLower);
            bool has_digit = password.Any(char.IsDigit);
            if (!has_upper || !has_lower || !has_digit)
            {
                return false;
            }
            return true;
        }
    }
}
