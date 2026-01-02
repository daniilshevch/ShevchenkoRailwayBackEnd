using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.AdminDTO
{
    public class AdminTicketsGroupByStartingStationDto
    {
        [JsonPropertyName("trip_starting_station_title")]
        public string Trip_Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("destination_groups")]
        public List<AdminDestinationTicketGroupDto> Destination_Groups { get; set; } = new List<AdminDestinationTicketGroupDto>();
    }
    public class AdminDestinationTicketGroupDto
    {
        [JsonPropertyName("trip_ending_station_title")]
        public string Trip_Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("passenger_trips_info")]
        public List<AdminPassengerTripInfoDto> Passenger_Trips_Info { get; set; } = new List<AdminPassengerTripInfoDto>();
    }
    public class AdminTicketsGroupByEndingStationDto
    {
        [JsonPropertyName("trip_ending_station_title")]
        public string Trip_Ending_Station_Title { get; set; } = null!;

        [JsonPropertyName("starting_station_groups")]
        public List<AdminStartingStationTicketGroupDto> Starting_Station_Groups { get; set; } = new List<AdminStartingStationTicketGroupDto>();
    }

    public class AdminStartingStationTicketGroupDto
    {
        [JsonPropertyName("trip_starting_station_title")]
        public string Trip_Starting_Station_Title { get; set; } = null!;

        [JsonPropertyName("passenger_trips_info")]
        public List<AdminPassengerTripInfoDto> Passenger_Trips_Info { get; set; } = new List<AdminPassengerTripInfoDto>();
    }
}
