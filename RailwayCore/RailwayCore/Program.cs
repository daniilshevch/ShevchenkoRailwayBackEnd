using RailwayCore.Context;
using RailwayCore.Models;
using RailwayCore.InternalServices.ModelServices;
using RailwayCore.InternalServices.CoreServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using RailwayCore.InternalDTO.CoreDTO;
using RailwayCore.InternalServices.SystemServices;
class Program
{
    public static void PrintList(List<InternalSinglePlaceDto> places)
    {
        foreach (InternalSinglePlaceDto singlePlace in places)
        {
            Console.WriteLine($"{singlePlace.Place_In_Carriage} - {singlePlace.Is_Free}");
        }
    }
    public static async Task Main(string[] args)
    {
        using (AppDbContext db = new AppDbContext())
        {
            IServiceCollection services = new ServiceCollection()
                .AddSingleton<AppDbContext>()
                .AddSingleton<RailwayBranchService>()
                .AddSingleton<StationService>()
                .AddSingleton<TrainRouteService>()
                .AddSingleton<TrainRouteOnDateService>()
                .AddSingleton<TrainRouteOnDateOnStationService>()
                .AddSingleton<PassengerCarriageService>()
                .AddSingleton<PassengerCarriageOnTrainRouteOnDateService>()
                .AddSingleton<FullTrainAssignementService>()
                .AddSingleton<FullTrainRouteSearchService>()
                .AddSingleton<FullTicketManagementService>()
                .AddSingleton<ConsoleRepresentationService>();
            IServiceProvider provider = services.BuildServiceProvider();

            RailwayBranchService? railway_branch_service = provider.GetService<RailwayBranchService>();
            StationService? station_service = provider.GetService<StationService>();
            TrainRouteService? train_route_service = provider.GetService<TrainRouteService>();
            TrainRouteOnDateService? train_route_on_date_service = provider.GetService<TrainRouteOnDateService>();
            TrainRouteOnDateOnStationService? train_route_on_date_on_station_service =
                provider.GetService<TrainRouteOnDateOnStationService>();
            PassengerCarriageService? passenger_carriage_service = provider.GetService<PassengerCarriageService>();
            PassengerCarriageOnTrainRouteOnDateService? passenger_carriage_on_train_route_on_date_service =
                provider.GetService<PassengerCarriageOnTrainRouteOnDateService>();
            FullTrainAssignementService? full_train_assignement_service =
                provider.GetService<FullTrainAssignementService>();
            FullTrainRouteSearchService? full_train_route_search_service = provider.GetService<FullTrainRouteSearchService>();
            FullTicketManagementService? ticket_booking_service = provider.GetService<FullTicketManagementService>();
            ConsoleRepresentationService? console_representation_service = provider.GetService<ConsoleRepresentationService>();
            if (railway_branch_service == null || station_service == null || train_route_service == null ||
                train_route_on_date_service == null || train_route_on_date_on_station_service == null ||
                passenger_carriage_service == null || passenger_carriage_on_train_route_on_date_service == null ||
                full_train_assignement_service == null || full_train_route_search_service == null ||
                ticket_booking_service == null || console_representation_service == null)
            {
                Console.WriteLine("Fail while initializing services");
                return;
            }

            /*
            await ticket_booking_service.CreateTicketBooking(new TicketBookingDto
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "26SH_2025_02_14",
                Starting_Station_Title = "Odesa-Holovna",
                Ending_Station_Title = "Zhmerynka",
                Place_In_Carriage = 14,
                Passenger_Carriage_Id = "01634547",
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko"
            });*/

            /*
            List<TrainRouteOnDateOnStation> list = await full_train_route_search_service.GetTrainStopsBetweenTwoStationsForTrainRouteOnDate("26SH_2025_02_14", "Odesa-Holovna", "Yasinia");
            foreach(TrainRouteOnDateOnStation stop in list)
            {
                Console.WriteLine(stop);
            }
            Console.WriteLine();
            List<TrainRouteOnDateOnStation> list2 = await full_train_route_search_service.GetTrainStopsBetweenNumbersForTrainRouteOnDate("26SH_2025_02_14", 0, 25);
            foreach (TrainRouteOnDateOnStation stop in list2)
            {
                Console.WriteLine(stop);
            }*/

            /*
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "01134546", "Rakhny", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "01634536", "Rakhny", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "01634546", "Rakhny", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "01634547", "Zhmerynka", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "01634549", "Rakhny", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "01634566", "Rakhny", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "04422331", "Rakhny", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            await console_representation_service.PrintPlacesInCarriageBetweenStations("26SH_2025_02_14", "02434566", "Rakhny", "Lviv");
            Console.WriteLine("--------------------------------------------------------");
            */
            /*
            await ticket_booking_service.CreateTicketBooking(new TicketBookingDto
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "26SH_2025_02_14",
                Starting_Station_Title = "Ivano-Frankivsk",
                Ending_Station_Title = "Yaremche",
                Place_In_Carriage = 18,
                Passenger_Carriage_Id = "04422331",
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko"
            });*/
            /*
            for (int i = 1; i < 36; i += 3)
            {
                await ticket_booking_service.CreateTicketBooking(new TicketBookingDtoWithCarriagePosition
                {
                    User_Id = 1,
                    Train_Route_On_Date_Id = "26SH_2025_02_14",
                    Starting_Station_Title = "Ivano-Frankivsk",
                    Ending_Station_Title = "Yaremche",
                    Place_In_Carriage = i,
                    Passenger_Carriage_Position_In_Squad = 6,
                    Passenger_Name = "Daniil",
                    Passenger_Surname = "Shevchenko"
                });
            }*/

            //await console_representation_service.WorkingSimulation();

            //List<PassengerCarriageOnTrainRouteOnDate> list = await full_train_route_search_service.GetPassengerCarriageAssignementsForSeveralTrainRoutesOnDate(new List<string> { "26SH_2025_02_14", "12SH_2025_02_14" });

            /*
            foreach(PassengerCarriageOnTrainRouteOnDate assignment in list)
            {
                Console.WriteLine(assignment.Passenger_Carriage.Id);
                Console.WriteLine(assignment.Train_Route_On_Date.Id);
                Console.WriteLine(assignment.Position_In_Squad);
                Console.WriteLine(assignment.Passenger_Carriage.Type_Of);
                Console.WriteLine("--------------------");
            }*/
            /*
            DateTime first = DateTime.Now;
            Console.WriteLine(first.ToString("HH:mm:ss.fff"));
            List<TrainRouteOnDate> routes_on_date = await full_train_route_search_service.SearchTrainRouteBetweenStationOnDate("Zhmerynka", "Ternopil", new DateOnly(2025,02,15));
            foreach(TrainRouteOnDate route in routes_on_date)
            {
                Console.WriteLine(route.Id);
            }
            Console.WriteLine("------------------------------------");
            List<CarriageAssignmentRepresentationDto> list = await ticket_booking_service.GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDate(routes_on_date.Select(r => r.Id).ToList(), "Ternopil", "Lviv");
            foreach(CarriageAssignmentRepresentationDto dto in list)
            {
                Console.WriteLine(dto.Carriage_Assignment.Train_Route_On_Date_Id);
                Console.WriteLine(dto.Carriage_Assignment.Position_In_Squad);
                Console.WriteLine(dto.Carriage_Assignment.Passenger_Carriage.Type_Of);
                PrintList(dto.Places_Availability);
                Console.WriteLine("-------------");
            }
            DateTime second = DateTime.Now;
            Console.WriteLine(second.ToString("HH:mm:ss.fff"));
            TimeSpan third = second - first;
            Console.WriteLine(third.TotalSeconds);
           */
            //Test of RailwayCore*/


            /*
            DateTime first = DateTime.Now;
            Console.WriteLine(first.ToString("HH:mm:ss.fff"));
            List<TrainRouteOnDateOnStation> train_stops = await full_train_route_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(new List<string> { "26SH_2025_02_14", "12SH_2025_02_14" });
            //List<TrainRouteOnDateOnStation> train_stops = await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate("26SH_2025_02_14");
            //List<TrainRouteOnDateOnStation> train_stops2 = await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate("12SH_2025_02_14");
            //train_stops = train_stops.Union(train_stops2).ToList();
            foreach (TrainRouteOnDateOnStation train_stop in train_stops)
            {
                Console.WriteLine(train_stop.Train_Route_On_Date.Id);
                Console.WriteLine(train_stop.Station.Title);
                Console.WriteLine($"{train_stop.Arrival_Time} - {train_stop.Departure_Time}");
                Console.WriteLine("--------------------");
                
            }
            DateTime second = DateTime.Now;
            Console.WriteLine(second.ToString("HH:mm:ss.fff"));
            TimeSpan third = second - first;
            Console.WriteLine(third.TotalSeconds);
            */

            /*
            DateTime t1 = DateTime.Now;
            Console.WriteLine(t1.ToString("HH:mm:ss.fff"));
            List<TrainRouteOnDate> SearchTrainRouteBetweenStationOnDate = await full_train_route_search_service.SearchTrainRouteBetweenStationOnDate("Ternopil", "Lviv", new DateOnly(2025, 02, 15));
            DateTime t2 = DateTime.Now;
            Console.WriteLine(t2.ToString("HH:mm:ss.fff"));
            TimeSpan t3 = t2 - t1;
            Console.WriteLine(t3.TotalSeconds);
            */
            /*
            DateTime first = DateTime.Now;
            Console.WriteLine(first.ToString("HH:mm:ss.fff"));
            Station station = await station_service.FindStationByTitle("Odesa-Holovna");
            Console.WriteLine(station.Id);
            DateTime second = DateTime.Now;
            Console.WriteLine(second.ToString("HH:mm:ss.fff"));
            TimeSpan third = second - first;
            Console.WriteLine(third.TotalSeconds);
            */
            /*
            string one = "Zhmerynka";
            string two = "Khmelnytskyi";
            DateTime first = DateTime.Now;
            Console.WriteLine(first.ToString("HH:mm:ss.fff"));
            List<TrainRouteOnDateArrivalDepartureTimeDto> routes_on_date = await full_train_route_search_service.SearchTrainRouteBetweenStationOnDate(one, two, new DateOnly(2025, 02, 15));

            Dictionary<string, TrainRouteOnDateCarriageAssignmentsRepresentationDto> list =
                await ticket_booking_service.
                GetAllPassengerCarriagesPlacesReportForSeveralTrainRoutesOnDateIntoRepresentationDto(routes_on_date.Select(r => r.Train_Route_On_Date.Id).ToList(),
                one, two);
            foreach(var stat in list)
            {
                
                var stats = stat.Value;
                Console.WriteLine($"Route: {stats.Train_Route_On_Date.Id}");
                Console.WriteLine($"Platskart: {stats.Platskart_Free} / {stats.Platskart_Total}");
                Console.WriteLine($"Coupe: {stats.Coupe_Free} / {stats.Coupe_Total}");
                Console.WriteLine($"SV: {stats.SV_Free} / {stats.SV_Total}");
                Console.WriteLine();
                foreach(var single in stats.Carriage_Statistics_List)
                {
                    Console.WriteLine($"Number: {single.Carriage_Assignment.Position_In_Squad}");
                    Console.WriteLine($"Type: {single.Carriage_Assignment.Passenger_Carriage.Type_Of}");
                    Console.WriteLine($"Places: {single.Free_Places} / {single.Total_Places}");
                    foreach(SinglePlace place in single.Places_Availability)
                    {
                        Console.WriteLine($"{place.Place_In_Carriage} - {place.Is_Free}");
                    }
                }
            }
            DateTime second = DateTime.Now;
            Console.WriteLine(second.ToString("HH:mm:ss.fff"));
            TimeSpan third = second - first;
            Console.WriteLine(third.TotalSeconds);
            
            */
            /*
            await ticket_booking_service.CreateTicketBooking(new TicketBookingDtoWithCarriagePosition
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "12SH_2025_02_14",
                Starting_Station_Title = "Ternopil",
                Ending_Station_Title = "Pidzamche",
                Place_In_Carriage = 3,
                Passenger_Carriage_Position_In_Squad = 2,
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko"
            });
            await ticket_booking_service.CreateTicketBooking(new TicketBookingDtoWithCarriagePosition
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "12SH_2025_02_14",
                Starting_Station_Title = "Odesa",
                Ending_Station_Title = "Pidzamche",
                Place_In_Carriage = 4,
                Passenger_Carriage_Position_In_Squad = 16,
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko"
            });
            await ticket_booking_service.CreateTicketBooking(new TicketBookingDtoWithCarriagePosition
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "12SH_2025_02_14",
                Starting_Station_Title = "Ternopil",
                Ending_Station_Title = "Lviv",
                Place_In_Carriage = 3,
                Passenger_Carriage_Position_In_Squad = 5,
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko"
            });
            await ticket_booking_service.CreateTicketBooking(new TicketBookingDtoWithCarriagePosition
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "12SH_2025_02_14",
                Starting_Station_Title = "Ternopil",
                Ending_Station_Title = "Pidzamche",
                Place_In_Carriage = 1,
                Passenger_Carriage_Position_In_Squad = 1,
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko"
            });
            */
            /*
            await ticket_booking_service.CreateTicketBooking(new TicketBookingDtoWithCarriagePosition
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "26SH_2025_02_14",
                Starting_Station_Title = "Odesa-Holovna",
                Ending_Station_Title = "Yasinia",
                Place_In_Carriage = 2,
                Passenger_Carriage_Position_In_Squad = 2,
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko",
                Ticket_Status = TicketStatus.Booked_And_Active
            });*/
            /*await train_route_service.AddTrainRoute(new TrainRouteDto
            {
                Id = "38SH",
                Is_Branded = true,
                Assignement_Type = AssignementType.Whole_Year,
                Speed_Type = SpeedType.Fast,
                Branded_Name = "White Acacia",
                Frequency_Type = FrequencyType.Daily,
                Quality_Class = TrainQualityClass.C,
                Railway_Branch_Title = "Odesa Railway"
            });*/


            /*

            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03830522",
                Type_Of = PassengerCarriageType.SV,
                Capacity = 18,
                Production_Year = 1986,
                Renewal_Fact = true,
                Renewal_Year = 2018,
                Air_Conditioning = true,
                Manufacturer = PassengerCarriageManufacturer.Amendorf,
                Quality_Class = PassengerCarriageQualityClass.A
            });
            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03834529",
                Type_Of = PassengerCarriageType.Coupe,
                Capacity = 36,
                Production_Year = 1986,
                Renewal_Fact = true,
                Renewal_Year = 2018,
                Air_Conditioning = true,
                Manufacturer = PassengerCarriageManufacturer.Amendorf,
                Quality_Class = PassengerCarriageQualityClass.B
            });
            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03835229",
                Type_Of = PassengerCarriageType.Coupe,
                Capacity = 40,
                Production_Year = 2023,
                Renewal_Fact = false,
                Air_Conditioning = true,
                Manufacturer = PassengerCarriageManufacturer.KVBZ,
                Quality_Class = PassengerCarriageQualityClass.A
            });
            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03861522",
                Type_Of = PassengerCarriageType.Coupe,
                Capacity = 18,
                Production_Year = 1986,
                Renewal_Fact = true,
                Renewal_Year = 2022,
                Air_Conditioning = true,
                Manufacturer = PassengerCarriageManufacturer.Amendorf,
                Quality_Class = PassengerCarriageQualityClass.B
            });

            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03830533",
                Type_Of = PassengerCarriageType.Coupe,
                Capacity = 36,
                Production_Year = 1976,
                Renewal_Fact = false,
                Renewal_Year = null,
                Air_Conditioning = false,
                Manufacturer = PassengerCarriageManufacturer.Amendorf,
                Quality_Class = PassengerCarriageQualityClass.C
            });
            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03834523",
                Type_Of = PassengerCarriageType.Coupe,
                Capacity = 36,
                Production_Year = 1986,
                Renewal_Fact = true,
                Renewal_Year = 2018,
                Air_Conditioning = true,
                Manufacturer = PassengerCarriageManufacturer.Amendorf,
                Quality_Class = PassengerCarriageQualityClass.A
            });
            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03835221",
                Type_Of = PassengerCarriageType.Coupe,
                Capacity = 36,
                Production_Year = 1977,
                Renewal_Fact = false,
                Air_Conditioning = false,
                Manufacturer = PassengerCarriageManufacturer.Amendorf,
                Quality_Class = PassengerCarriageQualityClass.C
            });
            await passenger_carriage_service.CreatePassengerCarriage(new PassengerCarriageDto
            {
                Id = "03861524",
                Type_Of = PassengerCarriageType.Coupe,
                Capacity = 18,
                Production_Year = 1986,
                Renewal_Fact = true,
                Renewal_Year = 2022,
                Air_Conditioning = true,
                Manufacturer = PassengerCarriageManufacturer.Amendorf,
                Quality_Class = PassengerCarriageQualityClass.B
            });*/
            /*
            await ticket_booking_service.CreateTicketBooking(new InternalTicketBookingDtoWithCarriagePosition
            {
                User_Id = 1,
                Train_Route_On_Date_Id = "38SH_2025_02_14",
                Starting_Station_Title = "Odesa-Holovna",
                Ending_Station_Title = "Zhmerynka",
                Place_In_Carriage = 33,
                Passenger_Carriage_Position_In_Squad = 2,
                Passenger_Name = "Daniil",
                Passenger_Surname = "Shevchenko",
                Ticket_Status = TicketStatus.Booked_And_Active
            }); */
        }

    }
}