using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO
{
    public class ExternalTrainRouteDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;
        [JsonPropertyName("is_branded")]
        public bool Is_Branded { get; set; } = false;
        [JsonPropertyName("quality_class")]
        public TrainQualityClass? Quality_Class { get; set; }
        [JsonPropertyName("trip_type")]
        public TripType? Trip_Type { get; set; }
        [JsonPropertyName("branded_name")]
        public string? Branded_Name { get; set; }
        [JsonPropertyName("speed_type")]
        public SpeedType? Speed_Type { get; set; }
        [JsonPropertyName("frequency_type")]
        public FrequencyType? Frequency_Type { get; set; }
        [JsonPropertyName("assignment_type")]
        public AssignementType? Assignement_Type { get; set; }
        [JsonPropertyName("railway_branch_title")]
        public string? Railway_Branch_Title { get; set; } = null!;
        [JsonPropertyName("train_route_coefficient")]
        public double? Train_Route_Coefficient { get; set; } = 1;
        public static explicit operator ExternalTrainRouteDto(TrainRoute input)
        {
            return new ExternalTrainRouteDto()
            {
                Id = input.Id,
                Is_Branded = input.Is_Branded,
                Quality_Class = input.Quality_Class,
                Trip_Type = input.Trip_Type,
                Branded_Name = input.Branded_Name,
                Speed_Type = input.Speed_Type,
                Frequency_Type = input.Frequency_Type,
                Assignement_Type = input.Assignement_Type,
                Railway_Branch_Title = input.Railway_Branch?.Title,
                Train_Route_Coefficient = input.Train_Route_Coefficient,
            };
        }
    }
}
