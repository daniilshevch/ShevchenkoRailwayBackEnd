using RailwayCore.Models;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO
{
    public class ExternalTrainStopUpdateDto
    {
        [JsonPropertyName("arrival_time")]
        public DateTime? Arrival_Time { get; set; }

        [JsonPropertyName("departure_time")]
        public DateTime? Departure_Time { get; set; }

        [JsonPropertyName("stop_type")]
        public StopType Stop_Type { get; set; } = StopType.Boarding;
        [JsonPropertyName("distance_from_starting_station")]
        public double? Distance_From_Starting_Station { get; set; }
        [JsonPropertyName("speed_on_section")]
        public double? Speed_On_Section { get; set; }
    } 
    
}
