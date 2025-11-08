using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices
{
    /// <summary>
    /// Даний сервіс містить функціонал, який пов'язаний з операціями з квитками, які виконуються зі сторони користувача(не системи)
    /// </summary>
    public class TicketUserManipulationService
    {
        private readonly AppDbContext context;
        public TicketUserManipulationService(AppDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Метод вертає список квитків, які приписані до акаунту користувача
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Даний метод проводить повернення квитка, раніше придбаного користувачем(по факту, ставить квитку в базі статус Returned) 
        /// </summary>
        /// <param name="ticket_id"></param>
        /// <returns></returns>
        public async Task<TicketBooking?> ReturnTicketBookingById(string ticket_id)
        {
            bool is_number = int.TryParse(ticket_id, out int number_id);
            TicketBooking? ticket_booking = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .FirstOrDefaultAsync(ticket => ticket.Id == number_id || ticket.Full_Ticket_Id.ToString() == ticket_id);
            if (ticket_booking is null)
            {
                return null;
            }
            ticket_booking.Ticket_Status = TicketStatus.Returned;
            context.Update(ticket_booking);
            await context.SaveChangesAsync();
            return ticket_booking;
        }
        /// <summary>
        /// Даний метод вертає користувача-власника квитка за числовим айді квитка або GUID
        /// </summary>
        /// <param name="ticket_id"></param>
        /// <returns></returns>
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
