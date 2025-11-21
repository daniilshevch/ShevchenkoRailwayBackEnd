using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.CompleteTicketBookingProcess;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces
{
    public interface ICompleteTicketBookingProcessingService
    {
        Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> CancelTicketBookingReservationForUser(ExternalOutputMediatorTicketBookingDto input_unfinished_ticket);
        Task<QueryResult<ExternalOutputCompletedTicketBookingDto>> CompleteTicketBookingProcessForAuthenticatedUser(ExternalOutputMediatorTicketBookingDto input_unfinished_ticket, ExternalInputPassengerInfoForCompletedTicketBookingDto passenger_info);
        Task DeleteAllExpiredBookings();
        Task<QueryResult<List<ExternalOutputMediatorTicketBookingDto>>> InitializeMultipleTicketBookingProcessForAuthenticatedUser(List<ExternalInputInitialTicketBookingDto> ticket_bookings_list);
        Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> InitializeTicketBookingProcessForAuthenticatedUser(ExternalInputInitialTicketBookingDto input);
        Task<QueryResult<ExternalOutputMediatorTicketBookingDto>> InitializeTicketBookingProcessForUser(ExternalInputInitialTicketBookingDto input, User user);
    }
}