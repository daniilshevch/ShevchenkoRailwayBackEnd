using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayCore.DTO;
using RailwayManagementSystemAPI.API_DTO;
using RailwayManagementSystemAPI.ClientServices;
using RailwayManagementSystemAPI.SystemServices;

namespace RailwayManagementSystemAPI.ClientControllers
{
    [ApiController]
    [Route("Client-API/[controller]")]
    public class CompletedTicketBookingController : ControllerBase
    {
        private readonly CompleteTicketBookingService complete_ticket_booking_service;
        public CompletedTicketBookingController(CompleteTicketBookingService complete_ticket_booking_service)
        {
            this.complete_ticket_booking_service = complete_ticket_booking_service;
        }
        [HttpPost("Initialize-Ticket-Booking")]
        public async Task<ActionResult<MediatorTicketBookingDto>> InitializeTicketBookingProcess(InitialTicketBookingDto input)
        {
            MediatorTicketBookingDto? mediator_ticket_booking = await complete_ticket_booking_service.InitializeTicketBookingProcess(input);
            if (mediator_ticket_booking == null)
            {
                Error? error = API_ErrorHandler.GetLastError();
                /*
                switch(error?.Error_Type)
                {
                    case ErrorType.NotFound:
                        return NotFound(error.Message);
                    case ErrorType.BadRequest:
                        return BadRequest(error.Message);
                    case ErrorType.Unathorized:
                        return Unauthorized(error.Message);
                    default:
                        return BadRequest("Unknown error");
                }*/
            }
            return Ok(mediator_ticket_booking);
        }
        [HttpPost("Complete-Ticket-Booking")]
        public async Task<ActionResult<CompletedTicketBookingDto?>> CompleteTicketBookingProcess(CompletedTicketBookingWithUserInfoDto input)
        {
            CompletedTicketBookingDto? completed_ticket_booking =
                await complete_ticket_booking_service.CompleteTicketBookingProcess(input.ticket_booking_dto, input.user_info_dto);
            if (completed_ticket_booking == null)
            {
                return BadRequest();
            }
            return Ok(completed_ticket_booking);
        }
    }
}
