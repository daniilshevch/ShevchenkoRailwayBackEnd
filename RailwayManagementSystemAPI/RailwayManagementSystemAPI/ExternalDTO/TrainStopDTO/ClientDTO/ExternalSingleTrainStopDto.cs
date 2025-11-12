using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO
{
    /// <summary>
    /// Даний клас представляє собою трансферний об'єкт для відображення одної зупинки 
    /// в розкладі руху рейсу поїзда між станціями поїздки пасажира. Список цих об'єктів входить в клас ExternalTrainRaceWithBookingsInfoDto
    /// </summary>
    [ClientDto]
    public class ExternalSingleTrainStopDto
    {
        [JsonPropertyName("station_title")]
        public string Station_Title { get; set; } = null!; //Назва станції
        [JsonPropertyName("arrival_time")]
        public DateTime? Arrival_Time { get; set; } //Час прибуття
        [JsonPropertyName("departure_time")]
        public DateTime? Departure_Time { get; set; } //Час відправлення
        [JsonPropertyName("stop_duration")]
        public TimeSpan? Stop_Duration { get; set; } //Тривалість зупинки
        [JsonPropertyName("is_part_of_trip")]
        public bool Is_Part_Of_Trip { get; set; }  //Чи є ця зупинка частиною маршруту бажаної поїздки
        [JsonPropertyName("is_final_trip_stop")]
        public bool Is_Final_Trip_Stop { get; set; } //Чи є ця зупинка останньою на маршруті бажаної поїздки
        [JsonPropertyName("distance_from_full_route_starting_station")]
        public double? Distance_From_Full_Route_Starting_Station { get; set; } //Відстань від початкової зупинки всього рейсу поїзда
    }
}
