using RailwayCore.Context;
using RailwayCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;

using System.Security.Claims;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
namespace RailwayManagementSystemAPI.ClientServices
{
    public class UserManagementService
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        public UserManagementService(AppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public async Task<QueryResult<User>> Register(ExternalInputRegisterUserDto input)
        {
            User? existing_user = await context.Users.FirstOrDefaultAsync(user => user.Email == input.Email || user.User_Name == input.User_Name);
            if(existing_user != null)
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
            if(input.Sex == "male")
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
            await context.Users.AddAsync(new_user);
            await context.SaveChangesAsync();
            return new SuccessQuery<User>(new_user);
        }
        public async Task<QueryResult<ExternalOutputLoginUserDto>> Login(ExternaInputlLoginUserDto input)
        {
            User? user = await context.Users.FirstOrDefaultAsync(user => user.Email == input.Email);
            if (user == null)
            {
                return new FailQuery<ExternalOutputLoginUserDto>(new Error(ErrorType.NotFound, "Can't find user with this email"));
            }
            PasswordHasher<User> password_hasher = new PasswordHasher<User>();
            PasswordVerificationResult verification_result = password_hasher.VerifyHashedPassword(user, user.Password, input.Password);
            if(verification_result == PasswordVerificationResult.Failed)
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
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.User_Name)
            };
            SecurityKey security_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtAuthentication:SecretKey"]!));
            SigningCredentials creds = new SigningCredentials(security_key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["JwtAuthentication:Issuer"],
                audience: configuration["JwtConfiguration:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
