using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface IUserProfileManagementService
    {
        Task<QueryResult<ExternalOutputUserProfileDto>> GetUserInfoForAuthenticatedUser();
        Task<QueryResult<Image>> GetProfileImage();
        Task<QueryResult> UploadProfileImage(IFormFile image_file);
        Task<QueryResult<string>> GetProfileImageFromGoogleUrl();
    }
}