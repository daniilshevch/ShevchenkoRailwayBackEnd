using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface IUserGoogleAccountAuthenticationService
    {
        Task<QueryResult<ExternalOutputLoginUserDto>> LoginWithGoogle(ClaimsPrincipal google_user);
    }
}