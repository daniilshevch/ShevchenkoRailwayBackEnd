using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface IUserAccountAuthenticationService
    {
        Task<QueryResult<ExternalOutputLoginUserDto>> Login(ExternaInputlLoginUserDto input);
        Task<QueryResult<User>> Register(ExternalInputRegisterUserDto input);
        string _GenerateJwtToken(User user);
    }
}