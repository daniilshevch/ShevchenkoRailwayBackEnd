using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models.ModelEnums.TicketBookingEnums;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces;

namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Implementations
{
    /// <summary>
    /// Даний сервіс містить функціонал, який пов'язаний з операціями з квитками, які виконуються зі сторони користувача(не системи)
    /// </summary>
    public class TicketUserManipulationService : ITicketUserManipulationService
    {
        private readonly string service_name = "TicketUserManipulationService";
        private readonly AppDbContext context;
        public TicketUserManipulationService(AppDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Метод вертає список квитків, які приписані до акаунту користувача(якщо вибрано параметр only_active = true, то тільки ті, які мають статус активних
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public async Task<List<TicketBooking>> GetAllTicketBookingsForUser(int user_id, bool only_active = true)
        {
            List<TicketBooking> ticket_bookings = new List<TicketBooking>();
            if (only_active)
            {
                ticket_bookings = await context.Ticket_Bookings
                    .Include(ticket_booking => ticket_booking.Starting_Station)
                    .Include(ticket_booking => ticket_booking.Ending_Station)
                    .Include(ticket_booking => ticket_booking.User)
                    .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                    .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                    .ThenInclude(train_route_on_date => train_route_on_date.Train_Route)
                    .Where(ticket_booking => ticket_booking.User_Id == user_id
                    && (ticket_booking.Ticket_Status == TicketStatus.Booked_And_Active || ticket_booking.Ticket_Status == TicketStatus.Booked_And_Used))
                    .ToListAsync();
            }
            else
            {
                ticket_bookings = await context.Ticket_Bookings
                    .Include(ticket_booking => ticket_booking.Starting_Station)
                    .Include(ticket_booking => ticket_booking.Ending_Station)
                    .Include(ticket_booking => ticket_booking.User)
                    .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                    .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                    .ThenInclude(train_route_on_date => train_route_on_date.Train_Route)
                    .Where(ticket_booking => ticket_booking.User_Id == user_id).ToListAsync();
            }
            return ticket_bookings;
        }
       /// <summary>
       /// Метод вертає всі архівні квитки користувача, які можуть бути безпосередньо в статусі Archieved, якщо поїздка успішно завершена, або
       /// мати статус Returned, якщо користувач повернув квитки і не здійснив поїздку
       /// </summary>
       /// <param name="user_id"></param>
       /// <returns></returns>
        public async Task<List<TicketBooking>> GetAllArchievedAndReturnedTicketBookingsForUser(int user_id)
        {
            List<TicketBooking> ticket_bookings = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Include(ticket_booking => ticket_booking.User)
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                .ThenInclude(train_route_on_date => train_route_on_date.Train_Route)
                .Where(ticket_booking => ticket_booking.User_Id == user_id
                && (ticket_booking.Ticket_Status == TicketStatus.Archieved || ticket_booking.Ticket_Status == TicketStatus.Returned))
                .ToListAsync();
            return ticket_bookings;
        }
        /// <summary>
        /// Метод вертає всі тимчасові резервації місць за користувачами під час бронювання(ті, що розпочались після натиснення на кнопку місця, але 
        /// ще не закінчились)
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public async Task<List<TicketBooking>> GetAllTicketBookingsInProgressForUser(int user_id)
        {
            List<TicketBooking> ticket_bookings = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Include(ticket_booking => ticket_booking.User)
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                .ThenInclude(train_route_on_date => train_route_on_date.Train_Route)
                .Where(ticket_booking => ticket_booking.User_Id == user_id
                && (ticket_booking.Ticket_Status == TicketStatus.Booking_In_Progress))
                .ToListAsync();
            return ticket_bookings;
        }
        /// <summary>
        /// Даний метод проводить повернення квитка, раніше придбаного користувачем(по факту, ставить квитку в базі статус Returned) 
        /// </summary>
        /// <param name="ticket_id"></param>
        /// <returns></returns>
        public async Task<QueryResult<TicketBooking>> ReturnTicketBookingById(string ticket_id)
        {
            bool is_number = int.TryParse(ticket_id, out int number_id);
            TicketBooking? ticket_booking = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .FirstOrDefaultAsync(ticket => ticket.Id == number_id || ticket.Full_Ticket_Id.ToString() == ticket_id);
            if (ticket_booking is null)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't find ticket booking with ID: {ticket_id}", annotation: service_name, unit: ProgramUnit.Core));
            }
            if (ticket_booking.Ticket_Status != TicketStatus.Booked_And_Active)
            {
                return new FailQuery<TicketBooking>(new Error(ErrorType.NotFound, $"Can't return NOT ACTIVE ticket booking. Current status: {ticket_booking.Ticket_Status}",
                    annotation: service_name, unit: ProgramUnit.Core));
            }
            ticket_booking.Ticket_Status = TicketStatus.Returned;
            context.Update(ticket_booking);
            await context.SaveChangesAsync();
            return new SuccessQuery<TicketBooking>(ticket_booking, new SuccessMessage($"Successfully returned ticket booking " +
                $"with ID: {ticket_booking.Full_Ticket_Id}", annotation: service_name, unit: ProgramUnit.Core));
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
