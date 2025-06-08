using RailwayManagementSystemAPI.API_DTO;
using RailwayCore.InternalServices.CoreServices;
using RailwayCore.Models;
using System.Diagnostics;
using RailwayCore.InternalDTO.CoreDTO;
using RailwayCore.InternalServices.SystemServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
    public class TrainRouteWithBookingsSearchService
    {
        private readonly FullTrainRouteSearchService full_train_route_search_service;
        private readonly FullTicketManagementService full_ticket_management_service;
        public TrainRouteWithBookingsSearchService(FullTrainRouteSearchService full_train_route_search_service, FullTicketManagementService full_ticket_management_service)
        {
            this.full_train_route_search_service = full_train_route_search_service;
            this.full_ticket_management_service = full_ticket_management_service;
        }


        [Refactored("v1", "19.04.2025")]
        public async Task<QueryResult<List<ExternalTrainRouteWithBookingsInfoDto>>> SearchTrainRoutesBetweenStationsWithBookingsInfo
            (string starting_station_title, string ending_station_title, DateOnly departure_date, bool admin_mode = false)
        {
            Stopwatch sw = Stopwatch.StartNew();
            //Отримуємо список поїздів, які проходять через дані станції в потрібному порядку в потрібну дату
            QueryResult<List<InternalTrainRaceDto>> train_routes_list_result = await full_train_route_search_service.SearchTrainRoutesBetweenStationsOnDate(starting_station_title,
                ending_station_title, departure_date);
            if (train_routes_list_result.Fail)
            {
                return new FailQuery<List<ExternalTrainRouteWithBookingsInfoDto>>(train_routes_list_result.Error);
            }
            List<InternalTrainRaceDto> appropriate_train_routes_on_date = train_routes_list_result.Value;

            //Беремо список айді знайдених поїздів(потрібно для функції ядра сервера, яке перевіряє бронювання для поїздів)
            List<string> appropriate_train_routes_on_date_ids =
                appropriate_train_routes_on_date.Select(train_route_on_date_info => train_route_on_date_info.Train_Route_On_Date.Id).ToList();

            QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>? train_routes_on_date_bookings_statistics_result = null;
            //Отримуємо інформацію про всі бронювання для всіх рейсів поїздів зі списку(ми отримали вище їх айді)
            if (admin_mode == false) //В залежності від того, який режим, включаємо чи не включаємо інформацію про пасажирів
            {
                train_routes_on_date_bookings_statistics_result = await full_ticket_management_service.
                    GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate(appropriate_train_routes_on_date_ids, starting_station_title, ending_station_title);
            }
            else
            {
                train_routes_on_date_bookings_statistics_result = await full_ticket_management_service.
                   GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics(appropriate_train_routes_on_date_ids, starting_station_title, ending_station_title);
            }
            if (train_routes_on_date_bookings_statistics_result.Fail)
            {
                return new FailQuery<List<ExternalTrainRouteWithBookingsInfoDto>>(train_routes_on_date_bookings_statistics_result.Error);
            }
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> ticket_bookings_info_for_appropriate_train_routes =
                train_routes_on_date_bookings_statistics_result.Value;

            //Ініціалізуємо список, де кожен елемент буде містити всю потрібну інформацію по одному конкретному поїзду
            List<ExternalTrainRouteWithBookingsInfoDto> total_train_routes_with_bookings_and_stations_info = new List<ExternalTrainRouteWithBookingsInfoDto>();

            //Проходимось по кожному поїзду зі списку поїздів, які проходять через дані станції і для яких ми знайшли статистику бронювань
            foreach (KeyValuePair<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> single_train_route_on_date_statistics
                in ticket_bookings_info_for_appropriate_train_routes)
            {
                //Отримуємо азагальну інформацію про даний маршрут поїзда(поки без бронювання), воно буде в першу чергу показуватись на сторінці списку знайдених поїздів
                InternalTrainRaceDto? current_train_route_trip_info = appropriate_train_routes_on_date.FirstOrDefault(train_race =>
                train_race.Train_Route_On_Date.Id == single_train_route_on_date_statistics.Key);
                if (current_train_route_trip_info is null)
                {
                    return new FailQuery<List<ExternalTrainRouteWithBookingsInfoDto>>(new Error(ErrorType.InternalServerError, $"Fail while searching info about train route on date " +
                        $"{single_train_route_on_date_statistics.Key}"));
                }

                //Отримуємо інформацію про рейс поїзда(але не пов'язану з бронюванням місць)
                DateTime departure_time_from_trip_starting_station = current_train_route_trip_info.Departure_Time_From_Desired_Starting_Station;
                DateTime arrival_time_to_trip_ending_station = current_train_route_trip_info.Arrival_Time_For_Desired_Ending_Station;
                string full_route_starting_station_title = current_train_route_trip_info.Full_Route_Starting_Stop.Station.Title;
                string full_route_ending_station_title = current_train_route_trip_info.Full_Route_Ending_Stop.Station.Title;

                double? trip_starting_stop_km_point = current_train_route_trip_info.Km_Point_Of_Desired_Starting_Station;
                double? trip_ending_stop_km_point = current_train_route_trip_info.Km_Point_Of_Desired_Ending_Station;
                double trip_distance = 0;
                if (trip_starting_stop_km_point is not null && trip_ending_stop_km_point is not null)
                {
                    trip_distance = (double)trip_ending_stop_km_point - (double)trip_starting_stop_km_point;
                }

                //Отримуємо список об'єктів, кожен з яких становить статистику бронювань в одному вагоні даного поїзда(внутрішній трансфер)
                List<InternalCarriageAssignmentRepresentationDto> internal_carriage_statistics_for_current_train_route_on_date =
                    single_train_route_on_date_statistics.Value.Carriage_Statistics_List;
                //Ініціалізовуємо аналогічний список об'єктів-статистик бронювань в одном вагоні поїзда, але призначений для зовнішньої демонстрації
                //Вся докладна інформація про вагон, включаючи номер в складі, тип, послуги та бронювання міститься тут (також вираховується ціна подорожі)
                List<ExternalSinglePassengerCarriageBookingsInfoDto> external_carriage_statistics_for_current_train_route_on_date =
                    internal_carriage_statistics_for_current_train_route_on_date.Select(single_carriage_info => new ExternalSinglePassengerCarriageBookingsInfoDto
                    {
                        Carriage_Position_In_Squad = single_carriage_info.Carriage_Assignment.Position_In_Squad,
                        Carriage_Type = TextEnumConvertationService.GetCarriageTypeIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Type_Of),
                        Quality_Class = TextEnumConvertationService.GetCarriageQualityClassIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Quality_Class),
                        Free_Places = single_carriage_info.Free_Places,
                        Total_Places = single_carriage_info.Total_Places,
                        Air_Conditioning = single_carriage_info.Carriage_Assignment.Factual_Air_Conditioning,
                        Food_Availability = single_carriage_info.Carriage_Assignment.Food_Availability,
                        Shower_Availability = single_carriage_info.Carriage_Assignment.Factual_Shower_Availability,
                        WiFi_Availability = single_carriage_info.Carriage_Assignment.Factual_Wifi_Availability,
                        Is_Inclusive = single_carriage_info.Carriage_Assignment.Factual_Is_Inclusive,
                        Places_Availability = single_carriage_info.Places_Availability,
                        Ticket_Price = PricingService.DefineTicketPrice(
                            carriage_type: single_carriage_info.Carriage_Assignment.Passenger_Carriage.Type_Of,
                            carriage_quality_class: single_carriage_info.Carriage_Assignment.Passenger_Carriage.Quality_Class,
                            distance: trip_distance,
                            _train_route_coefficient: current_train_route_trip_info.Train_Route_On_Date.Train_Route.Train_Route_Coefficient,
                            _train_race_coefficient: current_train_route_trip_info.Train_Route_On_Date.Train_Race_Coefficient,
                            departure_date: current_train_route_trip_info.Train_Route_On_Date.Departure_Date,
                            _average_speed: FullTrainAssignementService.
                            FindSpeedOnSection(trip_starting_stop_km_point, trip_ending_stop_km_point, departure_time_from_trip_starting_station, arrival_time_to_trip_ending_station)
                        ),
                    }).ToList();

                //Знаходимо мінімальну ціну для кожного типу вагона
                int min_platskart_price = 10000000;
                int min_coupe_price = 10000000;
                int min_sv_price = 1000000;


                //Отримуємо список всіх зупинок на маршруті в порядку слідування поїзда
                List<TrainRouteOnDateOnStation> train_stops_for_current_train_route_on_date = current_train_route_trip_info.Full_Route_Stops_List;
                List<ExternalSingleTrainStopDto> external_train_stops = new List<ExternalSingleTrainStopDto>();

                //Знаходимо номер за рахунком у маршруті початкової станції подорожі
                int trip_start_stop_index = train_stops_for_current_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == starting_station_title);
                //Знаходимо номер за рахунком у маршруті кінцевої станції подорожі
                int trip_end_stop_index = train_stops_for_current_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == ending_station_title);

                //Ініціалізуємо лічильник,який буде рахувати, яка за рахунком дана станція при переборі
                int current_stop_index = 0;
                // Перебираємо всі станції на маршруті, збираємо інформацію про них і додаємо в список зовнішніх трансферів
                foreach (TrainRouteOnDateOnStation current_train_stop in train_stops_for_current_train_route_on_date)
                {
                    DateTime? arrival_time_to_stop = current_train_stop.Arrival_Time;
                    DateTime? departure_time_from_stop = current_train_stop.Departure_Time;
                    TimeSpan? stop_duration = null;
                    if (arrival_time_to_stop is not null && departure_time_from_stop is not null)
                    {
                        stop_duration = (DateTime)departure_time_from_stop - (DateTime)arrival_time_to_stop;
                    }
                    bool is_part_of_trip = false;
                    if (current_stop_index >= trip_start_stop_index && current_stop_index <= trip_end_stop_index)
                    {
                        is_part_of_trip = true;
                    }
                    current_stop_index++;
                    //Додаємо зупинку в список зовнішніх трансферів
                    external_train_stops.Add(new ExternalSingleTrainStopDto()
                    {
                        Station_Title = current_train_stop.Station.Title,
                        Arrival_Time = arrival_time_to_stop,
                        Departure_Time = departure_time_from_stop,
                        Stop_Duration = stop_duration,
                        Is_Part_Of_Trip = is_part_of_trip,
                    });
                }
                //Додаємо в список статистик по кожному поїзду статистику для даного поїзда
                total_train_routes_with_bookings_and_stations_info.Add(new ExternalTrainRouteWithBookingsInfoDto()
                {
                    Full_Train_Route_On_Date_Id = current_train_route_trip_info.Train_Route_On_Date.Id,
                    Train_Route_Id = current_train_route_trip_info.Train_Route_On_Date.Train_Route_Id,
                    Train_Route_Branded_Name = current_train_route_trip_info.Train_Route_On_Date.Train_Route.Branded_Name,
                    Train_Route_Class = TextEnumConvertationService.GetTrainQualityClassIntoString(current_train_route_trip_info.Train_Route_On_Date.Train_Route.Quality_Class),
                    Trip_Starting_Station_Title = current_train_route_trip_info.Desired_Starting_Station.Station.Title,
                    Trip_Ending_Station_Title = current_train_route_trip_info.Desired_Ending_Station.Station.Title,
                    Trip_Starting_Station_Departure_Time = current_train_route_trip_info.Departure_Time_From_Desired_Starting_Station,
                    Trip_Ending_Station_Arrival_Time = current_train_route_trip_info.Arrival_Time_For_Desired_Ending_Station,
                    Total_Trip_Duration = current_train_route_trip_info.Arrival_Time_For_Desired_Ending_Station - current_train_route_trip_info.Departure_Time_From_Desired_Starting_Station,
                    Full_Route_Starting_Station_Title = current_train_route_trip_info.Full_Route_Starting_Stop.Station.Title,
                    Full_Route_Ending_Station_Title = current_train_route_trip_info.Full_Route_Ending_Stop.Station.Title,
                    Free_Platskart_Places = single_train_route_on_date_statistics.Value.Total_Place_Highlights.Platskart_Free,
                    Total_Platskart_Places = single_train_route_on_date_statistics.Value.Total_Place_Highlights.Platskart_Total,
                    Free_Coupe_Places = single_train_route_on_date_statistics.Value.Total_Place_Highlights.Coupe_Free,
                    Total_Coupe_Places = single_train_route_on_date_statistics.Value.Total_Place_Highlights.Coupe_Total,
                    Free_SV_Places = single_train_route_on_date_statistics.Value.Total_Place_Highlights.SV_Free,
                    Total_SV_Places = single_train_route_on_date_statistics.Value.Total_Place_Highlights.SV_Total,
                    Min_Platskart_Price = single_train_route_on_date_statistics.Value.Total_Place_Highlights.Min_Platskart_Price,
                    Min_Coupe_Price = single_train_route_on_date_statistics.Value.Total_Place_Highlights.Min_Coupe_Price,
                    Min_SV_Price = single_train_route_on_date_statistics.Value.Total_Place_Highlights.Min_SV_Price,
                    Carriage_Statistics_List = external_carriage_statistics_for_current_train_route_on_date,
                    Train_Stops_List = external_train_stops,
                    Average_Speed_On_Trip = FullTrainAssignementService.FindSpeedOnSection(trip_starting_stop_km_point, trip_ending_stop_km_point, departure_time_from_trip_starting_station, arrival_time_to_trip_ending_station)

                });


            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            List<ExternalTrainRouteWithBookingsInfoDto> ordered_total_train_routes_with_bookings_and_stations_info = 
                total_train_routes_with_bookings_and_stations_info.OrderBy(train_route_full_info => train_route_full_info.Trip_Starting_Station_Departure_Time).ToList();
            return new SuccessQuery<List<ExternalTrainRouteWithBookingsInfoDto>>(ordered_total_train_routes_with_bookings_and_stations_info);

        }
         





        [Checked("19.04.2025")]
        public List<ExternalSingleTrainStopDto> GetScheduleForSpecificTrainRouteOnDate(ExternalTrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_on_date.Train_Stops_List;
        }
        public List<ExternalSingleTrainStopDto> GetScheduleForSpecificTrainRouteOnDateFromGeneralList
            (List<ExternalTrainRouteWithBookingsInfoDto> train_routes_on_date_statistics_list, string train_route_id)
        {
            ExternalTrainRouteWithBookingsInfoDto? desired_train_route_on_date = train_routes_on_date_statistics_list
                .FirstOrDefault(train_route_on_date_info => train_route_on_date_info.Train_Route_Id == train_route_id);
            return GetScheduleForSpecificTrainRouteOnDate(desired_train_route_on_date);
        }


        public List<ExternalSinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForAllPassengerCarriagesForSpecificTrainRouteOnDate(ExternalTrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_on_date.Carriage_Statistics_List;
        }
        public List<ExternalSinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate
            (ExternalTrainRouteWithBookingsInfoDto train_route_on_date, PassengerCarriageType? carriage_type = null, PassengerCarriageQualityClass? quality_class = null)
        {
            List<ExternalSinglePassengerCarriageBookingsInfoDto> output_result = train_route_on_date.Carriage_Statistics_List;
            if (carriage_type != null)
            {
                output_result = output_result
                    .Where(carriage_info => carriage_info.Carriage_Type == TextEnumConvertationService.GetCarriageTypeIntoString((PassengerCarriageType)carriage_type)).ToList();
            }
            if (quality_class != null)
            {
                output_result = output_result
                    .Where(carriage_info => carriage_info.Quality_Class == TextEnumConvertationService.GetCarriageQualityClassIntoString((PassengerCarriageQualityClass)quality_class)).ToList();
            }
            return output_result;
        }












        [Archieved]
        [NotInUse]
        public async Task<List<ExternalTrainRouteWithBookingsInfoDto>?> SearchTrainRoutesBetweenStationsWithBookingsInfo_OLDVERSION
           (string starting_station_title, string ending_station_title, DateOnly departure_date)
        {
            DateTime first = DateTime.Now;
            Console.WriteLine(first);
            List<ExternalTrainRouteWithBookingsInfoDto> result_list = new List<ExternalTrainRouteWithBookingsInfoDto>();
            QueryResult<List<InternalTrainRaceDto>> appropriate_train_routes_on_date_result =
                await full_train_route_search_service.SearchTrainRoutesBetweenStationsOnDate(starting_station_title, ending_station_title, departure_date);

            List<InternalTrainRaceDto> appropriate_train_routes_on_date = appropriate_train_routes_on_date_result!.Value;
            if (appropriate_train_routes_on_date == null)
            {
                return null;
            }
            List<string> appropriate_train_routes_on_date_ids = appropriate_train_routes_on_date
                .Select(train_route_info => train_route_info.Train_Route_On_Date.Id).ToList();

            QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>> ticket_bookings_info_for_appropriate_train_routes_result =
          await full_ticket_management_service.GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics
          (appropriate_train_routes_on_date_ids, starting_station_title, ending_station_title);

            /*QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>> ticket_bookings_info_for_appropriate_train_routes_result =
                await ticket_booking_service.GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate
                (appropriate_train_routes_on_date_ids, starting_station_title, ending_station_title);*/
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> ticket_bookings_info_for_appropriate_train_routes =
                ticket_bookings_info_for_appropriate_train_routes_result.Value!;
            if (ticket_bookings_info_for_appropriate_train_routes == null)
            {
                return null;
            }
            foreach (KeyValuePair<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> single_train_route in ticket_bookings_info_for_appropriate_train_routes)
            {
                InternalTrainRaceDto? current_train_route_info = appropriate_train_routes_on_date
                    .FirstOrDefault(train_route_info => train_route_info.Train_Route_On_Date.Id == single_train_route.Key);
                if (current_train_route_info == null)
                {
                    return null;
                }
                DateTime trip_starting_station_departure_time = current_train_route_info.Departure_Time_From_Desired_Starting_Station;
                DateTime trip_ending_station_arrival_time = current_train_route_info.Arrival_Time_For_Desired_Ending_Station;

                double? trip_starting_station_km_point = current_train_route_info.Km_Point_Of_Desired_Starting_Station;
                double? trip_ending_station_km_point = current_train_route_info.Km_Point_Of_Desired_Ending_Station;
                double trip_distance = 0;
                if (trip_starting_station_km_point is not null && trip_ending_station_km_point is not null)
                {
                    trip_distance = (double)trip_ending_station_km_point - (double)trip_starting_station_km_point;
                }
                string full_route_starting_station_title = current_train_route_info.Full_Route_Starting_Stop.Station.Title;
                string full_route_ending_station_title = current_train_route_info.Full_Route_Ending_Stop.Station.Title;



                List<ExternalSinglePassengerCarriageBookingsInfoDto> carriage_bookings_info = single_train_route.Value.Carriage_Statistics_List
                    .Select(single_carriage_info => new ExternalSinglePassengerCarriageBookingsInfoDto
                    {
                        Carriage_Position_In_Squad = single_carriage_info.Carriage_Assignment.Position_In_Squad,
                        Carriage_Type = TextEnumConvertationService.GetCarriageTypeIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Type_Of),
                        Quality_Class = TextEnumConvertationService.GetCarriageQualityClassIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Quality_Class),
                        Ticket_Price = PricingService.DefineTicketPrice(
                            carriage_type: single_carriage_info.Carriage_Assignment.Passenger_Carriage.Type_Of,
                            carriage_quality_class: single_carriage_info.Carriage_Assignment.Passenger_Carriage.Quality_Class,
                            distance: trip_distance,
                            _train_route_coefficient: current_train_route_info.Train_Route_On_Date.Train_Route.Train_Route_Coefficient,
                            _train_race_coefficient: current_train_route_info.Train_Route_On_Date.Train_Race_Coefficient,
                            departure_date: current_train_route_info.Train_Route_On_Date.Departure_Date,
                            _average_speed: FullTrainAssignementService.FindSpeedOnSection(trip_starting_station_km_point, trip_ending_station_km_point, trip_starting_station_departure_time, trip_ending_station_arrival_time)
                        ),
                        Free_Places = single_carriage_info.Free_Places,
                        Total_Places = single_carriage_info.Total_Places,
                        Places_Availability = single_carriage_info.Places_Availability,
                        Air_Conditioning = single_carriage_info.Carriage_Assignment.Factual_Air_Conditioning,
                        Food_Availability = single_carriage_info.Carriage_Assignment.Food_Availability,
                        Shower_Availability = single_carriage_info.Carriage_Assignment.Factual_Shower_Availability

                    }).ToList();
                List<TrainRouteOnDateOnStation> train_stops_full_info = current_train_route_info.Full_Route_Stops_List;
                List<ExternalSingleTrainStopDto> train_stops_dto = new List<ExternalSingleTrainStopDto>();
                foreach (TrainRouteOnDateOnStation train_stop in train_stops_full_info)
                {
                    DateTime? arrival_time = train_stop.Arrival_Time;
                    DateTime? departure_time = train_stop.Departure_Time;
                    TimeSpan? stop_duration;
                    if (arrival_time == null || departure_time == null)
                    {
                        stop_duration = null;
                    }
                    else
                    {
                        stop_duration = (DateTime)departure_time - (DateTime)arrival_time;
                    }
                    bool is_part_of_trip;
                    if (arrival_time != null && departure_time != null)
                    {
                        is_part_of_trip = departure_time >= trip_starting_station_departure_time && arrival_time <= trip_ending_station_arrival_time;
                    }
                    else if (arrival_time == null && departure_time != null)
                    {
                        is_part_of_trip = departure_time >= trip_starting_station_departure_time;
                    }
                    else
                    {
                        is_part_of_trip = arrival_time <= trip_ending_station_arrival_time;
                    }
                    train_stops_dto.Add(new ExternalSingleTrainStopDto()
                    {
                        Station_Title = train_stop.Station.Title,
                        Arrival_Time = arrival_time,
                        Departure_Time = departure_time,
                        Stop_Duration = stop_duration,
                        Is_Part_Of_Trip = is_part_of_trip
                    });
                }

                result_list.Add(new ExternalTrainRouteWithBookingsInfoDto
                {
                    Train_Route_Id = single_train_route.Value.Train_Route_On_Date.Train_Route.Id,
                    Train_Route_Branded_Name = single_train_route.Value.Train_Route_On_Date.Train_Route.Branded_Name,
                    Train_Route_Class = TextEnumConvertationService.GetTrainQualityClassIntoString(single_train_route.Value.Train_Route_On_Date.Train_Route.Quality_Class),
                    Free_Platskart_Places = single_train_route.Value.Total_Place_Highlights.Platskart_Free,
                    Total_Platskart_Places = single_train_route.Value.Total_Place_Highlights.Platskart_Total,
                    Free_Coupe_Places = single_train_route.Value.Total_Place_Highlights.Coupe_Free,
                    Total_Coupe_Places = single_train_route.Value.Total_Place_Highlights.Coupe_Total,
                    Free_SV_Places = single_train_route.Value.Total_Place_Highlights.SV_Free,
                    Total_SV_Places = single_train_route.Value.Total_Place_Highlights.SV_Total,
                    Min_Platskart_Price = single_train_route.Value.Total_Place_Highlights.Min_Platskart_Price,
                    Min_Coupe_Price = single_train_route.Value.Total_Place_Highlights.Min_Coupe_Price,
                    Min_SV_Price = single_train_route.Value.Total_Place_Highlights.Min_SV_Price,
                    Trip_Starting_Station_Title = starting_station_title,
                    Trip_Ending_Station_Title = ending_station_title,
                    Trip_Starting_Station_Departure_Time = trip_starting_station_departure_time,
                    Trip_Ending_Station_Arrival_Time = trip_ending_station_arrival_time,
                    Full_Route_Starting_Station_Title = full_route_starting_station_title,
                    Full_Route_Ending_Station_Title = full_route_ending_station_title,
                    Total_Trip_Duration = trip_ending_station_arrival_time - trip_starting_station_departure_time,
                    Carriage_Statistics_List = carriage_bookings_info,
                    Train_Stops_List = train_stops_dto,
                    Average_Speed_On_Trip = FullTrainAssignementService.
                    FindSpeedOnSection(trip_starting_station_km_point, trip_ending_station_km_point, trip_starting_station_departure_time, trip_ending_station_arrival_time)

                });

            }
            DateTime second = DateTime.Now;
            TimeSpan third = second - first;
            Console.WriteLine($"Time: {third.TotalSeconds}");
            return result_list;

        }
    }
}
