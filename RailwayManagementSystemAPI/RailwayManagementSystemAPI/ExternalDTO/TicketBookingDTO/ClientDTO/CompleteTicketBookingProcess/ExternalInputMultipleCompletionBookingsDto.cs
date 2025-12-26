using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess
{
    public class ExternalInputMultipleCompletionBookingsDto
    {
        [JsonPropertyName("ticket_completion_info_list")]
        public List<ExternalInputTicketCompletionPairDto> Ticket_Completion_Info_List { get; set; } = null!;
    }
}
