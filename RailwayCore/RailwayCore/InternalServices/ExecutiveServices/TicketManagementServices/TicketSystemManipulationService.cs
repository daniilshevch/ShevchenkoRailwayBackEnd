using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices
{
    public class TicketSystemManipulationService
    {
        private readonly AppDbContext context;
        public TicketSystemManipulationService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<TicketBooking?> FindTicketBooking(int user_id, string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title, int place_in_carriage)
        {
            TicketBooking? ticket_booking = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .FirstOrDefaultAsync(ticket_booking =>
            ticket_booking.User_Id == user_id && ticket_booking.Train_Route_On_Date_Id == train_route_on_date_id
            && ticket_booking.Passenger_Carriage_Id == passenger_carriage_id && ticket_booking.Starting_Station.Title == starting_station_title
            && ticket_booking.Ending_Station.Title == ending_station_title && ticket_booking.Place_In_Carriage == place_in_carriage);
            return ticket_booking;
        }
        [Checked("02.05.2025")]
        public async Task<TicketBooking?> FindTicketBookingById(int ticket_booking_id)
        {
            TicketBooking? ticket_booking = await context.Ticket_Bookings.FirstOrDefaultAsync(ticket_booking => ticket_booking.Id == ticket_booking_id);
            return ticket_booking;
        }
        [Checked("02.05.2025")]
        public async Task UpdateTicketBooking(TicketBooking ticket_booking)
        {
            context.Ticket_Bookings.Update(ticket_booking);
            await context.SaveChangesAsync();
        }
        public async Task DeleteTicketBooking(TicketBooking ticket_booking)
        {
            context.Ticket_Bookings.Remove(ticket_booking);
            await context.SaveChangesAsync();
        }
    }
}
