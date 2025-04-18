using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RailwayCore.Context;
using RailwayCore.DTO;
using RailwayCore.Models;
using System.Xml.Serialization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Design;
using System.Net.Http.Headers;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System;
namespace RailwayCore.Services
{
    public class CarriagePlace
    {
        public string Carriage_Id { get; set; } = null!;
        public int Place { get; set; }
    }
    public class TrainRouteOnDateCarriagePlace
    {
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public string Carriage_Id { get; set; } = null!;
        public int Place { get; set; }
    }
    public class TicketBookingService
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateService train_route_on_date_service;
        private readonly StationService station_service;
        private readonly PassengerCarriageService passenger_carriage_service;
        private readonly FullTrainRouteSearchService full_train_route_search_service;
        private readonly TextService text_service = new TextService("TicketBookingService");
        public TicketBookingService(AppDbContext context, TrainRouteOnDateService train_route_on_date_service, StationService station_service, PassengerCarriageService passenger_carriage_service, FullTrainRouteSearchService full_train_route_search_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.station_service = station_service;
            this.passenger_carriage_service = passenger_carriage_service;
            this.full_train_route_search_service = full_train_route_search_service;
        }

        public async Task<TicketBooking?> CreateTicketBooking(TicketBookingDto input)
        {
            User? user_from_ticket = await context.Users.FindAsync(input.User_Id);
            if (user_from_ticket == null)
            {
                return null;
            }
            TrainRouteOnDate? train_route_on_date_from_ticket = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if (train_route_on_date_from_ticket == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            PassengerCarriage? passenger_carriage_from_ticket = await passenger_carriage_service.FindPassengerCarriageById(input.Passenger_Carriage_Id);
            if (passenger_carriage_from_ticket == null)
            {
                text_service.FailPostInform("Fail in PassengerCarriageService");
                return null;
            }
            Station? starting_station_from_ticket = await station_service.FindStationByTitle(input.Starting_Station_Title);
            Station? ending_station_from_ticket = await station_service.FindStationByTitle(input.Ending_Station_Title);
            if (starting_station_from_ticket == null || ending_station_from_ticket == null)
            {
                text_service.FailPostInform("Fail in StationService");
                return null;
            }
            //Check if train route on date passes through the stations from ticket
            List<TrainRouteOnDateOnStation>? train_stops_for_train_route_on_date_from_ticket = await
                full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_from_ticket.Id);
            if (train_stops_for_train_route_on_date_from_ticket == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            if (!train_stops_for_train_route_on_date_from_ticket.Any(train_stop => train_stop.Station_Id == starting_station_from_ticket.Id)
                || !train_stops_for_train_route_on_date_from_ticket.Any(train_stop => train_stop.Station_Id == ending_station_from_ticket.Id))
            {
                text_service.FailPostInform("Train route on date doesn't pass through these stations");
                return null;
            }
            int starting_station_from_ticket_number = train_stops_for_train_route_on_date_from_ticket.FindIndex(train_stop =>
            train_stop.Station_Id == starting_station_from_ticket.Id);
            int ending_station_from_ticket_number = train_stops_for_train_route_on_date_from_ticket.FindIndex(train_stop =>
            train_stop.Station_Id == ending_station_from_ticket.Id);
            if (starting_station_from_ticket_number >= ending_station_from_ticket_number)
            {
                text_service.FailPostInform("Train route on date doesn't pass through these stations(in this order)");
                return null;
            }
            //Check if train route on date contains this carriage in its squad
            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignements_for_train_route_on_date_from_ticket =
                await full_train_route_search_service.GetPassengerCarriageAssignementsForTrainRouteOnDate(train_route_on_date_from_ticket.Id);
            if (carriage_assignements_for_train_route_on_date_from_ticket == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            if (!carriage_assignements_for_train_route_on_date_from_ticket
                .Any(carriage_assignement => carriage_assignement.Passenger_Carriage_Id == passenger_carriage_from_ticket.Id))
            {
                text_service.FailPostInform("Train route on date doesn't contains these carriage in its squad");
                return null;
            }
            if (input.Place_In_Carriage > passenger_carriage_from_ticket.Capacity || input.Place_In_Carriage <= 0)
            {
                text_service.FailPostInform($"This carriage doesn't have place with ID: {input.Place_In_Carriage}");
                return null;
            }
            //Check if the place in carriage is available for booking between these stations on train route on date
            bool? _ticket_booking_availability = await CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(train_route_on_date_from_ticket.Id,
                starting_station_from_ticket.Title, ending_station_from_ticket.Title, passenger_carriage_from_ticket.Id,
                input.Place_In_Carriage);
            bool ticket_booking_availability;
            if (_ticket_booking_availability == null)
            {
                text_service.FailPostInform("Fail in checking ticket availability");
                return null;
            }
            else
            {
                ticket_booking_availability = (bool)_ticket_booking_availability;
            }
            if (ticket_booking_availability == false)
            {
                text_service.FailPostInform($"The place with ID: {input.Place_In_Carriage} in carriage {passenger_carriage_from_ticket.Id} has already been booked");
                return null;
            }
            TicketBooking ticket_booking = new TicketBooking()
            {
                User = user_from_ticket,
                Passenger_Carriage = passenger_carriage_from_ticket,
                Train_Route_On_Date = train_route_on_date_from_ticket,
                Starting_Station = starting_station_from_ticket,
                Ending_Station = ending_station_from_ticket,
                Place_In_Carriage = input.Place_In_Carriage,
                Passenger_Name = input.Passenger_Name,
                Passenger_Surname = input.Passenger_Surname,
                Booking_Time = DateTime.Now,
                Ticket_Status = input.Ticket_Status
            };
            await context.Ticket_Bookings.AddAsync(ticket_booking);
            await context.SaveChangesAsync();
            return ticket_booking;
        }
        public async Task<TicketBooking?> CreateTicketBooking(TicketBookingDtoWithCarriagePosition input)
        {
            PassengerCarriageOnTrainRouteOnDate? carriage_assignement = await
                context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignement => carriage_assignement.Passenger_Carriage)
                .Include(carriage_assignement => carriage_assignement.Train_Route_On_Date)
                .FirstOrDefaultAsync(carriage_assignement =>
                carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id && carriage_assignement.Position_In_Squad == input.Passenger_Carriage_Position_In_Squad);
            if (carriage_assignement is null)
            {
                text_service.FailPostInform($"There is no carriage with number {input.Passenger_Carriage_Position_In_Squad} in train route on date squad");
                return null;
            }
            PassengerCarriage passenger_carriage = carriage_assignement.Passenger_Carriage;
            TicketBookingDto ticket_booking_dto = new TicketBookingDto()
            {
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Passenger_Carriage_Id = passenger_carriage.Id,
                Starting_Station_Title = input.Starting_Station_Title,
                Ending_Station_Title = input.Ending_Station_Title,
                Place_In_Carriage = input.Place_In_Carriage,
                User_Id = input.User_Id,
                Passenger_Name = input.Passenger_Name,
                Passenger_Surname = input.Passenger_Surname,
                Ticket_Status = input.Ticket_Status

            };
            TicketBooking? ticket_booking = await CreateTicketBooking(ticket_booking_dto);
            if (ticket_booking == null)
            {
                text_service.FailPostInform("Fail while booking ticket");
                return null;
            }
            return ticket_booking;
        }
        [Crucial("Перевірка броні на одне конкретне місце в одному конкретному вагоні в рейсі на машруті між двома станціями")]
        public async Task<bool?> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title, string desired_ending_station_title,
           string passenger_carriage_id, int place_in_carriage)
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

            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }

            (List<int> left_part, List<int> central_part, List<int> right_part) =
                TicketBookingService.DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, desired_starting_station_title, desired_ending_station_title);

            bool is_available = !(await context.Ticket_Bookings.AnyAsync(ticket => ticket.Passenger_Carriage_Id == passenger_carriage_id
            && ticket.Train_Route_On_Date_Id == train_route_on_date_id && ticket.Place_In_Carriage == place_in_carriage
            && ((central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id)
             || (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id))))));

            return is_available;

        }

        [Executive("Перевірка броні на всі місця в одному конкретному вагоні в рейсі на машруті між двома станціями " +
            "з поверненням об'єтку, що містить призначення вагона і список вільних-зайнятих місць")]
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
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            (List<int> left_part, List<int> central_part, List<int> right_part) =
                TicketBookingService.DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, starting_station_title, ending_station_title);
            List<int> booked_places_in_carriage = await context.Ticket_Bookings.Where(ticket => ticket.Train_Route_On_Date_Id == train_route_on_date_id
            && ticket.Passenger_Carriage_Id == desired_carriage_assignment.Passenger_Carriage_Id && (
           (central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id))
           || (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id))))
                .Select(ticket => ticket.Place_In_Carriage).ToListAsync();
            int total_places_amount = desired_carriage_assignment.Passenger_Carriage.Capacity;
            List<SinglePlace> places_list = new List<SinglePlace>();
            for (int place_number = 1; place_number <= total_places_amount; place_number++)
            {
                if (booked_places_in_carriage.Contains(place_number))
                {
                    places_list.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = false });
                }
                else
                {
                    places_list.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = true });
                }
            }
            CarriageAssignmentRepresentationDto single_carriage_statistics = new CarriageAssignmentRepresentationDto
            {
                Carriage_Assignment = desired_carriage_assignment,
                Places_Availability = places_list
            };
            return single_carriage_statistics;
        }


        [Crucial("Перевірка броні на всі місця в усіх вагонах в рейсі на машруті між двома станціями " +
            "з поверненням об'єтку, що містить призначення вагона і список вільних-зайнятих місць")]
        public async Task<List<CarriageAssignmentRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForTrainRouteOnDate(string train_route_on_date_id, string starting_station_title, string ending_station_title)
        {
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
                await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (all_train_stops_of_train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in FullTrainRouteSearchService");
                return null;
            }
            (List<int> left_part, List<int> central_part, List<int> right_part) =
                TicketBookingService.DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, starting_station_title, ending_station_title);


            List<CarriagePlace> booked_places_in_all_carriages = await context.Ticket_Bookings.Where(ticket => ticket.Train_Route_On_Date_Id == train_route_on_date_id &&
            (
           (central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id))
           || (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id))))
                .Select(ticket => new CarriagePlace { Carriage_Id = ticket.Passenger_Carriage_Id, Place = ticket.Place_In_Carriage }).ToListAsync();


            List<CarriageAssignmentRepresentationDto> carriage_statistics_list = new List<CarriageAssignmentRepresentationDto>();

            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments = await full_train_route_search_service.GetPassengerCarriageAssignementsForTrainRouteOnDate(train_route_on_date_id);
            if (carriage_assignments == null)
            {
                return null;
            }
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments)
            {
                HashSet<int> booked_places_for_single_carriage = booked_places_in_all_carriages
                    .Where(carriage_place => carriage_place.Carriage_Id == carriage_assignment.Passenger_Carriage_Id)
                    .Select(carriage_place => carriage_place.Place).ToHashSet();

                int total_places_amount = carriage_assignment.Passenger_Carriage.Capacity;
                List<SinglePlace> places_list_for_single_carriage = new List<SinglePlace>();
                for (int place_number = 1; place_number <= total_places_amount; place_number++)
                {
                    if (booked_places_for_single_carriage.Contains(place_number))
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = false });
                    }
                    else
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = true });
                    }
                }
                carriage_statistics_list.Add(new CarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = carriage_assignment,
                    Places_Availability = places_list_for_single_carriage
                });
            }
            return carriage_statistics_list;

        }

        /*
        [Crucial("Перевірка броні на всі місця в усіх вагонах в декількох рейсах на машрутах між двома станціями " +
            "з поверненням об'єтку, що містить призначення вагона і список вільних-зайнятих місць)")]
        public async Task<List<CarriageAssignmentRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids_list, string starting_station_title, string ending_station_title)
        {
            List<List<TrainRouteOnDateOnStation>> all_train_stops_of_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach(string train_route_on_date_id in train_route_on_date_ids_list)
            {
                List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date =
              await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
                if(all_train_stops_of_train_route_on_date == null)
                {
                    return null;
                }
                all_train_stops_of_train_route_on_date_list.Add(all_train_stops_of_train_route_on_date);
            }
          
            (List<int> left_part, List<int> central_part, List<int> right_part) =
                TicketBookingService.DivideSectionIntoThreePartsForSeveralTrains(all_train_stops_of_train_route_on_date_list, starting_station_title, ending_station_title);


            List<TrainRouteOnDateCarriagePlace> booked_places_in_all_carriages = await context.Ticket_Bookings.Where(ticket => train_route_on_date_ids_list.Contains(ticket.Train_Route_On_Date_Id) &&
            (
           (central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id))
           || (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id))))
                .Select(ticket => new TrainRouteOnDateCarriagePlace {Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id, Carriage_Id = ticket.Passenger_Carriage_Id, Place = ticket.Place_In_Carriage }).ToListAsync();


            List<CarriageAssignmentRepresentationDto> carriage_statistics_list = new List<CarriageAssignmentRepresentationDto>();

            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments = await full_train_route_search_service.GetPassengerCarriageAssignementsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
            if (carriage_assignments == null)
            {
                return null;
            }
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments)
            {
                var booked_places_for_single_carriage = booked_places_in_all_carriages
                    .Where(carriage_place => carriage_place.Carriage_Id == carriage_assignment.Passenger_Carriage_Id)
                    .Select(carriage_place => carriage_place.Place).ToHashSet();
                int total_places_amount = carriage_assignment.Passenger_Carriage.Capacity;
                List<SinglePlace> places_list_for_single_carriage = new List<SinglePlace>();
                for (int place_number = 1; place_number <= total_places_amount; place_number++)
                {
                    if (booked_places_for_single_carriage.Contains(place_number))
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = false });
                    }
                    else
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = true });
                    }
                }
                carriage_statistics_list.Add(new CarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = carriage_assignment,
                    Places_Availability = places_list_for_single_carriage
                });
            }
            return carriage_statistics_list.OrderBy(stats => stats.Carriage_Assignment.Train_Route_On_Date_Id).ThenBy(stats => stats.Carriage_Assignment.Position_In_Squad).ToList();

        }
        */
        [Executive]
        public async Task<List<CarriageAssignmentRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids_list, string starting_station_title, string ending_station_title)
        {
            List<TrainRouteOnDateOnStation>? all_train_stops_of_all_train_routes_on_date = await full_train_route_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
            if (all_train_stops_of_all_train_routes_on_date == null)
            {
                return null;
            }
            List<List<TrainRouteOnDateOnStation>> all_train_stops_of_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach (string train_route_on_date_id in train_route_on_date_ids_list)
            {
                List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date = all_train_stops_of_all_train_routes_on_date
                    .Where(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_id).ToList();
                all_train_stops_of_train_route_on_date_list.Add(all_train_stops_of_train_route_on_date);
            }

            (List<int> left_part, List<int> central_part, List<int> right_part) =
                TicketBookingService.DivideSectionIntoThreePartsForSeveralTrains(all_train_stops_of_train_route_on_date_list, starting_station_title, ending_station_title);


            List<TrainRouteOnDateCarriagePlace> booked_places_in_all_carriages = await context.Ticket_Bookings.Where(ticket => train_route_on_date_ids_list.Contains(ticket.Train_Route_On_Date_Id) &&
            (
           (central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id))
           || (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id))))
                .Select(ticket => new TrainRouteOnDateCarriagePlace { Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id, Carriage_Id = ticket.Passenger_Carriage_Id, Place = ticket.Place_In_Carriage }).ToListAsync();


            List<CarriageAssignmentRepresentationDto> carriage_statistics_list = new List<CarriageAssignmentRepresentationDto>();

            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments = await full_train_route_search_service.GetPassengerCarriageAssignementsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
            if (carriage_assignments == null)
            {
                return null;
            }
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments)
            {
                HashSet<int> booked_places_for_single_carriage = booked_places_in_all_carriages
                    .Where(carriage_place => carriage_place.Carriage_Id == carriage_assignment.Passenger_Carriage_Id)
                    .Select(carriage_place => carriage_place.Place).ToHashSet();
                int total_places_amount = carriage_assignment.Passenger_Carriage.Capacity;
                List<SinglePlace> places_list_for_single_carriage = new List<SinglePlace>();
                for (int place_number = 1; place_number <= total_places_amount; place_number++)
                {
                    if (booked_places_for_single_carriage.Contains(place_number))
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = false });
                    }
                    else
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = true });
                    }
                }
                carriage_statistics_list.Add(new CarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = carriage_assignment,
                    Places_Availability = places_list_for_single_carriage
                });
            }
            return carriage_statistics_list.OrderBy(stats => stats.Carriage_Assignment.Train_Route_On_Date_Id).ThenBy(stats => stats.Carriage_Assignment.Position_In_Squad).ToList();

        }

        [Crucial("Перевірка броні на всі місця в усіх вагонах в декількох рейсах на машрутах між двома станціями " +
            "з поверненням об'єтку, що містить призначення вагона і список вільних-зайнятих місць)")]
        public async Task<Dictionary<string, TrainRouteOnDateCarriageAssignmentsRepresentationDto>?> GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDateIntoRepresentationDto(List<string> train_route_on_date_ids_list, string starting_station_title, string ending_station_title)
        {
            List<TrainRouteOnDate> train_route_on_date_list = context.Train_Routes_On_Date
                .Where(train_route_on_date => train_route_on_date_ids_list.Contains(train_route_on_date.Id)).ToList();
            List<TrainRouteOnDateOnStation>? all_train_stops_of_all_train_routes_on_date = await full_train_route_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
            if (all_train_stops_of_all_train_routes_on_date == null)
            {
                return null;
            }
            List<List<TrainRouteOnDateOnStation>> all_train_stops_of_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach (string train_route_on_date_id in train_route_on_date_ids_list)
            {
                List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date = all_train_stops_of_all_train_routes_on_date
                    .Where(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_id).ToList();
                all_train_stops_of_train_route_on_date_list.Add(all_train_stops_of_train_route_on_date);
            }

            (List<int> left_part, List<int> central_part, List<int> right_part) =
                TicketBookingService.DivideSectionIntoThreePartsForSeveralTrains(all_train_stops_of_train_route_on_date_list, starting_station_title, ending_station_title);


            List<TrainRouteOnDateCarriagePlace> booked_places_in_all_carriages = await context.Ticket_Bookings.Where(ticket => train_route_on_date_ids_list.Contains(ticket.Train_Route_On_Date_Id) &&
            (
           (central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id))
           || (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id))))
                .Select(ticket => new TrainRouteOnDateCarriagePlace { Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id, Carriage_Id = ticket.Passenger_Carriage_Id, Place = ticket.Place_In_Carriage }).ToListAsync();


            List<CarriageAssignmentRepresentationDto> carriage_statistics_list = new List<CarriageAssignmentRepresentationDto>();

            List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignments = await full_train_route_search_service.GetPassengerCarriageAssignementsForSeveralTrainRoutesOnDate(train_route_on_date_ids_list);
            if (carriage_assignments == null)
            {
                return null;
            }
            carriage_assignments = carriage_assignments.OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();
            Dictionary<string, TrainRouteOnDateCarriageAssignmentsRepresentationDto> carriage_statistics_list_for_every_train_route_on_date =
                new Dictionary<string, TrainRouteOnDateCarriageAssignmentsRepresentationDto>();
            foreach (string train_route_on_date_id in train_route_on_date_ids_list)
            {
                carriage_statistics_list_for_every_train_route_on_date[train_route_on_date_id] = new TrainRouteOnDateCarriageAssignmentsRepresentationDto
                {
                    Train_Route_On_Date = train_route_on_date_list.FirstOrDefault(train_route_on_date => train_route_on_date.Id == train_route_on_date_id),
                    Platskart_Free = 0,
                    Platskart_Total = 0,
                    Coupe_Free = 0,
                    Coupe_Total = 0,
                    SV_Free = 0,
                    SV_Total = 0
                };
            }
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignment in carriage_assignments)
            {
                HashSet<int> booked_places_for_single_carriage = booked_places_in_all_carriages
                    .Where(carriage_place => carriage_place.Carriage_Id == carriage_assignment.Passenger_Carriage_Id)
                    .Select(carriage_place => carriage_place.Place).ToHashSet();

                int booked_places_amount = 0;
                int total_places_amount = carriage_assignment.Passenger_Carriage.Capacity;
                string current_train_route_on_date_id = carriage_assignment.Train_Route_On_Date_Id;
                List<SinglePlace> places_list_for_single_carriage = new List<SinglePlace>();
                for (int place_number = 1; place_number <= total_places_amount; place_number++)
                {
                    if (booked_places_for_single_carriage.Contains(place_number))
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = false });
                        booked_places_amount++;
                    }
                    else
                    {
                        places_list_for_single_carriage.Add(new SinglePlace { Place_In_Carriage = place_number, Is_Free = true });
                    }
                }
                int free_places_amount = total_places_amount - booked_places_amount;
                carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Carriage_Statistics_List.Add(new CarriageAssignmentRepresentationDto
                {
                    Carriage_Assignment = carriage_assignment,
                    Places_Availability = places_list_for_single_carriage,
                    Total_Places = total_places_amount,
                    Free_Places = free_places_amount
                });
                switch (carriage_assignment.Passenger_Carriage.Type_Of)
                {
                    case PassengerCarriageType.Platskart:
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Platskart_Free += free_places_amount;
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Platskart_Total += total_places_amount;
                        break;
                    case PassengerCarriageType.Coupe:
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Coupe_Free += free_places_amount;
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].Coupe_Total += total_places_amount;
                        break;
                    case PassengerCarriageType.SV:
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].SV_Free += free_places_amount;
                        carriage_statistics_list_for_every_train_route_on_date[current_train_route_on_date_id].SV_Total += total_places_amount;
                        break;
                }

            }


            return carriage_statistics_list_for_every_train_route_on_date;
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
        public async Task<TicketBooking?> FindTicketBookingById(int ticket_booking_id)
        {
            TicketBooking? ticket_booking = await context.Ticket_Bookings.FirstOrDefaultAsync(ticket_booking => ticket_booking.Id == ticket_booking_id);
            return ticket_booking;
        }
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


























        //Перевантажена версія попереднього методу
        public async Task<CarriageAssignmentRepresentationDto?> GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDate carriage_assignment, string starting_station_title, string ending_station_title)
        {
            TrainRouteOnDate train_route_on_date = carriage_assignment.Train_Route_On_Date;
            return await GetSinglePassengerCarriagePlacesReportForTrainRouteOnDate(train_route_on_date.Id, carriage_assignment.Position_In_Squad, starting_station_title, ending_station_title);
        }





        [Crucial]
        [Algorithm("АЛГОРИТМ ТРЬОХ СЕКЦІЙ")]
        public static (List<int>, List<int>, List<int>) DivideSectionIntoThreeParts
            (List<TrainRouteOnDateOnStation> all_train_stops_of_train_route_on_date, string desired_starting_station_title, string desired_ending_station_title)
        {

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
            return (left_part, central_part, right_part);
        }
        [Crucial]
        [Algorithm("МОДИФІКОВАНИЙ АЛГОРИТМ ТРЬОХ СЕКЦІЙ(ДЛЯ ДЕКІЛЬКОХ ПОЇЗДІВ)")]
        public static (List<int>, List<int>, List<int>) DivideSectionIntoThreePartsForSeveralTrains(List<List<TrainRouteOnDateOnStation>> all_train_stops_of_train_route_on_date_list,
            string desired_starting_station_title, string desired_ending_station_title)
        {
            List<int> consolidated_left_part = new List<int>();
            List<int> consolidated_central_part = new List<int>();
            List<int> consolidated_right_part = new List<int>();
            foreach (List<TrainRouteOnDateOnStation> all_train_stops_of_train_route_on_date in all_train_stops_of_train_route_on_date_list)
            {
                (List<int> left_part_for_single_train, List<int> central_part_for_single_train, List<int> right_part_for_single_train) =
                    DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, desired_starting_station_title, desired_ending_station_title);
                consolidated_left_part = consolidated_left_part.Union(left_part_for_single_train).ToList();
                consolidated_central_part = consolidated_central_part.Union(central_part_for_single_train).ToList();
                consolidated_right_part = consolidated_right_part.Union(right_part_for_single_train).ToList();

            }
            return (consolidated_left_part, consolidated_central_part, consolidated_right_part);
        }





    }
}