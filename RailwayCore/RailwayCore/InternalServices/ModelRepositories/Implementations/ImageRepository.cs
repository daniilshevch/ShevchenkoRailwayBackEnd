using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayCore.Models;
using RailwayCore.Models.ModelEnums.ImageEnums;

namespace RailwayCore.InternalServices.ModelRepositories.Implementations
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext context;
        private readonly string service_name = "ImageRepository";
        public ImageRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<QueryResult<Image>> CreateUserProfileImage(UserProfileImageDto input)
        {
            UserProfile? user_profile = await context.User_Profiles.FirstOrDefaultAsync(user_profile => user_profile.User_Id == input.User_Id);
            if (user_profile is null)
            {
                return new FailQuery<Image>(new Error(ErrorType.NotFound, $"Can't find user profile for user with ID: {input.User_Id}"));
            }
            Image profile_image = new Image
            {
                Image_Data = input.Image_Data,
                Type = ImageType.Profile,
                User_Profile = user_profile
            };
            await context.Images.AddAsync(profile_image);
            await context.SaveChangesAsync();
            return new SuccessQuery<Image>(profile_image);
        }
        public async Task<QueryResult<Image>> GetUserProfileImage(int user_id)
        {
            UserProfile? user_profile = await context.User_Profiles.FirstOrDefaultAsync(user_profile => user_profile.User_Id == user_id);
            if (user_profile is null)
            {
                return new FailQuery<Image>(new Error(ErrorType.NotFound, $"Can't find user with ID: {user_id} or profile for this user", annotation: service_name, unit: ProgramUnit.Core));
            }
            Image? profile_image = await context.Images.OrderBy(image => image.Id).LastOrDefaultAsync(image => image.User_Profile_Id == user_profile.Id &&
            image.Type == ImageType.Profile);
            if (profile_image is null)
            {
                return new FailQuery<Image>(new Error(ErrorType.NotFound, $"Can't find profile image for user with ID: {user_id}", annotation: service_name, unit: ProgramUnit.Core));
            }
            return new SuccessQuery<Image>(profile_image, new SuccessMessage($"Successfully found profile image for user with ID: {user_id}", annotation: service_name, unit: ProgramUnit.Core));
        }
        public async Task<QueryResult<string>> GetUserProfileImageFromGoogleUrl(int user_id)
        {
            UserProfile? user_profile = await context.User_Profiles.FirstOrDefaultAsync(user_profile => user_profile.User_Id == user_id);
            if (user_profile is null)
            {
                return new FailQuery<string>(new Error(ErrorType.NotFound, $"Can't find user with ID: {user_id} or profile for this user", annotation: service_name, unit: ProgramUnit.Core));
            }
            if(user_profile.Profile_Picture_Url is null)
            {
                return new FailQuery<string>(new Error(ErrorType.NotFound, $"Can't find google picture url for user with ID: {user_id}", annotation: service_name, unit: ProgramUnit.Core));
            }
            return new SuccessQuery<string>(user_profile.Profile_Picture_Url, new SuccessMessage($"Successfully found google profile image url for user with ID: {user_id}", annotation: service_name, unit: ProgramUnit.Core));
        }
    }
}
