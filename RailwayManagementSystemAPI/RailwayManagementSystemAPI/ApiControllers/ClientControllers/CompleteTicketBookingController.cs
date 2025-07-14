using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayManagementSystemAPI.API_DTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [Route("Client-API/[controller]")]
    public class CompleteTicketBookingController : ControllerBase
    {

        private readonly CompleteTicketBookingService complete_ticket_booking_service;
        public CompleteTicketBookingController(CompleteTicketBookingService complete_ticket_booking_service)
        {
            this.complete_ticket_booking_service = complete_ticket_booking_service;
        }
        [HttpPost("Initialize-Ticket-Booking")]
        public async Task<ActionResult<ExternalOutputMediatorTicketBookingDto>> InitializeTicketBookingProcess([FromBody] ExternalInputInitialTicketBookingDto input)
        {
            QueryResult<ExternalOutputMediatorTicketBookingDto> ticket_booking_initialization_result =
                await complete_ticket_booking_service.InitializeTicketBookingProcessForAuthenticatedUser(input);
            if(ticket_booking_initialization_result.Fail)
            {
                return ticket_booking_initialization_result.GetErrorFromQueryResult<ExternalOutputMediatorTicketBookingDto, ExternalOutputMediatorTicketBookingDto>();
            }
            ExternalOutputMediatorTicketBookingDto mediator_ticket_booking_dto = ticket_booking_initialization_result.Value;

            return Created($"/tickets/{mediator_ticket_booking_dto.Id}", mediator_ticket_booking_dto);
        }
        [HttpPost("Complete-Ticket-Booking")]
        public async Task<ActionResult<ExternalOutputCompletedTicketBookingDto>> CompleteTicketBookingProcess([FromBody] ExternalInputCompletedTicketBookingWithPassengerInfoDto input)
        {
            QueryResult<ExternalOutputCompletedTicketBookingDto> ticket_booking_completion_result =
                await complete_ticket_booking_service.CompleteTicketBookingProcessForAuthenticatedUser(input.ticket_booking_dto, input.user_info_dto);
            if(ticket_booking_completion_result.Fail)
            {
                return ticket_booking_completion_result.GetErrorFromQueryResult<ExternalOutputCompletedTicketBookingDto, ExternalOutputCompletedTicketBookingDto>();
            }
            ExternalOutputCompletedTicketBookingDto completed_ticket_booking_dto = ticket_booking_completion_result.Value;
            return Created($"/tickets/{completed_ticket_booking_dto.Id}", completed_ticket_booking_dto);
        }

    }
}
