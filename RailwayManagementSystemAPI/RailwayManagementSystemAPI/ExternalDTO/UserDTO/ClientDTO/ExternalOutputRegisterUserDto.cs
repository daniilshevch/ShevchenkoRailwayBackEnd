using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO
{
    public class ExternalOutputRegisterUserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("surname")]
        public string Surname { get; set; } = null!;
        [JsonPropertyName("user_name")]
        public string User_Name { get; set; } = null!;
        [JsonPropertyName("sex")]
        public string? Sex { get; set; }
        [JsonPropertyName("phone_number")]
        public string? Phone_Number { get; set; }
        public string Role { get; set; } = null!;
        public static explicit operator ExternalOutputRegisterUserDto(User user)
        {
            return new ExternalOutputRegisterUserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                User_Name = user.User_Name,
                Sex = TextEnumConvertationService.GetUserSexIntoString(user.Sex),
                Phone_Number = user.Phone_Number,
                Role = TextEnumConvertationService.GetUserRoleIntoString(user.Role)
            };
        }

    }
}
