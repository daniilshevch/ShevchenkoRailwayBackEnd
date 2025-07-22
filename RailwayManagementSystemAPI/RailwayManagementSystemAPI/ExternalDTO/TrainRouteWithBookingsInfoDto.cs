using RailwayCore.Models;
using RailwayCore.InternalServices.CoreServices;
using System.Text.Json.Serialization;
namespace RailwayManagementSystemAPI.API_DTO
{
    [Checked("19.04.2025")]
    public class ExternalSinglePassengerCarriageBookingsInfoDto
    {
        [JsonPropertyName("carriage_position_in_squad")]
        public int Carriage_Position_In_Squad { get; set; } //Позиція вагона в складі поїзда
        [JsonPropertyName("carriage_type")]
        public string Carriage_Type { get; set; } = null!; //Тип вагона
        [JsonPropertyName("carriage_quality_class")]
        public string? Quality_Class { get; set; } //Підклас вагону
        [JsonPropertyName("ticket_price")]
        public int Ticket_Price { get; set; } //Ціна квитку в даному вагоні
        [JsonPropertyName("free_places")]
        public int Free_Places { get; set; } //Кількість вільних місць в вагоні
        [JsonPropertyName("total_places")]
        public int Total_Places { get; set; } //Загальна кількість місць в вагоні
        [JsonPropertyName("air_conditioning")]
        public bool Air_Conditioning { get; set; } //Наявність кондиціонування
        [JsonPropertyName("shower_availability")]
        public bool Shower_Availability { get; set; } //Наявність душу
        [JsonPropertyName("food_availability")]
        public bool Food_Availability { get; set; } //Доступність їжі в вагоні
        [JsonPropertyName("wifi_availability")]
        public bool WiFi_Availability { get; set; } //Наявність вай-фай
        [JsonPropertyName("is_inclusive")]
        public bool Is_Inclusive { get; set; } //Чи пристосований для перевезення пасажирів з відхиленнями
        [JsonPropertyName("places_availability_list")]
        public List<InternalSinglePlaceDto> Places_Availability { get; set; } = new List<InternalSinglePlaceDto>();

    }
    public class ExternalCarriageTypeGroupDto
    {
        [JsonPropertyName("free_places")]
        public int Free_Places { get; set; }
        [JsonPropertyName("total_places")]
        public int Total_Places { get; set; }
        [JsonPropertyName("min_price")]
        public double Min_Price { get; set; }
        [JsonPropertyName("max_price")]
        public double Max_Price { get; set; }
        [JsonPropertyName("carriage_quality_class_dictionary")]
        public Dictionary<string, ExternalCarriageTypeAndQualityGroupDto> Carriage_Quality_Class_Dictionary { get; set; } =
            new Dictionary<string, ExternalCarriageTypeAndQualityGroupDto>();
    }
    public class ExternalCarriageTypeAndQualityGroupDto
    {
        [JsonPropertyName("free_places")]
        public int Free_Places { get; set; }
        [JsonPropertyName("total_places")]
        public int Total_Places { get; set; }
        [JsonPropertyName("min_price")]
        public double Min_Price { get; set; }
        [JsonPropertyName("max_price")]
        public double Max_Price { get; set; }
        [JsonPropertyName("carriage_statistics_list")]
        public List<ExternalSinglePassengerCarriageBookingsInfoDto> Carriage_Statistics_List { get; set; } = new List<ExternalSinglePassengerCarriageBookingsInfoDto>();

    }

    [Checked("19.04.2025")]
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
    }
    [Checked("19.04.2025")]
    public class ExternalTrainRouteWithBookingsInfoDto
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
