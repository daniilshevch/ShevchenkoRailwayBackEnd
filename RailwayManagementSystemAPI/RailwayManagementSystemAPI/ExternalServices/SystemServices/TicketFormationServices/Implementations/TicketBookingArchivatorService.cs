using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces;
using RailwayCore.Migrations;
using RailwayCore.Models;
using RailwayCore.Models.ModelEnums.TicketBookingEnums;
namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Implementations
{
    public class TicketBookingArchivatorService: BackgroundService
    {
        private readonly string service_title = "TicketBookingArchivatorService";
        private readonly IServiceScopeFactory scope_factory;
        private readonly TimeSpan period = TimeSpan.FromMinutes(5);

        public TicketBookingArchivatorService(IServiceScopeFactory scope_factory)
        {
            this.scope_factory = scope_factory;
        }
        public async Task<QueryResult> ArchiveTicketsWithFinishedTrip(IQueryable<TicketBooking> ticket_bookings_with_finished_trip)
        {
            await ticket_bookings_with_finished_trip
                .ExecuteUpdateAsync(setter => setter
                .SetProperty(ticket => ticket.Ticket_Status, RailwayCore.Models.ModelEnums.TicketBookingEnums.TicketStatus.Archived));
            return new SuccessQuery(new SuccessMessage($"Tickets for finished trips have been successfully sent " +
                $"to archive at {DateTime.Now}", annotation: service_title, unit: ProgramUnit.SystemAPI));

        }
        protected override async Task ExecuteAsync(CancellationToken cancellation_token)
        {
            using PeriodicTimer timer = new PeriodicTimer(period);
            while(await timer.WaitForNextTickAsync(cancellation_token) && !cancellation_token.IsCancellationRequested)
            {
                try
                {
                    using(IServiceScope scope = scope_factory.CreateScope())
                    {
                        ITicketBookingTimerService ticket_booking_timer_service = scope.ServiceProvider.GetRequiredService<ITicketBookingTimerService>();
                        IQueryable<TicketBooking> ticket_bookings_with_finished_trip = ticket_booking_timer_service
                            .GetAllTicketBookingsWithFinishedTrip(TimeSpan.FromHours(3));
                        await ArchiveTicketsWithFinishedTrip(ticket_bookings_with_finished_trip);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
