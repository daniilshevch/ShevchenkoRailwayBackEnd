using RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.AdminDTO
{
    public class AdminFullTicketStatisticsInfoForPassengerCarriageDto
    {
        [JsonPropertyName("passenger_carriage_bookings_info")]
        public ExternalSinglePassengerCarriageBookingsInfoDto Passenger_Carriage_Bookings_Info { get; set; } = null!;
        [JsonPropertyName("ticket_groups_by_starting_station")]
        public List<AdminTicketsGroupByStartingStationDto> Ticket_Groups_By_Starting_Station { get; set; } = new List<AdminTicketsGroupByStartingStationDto>();
        [JsonPropertyName("ticket_groups_by_ending_station")]
        public List<AdminTicketsGroupByEndingStationDto> Ticket_Groups_By_Ending_Station { get; set; } = new List<AdminTicketsGroupByEndingStationDto>();
    }
}
