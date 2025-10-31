using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO
{
    /// <summary>
    /// Даний клас представляє собою інформацію про квиток, яка потрібна для його первинного резервування. Тут є лише загальна інформація про поїздку на цьому
    /// місці(поїзд, вагон, місце, станції прибуття та відправлення), але не конкретна інформація про пасажира та його подорож
    /// </summary>
    [ClientDto]
    public class ExternalInputInitialTicketBookingDto
    {
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!; //Айді рейсу поїзда
        [JsonPropertyName("passenger_carriage_position_in_squad")]
        public int Passenger_Carriage_Position_In_Squad { get; set; } //Номер вагона
        [JsonPropertyName("starting_station_title")]
        public string Starting_Station_Title { get; set; } = null!; //Початкова станція поїздки
        [JsonPropertyName("ending_station_title")]
        public string Ending_Station_Title { get; set; } = null!; //Кінцева станція поїздки
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; } //Місце в вагоні
    }
}
