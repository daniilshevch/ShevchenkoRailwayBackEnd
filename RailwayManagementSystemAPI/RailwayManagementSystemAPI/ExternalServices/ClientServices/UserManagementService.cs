using RailwayCore.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
    public class UserManagementService
    {
        private readonly AppDbContext db_context;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor http_context_accessor;
        public UserManagementService(AppDbContext db_context, IConfiguration configuration, IHttpContextAccessor http_context_accessor)
        {
            this.db_context = db_context;
            this.configuration = configuration;
            this.http_context_accessor = http_context_accessor;
        }
        public async Task<QueryResult<User>> Register(ExternalInputRegisterUserDto input)
        {
            User? existing_user = await db_context.Users.FirstOrDefaultAsync(user => user.Email == input.Email || user.User_Name == input.User_Name);
            if (existing_user != null)
            {
                if (existing_user.Email == input.Email)
                {
                    return new FailQuery<User>(new Error(ErrorType.BadRequest, "User with this email already exists"));
                }
                else
                {
                    return new FailQuery<User>(new Error(ErrorType.BadRequest, "User with this username already exists"));
                }
            }
            Sex sex;
            if (input.Sex == "male")
            {
                sex = Sex.Male;
            }
            else
            {
                sex = Sex.Female;
            }
            User new_user = new User
            {
                Email = input.Email,
                Surname = input.Surname,
                User_Name = input.User_Name,
                Name = input.Name,
                Sex = sex,
                Phone_Number = input.Phone_Number,
            };
            PasswordHasher<User> password_hasher = new PasswordHasher<User>();
            new_user.Password = password_hasher.HashPassword(new_user, input.Password);
            await db_context.Users.AddAsync(new_user);
            await db_context.SaveChangesAsync();
            return new SuccessQuery<User>(new_user);
        }
        public async Task<QueryResult<ExternalOutputLoginUserDto>> Login(ExternaInputlLoginUserDto input)
        {
            User? user = await db_context.Users.FirstOrDefaultAsync(user => user.Email == input.Email);
            if (user == null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.NotFound, "Can't find user with this email"));
            }
            PasswordHasher<User> password_hasher = new PasswordHasher<User>();
            PasswordVerificationResult verification_result = password_hasher.VerifyHashedPassword(user, user.Password, input.Password);
            if (verification_result == PasswordVerificationResult.Failed)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.Unauthorized, "Wrong password"));
            }
            string token = GenerateJwtToken(user);
            return new SuccessQuery<ExternalOutputLoginUserDto>(new ExternalOutputLoginUserDto
            {
                User_Id = user.Id,
                Token = token
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
        public async Task<QueryResult<User>> GetAuthenticatedUser()
        {
            HttpContext? http_context = http_context_accessor.HttpContext;
            if (http_context is null)
            {
                return new FailQuery<User>(new Error(ErrorType.InternalServerError, "Can't get access to HttpContext"));
            }
            ClaimsPrincipal user_principal = http_context.User;
            if (user_principal.Identity is null || !user_principal.Identity.IsAuthenticated)
            {
                return new FailQuery<User>(new Error(ErrorType.Unauthorized, "User is not authenticated"));
            }
            string? string_id = user_principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string_id is null)
            {
                return new FailQuery<User>(new Error(ErrorType.Unauthorized, "User is not authenticated"));
            }
            int id = Convert.ToInt32(string_id);
            User? user_info = await db_context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user_info is null)
            {
                return new FailQuery<User>(new Error(ErrorType.Unauthorized, "User is not authenticated"));
            }
            return new SuccessQuery<User>(user_info);
        }
    }
}
