using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Implementations;
using System.Collections.Generic;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces;
using MailKit.Search;
namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Client Controllers")]
    public class UserTicketManagementController: ControllerBase
    {
        private readonly IUserTicketManagementService user_ticket_management_service;
        private readonly IPdfTicketGeneratorService pdf_ticket_generator_service;
        public UserTicketManagementController(IUserTicketManagementService user_ticket_management_service,
            IPdfTicketGeneratorService pdf_ticket_generator_service)
        {
            this.user_ticket_management_service = user_ticket_management_service;
            this.pdf_ticket_generator_service = pdf_ticket_generator_service;   
        }

        [HttpGet("get-tickets-for-current-user")]
        public async Task<ActionResult<List<ExternalProfileTicketBookingDto>>> GetAllTicketBookingsForCurrentUser()
        {
            QueryResult<List<ExternalProfileTicketBookingDto>> ticket_selection_result = 
                await user_ticket_management_service.GetAllBookedTicketsForCurrentUser();
            if (ticket_selection_result.Fail)
            {
                return ticket_selection_result.GetErrorFromQueryResult<List<ExternalProfileTicketBookingDto>, List<ExternalProfileTicketBookingDto>>();
            }
            List<ExternalProfileTicketBookingDto> tickets_for_user = ticket_selection_result.Value;
            return tickets_for_user;
        }
        [HttpGet("get-grouped-tickets-for-current-user")]
        public async Task<ActionResult<List<ExternalTicketBookingGroupDto>>> GetAllBookedTicketsInGroupsForCurrentUser()
        {
            QueryResult<List<ExternalTicketBookingGroupDto>> ticket_bookings_get_result = 
                await user_ticket_management_service.GetAllBookedTicketsInGroupsForCurrentUser();
            if(ticket_bookings_get_result.Fail)
            {
                return ticket_bookings_get_result.GetErrorFromQueryResult<List<ExternalTicketBookingGroupDto>, List<ExternalTicketBookingGroupDto>>();
            }
            List<ExternalTicketBookingGroupDto> ticket_groups = ticket_bookings_get_result.Value;
            return Ok(ticket_groups);
        }
        [HttpDelete("return-ticket-for-current-user/{ticket_id}")]
        public async Task<ActionResult<ExternalProfileTicketBookingDto>> ReturnTicketBookingForUserById([FromRoute] string ticket_id)
        {
            QueryResult<ExternalProfileTicketBookingDto> ticket_returning_result = await user_ticket_management_service.ReturnTicketBookingForCurrentUserById(ticket_id);
            if(ticket_returning_result.Fail)
            {
                return ticket_returning_result.GetErrorFromQueryResult<ExternalProfileTicketBookingDto, ExternalProfileTicketBookingDto>();
            }
            ExternalProfileTicketBookingDto returned_ticket = ticket_returning_result.Value;
            return returned_ticket;
        }
        [HttpPost("download-ticket-pdf")]
        public IActionResult DownloadTicketPdf([FromBody] ExternalProfileTicketBookingDto ticket_booking)
        {
            pdf_ticket_generator_service.TranslateTicketIntoUkrainian(ticket_booking);
            byte[] pdf_file = pdf_ticket_generator_service.GenerateTicketPdf(ticket_booking);
            return File(pdf_file, "application/pdf", $"ticket_{ticket_booking.Full_Ticket_Id}.pdf");
        }
    }
}
