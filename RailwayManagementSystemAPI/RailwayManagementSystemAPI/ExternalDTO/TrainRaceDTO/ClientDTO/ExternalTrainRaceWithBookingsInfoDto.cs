using System.Text.Json.Serialization;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO;

namespace RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.ClientDTO
{
    /// <summary>
    /// Зовнішній трансферний об'єкт, який консолідує всю інформацію про рейс поїзда між станціями бажаної поїздки пасажира. Сюди входить інформація
    /// про саме курсування рейсу між станціями поїздки, склад поїзда з докладною інформацією про всі вагони, а також інформацію про вільні та заброньовані
    /// місця в вагонах в контексті поїздки пасажира. Також містить список всіх станцій рейса.
    /// </summary>
    [ClientDto]
    public class ExternalTrainRaceWithBookingsInfoDto
    {
        [JsonPropertyName("train_race_id")] //Айді рейсу
        public string Full_Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!; //Айді маршруту(не рейсу)
        [JsonPropertyName("train_route_branded_name")]
        public string? Train_Route_Branded_Name { get; set; } //Фірмова назва поїзда
        [JsonPropertyName("train_route_trip_type")]
        public string? Train_Route_Trip_Type { get; set; } = null!; //Тип курсування поїзда
        [JsonPropertyName("train_route_class")]
        public string? Train_Route_Class { get; set; } = null!; //Клас поїзда
        [JsonPropertyName("trip_starting_station_title")]
        public string Trip_Starting_Station_Title { get; set; } = null!;  //Назва початкової станції маршруту поїздки
        [JsonPropertyName("trip_starting_station_ukrainian_title")]
        public string? Trip_Starting_Station_Ukrainian_Title { get; set; } = null!;  //Назва початкової станції маршруту поїздки
        [JsonPropertyName("trip_ending_station_title")]
        public string Trip_Ending_Station_Title { get; set; } = null!;  //Назва кінцевої станції маршруту поїздки
        [JsonPropertyName("trip_ending_station_ukrainian_title")]
        public string? Trip_Ending_Station_Ukrainian_Title { get; set; } = null!;
        [JsonPropertyName("trip_starting_station_departure_time")]
        public DateTime Trip_Starting_Station_Departure_Time { get; set; }  //Час відправлення з початкової станції поїздки
        [JsonPropertyName("trip_ending_station_arrival_time")]
        public DateTime Trip_Ending_Station_Arrival_Time { get; set; }  //Час прибуття на кінцеву станцію поїздки
        [JsonPropertyName("total_trip_duration")]
        public TimeSpan Total_Trip_Duration { get; set; } //Загальний час поїздки 
        [JsonPropertyName("full_route_starting_station_title")]
        public string Full_Route_Starting_Station_Title { get; set; } = null!;  //Початкова станція всього маршруту даного поїзда
        [JsonPropertyName("full_route_starting_station_ukrainian_title")]
        public string? Full_Route_Starting_Station_Ukrainian_Title { get; set; } = null!;
        [JsonPropertyName("full_route_ending_station_title")]
        public string Full_Route_Ending_Station_Title { get; set; } = null!;  //Кінцева станція всього маршруту даного поїзда
        [JsonPropertyName("full_route_ending_station_ukrainian_title")]
        public string? Full_Route_Ending_Station_Ukrainian_Title { get; set; } = null!;
        [JsonPropertyName("average_speed_on_trip")]
        public double? Average_Speed_On_Trip { get; set; }  //Середня швидкість на маршруті поїздки
        [JsonPropertyName("is_cheapest")]
        public bool Is_Cheapest { get; set; } = false;
        [JsonPropertyName("is_fastest")]
        public bool Is_Fastest { get; set; } = false;
        [JsonPropertyName("carriage_statistics_list")]
        //Список об'єктів, що містять інформацію про кожен конкретний вагон в складі поїзда зі статистикою бронювань
        public List<ExternalSinglePassengerCarriageBookingsInfoDto> Carriage_Statistics_List { get; set; } = 
            new List<ExternalSinglePassengerCarriageBookingsInfoDto>();
        [JsonPropertyName("grouped_carriage_statistics_list")]
        public Dictionary<string, ExternalCarriageTypeGroupDto> Grouped_Carriage_Statistics_List { get; set; } =
            new Dictionary<string, ExternalCarriageTypeGroupDto>();
        [JsonPropertyName("train_schedule")]
        //Список станцій поїзда на маршруті прямування
        public List<ExternalSingleTrainStopDto> Train_Stops_List { get; set; } = new List<ExternalSingleTrainStopDto>();
    }
}
