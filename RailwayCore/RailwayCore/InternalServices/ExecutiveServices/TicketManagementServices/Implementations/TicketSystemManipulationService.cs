using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Implementations
{
    public class TicketSystemManipulationService : ITicketSystemManipulationService
    {
        private readonly string service_name = "TicketSystemManipulationService";
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
            TicketBooking? ticket_booking = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Include(ticket_booking => ticket_booking.User)
                .FirstOrDefaultAsync(ticket_booking => ticket_booking.Id == ticket_booking_id);
            return ticket_booking;
        }
        public async Task<TicketBooking?> FindTicketBookingByFullId(string full_ticket_id)
        {
            TicketBooking? ticket_booking = await context.Ticket_Bookings
               .Include(ticket_booking => ticket_booking.Passenger_Carriage)
               .Include(ticket_booking => ticket_booking.Starting_Station)
               .Include(ticket_booking => ticket_booking.Ending_Station)
               .Include(ticket_booking => ticket_booking.User)
               .FirstOrDefaultAsync(ticket_booking => ticket_booking.Full_Ticket_Id.ToString() == full_ticket_id);
            return ticket_booking;
        }
        public async Task<List<TicketBooking>> FindSeveralTicketBookingsById(List<int> ticket_booking_ids)
        {
            List<TicketBooking> ticket_bookings = await context.Ticket_Bookings
               .Include(ticket_booking => ticket_booking.Passenger_Carriage)
               .Include(ticket_booking => ticket_booking.Starting_Station)
               .Include(ticket_booking => ticket_booking.Ending_Station)
               .Include(ticket_booking => ticket_booking.User)
               .Where(ticket_booking => ticket_booking_ids.Contains(ticket_booking.Id)).ToListAsync();   
            return ticket_bookings;
        }
        /// <summary>
        /// Оновлює квиток в базі новими даними
        /// </summary>
        /// <param name="ticket_booking"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<QueryResult<TicketBooking>> UpdateTicketBooking(TicketBooking ticket_booking)
        {
            context.Ticket_Bookings.Update(ticket_booking);
            await context.SaveChangesAsync();
            return new SuccessQuery<TicketBooking>(ticket_booking, new SuccessMessage($"Ticket was successfully updated. Info:\n" +
                $"{ConsoleLogService.PrintTicketBooking(ticket_booking)}", annotation: service_name, unit: ProgramUnit.Core));
        }
        /// <summary>
        /// Видаляє квиток з бази
        /// </summary>
        /// <param name="ticket_booking"></param>
        /// <returns></returns>
        public async Task<QueryResult<TicketBooking>> DeleteTicketBooking(TicketBooking ticket_booking)
        {
            context.Ticket_Bookings.Remove(ticket_booking);
            await context.SaveChangesAsync();
            return new SuccessQuery<TicketBooking>(ticket_booking, new SuccessMessage($"Successfuly cancelled reservation for place in ticket:\n" +
                $"{ConsoleLogService.PrintTicketBooking(ticket_booking)}", annotation: service_name, unit: ProgramUnit.Core));
        }
    }
}
