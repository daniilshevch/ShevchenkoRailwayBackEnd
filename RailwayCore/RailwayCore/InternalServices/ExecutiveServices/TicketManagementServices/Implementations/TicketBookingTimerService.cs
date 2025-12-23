using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models.ModelEnums.TicketBookingEnums;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces;
namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Implementations
{
    public class TicketBookingTimerService : ITicketBookingTimerService
    {
        private readonly AppDbContext context;
        public TicketBookingTimerService(AppDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Даний метод отримує всі квитки, для яких вже поїздка була завершена. Це потрібно для того, щоб інший
        /// метод поміняв їх статус на Archieved
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IQueryable<TicketBooking> GetAllTicketBookingsWithFinishedTrip(TimeSpan delay)
        {
            DateTime active_threshold = DateTime.Now - delay;
            IQueryable<TicketBooking> ticket_bookings_with_finished_trip = context.Ticket_Bookings
                .Join(context.Train_Routes_On_Date_On_Stations,
                ticket => new { Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id, Station_Id = ticket.Ending_Station_Id },
                train_stop => new { Train_Route_On_Date_Id = train_stop.Train_Route_On_Date_Id, Station_Id = train_stop.Station_Id },
                (ticket, train_stop) => new { ticket, train_stop })
                .Where(ticket_stop_couple => ticket_stop_couple.train_stop.Arrival_Time < active_threshold
                && (ticket_stop_couple.ticket.Ticket_Status == TicketStatus.Booked_And_Active 
                || ticket_stop_couple.ticket.Ticket_Status == TicketStatus.Booked_And_Used))
                .Select(ticket_stop_couple => ticket_stop_couple.ticket);
            return ticket_bookings_with_finished_trip;
        }
        /// <summary>
        /// Даний метод отримує всі квитки, які були тимчасово зарезервовані під час покупки, але які так і не 
        /// були викуплені, а час їх тимчасової резервації було вичерпано.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TicketBooking>> GetAllExpiredTicketBookings()
        {
            List<TicketBooking> expired_ticket_bookings = await context.Ticket_Bookings
                .Where(ticket_booking => ticket_booking.Ticket_Status == TicketStatus.Booking_In_Progress
                && ticket_booking.Booking_Expiration_Time < DateTime.Now).ToListAsync();
            return expired_ticket_bookings;
        }
        /// <summary>
        /// Даний метод видаляє всі квитки, які були тимчасово зарезервовані під час покупки, але які так і не 
        /// були викуплені, а час їх тимчасової резервації було вичерпано.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllExpiredTickets()
        {
            List<TicketBooking> expired_ticket_bookings = await GetAllExpiredTicketBookings();
            context.Ticket_Bookings.RemoveRange(expired_ticket_bookings);
            await context.SaveChangesAsync();
        }
    }
}
