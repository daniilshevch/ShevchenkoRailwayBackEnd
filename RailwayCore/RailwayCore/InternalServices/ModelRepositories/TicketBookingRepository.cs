using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ModelRepositories
{
    public class TicketBookingForwardComparer: IComparer<TicketBooking>
    {
        private readonly Dictionary<string, int> StationsOrderDictionary;
        public TicketBookingForwardComparer(List<TrainRouteOnDateOnStation> train_stops)
        {
            StationsOrderDictionary = new Dictionary<string, int>();
            for(int stop_number = 0; stop_number < train_stops.Count; stop_number++)
            {
                StationsOrderDictionary.Add(train_stops[stop_number].Station.Title, stop_number);
            }
        }
        public int Compare(TicketBooking? first, TicketBooking? second)
        {
            if(first is null)
            {
                return -1;
            }    
            if(second is null)
            {
                return 1;
            }

            int first_ticket_starting_station_index = StationsOrderDictionary.GetValueOrDefault(first.Starting_Station.Title);
            int second_ticket_starting_station_index = StationsOrderDictionary.GetValueOrDefault(second.Starting_Station.Title);
            if (first_ticket_starting_station_index < second_ticket_starting_station_index)
            {
                return -1;
            }
            else if(first_ticket_starting_station_index > second_ticket_starting_station_index)
            {
                return 1;
            }
            else
            {
                int first_ticket_ending_station_index = StationsOrderDictionary.GetValueOrDefault(first.Ending_Station.Title);
                int second_ticket_ending_station_index = StationsOrderDictionary.GetValueOrDefault(second.Ending_Station.Title);
                if (first_ticket_ending_station_index < second_ticket_ending_station_index)
                {
                    return -1;
                }
                else if(first_ticket_ending_station_index > second_ticket_ending_station_index)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

        }
    }
    public class TicketBookingInvertedComparer : IComparer<TicketBooking>
    {
        private readonly Dictionary<string, int> StationsOrderDictionary;
        public TicketBookingInvertedComparer(List<TrainRouteOnDateOnStation> train_stops)
        {
            StationsOrderDictionary = new Dictionary<string, int>();
            for (int stop_number = 0; stop_number < train_stops.Count; stop_number++)
            {
                StationsOrderDictionary.Add(train_stops[stop_number].Station.Title, stop_number);
            }
        }
        public int Compare(TicketBooking? first, TicketBooking? second)
        {
            if (first is null)
            {
                return -1;
            }
            if (second is null)
            {
                return 1;
            }
            int first_ticket_ending_station_index = StationsOrderDictionary.GetValueOrDefault(first.Ending_Station.Title);
            int second_ticket_ending_station_index = StationsOrderDictionary.GetValueOrDefault(second.Ending_Station.Title);
            if (first_ticket_ending_station_index < second_ticket_ending_station_index)
            {
                return -1;
            }
            else if (first_ticket_ending_station_index > second_ticket_ending_station_index)
            {
                return 1;
            }
            else
            {
                int first_ticket_starting_station_index = StationsOrderDictionary.GetValueOrDefault(first.Starting_Station.Title);
                int second_ticket_starting_station_index = StationsOrderDictionary.GetValueOrDefault(second.Starting_Station.Title);
                if (first_ticket_starting_station_index < second_ticket_starting_station_index)
                {
                    return -1;
                }
                else if (first_ticket_starting_station_index > second_ticket_starting_station_index)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

        }
    }
    public class TicketBookingRepository
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateOnStationRepository train_route_on_date_on_station_repository;
        public TicketBookingRepository(AppDbContext context, 
            TrainRouteOnDateOnStationRepository train_route_on_date_on_station_repository)
        {
            this.context = context;
            this.train_route_on_date_on_station_repository = train_route_on_date_on_station_repository;
        }
        public async Task<QueryResult<List<TicketBooking>>> GetTicketBookingsForTrainRouteOnDate(string train_route_on_date_id, 
            bool inverted_sorting = false)
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
            QueryResult<List<TrainRouteOnDateOnStation>> train_stops_list_result = 
                await train_route_on_date_on_station_repository.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if(train_stops_list_result.Fail)
            {
                return new FailQuery<List<TicketBooking>>(train_stops_list_result.Error);
            }
            List<TrainRouteOnDateOnStation> train_stops_list = train_stops_list_result.Value;
            if (inverted_sorting == false)
            {
                ticket_bookings_for_train_route_on_date = ticket_bookings_for_train_route_on_date
                    .OrderBy(ticket_booking => ticket_booking, new TicketBookingForwardComparer(train_stops_list)).ToList();
            }
            else
            {
                ticket_bookings_for_train_route_on_date = ticket_bookings_for_train_route_on_date
                    .OrderBy(ticket_booking => ticket_booking, new TicketBookingInvertedComparer(train_stops_list)).ToList();
            }
            return new SuccessQuery<List<TicketBooking>>(ticket_bookings_for_train_route_on_date);
        }
        public async Task<QueryResult<List<TicketBooking>>> GetTicketBookingsForCarriageAssignment(string train_route_on_date_id, string passenger_carriage_id,
            bool inverted_sorting = false)
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
            QueryResult<List<TrainRouteOnDateOnStation>> train_stops_list_result =
    await train_route_on_date_on_station_repository.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (train_stops_list_result.Fail)
            {
                return new FailQuery<List<TicketBooking>>(train_stops_list_result.Error);
            }
            List<TrainRouteOnDateOnStation> train_stops_list = train_stops_list_result.Value;
            if (inverted_sorting == false)
            {
                ticket_bookings_for_carriage_assignment = ticket_bookings_for_carriage_assignment
                    .OrderBy(ticket_booking => ticket_booking, new TicketBookingForwardComparer(train_stops_list)).ToList();
            }
            else
            {
                ticket_bookings_for_carriage_assignment = ticket_bookings_for_carriage_assignment
                    .OrderBy(ticket_booking => ticket_booking, new TicketBookingInvertedComparer(train_stops_list)).ToList();
            }
            return new SuccessQuery<List<TicketBooking>>(ticket_bookings_for_carriage_assignment);
        }

    }
}
