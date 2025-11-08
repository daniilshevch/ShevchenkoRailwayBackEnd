using System.Text.Json.Serialization;
namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess
{
    /// <summary>
    /// Даний клас містить інформацію, яку пасажир вводить про себе та свою поїздку вже під час фінального оформлення 
    /// і придбання квитка
    /// </summary>
    [ClientDto]
    public class ExternalInputPassengerInfoForCompletedTicketBookingDto
    {
        [JsonPropertyName("passenger_name")]
        public string Passenger_Name { get; set; } = null!; //Ім'я пасажира
        [JsonPropertyName("passenger_surname")]
        public string Passenger_Surname { get; set; } = null!; //Прізвище пасажира
    }
}
