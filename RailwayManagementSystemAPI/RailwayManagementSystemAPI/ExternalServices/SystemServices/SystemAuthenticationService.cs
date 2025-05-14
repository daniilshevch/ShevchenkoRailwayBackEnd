using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.Models;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices
{
    public class SystemAuthenticationService
    {
        private readonly IHttpContextAccessor http_context_accessor;
        private readonly AppDbContext db_context;
        public SystemAuthenticationService(IHttpContextAccessor http_context_accessor, AppDbContext db_context)
        {
            this.http_context_accessor = http_context_accessor;
            this.db_context = db_context;
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
                return new FailQuery<User>(new Error(ErrorType.InternalServerError, $"Can't find user with id [{id}]"));
            }
            return new SuccessQuery<User>(user_info);
        }
    }
}
