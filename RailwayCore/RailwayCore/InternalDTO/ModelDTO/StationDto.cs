using RailwayCore.Models;
using RailwayCore.Models.ModelEnums.StationEnums;
using System.Text.Json.Serialization;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class StationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("register_id")]
        public string? Register_Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;
        [JsonPropertyName("location")]
        public string? Location { get; set; }
        [JsonPropertyName("type_of")]
        public StationType Type_Of { get; set; } = StationType.Mixed;
        [JsonPropertyName("locomotive_depot")]
        public string? Locomotive_Depot { get; set; }
        [JsonPropertyName("carriage_depot")]
        public string? Carriage_Depot { get; set; }
        [JsonPropertyName("railway_branch_title")]
        public string Railway_Branch_Title { get; set; } = null!;
        [JsonPropertyName("region")]
        public Region? Region { get; set; }

        public static explicit operator StationDto(Station input)
        {
            return new StationDto()
            {
                Id = input.Id,
                Register_Id = input.Register_Id,
                Title = input.Title,
                Location = input.Location,
                Type_Of = input.Type_Of,
                Locomotive_Depot = input.Locomotive_Depot,
                Carriage_Depot = input.Carriage_Depot,
                Railway_Branch_Title = input.Railway_Branch.Title,
                Region = input.Region
            };
        }
    }
            
}
