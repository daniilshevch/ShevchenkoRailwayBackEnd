using Microsoft.AspNetCore.Mvc;
using RailwayCore.InternalServices.ModelRepositories;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;

namespace RailwayManagementSystemAPI.ApiControllers.AdminControllers.ModelRepositoryControllers
{
    [ApiController]
    [Route("Admin-API")]
    [ApiExplorerSettings(GroupName = "Admin Controllers")]
    public class TicketBookingRepositoryController: ControllerBase
    {
        private readonly TicketBookingRepositoryService ticket_booking_repository_service;
        public TicketBookingRepositoryController(TicketBookingRepositoryService ticket_booking_repository_service)
        {
            this.ticket_booking_repository_service = ticket_booking_repository_service;
        }
        [HttpGet("get-ticket-bookings-for-train-race/{train_route_on_date_id}")]
        public async Task<ActionResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForTrainRouteOnDate([FromRoute] string train_route_on_date_id)
        {
            QueryResult<List<ExternalTicketBookingDto>> ticket_bookings_get_result =
                await ticket_booking_repository_service.GetTicketBookingsForTrainRouteOnDate(train_route_on_date_id);
            if (ticket_bookings_get_result.Fail)
            {
                return ticket_bookings_get_result.GetErrorFromQueryResult<List<ExternalTicketBookingDto>, List<ExternalTicketBookingDto>>();
            }
            return Ok(ticket_bookings_get_result.Value);
        }
        [HttpGet("get-ticket-bookings-for-carriage-assignment/{train_route_on_date_id}/{passenger_carriage_id}")]
        public async Task<ActionResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForCarriageAssignment(string train_route_on_date_id, string passenger_carriage_id)
        {
            QueryResult<List<ExternalTicketBookingDto>> ticket_bookings_get_result =
                await ticket_booking_repository_service.GetTicketBookingsForCarriageAssignment(train_route_on_date_id, passenger_carriage_id);
            if (ticket_bookings_get_result.Fail)
            {
                return ticket_bookings_get_result.GetErrorFromQueryResult<List<ExternalTicketBookingDto>, List<ExternalTicketBookingDto>>();
            }
            return Ok(ticket_bookings_get_result.Value);
        }
    }

}
