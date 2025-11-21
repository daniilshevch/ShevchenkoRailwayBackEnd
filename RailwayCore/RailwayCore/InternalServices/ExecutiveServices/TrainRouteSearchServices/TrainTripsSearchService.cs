using RailwayCore.Context;
using Microsoft.EntityFrameworkCore;
using RailwayCore.Models;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices
{
    /// <summary>
    /// Даний сервіс відповідає за пошук рейсів між двома станціями в дату або пошук рейсів, які проходять транзитом через
    /// конкретну станцію в певну дату. Він шукає лише рейси, не надає жодної інформації про  розклад руху поїзда на всьому маршруті, 
    /// склад поїзда або бронювання квитків в цьому рейсі поїзда
    /// </summary>
    [ExecutiveService]
    public class TrainTripsSearchService
    {
        private readonly string service_name = "TrainTripsSearchService";
        private readonly AppDbContext context;
        private readonly IStationRepository station_repository;
        public TrainTripsSearchService(AppDbContext context, IStationRepository station_repository)
        {
            this.context = context;
            this.station_repository = station_repository;
        }
        /// <summary>
        /// Функція є допоміжною в методі SearchTrainRoutesBetweenStationsOnDate. Вона шукає всі рейси, які проходять через дві вказані станції
        /// і проходять початкову в певну дату. Але ця функція повертає і поїзди, які їдуть в зворотньому напрямку, тому цей список
        /// потребує додаткової фільтрації
        /// </summary>
        /// <param name="start_station_title"></param>
        /// <param name="end_station_title"></param>
        /// <param name="trip_departure_date"></param>
        /// <returns></returns>
        [PartialLogicMethod]
        public async Task<List<TrainRouteOnDate>> _SearchAllTrainRoutesBetweenStationsOnDateWithNoPassingOrderConsideration
            (string start_station_title, string end_station_title, DateOnly trip_departure_date)
        {
            //Пошук поїздів, які проходять через 2 задані станції, причому відправляються з початкової станції в указану дату
            //(Порядок слідування між станціями не перевіряється => сюди будуть включені і поїзди, які їдуть у зворотньому напрямку)
            List<TrainRouteOnDate> possible_train_routes_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Train_Route)
                .Where(train_route_on_date => context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Station)
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Where(train_stop => train_stop.Departure_Time != null).Any(train_stop => train_stop.Station.Title == start_station_title
                && train_stop.Train_Route_On_Date_Id == train_route_on_date.Id && DateOnly.FromDateTime(train_stop.Departure_Time!.Value) == trip_departure_date) &&
                context.Train_Routes_On_Date_On_Stations.Any(train_stop => train_stop.Station.Title == end_station_title
                && train_stop.Train_Route_On_Date_Id == train_route_on_date.Id)).ToListAsync();
            return possible_train_routes_on_date;
        }
        /// <summary>
        /// Метод є допоміжним в функції SearchTrainRoutesBetweenStationsOnDate. Даний метод проводить фільтрацію з переданого первинного 
        /// списку поїздів і вертає тільки ті поїзди, які проходять бажані початкову та кінцеву станції подорожі в правильному порядку. 
        /// Крім того, метод одразу трансформує дані в трансферний об'єкт з основними характеристиками рейсу
        /// </summary>
        /// <param name="possible_train_routes_on_date"></param>
        /// <param name="start_station_title"></param>
        /// <param name="end_station_title"></param>
        /// <returns></returns>
        [PartialLogicMethod]
        public async Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> _FilterTrainRoutesBetweenStationsOnDateByPassingOrderAndTransformIntoTrainRaceDto(List<TrainRouteOnDate> possible_train_routes_on_date,
            string start_station_title, string end_station_title, DateOnly trip_departure_date)
        {
            //Тут з поїздів фільтруються ті, які проходять через бажані станції в потрібному порядку 
            List<InternalTrainRaceBetweenStationsDto> actual_train_routes_on_date = new List<InternalTrainRaceBetweenStationsDto>();
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
                //Порядковий номер стартової зупинки маршруту поїздки(не всього маршруту)
                int trip_start_station_index = train_stops_for_current_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == start_station_title);
                TrainRouteOnDateOnStation? trip_start_station = train_stops_for_current_train_route_on_date.FirstOrDefault(train_stop => train_stop.Station.Title == start_station_title);
                //Порядковий номер кінцевої зупинки маршруту поїздки
                int trip_end_station_index = train_stops_for_current_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == end_station_title);
                TrainRouteOnDateOnStation? trip_end_station = train_stops_for_current_train_route_on_date.FirstOrDefault(train_stop => train_stop.Station.Title == end_station_title);
                if (trip_start_station_index == -1 || trip_end_station_index == -1 || trip_start_station is null || trip_end_station is null)
                {
                    return new FailQuery<List<InternalTrainRaceBetweenStationsDto>>(new Error(ErrorType.NotFound, $"Can't find one of the stations", annotation: service_name, unit: ProgramUnit.Core));
                }
                //Отримання часів відправлення та прибуття з початкової та кінцевої станції
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
                    actual_train_routes_on_date.Add(new InternalTrainRaceBetweenStationsDto
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
            return new SuccessQuery<List<InternalTrainRaceBetweenStationsDto>>(actual_train_routes_on_date, new SuccessMessage($"Successfuly found races between" +
                $" {start_station_title} and {end_station_title} on date {trip_departure_date}", annotation: service_name, unit: ProgramUnit.Core));
        }


        /// <summary>
        /// Даний метод шукає рейси поїздів між двома станціями в певну дату. Фактично є основою для функції пошуку квитків в системі.
        /// Але надає лише інформацію про самі рейси, які курсують між станціями в конкретну дату. Ніяк не відповідає за визначення
        /// складу поїзда та броні квитків в цьому рейсі
        /// </summary>
        /// <param name="start_station_title"></param>
        /// <param name="end_station_title"></param>
        /// <param name="trip_departure_date"></param>
        /// <returns></returns>
        [Refactored("v1", "18.04.2025")]
        [Checked("24.10.2025")]
        [ExecutiveMethod]
        public async Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> SearchTrainRoutesBetweenStationsOnDate(string start_station_title, string end_station_title, DateOnly trip_departure_date)
        {
            Station? start_station = await station_repository.GetStationByTitle(start_station_title); // Пошук стартової зупинки
            Station? end_station = await station_repository.GetStationByTitle(end_station_title); // Пошук кінцевої зупинки
            if (start_station is null)
            {
                return new FailQuery<List<InternalTrainRaceBetweenStationsDto>>(new Error(ErrorType.NotFound, $"Can't find starting station with title: {start_station_title}", annotation: service_name, unit: ProgramUnit.Core));
            }
            if (end_station is null)
            {
                return new FailQuery<List<InternalTrainRaceBetweenStationsDto>>(new Error(ErrorType.NotFound, $"Can't find ending station with title: {end_station_title}", annotation: service_name, unit: ProgramUnit.Core));
            }
            //Пошук поїздів, які проходять через 2 задані станції, причому відправляються з початкової станції в указану дату
            //(Порядок слідування між станціями не перевіряється => сюди будуть включені і поїзди, які їдуть у зворотньому напрямку)
            List<TrainRouteOnDate> possible_train_routes_on_date = await
                _SearchAllTrainRoutesBetweenStationsOnDateWithNoPassingOrderConsideration(start_station_title, end_station_title, trip_departure_date);
            //Тут з поїздів фільтруються ті, які проходять через бажані станції в потрібному порядку. Також інформація трансформується
            //в трансферні об'єкти.
            QueryResult<List<InternalTrainRaceBetweenStationsDto>> actual_train_routes_on_date_get_result = await
                _FilterTrainRoutesBetweenStationsOnDateByPassingOrderAndTransformIntoTrainRaceDto(possible_train_routes_on_date, start_station_title, end_station_title, trip_departure_date);

            return actual_train_routes_on_date_get_result;
        }

        public async Task<QueryResult<List<InternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStationOnDate(string station_title, DateTime time, 
            TimeSpan? left_interval = null, TimeSpan? right_interval = null)
        {
            if(left_interval is null)
            {
                left_interval = TimeSpan.FromHours(3);
            }
            if(right_interval is null)
            {
                right_interval = TimeSpan.FromHours(3);
            }
            Station? station = await station_repository.GetStationByTitle(station_title);
            if(station is null)
            {
                return new FailQuery<List<InternalTrainRaceThroughStationDto>>(new Error(ErrorType.NotFound, $"Station {station_title} has not been found"));
            }
            List<TrainRouteOnDate> appropriate_train_routes_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Train_Route)
                .Where(train_route_on_date =>
                context.Train_Routes_On_Date_On_Stations.Include(train_stop => train_stop.Station)
                .Any(train_stop => train_stop.Station.Title == station_title &&
                train_stop.Train_Route_On_Date_Id == train_route_on_date.Id &&
                ((train_stop.Arrival_Time > time - left_interval && train_stop.Arrival_Time < time + right_interval) ||
                (train_stop.Departure_Time > time - left_interval && train_stop.Departure_Time < time + right_interval)))).ToListAsync();
            
            List<InternalTrainRaceThroughStationDto> final_train_routes_on_date = new List<InternalTrainRaceThroughStationDto>();
            foreach(TrainRouteOnDate train_race in appropriate_train_routes_on_date)
            {
                    List<TrainRouteOnDateOnStation> train_stops_for_current_train_route_on_date = await context.Train_Routes_On_Date_On_Stations
                .Include(train_stop => train_stop.Train_Route_On_Date)
                .Include(train_stop => train_stop.Station)
                .Where(train_stop => train_stop.Train_Route_On_Date_Id == train_race.Id)
                .OrderBy(train_stop => train_stop.Arrival_Time).ToListAsync();
                TrainRouteOnDateOnStation starting_stop = train_stops_for_current_train_route_on_date[0];
                TrainRouteOnDateOnStation ending_stop = train_stops_for_current_train_route_on_date[train_stops_for_current_train_route_on_date.Count - 1];
                TrainRouteOnDateOnStation current_stop = train_stops_for_current_train_route_on_date.FirstOrDefault(train_stop => train_stop.Station.Title == station_title)!;
                DateTime? arrival_time_to_current_stop = current_stop.Arrival_Time;
                DateTime? departure_time_from_current_stop = current_stop.Departure_Time;
                double? km_point_of_current_stop = current_stop.Distance_From_Starting_Station;
                final_train_routes_on_date.Add(new InternalTrainRaceThroughStationDto()
                {
                    Train_Route_On_Date = train_race,
                    Full_Route_Starting_Stop = starting_stop,
                    Full_Route_Ending_Stop = ending_stop,
                    Current_Stop = current_stop,
                    Arrival_Time_To_Current_Stop = arrival_time_to_current_stop,
                    Departure_Time_From_Current_Stop = departure_time_from_current_stop,
                    Full_Route_Stops_List = train_stops_for_current_train_route_on_date,
                    Km_Point_Of_Current_Station = km_point_of_current_stop
                });
            }
            return new SuccessQuery<List<InternalTrainRaceThroughStationDto>>(final_train_routes_on_date);
        }
    }
}
