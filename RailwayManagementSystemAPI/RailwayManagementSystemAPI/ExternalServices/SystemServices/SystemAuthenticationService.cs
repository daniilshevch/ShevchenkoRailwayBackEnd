using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices
{
    /// <summary>
    /// Даний сервіс є системним сервісом і призначений для отримання користувача, що аутентифікований в системі
    /// </summary>
    [SystemApiService]
    public class SystemAuthenticationService
    {
        private readonly string service_name = "SystemAuthenticationService";
        private readonly IHttpContextAccessor http_context_accessor;
        private readonly AppDbContext db_context;
        public SystemAuthenticationService(IHttpContextAccessor http_context_accessor, AppDbContext db_context)
        {
            this.http_context_accessor = http_context_accessor;
            this.db_context = db_context;
        }

        /// <summary>
        /// Отримує об'єкт користувача, що аутентифікований в системі(виконується в усіх запитах, які вимагають аутентифікацію та авторизацію)
        /// </summary>
        /// <returns></returns>
        [SystemApiMethod]
        public async Task<QueryResult<User>> GetAuthenticatedUser()
        {
            HttpContext? http_context = http_context_accessor.HttpContext;
            if (http_context is null)
            {
                return new FailQuery<User>(new Error(ErrorType.InternalServerError, "Can't get access to HttpContext", annotation: service_name, unit: ProgramUnit.SystemAPI));
            }
            ClaimsPrincipal user_principal = http_context.User;
            if (user_principal.Identity is null || !user_principal.Identity.IsAuthenticated)
            {
                return new FailQuery<User>(new Error(ErrorType.Unauthorized, "User is not authenticated", annotation: service_name, unit: ProgramUnit.SystemAPI));
            }
            string? string_id = user_principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string_id is null)
            {
                return new FailQuery<User>(new Error(ErrorType.Unauthorized, "User is not authenticated", annotation: service_name, unit: ProgramUnit.SystemAPI));
            }
            int id = Convert.ToInt32(string_id);
            User? user_info = await db_context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user_info is null)
            {
                return new FailQuery<User>(new Error(ErrorType.InternalServerError, $"Can't find user with id [{id}]", annotation: service_name, unit: ProgramUnit.SystemAPI));
            }
            return new SuccessQuery<User>(user_info, new SuccessMessage($"Successfuly authenticated user {ConsoleLogService.PrintUser(user_info)}", 
                annotation: service_name, unit: ProgramUnit.SystemAPI));
        }
    }
}
