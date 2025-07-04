using RailwayCore.Context;
using RailwayCore.InternalServices.ModelServices;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices
{
    public class TrainTripsSearchService
    {
        private readonly AppDbContext context;
        private readonly StationRepository station_service;
        public TrainTripsSearchService(AppDbContext context, StationRepository station_service)
        {
            this.context = context;
            this.station_service = station_service;
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

            //Пошук поїздів, які проходять через 2 задані станції, причому відправляються з початкової станції в указану дату
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
    }
}
