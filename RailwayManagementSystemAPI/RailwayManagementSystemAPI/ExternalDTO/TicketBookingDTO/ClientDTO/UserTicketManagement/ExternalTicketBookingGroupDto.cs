using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement
{
    /// <summary>
    /// Даний клас представляє з себе групу квитків для функціоналу перегляду користувачем своїх квитків. Групування проводиться на основі
    /// спільності рейсу поїзда та початкової і кінцевої станцій поїздки. Кожна група містить інформацію про поїздку, а також відповідний 
    /// список квитків
    /// </summary>
    [ClientDto]
    public class ExternalTicketBookingGroupDto
    {
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!;
        [JsonPropertyName("train_route_class")]
        public string Train_Route_Class { get; set; } = null!;
        [JsonPropertyName("train_route_branded_name")]
        public string Train_Route_Branded_Name { get; set; } = null!;   
        [JsonPropertyName("full_route_starting_station_title")]
        public string Full_Route_Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("full_route_ending_station_title")]
        public string Full_Route_Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("trip_starting_station_title")]
        public string Trip_Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("departure_time")]
        public DateTime? Departure_Time_From_Trip_Starting_Station { get; set; }
        [JsonPropertyName("trip_ending_station_title")]
        public string Trip_Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("arrival_time")]
        public DateTime? Arrival_Time_To_Trip_Ending_Station { get; set; }
        [JsonPropertyName("trip_duration")]
        public TimeSpan? Trip_Duration { get; set; }
        [JsonPropertyName("ticket_bookings_list")]
        public List<ExternalProfileTicketBookingDto> Ticket_Bookings_List { get; set; } = new List<ExternalProfileTicketBookingDto>();
    }
}
