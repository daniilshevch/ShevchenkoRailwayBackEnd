using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.ExternalDTO;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;

namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    public class UserTicketManagementController: ControllerBase
    {
        private readonly UserTicketManagementService user_ticket_management_service;
        public UserTicketManagementController(UserTicketManagementService user_ticket_management_service)
        {
            this.user_ticket_management_service = user_ticket_management_service;
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
        [HttpDelete("return-ticket-for-user/{ticket_id}")]
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
    }
}
