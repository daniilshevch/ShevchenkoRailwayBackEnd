using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    public class UserProfileManagementService : IUserProfileManagementService
    {
        private readonly SystemAuthenticationService system_authentication_service;
        private readonly IImageRepository image_repository;
        public UserProfileManagementService(SystemAuthenticationService system_authentication_service, IImageRepository image_repository)
        {
            this.system_authentication_service = system_authentication_service;
            this.image_repository = image_repository;
        }
        public async Task<QueryResult> UploadProfileImage(IFormFile image_file)
        {
            if (image_file is null || image_file.Length == 0)
            {
                return new FailQuery(new Error(ErrorType.BadRequest, "File is empty"));
            }
            string[] allowed_types = new string[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed_types.Contains(image_file.ContentType))
            {
                return new FailQuery(new Error(ErrorType.BadRequest, "Invalid file type"));
            }
            byte[] image_data;
            using (MemoryStream memory_stream = new MemoryStream())
            {
                await image_file.CopyToAsync(memory_stream);
                image_data = memory_stream.ToArray();
            }
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery(user_authentication_result.Error);
            }
            User authenticated_user = user_authentication_result.Value;
            UserProfileImageDto image_dto = new UserProfileImageDto()
            {
                Image_Data = image_data,
                User_Id = authenticated_user.Id
            };
            QueryResult<Image> image_creation_result = await image_repository.CreateUserProfileImage(image_dto);
            if (image_creation_result.Fail)
            {
                return new FailQuery(image_creation_result.Error);
            }
            return new SuccessQuery();
        }
        public async Task<QueryResult<Image>> GetProfileImage()
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<Image>(user_authentication_result.Error);
            }
            User authenticated_user = user_authentication_result.Value;
            QueryResult<Image> image_get_result = await image_repository.GetUserProfileImage(authenticated_user.Id);
            if (image_get_result.Fail)
            {
                return new FailQuery<Image>(image_get_result.Error);
            }
            return new SuccessQuery<Image>(image_get_result.Value);
        }
    }
}
