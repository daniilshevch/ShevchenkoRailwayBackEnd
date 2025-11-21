namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface IUserProfileManagementService
    {
        Task<QueryResult<Image>> GetProfileImage();
        Task<QueryResult> UploadProfileImage(IFormFile image_file);
    }
}