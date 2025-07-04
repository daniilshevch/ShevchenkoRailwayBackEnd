using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices
{
    public class TicketUserManipulationService
    {
        private readonly AppDbContext context;
        public TicketUserManipulationService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TicketBooking>> GetAllTicketBookingsForUser(int user_id)
        {
            List<TicketBooking> ticket_bookings = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Include(ticket_booking => ticket_booking.User)
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                .ThenInclude(train_route_on_date => train_route_on_date.Train_Route)
                .Where(ticket_booking => ticket_booking.User_Id == user_id).ToListAsync();
            return ticket_bookings;
        }
        public async Task<TicketBooking?> ReturnTicketBookingById(string ticket_id)
        {
            bool is_number = int.TryParse(ticket_id, out int number_id);
            TicketBooking? ticket_booking = await context.Ticket_Bookings.FirstOrDefaultAsync(ticket => ticket.Id == number_id || ticket.Full_Ticket_Id.ToString() == ticket_id);
            if (ticket_booking is null)
            {
                return null;
            }
            ticket_booking.Ticket_Status = TicketStatus.Returned;
            context.Update(ticket_booking);
            await context.SaveChangesAsync();
            return ticket_booking;
        }
        public async Task<User?> GetTicketOwner(string ticket_id)
        {
            bool is_number = int.TryParse(ticket_id, out int number_id);
            TicketBooking? ticket_booking = await context.Ticket_Bookings.FirstOrDefaultAsync(ticket => ticket.Id == number_id || ticket.Full_Ticket_Id.ToString() == ticket_id);
            if (ticket_booking is null)
            {
                return null;
            }
            return ticket_booking.User;
        }

    }
}
