using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.UserDTO.ClientDTO
{
    public class ExternalInputRegisterUserDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;
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
    }
}
