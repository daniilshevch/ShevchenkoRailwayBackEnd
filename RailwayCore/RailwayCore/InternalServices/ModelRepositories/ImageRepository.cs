using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ModelRepositories
{
    public class ImageRepository
    {
        private readonly AppDbContext context;
        public ImageRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<QueryResult<Image>> CreateUserProfileImage(UserProfileImageDto input)
        {
            UserProfile? user_profile = await context.User_Profiles.FirstOrDefaultAsync(user_profile => user_profile.User_Id == input.User_Id);
            if(user_profile is null)
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
                return new FailQuery<Image>(new Error(ErrorType.NotFound, $"Can't find user with ID: {user_id} or profile for this user"));
            }
            Image? profile_image = await context.Images.OrderBy(image => image.Id).LastOrDefaultAsync(image => image.User_Profile_Id == user_profile.Id &&
            image.Type == ImageType.Profile);
            if (profile_image is null)
            {
                return new FailQuery<Image>(new Error(ErrorType.NotFound, $"Can't find profile image for user with ID: {user_id}"));
            }
            return new SuccessQuery<Image>(profile_image);
        }
    }
}
