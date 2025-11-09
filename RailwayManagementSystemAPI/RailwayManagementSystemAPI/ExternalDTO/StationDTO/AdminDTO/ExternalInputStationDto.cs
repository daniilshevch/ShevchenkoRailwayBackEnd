using Newtonsoft.Json;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models.ModelEnums.StationEnums;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.StationDTO.AdminDTO
{
    public class ExternalInputStationDto
    {
        [JsonPropertyName("register_id")]
        public string? Register_Id { get; set; } 
        [JsonProperty("title")]
        public string Title { get; set; } = null!; 
        [JsonPropertyName("ukrainian_title")]
        public string? Ukrainian_Title { get; set; }
        [JsonPropertyName("location")]
        public string? Location { get; set; } 
        [JsonPropertyName("type_of")]
        public StationType Type_Of { get; set; } = StationType.Mixed;
        [JsonPropertyName("region")]
        public Region Region { get; set; } 
        [JsonPropertyName("locomotive_depot")]
        public string? Locomotive_Depot { get; set; } 
        [JsonPropertyName("carriage_depot")]
        public string? Carriage_Depot { get; set; }
        [JsonPropertyName("railway_branch_title")]
        public string Railway_Branch_Title { get; set; } = null!; 

        public static explicit operator StationDto(ExternalInputStationDto input)
        {
            return new StationDto()
            {
                Register_Id = input.Register_Id,
                Title = input.Title,
                Location = input.Location,
                Type_Of = input.Type_Of,
                Region = input.Region,
                Locomotive_Depot = input.Locomotive_Depot,
                Carriage_Depot = input.Carriage_Depot,
                Railway_Branch_Title = input.Railway_Branch_Title
            };
        }
    }
}
