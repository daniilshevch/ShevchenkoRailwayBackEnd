using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.ClientServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using System.Collections.Generic;

namespace RailwayManagementSystemAPI.ApiControllers.ClientControllers
{
    [ApiExplorerSettings(GroupName = "Client Controllers")]
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
    }
}
