using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.CarriageAssistantServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using Swashbuckle.AspNetCore.Annotations;

namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.CarriageAssistantControllers
{
    [ApiController]
    [Route("Admin-API")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    [Authorize(Roles = "Administrator")]
    public class CarriageAssistantController: ControllerBase
    {
        private readonly ICarriageAssistantTicketAggregationService carriage_assistant_ticket_aggregation_service;
        public CarriageAssistantController(ICarriageAssistantTicketAggregationService carriage_assistant_ticket_aggregation_service)
        {
            this.carriage_assistant_ticket_aggregation_service = carriage_assistant_ticket_aggregation_service;
        }

        [HttpGet("get-grouped-ticket-bookings-by-stations-for-train-race/{train_route_on_date_id}")]
        public async Task<ActionResult<AdminFullTicketStatisticsInfoForPassengerCarriageDto>> GetGroupedTicketBookingsByStationsForTrainRace
            ([FromRoute] string train_route_on_date_id, [FromQuery] int carriage_position_in_squad, 
            [FromQuery] string? trip_starting_station_title, [FromQuery] string? trip_ending_station_title,
            [FromQuery] bool group_by_ending_station_first = false)
        {
            QueryResult<AdminFullTicketStatisticsInfoForPassengerCarriageDto> full_ticket_statistitics_get_result = await
                carriage_assistant_ticket_aggregation_service.GetGroupedTicketBookingsByStationsForTrainRace(train_route_on_date_id,
                carriage_position_in_squad, trip_starting_station_title, trip_ending_station_title, group_by_ending_station_first);
            if(full_ticket_statistitics_get_result.Fail)
            {
                return full_ticket_statistitics_get_result
                    .GetErrorFromQueryResult<AdminFullTicketStatisticsInfoForPassengerCarriageDto, AdminFullTicketStatisticsInfoForPassengerCarriageDto>();
            }
            return Ok(full_ticket_statistitics_get_result.Value);
        }

    }
}
