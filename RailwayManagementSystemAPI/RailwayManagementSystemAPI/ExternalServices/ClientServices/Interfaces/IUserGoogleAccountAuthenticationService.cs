using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface IUserGoogleAccountAuthenticationService
    {
        Task<QueryResult<ExternalOutputLoginUserDto>> LoginWithGoogle();
    }
}