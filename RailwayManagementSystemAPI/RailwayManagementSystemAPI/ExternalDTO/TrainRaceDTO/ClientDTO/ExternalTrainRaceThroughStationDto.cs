using Newtonsoft.Json;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO;
using System.Text.Json.Serialization;
namespace RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.ClientDTO
{
    public class ExternalTrainRaceThroughStationDto
    {
        [JsonPropertyName("train_race_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!;
        [JsonPropertyName("full_route_starting_station_title")]
        public string Full_Route_Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("full_route_ending_station_title")]
        public string Full_Route_Ending_Station_Title{ get; set; } = null!;
        [JsonPropertyName("arrival_time_to_current_stop")]
        public DateTime? Arrival_Time_To_Current_Stop { get; set; }
        [JsonPropertyName("departure_time_from_current_stop")]
        public DateTime? Departure_Time_From_Current_Stop { get; set; }
        [JsonPropertyName("km_point_of_current_stop")]
        public double? Km_Point_Of_Current_Station { get; set; }
        [JsonPropertyName("full_route_stops_list")]
        public List<ExternalSingleTrainStopDto> Full_Route_Stops_List { get; set; } = new List<ExternalSingleTrainStopDto>();
    }
}
