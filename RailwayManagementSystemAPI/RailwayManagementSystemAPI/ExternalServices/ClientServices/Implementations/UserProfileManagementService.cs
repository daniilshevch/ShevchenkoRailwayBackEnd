using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayCore.InternalServices.SystemServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.SystemAuthenticationServices;
using RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations
{
    /// <summary>
    /// Даний сервіс відповідає за всі операції, що пов'язані з профілем користувача
    /// </summary>
    public class UserProfileManagementService : IUserProfileManagementService
    {
        private readonly SystemAuthenticationService system_authentication_service;
        private readonly IImageRepository image_repository;
        private readonly string service_name = "UserProfileManagementService";
        public UserProfileManagementService(SystemAuthenticationService system_authentication_service, IImageRepository image_repository)
        {
            this.system_authentication_service = system_authentication_service;
            this.image_repository = image_repository;
        }
        public async Task<QueryResult<ExternalOutputUserProfileDto>> GetUserInfoForAuthenticatedUser()
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<ExternalOutputUserProfileDto>(user_authentication_result.Error);
            }
            User authenticated_user = user_authentication_result.Value;
            ExternalOutputUserProfileDto user_profile = (ExternalOutputUserProfileDto)authenticated_user;
            QueryResult<Image> image_get_result_for_user = await image_repository.GetUserProfileImage(authenticated_user.Id);
            if (!image_get_result_for_user.Fail)
            {
                user_profile.User_Profile_Image = image_get_result_for_user.Value.Image_Data;
            }
            QueryResult<string> google_image_get_result_for_user = await image_repository.GetUserProfileImageFromGoogleUrl(authenticated_user.Id);
            if(!google_image_get_result_for_user.Fail)
            {
                user_profile.User_Google_Profile_Image_Url = google_image_get_result_for_user.Value;
            }
            return new SuccessQuery<ExternalOutputUserProfileDto>(user_profile, new SuccessMessage($"Successfully got profile info" +
                $"for user: {ConsoleLogService.PrintUser(authenticated_user)}", annotation: service_name, unit: ProgramUnit.ClientAPI));

        }
        /// <summary>
        /// Метод проводить встановлення зображення для профіля користувача
        /// </summary>
        /// <param name="image_file"></param>
        /// <returns></returns>
        public async Task<QueryResult> UploadProfileImage(IFormFile image_file)
        {
            if (image_file is null || image_file.Length == 0)
            {
                return new FailQuery(new Error(ErrorType.BadRequest, "File is empty", annotation: service_name, unit: ProgramUnit.ClientAPI));
            }
            string[] allowed_types = new string[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed_types.Contains(image_file.ContentType))
            {
                return new FailQuery(new Error(ErrorType.BadRequest, "Invalid file type", annotation: service_name, unit: ProgramUnit.ClientAPI));
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
            return new SuccessQuery(new SuccessMessage($"Successfully uploaded profile image", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }
        /// <summary>
        /// Даний метод вертає зображення, яке встановлене для профіля через внутрішню систему програми(не сервіси гугл)
        /// </summary>
        /// <returns></returns>
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
            return new SuccessQuery<Image>(image_get_result.Value, 
                new SuccessMessage($"Succesfully got profile image for user: " +
                $"{ConsoleLogService.PrintUser(authenticated_user)}", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }
        /// <summary>
        /// Даний метод вертає посилання на зображення профілю, що було отримане через сервіси гугл(записане в User_Profile для користувача, може бути null))
        /// </summary>
        /// <returns></returns>
        public async Task<QueryResult<string>> GetProfileImageFromGoogleUrl()
        {
            QueryResult<User> user_authentication_result = await system_authentication_service.GetAuthenticatedUser();
            if (user_authentication_result.Fail)
            {
                return new FailQuery<string>(user_authentication_result.Error);
            }
            User authenticated_user = user_authentication_result.Value;
            QueryResult<string> image_url_get_result = await image_repository.GetUserProfileImageFromGoogleUrl(authenticated_user.Id);
            if (image_url_get_result.Fail)
            {
                return new FailQuery<string>(image_url_get_result.Error);
            }
            return new SuccessQuery<string>(image_url_get_result.Value,
                new SuccessMessage($"Succesfully got google profile image url for user: " +
                $"{ConsoleLogService.PrintUser(authenticated_user)}", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }
    }
}
