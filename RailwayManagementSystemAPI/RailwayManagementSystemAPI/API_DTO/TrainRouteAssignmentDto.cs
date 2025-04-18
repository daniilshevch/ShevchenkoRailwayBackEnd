using RailwayCore.DTO;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.API_DTO
{
    public class TrainRouteWithScheduleAssignmentDto
    {
        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!;
        [JsonPropertyName("departure_date")]
        public DateOnly Departure_Date { get; set; }
        [JsonPropertyName("creation_option")]
        public bool Creation_Option { get; set; }
        [JsonPropertyName("train_stops")]
        public List<TrainStopWithArrivalAndDepartureTimeDto> Train_Stops { get; set; } = new List<TrainStopWithArrivalAndDepartureTimeDto>();
    }
    public class TrainRouteWithSquadAssignmentDto
    {
        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!;
        [JsonPropertyName("departure_date")]
        public DateOnly Departure_Date { get; set; }
        [JsonPropertyName("creation_option")]
        public bool Creation_Option { get; set; }
        [JsonPropertyName("carriage_assignments")]
        public List<CarriageAssignementWithoutRouteDTO> Carriage_Assignments { get; set; } = new List<CarriageAssignementWithoutRouteDTO>();
    }
    public class TrainRouteWithScheduleAndSquadAssignmentDto
    {

        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!;
        [JsonPropertyName("departure_date")]
        public DateOnly Departure_Date { get; set; }
        [JsonPropertyName("creation_option")]
        public bool Creation_Option { get; set; }
        [JsonPropertyName("train_stops")]
        public List<TrainStopWithArrivalAndDepartureTimeDto> Train_Stops { get; set; } = new List<TrainStopWithArrivalAndDepartureTimeDto>();
        [JsonPropertyName("carriage_assignments")]
        public List<CarriageAssignementWithoutRouteDTO> Carriage_Assignments { get; set; } = new List<CarriageAssignementWithoutRouteDTO>();
    }
}
