using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations;
using System.Security.Claims;

namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Client Controllers")]
    [Route("Client-API/[controller]")]
    public class CompleteTicketBookingProcessingController : ControllerBase
    {

        private readonly ICompleteTicketBookingProcessingService complete_ticket_booking_processing_service;
        public CompleteTicketBookingProcessingController(ICompleteTicketBookingProcessingService complete_ticket_booking_processing_service)
        {
            this.complete_ticket_booking_processing_service = complete_ticket_booking_processing_service;
        }
        [HttpPost("Initialize-Ticket-Booking")]
        public async Task<ActionResult<ExternalOutputMediatorTicketBookingDto>> InitializeTicketBookingProcess([FromBody] ExternalInputInitialTicketBookingDto input)
        {
            QueryResult<ExternalOutputMediatorTicketBookingDto> ticket_booking_initialization_result =
                await complete_ticket_booking_processing_service.InitializeTicketBookingProcessForAuthenticatedUser(input);
            if(ticket_booking_initialization_result.Fail)
            {
                return ticket_booking_initialization_result.GetErrorFromQueryResult<ExternalOutputMediatorTicketBookingDto, ExternalOutputMediatorTicketBookingDto>();
            }
            ExternalOutputMediatorTicketBookingDto mediator_ticket_booking_dto = ticket_booking_initialization_result.Value;

            return Created($"/tickets/{mediator_ticket_booking_dto.Id}", mediator_ticket_booking_dto);
        }
        [HttpPost("Initialize-Multiple-Ticket-Bookings")]
        public async Task<ActionResult<List<ExternalOutputMediatorTicketBookingDto>>> InitializeMultipleTicketBookingProcess([FromBody] List<ExternalInputInitialTicketBookingDto> input)
        {
            QueryResult<List<ExternalOutputMediatorTicketBookingDto>> ticket_booking_initialization_result =
                await complete_ticket_booking_processing_service.InitializeMultipleTicketBookingProcessForAuthenticatedUser(input);
            if (ticket_booking_initialization_result.Fail)
            {
                return ticket_booking_initialization_result.GetErrorFromQueryResult<List<ExternalOutputMediatorTicketBookingDto>, List<ExternalOutputMediatorTicketBookingDto>>();
            }
            List<ExternalOutputMediatorTicketBookingDto> mediator_ticket_booking_dto_list = ticket_booking_initialization_result.Value;

            return Ok(mediator_ticket_booking_dto_list);
        }
        [HttpPost("Complete-Ticket-Booking")]
        public async Task<ActionResult<ExternalOutputCompletedTicketBookingDto>> CompleteTicketBookingProcess([FromBody] ExternalInputCompletedTicketBookingWithPassengerInfoDto input)
        {
            QueryResult<ExternalOutputCompletedTicketBookingDto> ticket_booking_completion_result =
                await complete_ticket_booking_processing_service.CompleteTicketBookingProcessForAuthenticatedUser(input.ticket_booking_dto, input.user_info_dto);
            if(ticket_booking_completion_result.Fail)
            {
                return ticket_booking_completion_result.GetErrorFromQueryResult<ExternalOutputCompletedTicketBookingDto, ExternalOutputCompletedTicketBookingDto>();
            }
            ExternalOutputCompletedTicketBookingDto completed_ticket_booking_dto = ticket_booking_completion_result.Value;
            return Created($"/tickets/{completed_ticket_booking_dto.Id}", completed_ticket_booking_dto);
        }
        [HttpDelete("Cancel-Ticket-Booking-Reservation")]
        public async Task<ActionResult<ExternalOutputMediatorTicketBookingDto>> CancelTicketBookingReservationForUser(ExternalOutputMediatorTicketBookingDto input_unfinished_ticket)
        {
            QueryResult<ExternalOutputMediatorTicketBookingDto> booking_cancellation_result = await
                complete_ticket_booking_processing_service.CancelTicketBookingReservationForUser(input_unfinished_ticket);
            if(booking_cancellation_result.Fail)
            {
                return booking_cancellation_result
                    .GetErrorFromQueryResult<ExternalOutputMediatorTicketBookingDto, ExternalOutputMediatorTicketBookingDto>();
            }
            return Ok(booking_cancellation_result.Value);
        }

    }
}
