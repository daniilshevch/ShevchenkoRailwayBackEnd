using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess
{
    public class ExternalInputTicketCompletionPairDto
    {
        [JsonPropertyName("mediator_ticket_booking")]
        public ExternalOutputMediatorTicketBookingDto Mediator_Ticket_Booking { get; set; } = null!;
        [JsonPropertyName("passenger_info")]
        public ExternalInputPassengerInfoForCompletedTicketBookingDto Passenger_Info { get; set; } = null!;
    }
}
