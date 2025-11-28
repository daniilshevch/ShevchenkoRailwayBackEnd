using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUser(User user);
        Task AddUserProfile(UserProfile user_profile);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByEmailOrUsername(string email, string user_name);
        Task UpdateUser(User user);
    }
}