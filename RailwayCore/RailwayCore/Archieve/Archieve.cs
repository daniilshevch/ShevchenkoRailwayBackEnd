using RailwayCore.DTO;
using RailwayCore.Models;
using RailwayCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RailwayCore.Archieve.Archieve;

namespace RailwayCore.Archieve
{
    internal class Archieve
    {
        /*
        RailwayBranchService railway_branch_service = new RailwayBranchService(db);
        StationService station_service = new StationService(db, railway_branch_service);
        TrainRouteService train_route_service = new TrainRouteService(db, railway_branch_service);
        TrainRouteOnDateService train_route_on_date_service = new TrainRouteOnDateService(db, train_route_service);
        TrainRouteOnDateOnStationService train_route_on_date_on_station_service =
            new TrainRouteOnDateOnStationService(db, train_route_on_date_service, station_service);
        PassengerCarriageService passenger_carriage_service = new PassengerCarriageService(db, station_service);
        PassengerCarriageOnTrainRouteOnDateService passenger_carriage_on_train_route_on_date_service = new PassengerCarriageOnTrainRouteOnDateService
            (db, train_route_on_date_service, passenger_carriage_service);

        FullTrainAssignementService full_train_assignement_service = new FullTrainAssignementService(db, train_route_on_date_service,
            train_route_on_date_on_station_service, passenger_carriage_on_train_route_on_date_service, station_service, passenger_carriage_service);
        FullTrainRouteSearchService full_train_route_search_service = new FullTrainRouteSearchService(db, train_route_on_date_service, station_service, passenger_carriage_on_train_route_on_date_service);
        TicketBookingService ticket_booking_service = new TicketBookingService(db, train_route_on_date_service, station_service, passenger_carriage_service, full_train_route_search_service);
        ConsoleRepresentationService console_representation_service = new ConsoleRepresentationService(full_train_assignement_service, full_train_route_search_service, passenger_carriage_service, train_route_on_date_service, ticket_booking_service);
        */
        [Archieved]
        public async Task<bool?> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title, string desired_ending_station_title,
       string passenger_carriage_id, int place_in_carriage, bool safe_mode = true)
        {
            /*
            if (safe_mode)
            {
                TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
                if (train_route_on_date == null)
                {
                    text_service.FailPostInform("Fail in TrainRouteOnDateService");
                    return null;
                }
                PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(passenger_carriage_id);
                if (passenger_carriage == null)
                {
                    text_service.FailPostInform("Fail in PassengerCarriageService");
                    return null;
                }
                Station? desired_starting_station = await station_service.FindStationByTitle(desired_starting_station_title);
                Station? desired_ending_station = await station_service.FindStationByTitle(desired_ending_station_title);
                if (desired_starting_station == null || desired_ending_station == null)
                {
                    text_service.FailPostInform("Fail in StationService");
                    return null;
                }
            }
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }


            int desired_starting_stop_index = all_train_stops_of_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == desired_starting_station_title);
            int desired_ending_stop_index = all_train_stops_of_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == desired_ending_station_title);
            int total_stops_amount = all_train_stops_of_train_route_on_date.Count;

            //Перевірка бронювання квитків, у яких початкова і кінцева станція знаходяться по різні боки від тої зони, на яку ми хочемо купити квиток
            //(зона, на яку ми хочемо купити квиток, повністю лежить в межах зони іншого купленого квитка)
            for (int begin_stop_index = 0; begin_stop_index <= desired_starting_stop_index; begin_stop_index++)
            {
                for (int end_stop_index = desired_ending_stop_index; end_stop_index < total_stops_amount; end_stop_index++)
                {
                    Station? possible_start_station_of_existing_booking = all_train_stops_of_train_route_on_date[begin_stop_index].Station;
                    Station? possible_end_station_of_existing_booking = all_train_stops_of_train_route_on_date[end_stop_index].Station;
                    if (await context.Ticket_Bookings.AnyAsync(ticket => ticket.Passenger_Carriage_Id == passenger_carriage_id
                    && ticket.Train_Route_On_Date_Id == train_route_on_date_id && ticket.Place_In_Carriage == place_in_carriage
                    && ticket.Starting_Station_Id == possible_start_station_of_existing_booking.Id
                    && ticket.Ending_Station_Id == possible_end_station_of_existing_booking.Id))
                    {
                        return false;
                    }
                }
            }
            //Перевірка бронювання квитків, де хоча б одна зі станцій(початкова, кінцева або обидві) лежать в межах тої зони, на яку ми хочемо купити квиток
            //1) Обидві станції існуючого бронювання лежать в межах зони, на яку ми хочемо купити квиток(зона, в якій існує бронювання, повністю лежить 
            //в межах зони, на яку ми хочемо купити квиток

            //2) Початкова станція існуючого бронювання лежить лівіше зони покупки квитка, а кінцева лежить в межах зони покупки(частковий перетин зліва)
            //3) Початкова станція існуючого бронювання лежить в межах зони покупки квитка, а кінцева лежить правіше зони покупки(частковий перетин справа) 

            for (int stop_index = desired_starting_stop_index; stop_index <= desired_ending_stop_index; stop_index++)
            {
                Station? current_checking_station = all_train_stops_of_train_route_on_date[stop_index].Station;
                if (await context.Ticket_Bookings.AnyAsync(ticket => ticket.Passenger_Carriage_Id == passenger_carriage_id
                && ticket.Train_Route_On_Date_Id == train_route_on_date_id && ticket.Place_In_Carriage == place_in_carriage
                && ((ticket.Starting_Station_Id == current_checking_station.Id && stop_index != desired_ending_stop_index)
                || (ticket.Ending_Station_Id == current_checking_station.Id && stop_index != desired_starting_stop_index))))
                {
                    return false;
                }
            }

            return true;
            */
            return null;
        }








        //Alternative Train Route Search
        /*
        IEnumerable<TrainRouteOnDateOnStation> all_possible_train_stops = context.Train_Routes_On_Date_On_Stations
            .Include(train_stop => train_stop.Train_Route_On_Date)
            .Include(train_stop => train_stop.Station)
            .Where(train_stop => train_stop.Departure_Time.HasValue);

        List<TrainRouteOnDate> train_routes_on_date = await context.Train_Routes_On_Date
            .Include(train_route_on_date => train_route_on_date.Train_Stops)
            .Where(train_route_on_date_from_list => all_possible_train_stops.Any(train_stop =>
            train_stop.Train_Route_On_Date_Id == train_route_on_date_from_list.Id
            && DateOnly.FromDateTime((DateTime)train_stop.Departure_Time!) == departure_date
            && train_stop.Station.Title == start_station_title) &&
            all_possible_train_stops.Any(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_from_list.Id
            && train_stop.Station.Title == end_station_title)
         ).ToListAsync();*/






        [Archieved]
        public async Task PrintPlacesInPlatskartBetweenStations(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
        {
            /*
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
            if (train_route_on_date == null)
            {
                return;
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(passenger_carriage_id);
            if (passenger_carriage == null)
            {
                return;
            }
            int capacity = passenger_carriage.Capacity;
            for (int place = 2; place <= 36; place += 2)
            {
                bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id,
                    starting_station_title, ending_station_title, passenger_carriage_id, place);
                if (_place_availability == null)
                {
                    return;
                }
                bool place_availability = (bool)_place_availability;
                if (place_availability)
                {
                    if (place < 10)
                    {
                        if (place % 4 == 0)
                        {
                            Console.Write($"{TextService.Yellow}0{place}{TextService.Reset}|");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                        }
                    }
                    else
                    {
                        if (place % 4 == 0)
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                        }
                    }
                }
                else
                {
                    if (place < 10)
                    {
                        if (place % 4 == 0)
                        {
                            Console.Write($"0{place}|");
                        }
                        else
                        {
                            Console.Write($"0{place} ");
                        }
                    }
                    else
                    {
                        if (place % 4 == 0)
                        {
                            Console.Write($"{place}|");
                        }
                        else
                        {
                            Console.Write($"{place} ");
                        }
                    }
                }
            }
            Console.WriteLine();
            for (int place = 1; place <= 36; place += 2)
            {
                bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id,
                    starting_station_title, ending_station_title, passenger_carriage_id, place);
                if (_place_availability == null)
                {
                    return;
                }
                bool place_availability = (bool)_place_availability;
                if (place_availability)
                {
                    if (place < 10)
                    {
                        if ((place + 1) % 4 == 0)
                        {
                            Console.Write($"{TextService.Yellow}0{place}{TextService.Reset}|");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                        }
                    }
                    else
                    {
                        if ((place + 1) % 4 == 0)
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                        }
                    }
                }
                else
                {
                    if (place < 10)
                    {
                        if (place % 2 == 1)
                        {
                            Console.Write($"0{place}|");
                        }
                        else
                        {
                            Console.Write($"0{place} ");
                        }
                    }
                    else
                    {
                        if (place % 2 == 1)
                        {
                            Console.Write($"{place}|");
                        }
                        else
                        {
                            Console.Write($"{place} ");
                        }
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            for (int place = 54; place >= 37; place--)
            {
                bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id,
                    starting_station_title, ending_station_title, passenger_carriage_id, place);
                if (_place_availability == null)
                {
                    return;
                }
                bool place_availability = (bool)_place_availability;
                if (place_availability)
                {
                    if (place % 2 == 1)
                    {
                        Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                    }
                    else
                    {
                        if (place < 10)
                        {
                            Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                        }
                    }
                }
                else
                {
                    if (place % 2 == 1)
                    {
                        Console.Write($"{place}|");
                    }
                    else
                    {
                        if (place < 10)
                        {
                            Console.Write($"0{place}{TextService.Reset} ");
                        }
                        else
                        {
                            Console.Write($"{place} ");
                        }
                    }
                }
            }
            Console.WriteLine();
        }
        public async Task PrintPlacesInCoupeBetweenStations(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
        {
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
            if (train_route_on_date == null)
            {
                return;
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(passenger_carriage_id);
            if (passenger_carriage == null)
            {
                return;
            }
            int capacity = passenger_carriage.Capacity;
            for (int place = 2; place <= capacity; place += 2)
            {
                bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id,
                    starting_station_title, ending_station_title, passenger_carriage_id, place);
                if (_place_availability == null)
                {
                    return;
                }
                bool place_availability = (bool)_place_availability;
                if (place_availability)
                {
                    if (place % 4 == 0)
                    {
                        Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                    }
                    else
                    {
                        Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                    }
                }
                else
                {
                    if (place % 4 == 0)
                    {
                        Console.Write($"{place}|");
                    }
                    else
                    {
                        Console.Write($"{place} ");
                    }
                }
            }
            Console.WriteLine();
            for (int place = 1; place <= capacity; place += 2)
            {
                bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id,
                    starting_station_title, ending_station_title, passenger_carriage_id, place);
                if (_place_availability == null)
                {
                    return;
                }
                bool place_availability = (bool)_place_availability;
                if (place_availability)
                {
                    if ((place + 1) % 4 == 0)
                    {
                        Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                    }
                    else
                    {
                        if (place == 9)
                        {
                            Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                        }
                    }
                }
                else
                {
                    if ((place + 1) % 4 == 0)
                    {
                        Console.Write($"{place}|");
                    }
                    else
                    {
                        if (place == 9)
                        {
                            Console.Write($"0{place}{TextService.Reset} ");
                        }
                        else
                        {
                            Console.Write($"{place} ");
                        }
                    }
                }
            }
            Console.WriteLine();
        }
        

        public async Task PrintPlacesInSVBetweenStations(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
        {
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
            if (train_route_on_date == null)
            {
                return;
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(passenger_carriage_id);
            if (passenger_carriage == null)
            {
                return;
            }
            int capacity = passenger_carriage.Capacity;
            for (int place = 1; place <= capacity; place++)
            {
                bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id,
                    starting_station_title, ending_station_title, passenger_carriage_id, place);
                if (_place_availability == null)
                {
                    return;
                }
                bool place_availability = (bool)_place_availability;
                if (place_availability)
                {
                    if (place % 2 == 0)
                    {
                        Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                    }
                    else
                    {
                        if (place == 9)
                        {
                            Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                        }
                    }
                }
                else
                {
                    if (place % 2 == 0)
                    {
                        Console.Write($"{place}|");
                    }
                    else
                    {
                        if (place == 9)
                        {
                            Console.Write($"0{place}{TextService.Reset} ");
                        }
                        else
                        {
                            Console.Write($"{place} ");
                        }
                    }
                }
            }
            Console.WriteLine();
            */

        }










        [Archieved]
        public async Task<List<int>?> GetAllBookedTicketPlacesInCarriageBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title, string desired_ending_station_title,
   string passenger_carriage_id)
        {
            /*
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                /await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }

            int desired_starting_stop_index = all_train_stops_of_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == desired_starting_station_title);
            int desired_ending_stop_index = all_train_stops_of_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == desired_ending_station_title);
            int total_stops_amount = all_train_stops_of_train_route_on_date.Count;
            List<int> left_part = new List<int>();
            List<int> central_part = new List<int>();
            List<int> right_part = new List<int>();
            for (int stop_index = 0; stop_index <= desired_starting_stop_index; stop_index++)
            {
                left_part.Add(all_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            for (int stop_index = desired_starting_stop_index + 1; stop_index < desired_ending_stop_index; stop_index++)
            {
                central_part.Add(all_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            for (int stop_index = desired_ending_stop_index; stop_index < total_stops_amount; stop_index++)
            {
                right_part.Add(all_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            List<int> booked_places = await context.Ticket_Bookings
                .Where(ticket => ticket.Passenger_Carriage_Id == passenger_carriage_id && ticket.Train_Route_On_Date_Id == train_route_on_date_id
            && ((central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id))
            && (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id))))
                .Select(ticket => ticket.Place_In_Carriage).ToListAsync();
            return booked_places;
            */
            return null;
        }
        [Archieved]
        public class APIOutputService
        {
            private readonly FullTrainRouteSearchService full_train_route_search_service;
            private readonly TicketBookingService ticket_booking_service;
            private readonly TextService text_service = new TextService("APIOutputService");
            public APIOutputService(FullTrainRouteSearchService full_train_route_search_service, TicketBookingService ticket_booking_service)
            {
                this.full_train_route_search_service = full_train_route_search_service;
                this.ticket_booking_service = ticket_booking_service;

            }
            /*
            public async Task<CarriageAssignmentRepresentationDto?> GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(string train_route_on_date_id, int position_in_squad, string starting_station_title, string ending_station_title)
            {
                List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments_for_train_route_on_date = await full_train_route_search_service.GetPassengerCarriageAssignementsForTrainRouteOnDate(train_route_on_date_id);
                if (carriage_assignments_for_train_route_on_date == null)
                {
                    return null;
                }
                PassengerCarriageOnTrainRouteOnDate? desired_carriage_assignment = carriage_assignments_for_train_route_on_date.FirstOrDefault(carriage_assignment => carriage_assignment.Position_In_Squad == position_in_squad);
                if (desired_carriage_assignment == null)
                {
                    return null;
                }
                int carriage_capacity = desired_carriage_assignment.Passenger_Carriage.Capacity;
                List<SinglePlace> Places_List = new List<SinglePlace>();
                for (int place_number = 1; place_number <= carriage_capacity; place_number++)
                {
                    bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id, starting_station_title, ending_station_title, desired_carriage_assignment.Passenger_Carriage.Id, place_number);
                    if(_place_availability == null)
                    {
                        return null;
                    }
                    bool place_availability = (bool)_place_availability;
                    Places_List.Add(new SinglePlace()
                    {
                        Place_In_Carriage = place_number,
                        Is_Free = place_availability
                    });
                }
                return new CarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = desired_carriage_assignment,
                    Places_Availability = Places_List
                };
            }*/


            [Archieved]
            public async Task<CarriageAssignmentRepresentationDto?> GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
            {
                List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments_for_train_route_on_date = await full_train_route_search_service.GetPassengerCarriageAssignementsForTrainRouteOnDate(train_route_on_date_id);
                if (carriage_assignments_for_train_route_on_date == null)
                {
                    return null;
                }
                PassengerCarriageOnTrainRouteOnDate? desired_carriage_assignment = carriage_assignments_for_train_route_on_date.FirstOrDefault(carriage_assignment => carriage_assignment.Passenger_Carriage_Id == passenger_carriage_id);
                if (desired_carriage_assignment == null)
                {
                    return null;
                }
                int carriage_capacity = desired_carriage_assignment.Passenger_Carriage.Capacity;
                List<SinglePlace> Places_List = new List<SinglePlace>();
                for (int place_number = 1; place_number <= carriage_capacity; place_number++)
                {
                    bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_id, starting_station_title, ending_station_title, desired_carriage_assignment.Passenger_Carriage.Id, place_number);
                    if (_place_availability == null)
                    {
                        return null;
                    }
                    bool place_availability = (bool)_place_availability;
                    Places_List.Add(new SinglePlace()
                    {
                        Place_In_Carriage = place_number,
                        Is_Free = place_availability
                    });
                }
                return new CarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = desired_carriage_assignment,
                    Places_Availability = Places_List
                };
            }
            [Archieved]
            public async Task<CarriageAssignmentRepresentationDto?> GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDate desired_carriage_assignment, string starting_station_title, string ending_station_title)
            {
                int carriage_capacity = desired_carriage_assignment.Passenger_Carriage.Capacity;
                List<SinglePlace> Places_List = new List<SinglePlace>();
                for (int place_number = 1; place_number <= carriage_capacity; place_number++)
                {
                    bool? _place_availability = await ticket_booking_service.CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(desired_carriage_assignment.Train_Route_On_Date_Id, starting_station_title, ending_station_title, desired_carriage_assignment.Passenger_Carriage.Id, place_number);
                    if (_place_availability == null)
                    {
                        return null;
                    }
                    bool place_availability = (bool)_place_availability;
                    Places_List.Add(new SinglePlace()
                    {
                        Place_In_Carriage = place_number,
                        Is_Free = place_availability
                    });
                }
                return new CarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = desired_carriage_assignment,
                    Places_Availability = Places_List
                };
            }
            [Archieved]
            public async Task<List<CarriageAssignmentRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForTrainRouteOnDate(string train_route_on_date_id, string starting_station_title, string ending_station_title)
            {
                List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments_for_train_route_on_date = await full_train_route_search_service.GetPassengerCarriageAssignementsForTrainRouteOnDate(train_route_on_date_id);
                if (carriage_assignments_for_train_route_on_date == null)
                {
                    return null;
                }
                List<CarriageAssignmentRepresentationDto> result_list = new List<CarriageAssignmentRepresentationDto>();
                foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments_for_train_route_on_date)
                {
                    CarriageAssignmentRepresentationDto? single_carriage_statistics = await GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(carriage_assignment, starting_station_title, ending_station_title);
                    if (single_carriage_statistics == null)
                    {
                        return null;
                    }
                    result_list.Add(single_carriage_statistics);
                }
                return result_list;
            }

            [Archieved]
            public async Task<List<CarriageAssignmentRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids_list, string starting_station_title, string ending_station_title)
            {
                List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments_for_several_train_routes_on_date = await full_train_route_search_service.GetPassengerCarriageAssignementsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
                if (carriage_assignments_for_several_train_routes_on_date == null)
                {
                    return null;
                }
                List<CarriageAssignmentRepresentationDto> result_list = new List<CarriageAssignmentRepresentationDto>();
                foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments_for_several_train_routes_on_date)
                {
                    CarriageAssignmentRepresentationDto? single_carriage_statistics = await GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(carriage_assignment, starting_station_title, ending_station_title);
                    if (single_carriage_statistics == null)
                    {
                        return null;
                    }
                    result_list.Add(single_carriage_statistics);
                }
                return result_list;
            }




            [Archieved]
            public static int CountFreePlacesFromCarriageAssignmentRepresentation(CarriageAssignmentRepresentationDto carriage_assignment_statistics)
            {
                int free_places = 0;
                List<SinglePlace> places = carriage_assignment_statistics.Places_Availability;
                foreach (SinglePlace place in places)
                {
                    if (place.Is_Free)
                    {
                        free_places++;
                    }
                }
                return free_places;
            }
            [Archieved]
            public void PrintPlacesInCoupeBetweenStations(CarriageAssignmentRepresentationDto carriage_assignment_statistics, string starting_station_title, string ending_station_title)
            {
                int capacity = carriage_assignment_statistics.Carriage_Assignment.Passenger_Carriage.Capacity;
                for (int place = 2; place <= capacity; place += 2)
                {
                    bool? _place_availability = carriage_assignment_statistics.Places_Availability[place - 1].Is_Free;
                    if (_place_availability == null)
                    {
                        return;
                    }
                    bool place_availability = (bool)_place_availability;
                    if (place_availability)
                    {
                        if (place % 4 == 0)
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                        }
                        else
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                        }
                    }
                    else
                    {
                        if (place % 4 == 0)
                        {
                            Console.Write($"{place}|");
                        }
                        else
                        {
                            Console.Write($"{place} ");
                        }
                    }
                }
                Console.WriteLine();
                for (int place = 1; place <= capacity; place += 2)
                {
                    bool? _place_availability = carriage_assignment_statistics.Places_Availability[place - 1].Is_Free;
                    if (_place_availability == null)
                    {
                        return;
                    }
                    bool place_availability = (bool)_place_availability;
                    if (place_availability)
                    {
                        if ((place + 1) % 4 == 0)
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                        }
                        else
                        {
                            if (place == 9)
                            {
                                Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                            }
                            else
                            {
                                Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                            }
                        }
                    }
                    else
                    {
                        if ((place + 1) % 4 == 0)
                        {
                            Console.Write($"{place}|");
                        }
                        else
                        {
                            if (place == 9)
                            {
                                Console.Write($"0{place}{TextService.Reset} ");
                            }
                            else
                            {
                                Console.Write($"{place} ");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
            [Archieved]
            public async Task PrintPlacesInCoupeBetweenStations(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
            {
                /*
                CarriageAssignmentRepresentationDto? carriage_assignment_statistics = await api_output_service.GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title);
                if (carriage_assignment_statistics == null)
                {
                    return;
                }
                PrintPlacesInCoupeBetweenStations(carriage_assignment_statistics, starting_station_title, ending_station_title);
                */
            }
            [Archieved]
            public void PrintPlacesInSVBetweenStations(CarriageAssignmentRepresentationDto carriage_assignment_statistics, string starting_station_title, string ending_station_title)
            {
                int capacity = carriage_assignment_statistics.Carriage_Assignment.Passenger_Carriage.Capacity;

                for (int place = 1; place <= capacity; place++)
                {
                    bool? _place_availability = carriage_assignment_statistics.Places_Availability[place - 1].Is_Free;
                    if (_place_availability == null)
                    {
                        return;
                    }
                    bool place_availability = (bool)_place_availability;
                    if (place_availability)
                    {
                        if (place % 2 == 0)
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                        }
                        else
                        {
                            if (place == 9)
                            {
                                Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                            }
                            else
                            {
                                Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                            }
                        }
                    }
                    else
                    {
                        if (place % 2 == 0)
                        {
                            Console.Write($"{place}|");
                        }
                        else
                        {
                            if (place == 9)
                            {
                                Console.Write($"0{place}{TextService.Reset} ");
                            }
                            else
                            {
                                Console.Write($"{place} ");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
            [Archieved]
            public async Task PrintPlacesInSVBetweenStations(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
            {
                /*
                CarriageAssignmentRepresentationDto? carriage_assignment_statistics = await api_output_service.GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title);
                if (carriage_assignment_statistics == null)
                {
                    return;
                }
                PrintPlacesInSVBetweenStations(carriage_assignment_statistics, starting_station_title, ending_station_title);
                */
            }
            [Archieved]
            public void PrintPlacesInPlatskartBetweenStations(CarriageAssignmentRepresentationDto carriage_assignment_statistics, string starting_station_title, string ending_station_title)
            {
                int capacity = carriage_assignment_statistics.Carriage_Assignment.Passenger_Carriage.Capacity;
                for (int place = 2; place <= 36; place += 2)
                {
                    bool? _place_availability = carriage_assignment_statistics.Places_Availability[place - 1].Is_Free;
                    if (_place_availability == null)
                    {
                        return;
                    }
                    bool place_availability = (bool)_place_availability;
                    if (place_availability)
                    {
                        if (place < 10)
                        {
                            if (place % 4 == 0)
                            {
                                Console.Write($"{TextService.Yellow}0{place}{TextService.Reset}|");
                            }
                            else
                            {
                                Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                            }
                        }
                        else
                        {
                            if (place % 4 == 0)
                            {
                                Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                            }
                            else
                            {
                                Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                            }
                        }
                    }
                    else
                    {
                        if (place < 10)
                        {
                            if (place % 4 == 0)
                            {
                                Console.Write($"0{place}|");
                            }
                            else
                            {
                                Console.Write($"0{place} ");
                            }
                        }
                        else
                        {
                            if (place % 4 == 0)
                            {
                                Console.Write($"{place}|");
                            }
                            else
                            {
                                Console.Write($"{place} ");
                            }
                        }
                    }
                }
                Console.WriteLine();
                for (int place = 1; place <= 36; place += 2)
                {
                    bool? _place_availability = carriage_assignment_statistics.Places_Availability[place - 1].Is_Free;
                    if (_place_availability == null)
                    {
                        return;
                    }
                    bool place_availability = (bool)_place_availability;
                    if (place_availability)
                    {
                        if (place < 10)
                        {
                            if ((place + 1) % 4 == 0)
                            {
                                Console.Write($"{TextService.Yellow}0{place}{TextService.Reset}|");
                            }
                            else
                            {
                                Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                            }
                        }
                        else
                        {
                            if ((place + 1) % 4 == 0)
                            {
                                Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                            }
                            else
                            {
                                Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                            }
                        }
                    }
                    else
                    {
                        if (place < 10)
                        {
                            if (place % 2 == 1)
                            {
                                Console.Write($"0{place}|");
                            }
                            else
                            {
                                Console.Write($"0{place} ");
                            }
                        }
                        else
                        {
                            if (place % 2 == 1)
                            {
                                Console.Write($"{place}|");
                            }
                            else
                            {
                                Console.Write($"{place} ");
                            }
                        }
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
                for (int place = 54; place >= 37; place--)
                {
                    bool? _place_availability = carriage_assignment_statistics.Places_Availability[place - 1].Is_Free;
                    if (_place_availability == null)
                    {
                        return;
                    }
                    bool place_availability = (bool)_place_availability;
                    if (place_availability)
                    {
                        if (place % 2 == 1)
                        {
                            Console.Write($"{TextService.Yellow}{place}{TextService.Reset}|");
                        }
                        else
                        {
                            if (place < 10)
                            {
                                Console.Write($"{TextService.Yellow}0{place}{TextService.Reset} ");
                            }
                            else
                            {
                                Console.Write($"{TextService.Yellow}{place}{TextService.Reset} ");
                            }
                        }
                    }
                    else
                    {
                        if (place % 2 == 1)
                        {
                            Console.Write($"{place}|");
                        }
                        else
                        {
                            if (place < 10)
                            {
                                Console.Write($"0{place}{TextService.Reset} ");
                            }
                            else
                            {
                                Console.Write($"{place} ");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
            [Archieved]
            public async Task PrintPlacesInPlatskartBetweenStations(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
            {
                /*
                CarriageAssignmentRepresentationDto? carriage_assignment_statistics = await api_output_service.GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title);
                if (carriage_assignment_statistics == null)
                {
                    return;
                }
                PrintPlacesInPlatskartBetweenStations(carriage_assignment_statistics, starting_station_title, ending_station_title);
                */
            }
            [Archieved]

            public async Task PrintPlacesInCarriageBetweenStations(CarriageAssignmentRepresentationDto carriage_assignment_statistics, string starting_station_title, string ending_station_title)
            {
                TrainRouteOnDate train_route_on_date = carriage_assignment_statistics.Carriage_Assignment.Train_Route_On_Date;
                PassengerCarriage passenger_carriage = carriage_assignment_statistics.Carriage_Assignment.Passenger_Carriage;
                PassengerCarriageType type = passenger_carriage.Type_Of;
                switch (type)
                {
                    case PassengerCarriageType.Platskart:
                        PrintPlacesInPlatskartBetweenStations(carriage_assignment_statistics, starting_station_title, ending_station_title);
                        break;
                    case PassengerCarriageType.Coupe:
                        PrintPlacesInCoupeBetweenStations(carriage_assignment_statistics, starting_station_title, ending_station_title);
                        break;
                    case PassengerCarriageType.SV:
                        PrintPlacesInSVBetweenStations(carriage_assignment_statistics, starting_station_title, ending_station_title);
                        break;
                }
            }
            [Archieved]
            public async Task PrintPlacesInCarriageBetweenStations(string train_route_on_date_id, string passenger_carriage_id, string starting_station_title, string ending_station_title)
            {
                /*
                PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(passenger_carriage_id);
                if (passenger_carriage == null)
                {
                    return;
                }
                PassengerCarriageType type = passenger_carriage.Type_Of;
                switch (type)
                {
                    case PassengerCarriageType.Platskart:
                        await PrintPlacesInPlatskartBetweenStations(train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title);
                        break;
                    case PassengerCarriageType.Coupe:
                        await PrintPlacesInCoupeBetweenStations(train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title);
                        break;
                    case PassengerCarriageType.SV:
                        await PrintPlacesInSVBetweenStations(train_route_on_date_id, passenger_carriage_id, starting_station_title, ending_station_title);
                        break;

                }
                */
            }
            [Archieved]
            public async Task PrintPlacesBetweenStationForAllCarriagesForTrainRouteOnDate(string train_route_on_date_id, string starting_station_title, string ending_station_title)
            {
                /*
                List<CarriageAssignmentRepresentationDto>? carriage_statistics = await api_output_service.GetAllPassengerCarriagesPlacesReportForTrainRouteOnDate(train_route_on_date_id, starting_station_title, ending_station_title);
                if (carriage_statistics == null)
                {
                    return;
                }
                int free_sv_places = 0;
                int total_sv_places = 0;
                int free_coupe_places = 0;
                int total_coupe_places = 0;
                int free_platskart_places = 0;
                int total_platskart_places = 0;

                foreach (CarriageAssignmentRepresentationDto single_carriage_statistics in carriage_statistics)
                {
                    PassengerCarriage carriage = single_carriage_statistics.Carriage_Assignment.Passenger_Carriage;
                    PassengerCarriageType type = carriage.Type_Of;
                    int free_places = APIOutputService.CountFreePlacesFromCarriageAssignmentRepresentation(single_carriage_statistics);
                    switch (type)
                    {
                        case PassengerCarriageType.SV:
                            free_sv_places += free_places;
                            total_sv_places += carriage.Capacity;
                            break;
                        case PassengerCarriageType.Coupe:
                            free_coupe_places += free_places;
                            total_coupe_places += carriage.Capacity;
                            break;
                        case PassengerCarriageType.Platskart:
                            free_platskart_places += free_places;
                            total_platskart_places += carriage.Capacity;
                            break;
                    }
                }
                Console.WriteLine($"SV: {free_sv_places} / {total_sv_places}");
                Console.WriteLine($"Coupe: {free_coupe_places} / {total_coupe_places}");
                Console.WriteLine($"Platskart: {free_platskart_places} / {total_platskart_places}");

                foreach (CarriageAssignmentRepresentationDto single_carriage_statistics in carriage_statistics)
                {
                    int position_in_squad = single_carriage_statistics.Carriage_Assignment.Position_In_Squad;
                    PassengerCarriageType type = single_carriage_statistics.Carriage_Assignment.Passenger_Carriage.Type_Of;
                    int capacity = single_carriage_statistics.Carriage_Assignment.Passenger_Carriage.Capacity;
                    Console.WriteLine($"ВАГОН # {position_in_squad}({type} - {capacity} мiсць)");
                    await PrintPlacesInCarriageBetweenStations(single_carriage_statistics, starting_station_title, ending_station_title);
                    Console.WriteLine("-------------------------------------------------------------");
                }
                */

            }
            [Archieved]
            public async Task GetPlacesBetweenStationForAllCarriagesForTrainRouteOnDate(string train_route_on_date_id, string starting_station_title, string ending_station_title)
            {
                /*
                List<CarriageAssignmentRepresentationDto>? carriage_statistics = await api_output_service.GetAllPassengerCarriagesPlacesReportForTrainRouteOnDate(train_route_on_date_id, starting_station_title, ending_station_title);
                if (carriage_statistics == null)
                {
                    return;
                }
                int free_sv_places = 0;
                int total_sv_places = 0;
                int free_coupe_places = 0;
                int total_coupe_places = 0;
                int free_platskart_places = 0;
                int total_platskart_places = 0;

                foreach (CarriageAssignmentRepresentationDto single_carriage_statistics in carriage_statistics)
                {
                    PassengerCarriage carriage = single_carriage_statistics.Carriage_Assignment.Passenger_Carriage;
                    PassengerCarriageType type = carriage.Type_Of;
                    int free_places = APIOutputService.CountFreePlacesFromCarriageAssignmentRepresentation(single_carriage_statistics);
                    switch (type)
                    {
                        case PassengerCarriageType.SV:
                            free_sv_places += free_places;
                            total_sv_places += carriage.Capacity;
                            break;
                        case PassengerCarriageType.Coupe:
                            free_coupe_places += free_places;
                            total_coupe_places += carriage.Capacity;
                            break;
                        case PassengerCarriageType.Platskart:
                            free_platskart_places += free_places;
                            total_platskart_places += carriage.Capacity;
                            break;
                    }
                }
                Console.WriteLine($"SV: {free_sv_places} / {total_sv_places}");
                Console.WriteLine($"Coupe: {free_coupe_places} / {total_coupe_places}");
                Console.WriteLine($"Platskart: {free_platskart_places} / {total_platskart_places}");
                */

            }
            [Archieved]
            public async Task GetPlacesBetweenStationForAllCarriagesForSeveralTrainRoutesOnDate(List<TrainRouteOnDate> train_routes_on_date_list, string starting_station_title, string ending_station_title)
            {
                /*
                List<CarriageAssignmentRepresentationDto>? carriage_statistics_for_several_trains = await api_output_service
                    .GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDate(train_routes_on_date_list
                    .Select(train_route_on_date => train_route_on_date.Id).ToList(), starting_station_title, ending_station_title);
                if (carriage_statistics_for_several_trains == null)
                {
                    return;
                }
                Dictionary<string, int> free_sv_places_for_train_route_on_date = new Dictionary<string, int>();
                Dictionary<string, int> total_sv_places_for_train_route_on_date = new Dictionary<string, int>();
                Dictionary<string, int> free_coupe_places_for_train_route_on_date = new Dictionary<string, int>();
                Dictionary<string, int> total_coupe_places_for_train_route_on_date = new Dictionary<string, int>();
                Dictionary<string, int> free_platskart_places_for_train_route_on_date = new Dictionary<string, int>();
                Dictionary<string, int> total_platskart_places_for_train_route_on_date = new Dictionary<string, int>();
                foreach (CarriageAssignmentRepresentationDto single_carriage_statistics in carriage_statistics_for_several_trains)
                {
                    TrainRouteOnDate train_route_on_date = single_carriage_statistics.Carriage_Assignment.Train_Route_On_Date;
                    PassengerCarriage carriage = single_carriage_statistics.Carriage_Assignment.Passenger_Carriage;
                    PassengerCarriageType type = carriage.Type_Of;
                    free_sv_places_for_train_route_on_date[train_route_on_date.Id] = 0;
                    total_sv_places_for_train_route_on_date[train_route_on_date.Id] = 0;
                    free_coupe_places_for_train_route_on_date[train_route_on_date.Id] = 0;
                    total_coupe_places_for_train_route_on_date[train_route_on_date.Id] = 0;
                    free_platskart_places_for_train_route_on_date[train_route_on_date.Id] = 0;
                    total_platskart_places_for_train_route_on_date[train_route_on_date.Id] = 0;
                    int free_places = APIOutputService.CountFreePlacesFromCarriageAssignmentRepresentation(single_carriage_statistics);
                    switch (type)
                    {
                        case PassengerCarriageType.SV:
                            free_sv_places_for_train_route_on_date[train_route_on_date.Id] += free_places;
                            total_sv_places_for_train_route_on_date[train_route_on_date.Id] += carriage.Capacity;
                            break;
                        case PassengerCarriageType.Coupe:
                            free_coupe_places_for_train_route_on_date[train_route_on_date.Id] += free_places;
                            total_coupe_places_for_train_route_on_date[train_route_on_date.Id] += carriage.Capacity;
                            break;
                        case PassengerCarriageType.Platskart:
                            free_platskart_places_for_train_route_on_date[train_route_on_date.Id] += free_places;
                            total_platskart_places_for_train_route_on_date[train_route_on_date.Id] += carriage.Capacity;
                            break;
                    }
                }
                foreach (TrainRouteOnDate train_route_on_date in train_routes_on_date_list)
                {
                    int free_sv_places = free_sv_places_for_train_route_on_date[train_route_on_date.Id];
                    int total_sv_places = total_sv_places_for_train_route_on_date[train_route_on_date.Id];
                    int free_coupe_places = free_coupe_places_for_train_route_on_date[train_route_on_date.Id];
                    int total_coupe_places = total_coupe_places_for_train_route_on_date[train_route_on_date.Id];
                    int free_platskart_places = free_platskart_places_for_train_route_on_date[train_route_on_date.Id];
                    int total_platskart_places = total_platskart_places_for_train_route_on_date[train_route_on_date.Id];

                    Console.WriteLine($"{train_route_on_date.Id}");
                    Console.WriteLine($"SV: {free_sv_places} / {total_sv_places}");
                    Console.WriteLine($"Coupe: {free_coupe_places} / {total_coupe_places}");
                    Console.WriteLine($"Platskart: {free_platskart_places} / {total_platskart_places}");
                
                }
                */
                /*
public async Task GetPassengerCarriagesPlacesReportForTrainRouteOnDateBetweenStations(string train_route_on_date_id, string starting_station, string ending_station)
{
    List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments_for_train_route_on_date = await
        GetPassengerCarriageAssignementsForTrainRouteOnDate(train_route_on_date_id);
    if(carriage_assignments_for_train_route_on_date == null)
    {
        text_service.FailPostInform("Fail while getting carriages for train route on date");
        return;
    }

}
*/
            }
        }
    }
}
