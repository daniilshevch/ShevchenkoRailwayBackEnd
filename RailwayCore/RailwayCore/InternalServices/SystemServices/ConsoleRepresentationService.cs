using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using RailwayCore.InternalDTO;
using RailwayCore.Models;
using RailwayCore.InternalServices.CoreServices;
using RailwayCore.InternalServices.ModelServices;
using System.Runtime.InteropServices;

namespace RailwayCore.InternalServices.SystemServices
{
    public class ConsoleRepresentationService
    {
        private readonly TrainRouteOnDateService train_route_on_date_service;
        private readonly FullTrainAssignementService full_train_assignement_service;
        private readonly FullTrainRouteSearchService full_train_route_search_service;
        private readonly PassengerCarriageService passenger_carriage_service;
        private readonly FullTicketManagementService full_ticket_booking_service;
        private readonly TextService text_service = new TextService("ConsoleRepresentationService");
        public ConsoleRepresentationService(FullTrainAssignementService full_train_assignement_service, FullTrainRouteSearchService full_train_route_search_service,
            PassengerCarriageService passenger_carriage_service, TrainRouteOnDateService train_route_on_date_service, FullTicketManagementService full_ticket_booking_service)
        {
            this.full_train_assignement_service = full_train_assignement_service;
            this.full_train_route_search_service = full_train_route_search_service;
            this.passenger_carriage_service = passenger_carriage_service;
            this.train_route_on_date_service = train_route_on_date_service;
            this.full_ticket_booking_service = full_ticket_booking_service;
        }






        public async Task WorkingSimulation()
        {
            /*
            Console.Write("Початкова станцiя: ");
            string? Starting_Station = Console.ReadLine();
            Console.Write("Кiнцева станцiя: ");
            string? Ending_Station = Console.ReadLine();
            Console.Write("Дата поїздки: ");
            string? date_string = Console.ReadLine();
            string[] attributes = date_string.Split("-");
            int year = Convert.ToInt32(attributes[0]);
            int month = Convert.ToInt32(attributes[1]);
            int day = Convert.ToInt32(attributes[2]);
            DateOnly date = new DateOnly(year, month, day);
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            List<TrainRouteOnDate>? train_routes_on_date = await full_train_route_search_service.SearchTrainRouteBetweenStationOnDate(Starting_Station, Ending_Station, date);
            if (train_routes_on_date == null)
            {
                return;
            }
            
            Console.WriteLine("Знайденi поїзди: ");
            foreach (TrainRouteOnDate train_race in train_routes_on_date)
            {
                TrainRouteOnDateOnStation? starting_stop = await full_train_route_search_service.GetStartingTrainStopForTrainRouteOnDate(train_race.Id);
                TrainRouteOnDateOnStation? ending_stop = await full_train_route_search_service.GetEndingTrainStopForTrainRouteOnDate(train_race.Id);
                if (starting_stop == null || ending_stop == null)
                {
                    continue;
                }
                Console.WriteLine($"{train_race.Train_Route.Id} {starting_stop.Station.Title}({starting_stop.Departure_Time}) - {ending_stop.Station.Title}({ending_stop.Arrival_Time})");
                await GetPlacesBetweenStationForAllCarriagesForTrainRouteOnDate(train_race.Id, Starting_Station, Ending_Station);
            }
            //await GetPlacesBetweenStationForAllCarriagesForSeveralTrainRoutesOnDate(train_routes_on_date, Starting_Station, Ending_Station);
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.Write($"Виберiть поїзд(1-{train_routes_on_date.Count}): ");
            int choice = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.WriteLine("Вагони: ");
            TrainRouteOnDate chosen_train_route_on_date = train_routes_on_date[choice - 1];
            //await PrintPlacesBetweenStationForAllCarriagesForTrainRouteOnDate(chosen_train_route_on_date.Id, Starting_Station, Ending_Station);
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            Console.WriteLine("Розклад:");
            List<TrainRouteOnDateOnStation>? train_stops = await full_train_route_search_service.GetTrainStopsForTrainRouteOnDate(chosen_train_route_on_date.Id);
            if (train_stops == null)
            {
                return;
            }
            foreach (TrainRouteOnDateOnStation train_stop in train_stops)
            {
                if (train_stop.Station.Title == Starting_Station)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.WriteLine(train_stop);
                if (train_stop.Station.Title == Ending_Station)
                {
                    Console.ResetColor();
                }
            }
            */
        }
    }
}





/*
public async Task PrintPlacesBetweenStationForAllCarriagesForTrainRouteOnDate(string train_route_on_date_id, string starting_station_title, string ending_station_title)
{
    List<PassengerCarriageOnTrainRouteOnDate>? passenger_carriage_assignements = await full_train_route_search_service.GetPassengerCarriageAssignementsForTrainRouteOnDate(train_route_on_date_id);
    if(passenger_carriage_assignements == null)
    {
        text_service.FailPostInform("Fail in FullTrainRouteSearchService");
        return;
    }
    foreach(PassengerCarriageOnTrainRouteOnDate carriage_assignement in passenger_carriage_assignements)
    {
        Console.WriteLine($"ВАГОН # {carriage_assignement.Position_In_Squad}({carriage_assignement.Passenger_Carriage.Type_Of} - {carriage_assignement.Passenger_Carriage.Capacity} мiсць)");
        await PrintPlacesInCarriageBetweenStations(carriage_assignement.Train_Route_On_Date_Id, carriage_assignement.Passenger_Carriage_Id, starting_station_title, ending_station_title);
        Console.WriteLine("-------------------------------------------------------------");
    }
}*/



