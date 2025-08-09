using RailwayCore.InternalServices.ModelRepositories;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO;

namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices
{
    public class TicketBookingRepositoryService
    {
        public readonly TicketBookingRepository ticket_booking_repository;
        public TicketBookingRepositoryService(TicketBookingRepository ticket_booking_repository)
        {
            this.ticket_booking_repository = ticket_booking_repository;
        }
        public async Task<QueryResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForTrainRouteOnDate(string train_route_on_date_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result = 
                await ticket_booking_repository.GetTicketBookingsForTrainRouteOnDate(train_route_on_date_id);
            if(ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketBookingDto>>(ticket_bookings_get_result.Error);
            }
            List<ExternalTicketBookingDto> ticket_bookings = ticket_bookings_get_result.Value.Select(ticket_booking =>
                (ExternalTicketBookingDto)ticket_booking).ToList();
            return new SuccessQuery<List<ExternalTicketBookingDto>>(ticket_bookings);
        }
        public async Task<QueryResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForCarriageAssignment(string train_route_on_date_id, string passenger_carriage_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result =
                await ticket_booking_repository.GetTicketBookingsForCarriageAssignment(train_route_on_date_id, passenger_carriage_id);
            if (ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketBookingDto>>(ticket_bookings_get_result.Error);
            }
            List<ExternalTicketBookingDto> ticket_bookings = ticket_bookings_get_result.Value.Select(ticket_booking =>
                (ExternalTicketBookingDto)ticket_booking).ToList();
            return new SuccessQuery<List<ExternalTicketBookingDto>>(ticket_bookings);
        }
    }
}
