using RailwayCore.Models;
using RailwayCore.InternalServices.ExecutiveServices.ExecutiveDTO.TicketManagementDTO;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Implementations;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Interfaces;
namespace RailwayCore.InternalServices.CoreServices
{
    /// <summary>
    /// Даний сервіс відповідає за весь функціонал, пов'язаний з квитками та бронями. Є сервісом-оркестратором, тобто надбудовою над
    /// іншими сервісами, які безпосередньо виконують квитковий функіонал: пошук бронювання на рейси, безпосереднє утворення квитків
    /// в базі, контроль таймера бронювання квитків, системні та користувацькі маніпуляції з квитками 
    /// </summary>
    [CoreService]
    public class FullTicketManagementService
    {
        private readonly ITicketAvailabilityCheckService ticket_availability_check_service;
        private readonly ITicketAllocationService ticket_allocation_service;
        private readonly ITicketBookingTimerService ticket_booking_timer_service;
        private readonly ITicketSystemManipulationService ticket_system_manipulation_service;
        private readonly ITicketUserManipulationService ticket_user_manipulation_service;

        public FullTicketManagementService(ITicketAvailabilityCheckService ticket_availability_check_service, ITicketAllocationService ticket_allocation_service, 
            ITicketBookingTimerService ticket_booking_timer_service, ITicketSystemManipulationService ticket_system_manipulation_service, 
            ITicketUserManipulationService ticket_user_manipulation_service)
        {
            this.ticket_availability_check_service = ticket_availability_check_service;
            this.ticket_allocation_service = ticket_allocation_service;
            this.ticket_booking_timer_service = ticket_booking_timer_service;
            this.ticket_system_manipulation_service = ticket_system_manipulation_service;
            this.ticket_user_manipulation_service = ticket_user_manipulation_service;
        }

        //TicketAllocationService
        [CoreMethod]
        public async Task<QueryResult<TicketBooking>> CreateTicketBooking(InternalTicketBookingDto input)
        {
            return await ticket_allocation_service.CreateTicketBooking(input);
        }
        [CoreMethod]
        public async Task<QueryResult<TicketBooking>> CreateTicketBookingWithCarriagePositionInSquad(InternalTicketBookingWithCarriagePositionDto input)
        {
            return await ticket_allocation_service.CreateTicketBookingWithCarriagePositionInSquad(input);
        }
        //TicketAvailabilityCheckService
        [CoreMethod]
        public async Task<QueryResult<bool>> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title,
            string desired_ending_station_title, string passenger_carriage_id, int place_in_carrriage)
        {
            return await ticket_availability_check_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id, desired_starting_station_title, desired_ending_station_title, passenger_carriage_id, place_in_carrriage);
        }
        [CoreMethod]
        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate
            (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {
            return await ticket_availability_check_service.GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate(train_route_on_date_ids, starting_station_title, ending_station_title);
        }
        [CoreMethod]
        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics
    (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {
            return await ticket_availability_check_service.GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics(train_route_on_date_ids, starting_station_title, ending_station_title);
        }
        //TicketSystemManipulationService
        [CoreMethod]
        public async Task<TicketBooking?> FindTicketBooking(int user_id, string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title, int place_in_carriage)
        {
            return await ticket_system_manipulation_service.FindTicketBooking(user_id, train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title, place_in_carriage);
        }
        /// <summary>
        /// Знаходить квиток по айді
        /// </summary>
        /// <param name="ticket_booking_id"></param>
        /// <returns></returns>
        [CoreMethod]
        public async Task<TicketBooking?> FindTicketBookingById(int ticket_booking_id)
        {
            return await ticket_system_manipulation_service.FindTicketBookingById(ticket_booking_id);
        }
        /// <summary>
        /// Оновлює квиток
        /// </summary>
        /// <param name="ticket_booking"></param>
        /// <returns></returns>
        [CoreMethod]
        public async Task<QueryResult<TicketBooking>> UpdateTicketBooking(TicketBooking ticket_booking)
        {
            return await ticket_system_manipulation_service.UpdateTicketBooking(ticket_booking);
        }
        /// <summary>
        /// Видаляє квиток
        /// </summary>
        /// <param name="ticket_booking"></param>
        /// <returns></returns>
        [CoreMethod]
        public async Task<QueryResult<TicketBooking>> DeleteTicketBooking(TicketBooking ticket_booking)
        {
            return await ticket_system_manipulation_service.DeleteTicketBooking(ticket_booking);
        }
        //TicketBookingTimerService
        [CoreMethod]
        public async Task<List<TicketBooking>> GetAllExpiredTicketBookings()
        {
            return await ticket_booking_timer_service.GetAllExpiredTicketBookings();
        }
        [CoreMethod]
        public async Task DeleteAllExpiredTickets()
        {
            await ticket_booking_timer_service.DeleteAllExpiredTickets();
        }
        //TicketUserManipulationService
        [CoreMethod]
        public async Task<List<TicketBooking>> GetAllTicketBookingsForUser(int user_id)
        {
            return await ticket_user_manipulation_service.GetAllTicketBookingsForUser(user_id);
        }
        [CoreMethod]
        public async Task<QueryResult<TicketBooking>> ReturnTicketBookingById(string ticket_id)
        {
            return await ticket_user_manipulation_service.ReturnTicketBookingById(ticket_id);
        }
        [CoreMethod]
        public async Task<User?> GetTicketOwner(string ticket_id)
        {
            return await ticket_user_manipulation_service.GetTicketOwner(ticket_id);
        }
    }
}
    