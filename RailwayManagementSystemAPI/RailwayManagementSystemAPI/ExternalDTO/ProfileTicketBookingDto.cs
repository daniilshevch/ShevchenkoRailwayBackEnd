using RailwayCore.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO
{
    public class ExternalProfileTicketBookingDto
    {
        [JsonPropertyName("full_ticket_id")]
        public Guid? Full_Ticket_Id { get; set; }
        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!;
        [JsonPropertyName("full_route_starting_station_title")]
        public string Full_Route_Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("full_route_ending_station_title")]
        public string Full_Route_Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("trip_starting_station_title")]
        public string Trip_Starting_Station_Title { get;set; } = null!;
        [JsonPropertyName("departure_time")]
        public DateTime? Departure_Time_From_Trip_Starting_Station { get; set; }
        [JsonPropertyName("trip_ending_station_title")]
        public string Trip_Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("arrival_time")]
        public DateTime? Arrival_Time_To_Trip_Ending_Station { get; set; }
        [JsonPropertyName("trip_duration")]
        public TimeSpan? Trip_Duration { get; set; }
        [JsonPropertyName("carriage_position_in_squad")]
        public int? Passenger_Carriage_Position_In_Squad { get; set; }
        [JsonPropertyName("carriage_type")]
        public string Carriage_Type { get; set; } = null!;
        [JsonPropertyName("carriage_quality_class")]
        public string? Carriage_Quality_Class { get; set; }
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; }
        [JsonPropertyName("passenger_name")]
        public string Passenger_Name { get; set; } = null!;
        [JsonPropertyName("passenger_surname")]
        public string Passenger_Surname { get; set; } = null!;
        [JsonPropertyName("ticket_status")]
        public string Ticket_Status { get; set; } 
    }
}
