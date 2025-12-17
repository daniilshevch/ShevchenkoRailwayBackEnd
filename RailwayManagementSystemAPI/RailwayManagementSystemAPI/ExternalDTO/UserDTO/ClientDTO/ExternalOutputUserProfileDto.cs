using RailwayCore.Models;
using RailwayCore.Models.ModelEnums.UserEnums;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO
{
    public class ExternalOutputUserProfileDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } 
        [JsonPropertyName("surname")]
        public string Surname { get; set; } = null!;
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
        [JsonPropertyName("user_name")]
        public string User_Name { get; set; } = null!;
        [JsonPropertyName("sex")]
        public Sex? Sex { get; set; }
        [JsonPropertyName("phone_number")]
        public string? Phone_Number { get; set; }
        [JsonPropertyName("user_profile_image")]
        public byte[]? User_Profile_Image { get; set; }
        [JsonPropertyName("user_google_profile_image_url")]
        public string? User_Google_Profile_Image_Url { get; set; }
        
        public static explicit operator ExternalOutputUserProfileDto(User user)
        {
            return new ExternalOutputUserProfileDto()
            {
                Id = user.Id,
                Surname = user.Surname,
                Name = user.Name,
                Email = user.Email,
                User_Name = user.User_Name,
                Sex = user.Sex,
                Phone_Number = user.Phone_Number
            };
        }
    }
}
