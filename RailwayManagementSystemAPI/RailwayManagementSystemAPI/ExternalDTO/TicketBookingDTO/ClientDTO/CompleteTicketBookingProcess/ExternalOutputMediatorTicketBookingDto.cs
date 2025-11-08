using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess
{
    /// <summary>
    /// Даний клас повертає проміжну інформацію про ініціалізацію бронювання квитка. Це етап, коли користувач вже натиснув на місце в вагоні
    /// і вже почав заповнювати інформацію про пасажира, але ще в процесі. Квиток перебуває в статусі Booking_In_Progress. Метод, який проводить 
    /// проміжну резервацію квитка(місця в вагоні), вертає цей клас як результат своєї дії.
    /// </summary>
    [ClientDto]
    public class ExternalOutputMediatorTicketBookingDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } //Айді квитка
        [JsonPropertyName("full_ticket_id")] //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!НЕБЕЗПЕЧНА ЗОНА
        public string? Full_Ticket_Id { get; set; } //Повний айді квитка(GUID)
        [JsonPropertyName("user_id")]
        public int User_Id { get; set; } //Айді користувача
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!; //Айді рейсу поїзда
        [JsonPropertyName("passenger_carriage_position_in_squad")]
        public int? Passenger_Carriage_Position_In_Squad { get; set; } //Номер вагона в складі
        [JsonPropertyName("passenger_carriage_id")]
        public string Passenger_Carriage_Id { get; set; } = null!; //Айді вагона
        [JsonPropertyName("starting_station_title")]
        public string Starting_Station_Title { get; set; } = null!; //Початкова станція поїздки
        [JsonPropertyName("ending_station_title")]
        public string Ending_Station_Title { get; set; } = null!; //Кінцева станція поїздки
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; } //Номер в вагоні
        [JsonPropertyName("ticket_status")]
        public string TicketStatus { get; set; } = null!; //Статус квитка
        [JsonPropertyName("booking_initialization_time")]
        public DateTime Booking_Initializing_Time { get; set; } //Час початкова тимчасової резервації місця(місце при натисненні і початку оформлення квитка тимчасово в системі показується, як зайняте)
        [JsonPropertyName("booking_expiration_time")]
        public DateTime? Booking_Expiration_Time { get; set; } //Час, через який тимчасова резервація за користувачем, закінчиться

    }
}
