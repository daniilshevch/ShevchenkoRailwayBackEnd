using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RailwayCore.Context;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ModelRepositories
{
    public class TicketBookingRepository
    {
        private readonly AppDbContext context;
        public TicketBookingRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<QueryResult<List<TicketBooking>>> GetTicketBookingsForTrainRouteOnDate(string train_route_on_date_id)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date.FirstOrDefaultAsync(train_route_on_date =>
                train_route_on_date.Id == train_route_on_date_id);
            if(train_route_on_date is null)
            {
                return new FailQuery<List<TicketBooking>>(new Error(ErrorType.NotFound, $"Can't find train race with ID: {train_route_on_date_id}"));
            }
            List<TicketBooking> ticket_bookings_for_train_route_on_date = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Where(ticket_booking =>
                ticket_booking.Train_Route_On_Date_Id == train_route_on_date_id)
                .ToListAsync();
            return new SuccessQuery<List<TicketBooking>>(ticket_bookings_for_train_route_on_date);
        }
        public async Task<QueryResult<List<TicketBooking>>> GetTicketBookingsForCarriageAssignment(string train_route_on_date_id, string passenger_carriage_id)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date.FirstOrDefaultAsync(train_route_on_date =>
                train_route_on_date.Id == train_route_on_date_id);
            if (train_route_on_date is null)
            {
                return new FailQuery<List<TicketBooking>>(new Error(ErrorType.NotFound, $"Can't find train race with ID: {train_route_on_date_id}"));
            }
            PassengerCarriage? passenger_carriage = await context.Passenger_Carriages.FirstOrDefaultAsync(passenger_carriage =>
                passenger_carriage.Id == passenger_carriage_id);
            if (passenger_carriage is null)
            {
                return new FailQuery<List<TicketBooking>>(new Error(ErrorType.NotFound, $"Can't find passenger carriage with ID: {passenger_carriage_id}"));
            }
            List<TicketBooking> ticket_bookings_for_carriage_assignment = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Where(ticket_booking =>
                ticket_booking.Train_Route_On_Date_Id == train_route_on_date_id && ticket_booking.Passenger_Carriage_Id == passenger_carriage_id)
                .ToListAsync();
            return new SuccessQuery<List<TicketBooking>>(ticket_bookings_for_carriage_assignment);
        }

    }
}
