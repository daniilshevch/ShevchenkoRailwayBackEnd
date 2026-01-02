using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.AdminDTO
{
    public class AdminPassengerTripInfoDto
    {
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; }
        [JsonPropertyName("passenger_trip_info")]
        public ExternalPassengerTripInfoDto Passenger_Trip_Info { get; set; } = null!;
    }
}
