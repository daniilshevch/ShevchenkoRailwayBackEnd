using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO
{
    public class ExternalCarriageAssignmentUpdateDto
    {
        [JsonPropertyName("position_in_squad")]
        public int? Position_In_Squad { get; set; }
        [JsonPropertyName("is_for_woman")]
        public bool? Is_For_Woman { get; set; } = false;
        [JsonPropertyName("is_for_children")]
        public bool? Is_For_Children { get; set; } = false;
        [JsonPropertyName("factual_air_conditioning")]
        public bool? Factual_Air_Conditioning { get; set; } = false;
        [JsonPropertyName("factual_shower_availability")]
        public bool? Factual_Shower_Availability { get; set; } = false;
        [JsonPropertyName("factual_is_inclusive")]
        public bool? Factual_Is_Inclusive { get; set; } = false;
        [JsonPropertyName("food_availability")]
        public bool? Food_Availability { get; set; } = false;
        
    }
}
