using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.Services;
using RailwayCore.Models;
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

[Checked("18.04.2025")]
public class  InternalTrainRaceDto  //Внутрішній трансфер(використовується в подальшому на сторінці пошуку поїздів між станціями)
{
    public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!;
    public DateTime Departure_Time_From_Desired_Starting_Station { get; set; }
    public TrainRouteOnDateOnStation Desired_Starting_Station { get; set; } = null!;
    public DateTime Arrival_Time_For_Desired_Ending_Station { get; set; }
    public TrainRouteOnDateOnStation Desired_Ending_Station { get; set; } = null!;
    public double? Km_Point_Of_Desired_Starting_Station { get; set; }
    public double? Km_Point_Of_Desired_Ending_Station { get; set; }
    public TrainRouteOnDateOnStation Full_Route_Starting_Stop { get; set; } = null!;
    public TrainRouteOnDateOnStation Full_Route_Ending_Stop { get; set; } = null!;
    public List<TrainRouteOnDateOnStation> Full_Route_Stops_List { get; set; } = new List<TrainRouteOnDateOnStation>();

}
namespace RailwayCore.Services
{
    public class FullTrainRouteSearchService
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateService train_route_on_date_service;
        private readonly StationService station_service;
        private readonly PassengerCarriageOnTrainRouteOnDateService passenger_carriage_on_train_route_on_date_service;
        private readonly TextService text_service = new TextService("FullTrainRouteSearchService");
        public FullTrainRouteSearchService(AppDbContext context, TrainRouteOnDateService train_route_on_date_service, StationService station_service, PassengerCarriageOnTrainRouteOnDateService passenger_carriage_on_train_route_on_date_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.station_service = station_service;
            this.passenger_carriage_on_train_route_on_date_service = passenger_carriage_on_train_route_on_date_service;
        }

        [Refactored("v1", "18.04.2025")]
        [Crucial]
        public async Task<QueryResult<List<InternalTrainRaceDto>>> SearchTrainRoutesBetweenStationsOnDate(string start_station_title, string end_station_title, DateOnly trip_departure_date)
        {
            Station? start_station = await station_service.FindStationByTitle(start_station_title); // Пошук стартової зупинки
            Station? end_station = await station_service.FindStationByTitle(end_station_title); // Пошук кінцевої зупинки
            if (start_station is null)
            {
                return new FailQuery<List<InternalTrainRaceDto>>(new Error(ErrorType.NotFound, $"Can't find starting station with title: {start_station_title}"));
            }
            if (end_station is null)
            {
                return new FailQuery<List<InternalTrainRaceDto>>(new Error(ErrorType.NotFound, $"Can't find ending station with title: {end_station_title}"));
            }

            //Пошук поїздів, які проходять через 2 задані станції, причому відправляються з початкової станції в указана дату
            //(Порядок слідування між станціями не перевіряються => сюди будуть включені і поїзди, які їдуть у зворотньому напрямку)
            List<TrainRouteOnDate> possible_train_routes_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Train_Route)
                .Where(train_route_on_date => context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Station)
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Where(train_stop => train_stop.Departure_Time != null).Any(train_stop => train_stop.Station.Title == start_station_title
                && train_stop.Train_Route_On_Date_Id == train_route_on_date.Id && DateOnly.FromDateTime(train_stop.Departure_Time!.Value) == trip_departure_date) &&
                context.Train_Routes_On_Date_On_Stations.Any(train_stop => train_stop.Station.Title == end_station_title
                && train_stop.Train_Route_On_Date_Id == train_route_on_date.Id)).ToListAsync();

            //Тут з поїздів фільтруються ті, які проходять через бажані станції в потрібному порядку 
            List<InternalTrainRaceDto> actual_train_routes_on_date = new List<InternalTrainRaceDto>();
            foreach (TrainRouteOnDate train_route_on_date in possible_train_routes_on_date)
            {
                //Зупинки для даного маршруту в порядку слідування 
                List<TrainRouteOnDateOnStation> train_stops_for_current_train_route_on_date = await context.Train_Routes_On_Date_On_Stations
                    .Include(train_stop => train_stop.Train_Route_On_Date)
                    .Include(train_stop => train_stop.Station)
                    .Where(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date.Id)
                    .OrderBy(train_stop => train_stop.Arrival_Time).ToListAsync();
                //Початкова зупинка для всього маршруту
                TrainRouteOnDateOnStation full_route_start_stop = train_stops_for_current_train_route_on_date[0];
                //Кінцева зупинка для всього маршруту
                TrainRouteOnDateOnStation full_route_ending_stop = train_stops_for_current_train_route_on_date[train_stops_for_current_train_route_on_date.Count - 1];
                //Порядок стартової зупинки маршруту поїздки(не всього маршруту)
                int trip_start_station_index = train_stops_for_current_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == start_station_title);
                TrainRouteOnDateOnStation? trip_start_station = train_stops_for_current_train_route_on_date.FirstOrDefault(train_stop => train_stop.Station.Title == start_station_title);
                //Порядок кінцевої зупинки маршруту поїздки
                int trip_end_station_index = train_stops_for_current_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == end_station_title);
                TrainRouteOnDateOnStation? trip_end_station = train_stops_for_current_train_route_on_date.FirstOrDefault(train_stop => train_stop.Station.Title == end_station_title);
                if (trip_start_station_index == -1 || trip_end_station_index == -1 || trip_start_station is null || trip_end_station is null)
                {
                    return new FailQuery<List<InternalTrainRaceDto>>(new Error(ErrorType.NotFound, $"Can't find one of the stations"));
                }
                DateTime? trip_start_station_departure_time = trip_start_station.Departure_Time;
                DateTime? trip_end_station_arrival_time = trip_end_station.Arrival_Time;
                if (trip_start_station_departure_time is null || trip_end_station_arrival_time is null)
                {
                    continue;
                }
                DateTime _trip_start_station_departure_time = (DateTime)trip_start_station_departure_time;
                DateTime _trip_end_station_arrival_time = (DateTime)trip_end_station_arrival_time;
                //Якщо стартова зупинка поїздки йде до кінцевої зупинки поїздки(поїзд проходить зупинки в потрібному напрямку), то він додається в список
                if (trip_start_station_index < trip_end_station_index)
                {
                    actual_train_routes_on_date.Add(new InternalTrainRaceDto
                    {
                        Train_Route_On_Date = train_route_on_date,
                        Departure_Time_From_Desired_Starting_Station = _trip_start_station_departure_time,
                        Arrival_Time_For_Desired_Ending_Station = _trip_end_station_arrival_time,
                        Desired_Starting_Station = trip_start_station,
                        Desired_Ending_Station = trip_end_station,
                        Km_Point_Of_Desired_Starting_Station = trip_start_station.Distance_From_Starting_Station,
                        Km_Point_Of_Desired_Ending_Station = trip_end_station.Distance_From_Starting_Station,
                        Full_Route_Starting_Stop = full_route_start_stop,
                        Full_Route_Ending_Stop = full_route_ending_stop,
                        Full_Route_Stops_List = train_stops_for_current_train_route_on_date
                    });
                }
            }
            return new SuccessQuery<List<InternalTrainRaceDto>>(actual_train_routes_on_date);
        }

        [Checked("18.04.2025")]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_on_date_id, bool order_mode = true)
        {
            //TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Train_Stops)
                .ThenInclude(train_stop => train_stop.Station)
                .Include(train_route_on_date => train_route_on_date.Train_Route)
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == train_route_on_date_id);
            if (train_route_on_date is null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            List<TrainRouteOnDateOnStation> train_stops = train_route_on_date.Train_Stops;
            if (order_mode)
            {
                train_stops = train_stops.OrderBy(train_stop => train_stop.Arrival_Time).ToList();
            }
            return train_stops;
        }

        [Checked("18.04.2025")]
        [Peripheral]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            string train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date);
            return await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
        }

        [Refactored("v1", "18.04.2025")]
        [Crucial]
        public async Task<List<TrainRouteOnDateOnStation>> GetTrainStopsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            List<TrainRouteOnDateOnStation> train_stops_for_several_train_routes_on_date = await context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Include(train_stop => train_stop.Station)
                .Where(train_stop => train_route_on_date_ids.Contains(train_stop.Train_Route_On_Date_Id))
                .OrderBy(train_stop => train_stop.Train_Route_On_Date_Id).ThenBy(train_stop => train_stop.Arrival_Time).ToListAsync();
            return train_stops_for_several_train_routes_on_date;
        }
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsBetweenTwoStationsForTrainRouteOnDate(string train_route_on_date_id, string first_station_title, string second_station_title)
        {
            List<TrainRouteOnDateOnStation>? train_stops = await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (train_stops == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            Station? starting_station = await station_service.FindStationByTitle(first_station_title);
            Station? ending_station = await station_service.FindStationByTitle(second_station_title);
            if (starting_station is null || ending_station is null)
            {
                text_service.FailPostInform("Fail in StationService");
                return null;
            }
            int starting_station_number = train_stops.FindIndex(train_route => train_route.Station.Title == first_station_title);
            int ending_station_number = train_stops.FindIndex(train_route => train_route.Station.Title == second_station_title);
            if (starting_station_number == -1 || ending_station_number == -1)
            {
                text_service.FailPostInform("Train route on date doesn't pass through the station");
                return null;
            }
            List<TrainRouteOnDateOnStation> final_train_stops = new List<TrainRouteOnDateOnStation>();
            for (int station_number = 0; station_number < train_stops.Count; station_number++)
            {
                if (station_number >= starting_station_number && station_number <= ending_station_number)
                {
                    final_train_stops.Add(train_stops[station_number]);
                }
            }
            return final_train_stops;

        }

        [Peripheral]
        public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsBetweenNumbersForTrainRouteOnDate(string train_route_on_date_id, int start_index, int end_index)
        {
            List<TrainRouteOnDateOnStation>? train_stops = await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id);
            if (train_stops == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            if (start_index < 0 || start_index >= train_stops.Count)
            {
                text_service.FailPostInform("Invalid station index");
                return null;
            }
            if (end_index < 0 || end_index >= train_stops.Count || end_index < start_index)
            {
                text_service.FailPostInform("Invalid station index");
                return null;
            }
            List<TrainRouteOnDateOnStation> final_train_stops = new List<TrainRouteOnDateOnStation>();
            for (int station_number = 0; station_number < train_stops.Count; station_number++)
            {
                if (station_number >= start_index && station_number <= end_index)
                {
                    final_train_stops.Add(train_stops[station_number]);
                }
            }
            return final_train_stops;

        }

        [Checked("18.04.2025")]
        [Peripheral]
        public async Task<List<Station>?> GetStationsForTrainRouteOnDate(string train_route_on_date_id)
        {
            return (await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id))?.
               Select(train_stop => train_stop.Station).ToList();
        }

        [Checked("18.04.2025")]
        [Executive]
        public async Task<TrainRouteOnDateOnStation?> GetStartingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            List<TrainRouteOnDateOnStation>? train_stops = await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode: true);
            if (train_stops == null)
            {
                text_service.FailPostInform($"Can't find stops for train route on date");
                return null;
            }
            return train_stops[0];
        }

        [Checked("18.04.2025")]
        [Executive]
        public async Task<TrainRouteOnDateOnStation?> GetEndingTrainStopForTrainRouteOnDate(string train_route_on_date_id)
        {
            List<TrainRouteOnDateOnStation>? train_stops = await GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode: true);
            if (train_stops == null)
            {
                text_service.FailPostInform($"Can't find stops for train route on date");
                return null;
            }
            return train_stops[train_stops.Count - 1];
        }
        public async Task<TrainRouteOnDateOnStation?> GetTrainStopInfoByTrainRouteOnDateIdAndStationId(string train_route_on_date_id, int station_id)
        {
            return await context.Train_Routes_On_Date_On_Stations
                .FirstOrDefaultAsync(train_stop => train_stop.Train_Route_On_Date_Id == train_route_on_date_id &&
                train_stop.Station_Id == station_id);
        }
        [Refactored("v1", "18.04.2025")]
        [Executive]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_on_date_id)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Carriage_Assignements)
                .ThenInclude(carriage_assignment => carriage_assignment.Passenger_Carriage)
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == train_route_on_date_id);
            if (train_route_on_date is null)
            {
                return null;
            }
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments = train_route_on_date.Carriage_Assignements
                .OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();
            return carriage_assignments;
        }

        [Checked("18.04.2025")]
        [Peripheral]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            string train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(train_route_id, departure_date);
            return await GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_id);
        }

        [Refactored("v1", "18.04.2025")]
        [Crucial]
        public async Task<List<PassengerCarriageOnTrainRouteOnDate>> GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids)
        {
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignment => carriage_assignment.Train_Route_On_Date)
                .Include(carriage_assignment => carriage_assignment.Passenger_Carriage)
                .Where(carriage_assignment => train_route_on_date_ids.Contains(carriage_assignment.Train_Route_On_Date_Id))
                .OrderBy(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id)
                .ThenBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToListAsync();
            return carriage_assignments;
        }

        [Checked("18.04.2025")]
        [Peripheral]
        public async Task<List<PassengerCarriage>?> GetPassengerCarriagesForTrainRouteOnDate(string train_route_on_date_id)
        {
            return (await GetPassengerCarriageAssignmentsForTrainRouteOnDate(train_route_on_date_id))?.
                Select(train_assignement => train_assignement.Passenger_Carriage).ToList();
        }

















        //public async Task<List<TrainRouteOnDateArrivalDepartureTimeDto>?> SearchTrainRouteBetweenStationOnDate(string start_station_title, string end_station_title,
        //    DateOnly departure_date)
        //{
        //    Station? start_station = await station_service.FindStationByTitle(start_station_title);
        //    Station? end_station = await station_service.FindStationByTitle(end_station_title);
        //    if (start_station == null || end_station == null)
        //    {
        //        text_service.FailPostInform("Fail in StationService");
        //        return null;
        //    }

        //    List<TrainRouteOnDate> possible_train_routes_on_date = await context.Train_Routes_On_Date
        //        .Include(train_route_on_date => train_route_on_date.Train_Route)
        //        .Include(train_route_on_date => train_route_on_date.Train_Stops)
        //        .ThenInclude(train_stop => train_stop.Station)
        //        .Where(train_route_on_date => train_route_on_date.Train_Stops.Where(train_stop => train_stop.Departure_Time.HasValue)
        //        .Any(train_stop => DateOnly.FromDateTime((DateTime)train_stop.Departure_Time!) == departure_date
        //        && train_stop.Station.Title == start_station_title)
        //        && train_route_on_date.Train_Stops
        //        .Any(train_stop => train_stop.Station.Title == end_station_title)).ToListAsync();

        //    List<TrainRouteOnDateArrivalDepartureTimeDto> final_train_routes_on_date =
        //        new List<TrainRouteOnDateArrivalDepartureTimeDto>();
        //    foreach (TrainRouteOnDate train_route_on_date in possible_train_routes_on_date)
        //    {
        //        List<TrainRouteOnDateOnStation> train_stops_for_train_route_on_date = train_route_on_date
        //            .Train_Stops.OrderBy(train_stop => train_stop.Arrival_Time).ToList();
        //        int full_route_stops_count = train_stops_for_train_route_on_date.Count;
        //        TrainRouteOnDateOnStation? full_route_starting_stop = train_stops_for_train_route_on_date[0];
        //        TrainRouteOnDateOnStation? full_route_ending_stop = train_stops_for_train_route_on_date[full_route_stops_count - 1];

        //        TrainRouteOnDateOnStation? desired_start_station = train_stops_for_train_route_on_date.FirstOrDefault(train_stop =>
        //        train_stop.Station.Title == start_station_title);
        //        TrainRouteOnDateOnStation? desired_end_station = train_stops_for_train_route_on_date.FirstOrDefault(train_stop =>
        //        train_stop.Station.Title == end_station_title);
        //        if (desired_start_station is null || desired_end_station is null)
        //        {
        //            Console.WriteLine("Can't find one of the stations");
        //            return null;
        //        }
        //        DateTime? _desired_start_station_departure_time = desired_start_station.Departure_Time;
        //        DateTime? _desired_end_station_arrival_time = desired_end_station.Arrival_Time;
        //        if (_desired_start_station_departure_time == null || _desired_end_station_arrival_time == null)
        //        {
        //            continue;
        //        }
        //        DateTime desired_start_station_departure_time = (DateTime)_desired_start_station_departure_time;
        //        DateTime desired_end_station_arrival_time = (DateTime)_desired_end_station_arrival_time;

        //        if (desired_start_station.Departure_Time is null || desired_end_station.Arrival_Time is null)
        //        {
        //            continue;
        //        }
        //        if (desired_start_station.Departure_Time < desired_end_station.Arrival_Time)
        //        {
        //            final_train_routes_on_date.Add(new TrainRouteOnDateArrivalDepartureTimeDto
        //            {
        //                Train_Route_On_Date = train_route_on_date,
        //                Departure_Time_From_Desired_Starting_Station = desired_start_station_departure_time,
        //                Arrival_Time_For_Desired_Ending_Station = desired_end_station_arrival_time,
        //                Route_Starting_Stop = full_route_starting_stop,
        //                Route_Ending_Stop = full_route_ending_stop,
        //                Full_Route_Stops_List = train_stops_for_train_route_on_date,
        //                Km_Point_Of_Desired_Starting_Station = desired_start_station.Distance_From_Starting_Station,
        //                Km_Point_Of_Desired_Ending_Station = desired_end_station.Distance_From_Starting_Station

        //            });
        //        }

        //    }
        //    return final_train_routes_on_date.OrderBy(train_route => train_route.Departure_Time_From_Desired_Starting_Station).ToList();
        //}
    }
}







