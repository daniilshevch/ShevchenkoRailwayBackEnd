using RailwayCore.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using RailwayManagementSystemAPI.ExternalServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
    public class UserAccountManagementService
    {
        private readonly AppDbContext db_context;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor http_context_accessor;
        private readonly PasswordHasher<User> password_hasher = new PasswordHasher<User>();
        public UserAccountManagementService(AppDbContext db_context, IConfiguration configuration, IHttpContextAccessor http_context_accessor)
        {
            this.db_context = db_context;
            this.configuration = configuration;
            this.http_context_accessor = http_context_accessor;
        }
        [Refactored("v1", "02.05.2025")]
        public async Task<QueryResult<User>> Register(ExternalInputRegisterUserDto input)
        {
            User? existing_user = await db_context.Users.FirstOrDefaultAsync(user => user.Email == input.Email || user.User_Name == input.User_Name);
            if (existing_user != null)
            {
                if (existing_user.Email == input.Email)
                {
                    return new FailQuery<User>(new Error(ErrorType.Conflict, $"User with email [{input.Email}] already exists"));
                }
                else
                {
                    return new FailQuery<User>(new Error(ErrorType.Conflict, $"User with username [{input.User_Name}] already exists"));
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
            bool appropriate_password = VerifyPassword(input.Password);
            if(!appropriate_password)
            {
                return new FailQuery<User>(new Error(ErrorType.BadRequest, "Password must be between 8 and 24 symbols long, contain at least one uppercase letter," +
                    "lowercase letter and number"));
            }
            new_user.Password = password_hasher.HashPassword(new_user, input.Password);
            await db_context.Users.AddAsync(new_user);
            await db_context.SaveChangesAsync();
            return new SuccessQuery<User>(new_user);
        }
        [Checked("02.05.2025")]
        public async Task<QueryResult<ExternalOutputLoginUserDto>> Login(ExternaInputlLoginUserDto input)
        {
            User? user = await db_context.Users.FirstOrDefaultAsync(user => user.Email == input.Email);
            if (user == null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, "Invalid email or password"));
            }
            PasswordVerificationResult verification_result = password_hasher.VerifyHashedPassword(user, user.Password, input.Password);
            if (verification_result == PasswordVerificationResult.Failed)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, "Invalid email or password"));
            }
            string token = GenerateJwtToken(user);
            return new SuccessQuery<ExternalOutputLoginUserDto>(new ExternalOutputLoginUserDto
            {
                User_Id = user.Id,
                Token = token,
            });

        }
        public string GenerateJwtToken(User user)
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

        public static bool VerifyPassword(string password)
        {
            if(password.Length < 8 || password.Length > 24)
            {
                return false;
            }
            bool has_upper = password.Any(char.IsUpper);
            bool has_lower = password.Any(char.IsLower);
            bool has_digit = password.Any(char.IsDigit);
            if(!has_upper || !has_lower || !has_digit)
            {
                return false;
            }
            return true;
        }
    }
}
