using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess
{
    /// <summary>
    /// Даний клас містить фінальну інфомрацію про повністю оформлений і придбаний квиток. Цей клас є типом, який 
    /// повертає метод, який проводить фінальне бронювання квитка для користувача(тому клас External)
    /// </summary>
    public class ExternalOutputCompletedTicketBookingDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } //Айді квитка
        [JsonPropertyName("user_id")]
        public int User_Id { get; set; } //Айді користувача, який придбав квиток
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!; //Айді рейсу поїзда
        [JsonPropertyName("passenger_carrriage_position_in_squad")]
        public int? Passenger_Carriage_Position_In_Squad { get; set; } //Номер вагона
        [JsonPropertyName("passenger_carriage_id")]
        public string Passenger_Carriage_Id { get; set; } = null!; //Айді вагона
        [JsonPropertyName("starting_station_title")]
        public string Starting_Station_Title { get; set; } = null!; //Початкова станція поїздки
        [JsonPropertyName("ending_station_title")]
        public string Ending_Station_Title { get; set; } = null!; //Кінцева станція поїздки
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; } //Місце в вагоні
        [JsonPropertyName("booking_completion_time")]
        public DateTime Booking_Completion_Time { get; set; } //Час фінального оформлення бронювання квитка(покупки)
        [JsonPropertyName("ticket_status")]
        public string Ticket_Status { get; set; } = null!; //Статус квитка(в випадку успіху - Booked_And_Active, якщо невдало - то Booking_Failed(для випадку купівлі декількох квитків, якшо квиток один - до етапу повернення дто буде FailQuery)
        [JsonPropertyName("passenger_name")]
        public string Passenger_Name { get; set; } = null!; //Ім'я пасажира
        [JsonPropertyName("passenger_surname")]
        public string Passenger_Surname { get; set; } = null!; //Прізвище пасажира
    }
}
