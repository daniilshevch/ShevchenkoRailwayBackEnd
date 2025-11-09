using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models.ModelEnums.TicketBookingEnums;
namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices
{
    public class TicketBookingTimerService
    {
        private readonly AppDbContext context;
        public TicketBookingTimerService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TicketBooking>> GetAllExpiredTicketBookings()
        {
            List<TicketBooking> expired_ticket_bookings = await context.Ticket_Bookings
                .Where(ticket_booking => ticket_booking.Ticket_Status == TicketStatus.Booking_In_Progress
                && ticket_booking.Booking_Expiration_Time < DateTime.Now).ToListAsync();
            return expired_ticket_bookings;
        }
        public async Task DeleteAllExpiredTickets()
        {
            List<TicketBooking> expired_ticket_bookings = await GetAllExpiredTicketBookings();
            context.Ticket_Bookings.RemoveRange(expired_ticket_bookings);
            await context.SaveChangesAsync();
        }
    }
}
