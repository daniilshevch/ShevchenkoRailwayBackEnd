using RailwayCore.InternalDTO.ModelDTO;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface IImageRepository
    {
        Task<QueryResult<Image>> CreateUserProfileImage(UserProfileImageDto input);
        Task<QueryResult<Image>> GetUserProfileImage(int user_id);
    }
}