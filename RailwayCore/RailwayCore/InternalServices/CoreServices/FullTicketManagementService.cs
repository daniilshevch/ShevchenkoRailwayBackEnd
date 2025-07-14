using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ExecutiveServices;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using System.Reflection.Metadata.Ecma335;
namespace RailwayCore.InternalServices.CoreServices
{

    public class FullTicketManagementService
    {
        private readonly TicketAvailabilityCheckService ticket_availability_check_service;
        private readonly TicketAllocationService ticket_allocation_service;
        private readonly TicketBookingTimerService ticket_booking_timer_service;
        private readonly TicketSystemManipulationService ticket_system_manipulation_service;
        private readonly TicketUserManipulationService ticket_user_manipulation_service;

        public FullTicketManagementService(TicketAvailabilityCheckService ticket_availability_check_service, TicketAllocationService ticket_allocation_service, 
            TicketBookingTimerService ticket_booking_timer_service, TicketSystemManipulationService ticket_system_manipulation_service, 
            TicketUserManipulationService ticket_user_manipulation_service)
        {
            this.ticket_availability_check_service = ticket_availability_check_service;
            this.ticket_allocation_service = ticket_allocation_service;
            this.ticket_booking_timer_service = ticket_booking_timer_service;
            this.ticket_system_manipulation_service = ticket_system_manipulation_service;
            this.ticket_user_manipulation_service = ticket_user_manipulation_service;
        }

        public async Task<QueryResult<TicketBooking>> CreateTicketBooking(InternalTicketBookingDto input)
        {
            return await ticket_allocation_service.CreateTicketBooking(input);
        }

        public async Task<QueryResult<TicketBooking>> CreateTicketBookingWithCarriagePositionInSquad(InternalTicketBookingDtoWithCarriagePosition input)
        {
            return await ticket_allocation_service.CreateTicketBookingWithCarriagePositionInSquad(input);
        }

        public async Task<QueryResult<bool>> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title,
            string desired_ending_station_title, string passenger_carriage_id, int place_in_carrriage)
        {
            return await ticket_availability_check_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id, desired_starting_station_title, desired_ending_station_title, passenger_carriage_id, place_in_carrriage);
        }

        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate
            (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {
            return await ticket_availability_check_service.GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate(train_route_on_date_ids, starting_station_title, ending_station_title);
        }

        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics
    (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {
            return await ticket_availability_check_service.GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics(train_route_on_date_ids, starting_station_title, ending_station_title);
        }

        public async Task<TicketBooking?> FindTicketBooking(int user_id, string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title, int place_in_carriage)
        {
            return await ticket_system_manipulation_service.FindTicketBooking(user_id, train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title, place_in_carriage);
        }
        public async Task<TicketBooking?> FindTicketBookingById(int ticket_booking_id)
        {
            return await ticket_system_manipulation_service.FindTicketBookingById(ticket_booking_id);
        }
        [Checked("02.05.2025")]
        public async Task UpdateTicketBooking(TicketBooking ticket_booking)
        {
            await ticket_system_manipulation_service.UpdateTicketBooking(ticket_booking);
        }
        public async Task DeleteTicketBooking(TicketBooking ticket_booking)
        {
            await ticket_system_manipulation_service.DeleteTicketBooking(ticket_booking);
        }

        public async Task<List<TicketBooking>> GetAllExpiredTicketBookings()
        {
            return await ticket_booking_timer_service.GetAllExpiredTicketBookings();
        }
        public async Task DeleteAllExpiredTickets()
        {
            await ticket_booking_timer_service.DeleteAllExpiredTickets();
        }

        public async Task<List<TicketBooking>> GetAllTicketBookingsForUser(int user_id)
        {
            return await ticket_user_manipulation_service.GetAllTicketBookingsForUser(user_id);
        }
        public async Task<TicketBooking?> ReturnTicketBookingById(string ticket_id)
        {
            return await ticket_user_manipulation_service.ReturnTicketBookingById(ticket_id);
        }
        public async Task<User?> GetTicketOwner(string ticket_id)
        {
            return await ticket_user_manipulation_service.GetTicketOwner(ticket_id);
        }

















    }
}
    