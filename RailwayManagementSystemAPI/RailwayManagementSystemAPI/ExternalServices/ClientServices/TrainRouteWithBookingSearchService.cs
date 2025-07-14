using RailwayManagementSystemAPI.API_DTO;
using RailwayCore.InternalServices.CoreServices;
using RailwayCore.Models;
using System.Diagnostics;
using RailwayCore.InternalServices.SystemServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using RailwayManagementSystemAPI.ExternalDTO;
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
            QueryResult<List<InternalTrainRaceBetweenStationsDto>> train_routes_list_result = await full_train_route_search_service.SearchTrainRoutesBetweenStationsOnDate(starting_station_title,
                ending_station_title, departure_date);
            if (train_routes_list_result.Fail)
            {
                return new FailQuery<List<ExternalTrainRouteWithBookingsInfoDto>>(train_routes_list_result.Error);
            }
            List<InternalTrainRaceBetweenStationsDto> appropriate_train_routes_on_date = train_routes_list_result.Value;

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
                InternalTrainRaceBetweenStationsDto? current_train_route_trip_info = appropriate_train_routes_on_date.FirstOrDefault(train_race =>
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
                    new List<ExternalSinglePassengerCarriageBookingsInfoDto>();

                Dictionary<string, ExternalCarriageTypeGroupDto> external_carriage_type_groups =
                    new Dictionary<string, ExternalCarriageTypeGroupDto>();

                //Перетворюємо внутрішні трансферні об'єкти вагонів у зовнішні

                foreach (InternalCarriageAssignmentRepresentationDto single_carriage_info in internal_carriage_statistics_for_current_train_route_on_date)
                {
                    string carriage_type = TextEnumConvertationService.
                        GetCarriageTypeIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Type_Of);
                    string quality_class = TextEnumConvertationService.
                        GetCarriageQualityClassIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Quality_Class) ?? "default";
                    ExternalSinglePassengerCarriageBookingsInfoDto external_single_carriage_info_dto = new ExternalSinglePassengerCarriageBookingsInfoDto()
                    {
                        Carriage_Position_In_Squad = single_carriage_info.Carriage_Assignment.Position_In_Squad,
                        Carriage_Type = carriage_type,
                        Quality_Class = quality_class,
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
                            FindSpeedOnSection(trip_starting_stop_km_point, trip_ending_stop_km_point, departure_time_from_trip_starting_station, arrival_time_to_trip_ending_station))
                    };
                    external_carriage_statistics_for_current_train_route_on_date.Add(external_single_carriage_info_dto);

                    if(!external_carriage_type_groups.TryGetValue(carriage_type, out ExternalCarriageTypeGroupDto? carriage_type_group))
                    {
                        carriage_type_group = new ExternalCarriageTypeGroupDto();
                        external_carriage_type_groups[carriage_type] = carriage_type_group;
                    }
                    if (!carriage_type_group.Carriage_Quality_Class_Dictionary.TryGetValue(quality_class, 
                        out ExternalCarriageTypeAndQualityGroupDto? carriage_type_and_quality_group))
                    {
                        carriage_type_and_quality_group = new ExternalCarriageTypeAndQualityGroupDto();
                        carriage_type_group.Carriage_Quality_Class_Dictionary[quality_class] = carriage_type_and_quality_group;
                    }
                    carriage_type_and_quality_group.Carriage_Statistics_List.Add(external_single_carriage_info_dto);
                }

                foreach(KeyValuePair<string, ExternalCarriageTypeGroupDto> carriage_type_group in external_carriage_type_groups)
                {
                    Dictionary<string, ExternalCarriageTypeAndQualityGroupDto> carriage_quality_class_dictionary = 
                        carriage_type_group.Value.Carriage_Quality_Class_Dictionary;
                    int type_group_free_places = 0;
                    int type_group_total_places = 0;
                    int type_group_min_price = 10000000;
                    int type_group_max_price = 0;
                    foreach (KeyValuePair<string, ExternalCarriageTypeAndQualityGroupDto> carriage_type_and_quality_group in carriage_quality_class_dictionary)
                    {
                        int quality_class_free_places = 0;
                        int quality_class_total_places = 0;
                        int quality_class_min_price = 10000000;
                        int quality_class_max_price = 0;
                        List<ExternalSinglePassengerCarriageBookingsInfoDto> carriages_list = carriage_type_and_quality_group.Value.Carriage_Statistics_List;
                        foreach(ExternalSinglePassengerCarriageBookingsInfoDto single_carriage in carriages_list)
                        {
                            quality_class_free_places += single_carriage.Free_Places;
                            quality_class_total_places += single_carriage.Total_Places;
                            if(single_carriage.Ticket_Price < quality_class_min_price)
                            {
                                quality_class_min_price = single_carriage.Ticket_Price;
                            }
                            if(single_carriage.Ticket_Price > quality_class_max_price)
                            {
                                quality_class_max_price = single_carriage.Ticket_Price;
                            }
                        }
                        carriage_type_and_quality_group.Value.Free_Places = quality_class_free_places;
                        carriage_type_and_quality_group.Value.Total_Places = quality_class_total_places;
                        carriage_type_and_quality_group.Value.Min_Price = quality_class_min_price;
                        carriage_type_and_quality_group.Value.Max_Price = quality_class_max_price;

                        type_group_free_places += quality_class_free_places;
                        type_group_total_places += quality_class_total_places;
                        if(quality_class_min_price < type_group_min_price)
                        {
                            type_group_min_price = quality_class_min_price;
                        }
                        if(quality_class_max_price > type_group_max_price)
                        {
                            type_group_max_price = quality_class_max_price;
                        }
                    }
                    carriage_type_group.Value.Free_Places = type_group_free_places;
                    carriage_type_group.Value.Total_Places = type_group_total_places;
                    carriage_type_group.Value.Min_Price = type_group_min_price;
                    carriage_type_group.Value.Max_Price = type_group_max_price;
                }

                


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
                    Carriage_Statistics_List = external_carriage_statistics_for_current_train_route_on_date,
                    Grouped_Carriage_Statistics_List = external_carriage_type_groups,
                    Train_Stops_List = external_train_stops,
                    Average_Speed_On_Trip = FullTrainAssignementService.FindSpeedOnSection(trip_starting_stop_km_point, trip_ending_stop_km_point, departure_time_from_trip_starting_station, arrival_time_to_trip_ending_station)

                });


            }
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Train search time: ");
            Console.ResetColor();
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            List<ExternalTrainRouteWithBookingsInfoDto> ordered_total_train_routes_with_bookings_and_stations_info =
                total_train_routes_with_bookings_and_stations_info.OrderBy(train_route_full_info => train_route_full_info.Trip_Starting_Station_Departure_Time).ToList();
            return new SuccessQuery<List<ExternalTrainRouteWithBookingsInfoDto>>(ordered_total_train_routes_with_bookings_and_stations_info);

        }
        








        public async Task<QueryResult<List<ExternalTrainRaceThroughStationDto>>> SearchTrainRoutesThroughStation(string station_title, DateTime time,
            TimeSpan? left_interval = null, TimeSpan? right_interval = null)
        {
            QueryResult<List<InternalTrainRaceThroughStationDto>> train_races_result = 
                await full_train_route_search_service.SearchTrainRoutesThroughStationOnDate(station_title, time, left_interval, right_interval);
            if(train_races_result.Fail)
            {
                return new FailQuery<List<ExternalTrainRaceThroughStationDto>>(train_races_result.Error);
            }
            List<InternalTrainRaceThroughStationDto> train_races = train_races_result.Value;
            List<ExternalTrainRaceThroughStationDto> external_train_races = train_races.Select(train_race => new ExternalTrainRaceThroughStationDto()
            {
                Train_Route_On_Date_Id = train_race.Train_Route_On_Date.Id,
                Train_Route_Id = train_race.Train_Route_On_Date.Train_Route_Id,
                Arrival_Time_To_Current_Stop = train_race.Arrival_Time_To_Current_Stop,
                Departure_Time_From_Current_Stop = train_race.Departure_Time_From_Current_Stop,
                Full_Route_Starting_Station_Title = train_race.Full_Route_Starting_Stop.Station.Title,
                Full_Route_Ending_Station_Title = train_race.Full_Route_Ending_Stop.Station.Title,
                Full_Route_Stops_List = train_race.Full_Route_Stops_List.Select(train_stop => new ExternalSingleTrainStopDto()
                {
                    Arrival_Time = train_stop.Arrival_Time,
                    Departure_Time = train_stop.Departure_Time,
                    Is_Part_Of_Trip = false,
                    Station_Title = train_stop.Station.Title,
                    Stop_Duration = train_stop.Departure_Time - train_stop.Arrival_Time
                }).ToList()
            }).ToList();
            return new SuccessQuery<List<ExternalTrainRaceThroughStationDto>>(external_train_races);
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
    }
}
    











  