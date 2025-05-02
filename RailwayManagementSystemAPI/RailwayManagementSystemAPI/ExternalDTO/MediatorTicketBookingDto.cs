using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.API_DTO
{
    public class ExternalOutputMediatorTicketBookingDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("user_id")]
        public int User_Id { get; set; }
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("passenger_carrriage_position_in_squad")]
        public int? Passenger_Carriage_Position_In_Squad { get; set; }
        [JsonPropertyName("passenger_carriage_id")]
        public string Passenger_Carriage_Id { get; set; } = null!;
        [JsonPropertyName("starting_station_title")]
        public string Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("ending_station_title")]
        public string Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; }
        [JsonPropertyName("ticket_status")]
        public string TicketStatus { get; set; } = null!;
        [JsonPropertyName("booking_initializing_time")]
        public DateTime Booking_Initializing_Time { get; set; }
        public DateTime? Booking_Expiration_Time { get; set; }

    }
}
