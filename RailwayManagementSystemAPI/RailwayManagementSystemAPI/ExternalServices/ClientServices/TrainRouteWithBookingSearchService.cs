using RailwayCore.InternalServices.CoreServices;
using RailwayCore.Models;
using System.Diagnostics;
using RailwayManagementSystemAPI.ExternalServices.SystemServices;
using RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models.ModelEnums.PassengerCarriageEnums;

namespace RailwayManagementSystemAPI.ExternalServices.ClientServices
{
    /// <summary>
    /// Клас потрібен для коректного сортування груп вагонів різних типів(чисто косметичний ефект)
    /// </summary>
    public class CarriageTypeComparer: IComparer<string>
    {
        private static readonly List<string> CorrectTypesOrder = new List<string> { "Platskart", "Coupe", "SV" };
        public int Compare(string? x, string? y)
        {
            int indexX = CorrectTypesOrder.IndexOf(x ?? "");
            int indexY = CorrectTypesOrder.IndexOf(y ?? "");
            return indexX.CompareTo(indexY);
        }
    }
    /// <summary>
    /// Клас потрібен для коректного сортування груп вагонів різних класів в межах одного типу(чисто косметичний ефект)
    /// </summary>
    public class QualityClassComparer : IComparer<string>
    {
        private static readonly List<string> CorrectQualityClassesOrder = new List<string> { "C", "B", "A" };
        public int Compare(string? x, string? y)
        {
            int indexX = CorrectQualityClassesOrder.IndexOf(x ?? "");
            int indexY = CorrectQualityClassesOrder.IndexOf(y ?? "");
            return indexX.CompareTo(indexY);
        }
    }
    /// <summary>
    /// Цей клас проводить пошук доступних рейсів поїздів між двома станціями в певну дату. З одного боку він шукає самі рейси,
    /// які доступні для поїздки користувача, а з іншого - одразу і видає всю інформацію про склад цих поїздів та наявні місця в 
    /// них. Виконує роль агрегації функції декількох сервісів з RailwayCore і кінцевий функціонал пошуку поїздів для користувачів.
    /// </summary>
    [ClientApiService]
    public class TrainRouteWithBookingsSearchService
    {
        private readonly string service_name = "TrainRouteWithBookingsSearchService";
        private readonly FullTrainRouteSearchService full_train_route_search_service; 
        private readonly FullTicketManagementService full_ticket_management_service;
        public TrainRouteWithBookingsSearchService(FullTrainRouteSearchService full_train_route_search_service, FullTicketManagementService full_ticket_management_service)
        {
            this.full_train_route_search_service = full_train_route_search_service;
            this.full_ticket_management_service = full_ticket_management_service;
        }

        [PartialLogicMethod]
        public async Task<QueryResult<List<InternalTrainRaceBetweenStationsDto>>> _GetAppropriateTrainRoutesBetweenStationsOnDate(string starting_station_title, string ending_station_title, DateOnly departure_date)
        {
            //Отримуємо список поїздів, які проходять через дані станції в потрібному порядку в потрібну дату
            QueryResult<List<InternalTrainRaceBetweenStationsDto>> train_routes_list_result = await full_train_route_search_service.SearchTrainRoutesBetweenStationsOnDate(starting_station_title,
                ending_station_title, departure_date);
            if (train_routes_list_result.Fail)
            {
                return new FailQuery<List<InternalTrainRaceBetweenStationsDto>>(train_routes_list_result.Error);
            }
            List<InternalTrainRaceBetweenStationsDto> appropriate_train_routes_on_date = train_routes_list_result.Value;

            return new SuccessQuery<List<InternalTrainRaceBetweenStationsDto>>(appropriate_train_routes_on_date, new SuccessMessage($"Successfuly got train races passing between" +
                $" {starting_station_title} and {ending_station_title} on {departure_date}", annotation: service_name, unit: ProgramUnit.ClientAPI));
        }

        [PartialLogicMethod]
        public QueryResult<List<ExternalTrainRaceWithBookingsInfoDto>> _CreateListOfExternalTrainRaceWithBookingsInfoDto
            (List<InternalTrainRaceBetweenStationsDto> appropriate_train_routes_on_date,
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> ticket_bookings_info_for_appropriate_train_routes,
            string starting_station_title, string ending_station_title)
        {
            //Ініціалізуємо список, де кожен елемент буде містити всю потрібну інформацію по одному конкретному поїзду
            List<ExternalTrainRaceWithBookingsInfoDto> total_train_routes_with_bookings_and_stations_info = new List<ExternalTrainRaceWithBookingsInfoDto>();

            //Проходимось по кожному поїзду зі списку поїздів, які проходять через дані станції і для яких ми знайшли статистику бронювань
            foreach (KeyValuePair<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> single_train_route_on_date_statistics
                in ticket_bookings_info_for_appropriate_train_routes)
            {
                //Отримуємо азагальну інформацію про даний маршрут поїзда(поки без бронювання), воно буде в першу чергу показуватись на сторінці списку знайдених поїздів
                //Тут ми ітеруємось по списку об'єктів InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto, які містять інформацію про склади та броні в рейсах.
                //А для кожного такого об'єкту знаходимо відповідний йому об'єкт InternalTrainRaceBetweenStationsDto, який містить інформацію по розкладу руху того самого рейсу.
                //І, комбінуючи інформацію з обох внутрішніх трансферних об'єктів, ми створюємо зовнішній трансферний об'єкт ExternalTrainRaceWithBookingsInfoDto.

               /////////////////////////////ЗІСТАВЛЕННЯ ОБ'ЄКТУ РОЗКЛАДУ РУХУ ПОЇЗДА ТА ОБ'ЄКТУ СКЛАДУ ТА БРОНІ МІСЦЬ В ПОЇЗДІ//////////////////////////// 
                
                InternalTrainRaceBetweenStationsDto? current_train_route_trip_info = appropriate_train_routes_on_date.FirstOrDefault(train_race =>
                train_race.Train_Route_On_Date.Id == single_train_route_on_date_statistics.Key);
                if (current_train_route_trip_info is null)
                {
                    return new FailQuery<List<ExternalTrainRaceWithBookingsInfoDto>>(new Error(ErrorType.InternalServerError, $"Fail while searching info about train route on date " +
                        $"{single_train_route_on_date_statistics.Key}", annotation: service_name, unit: ProgramUnit.ClientAPI));
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
                List<InternalSinglePassengerCarriageAssignmentRepresentationDto> internal_carriage_statistics_for_current_train_route_on_date =
                    single_train_route_on_date_statistics.Value.Carriage_Statistics_List;
                //Ініціалізовуємо аналогічний список об'єктів-статистик бронювань в одном вагоні поїзда, але призначений для зовнішньої демонстрації
                //Вся докладна інформація про вагон, включаючи номер в складі, тип, послуги та бронювання міститься тут (також вираховується ціна подорожі)
                List<ExternalSinglePassengerCarriageBookingsInfoDto> external_carriage_statistics_for_current_train_route_on_date =
                    new List<ExternalSinglePassengerCarriageBookingsInfoDto>();

                Dictionary<string, ExternalCarriageTypeGroupDto> external_carriage_type_groups =
                    new Dictionary<string, ExternalCarriageTypeGroupDto>();

                /////////////////////////////ЗАГАЛЬНЕ ПЕРЕТВОРЕННЯ ВНУТРІШНІХ ТРАНСФЕРІВ У ЗОВНІШНІ//////////////////////////// 


                //Перетворюємо внутрішні трансферні об'єкти вагонів у зовнішні

                foreach (InternalSinglePassengerCarriageAssignmentRepresentationDto single_carriage_info in internal_carriage_statistics_for_current_train_route_on_date)
                {
                    string carriage_type = TextEnumConvertationService.
                        GetCarriageTypeIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Type_Of);
                    string quality_class = TextEnumConvertationService.
                        GetCarriageQualityClassIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Quality_Class) ?? "default";

                    //Формуємо загальні характеристики вагона в складі рейсу з внутрішнього об'єкту
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
                    //Додаємо цей зовнішній трансферний об'єкт в список вагонів рейсу
                    external_carriage_statistics_for_current_train_route_on_date.Add(external_single_carriage_info_dto);

                    //Формуємо групи вагонів за типом та класом
                    if (!external_carriage_type_groups.TryGetValue(carriage_type, out ExternalCarriageTypeGroupDto? carriage_type_group))
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

                /////////////////////////////ЗБІР СТАТИСИТИКИ ПО ВАГОНАХ В РЕЙСІ//////////////////////////// 


                //Проводимо докладний збір аналітики по типах та класах вагонів
                foreach (KeyValuePair<string, ExternalCarriageTypeGroupDto> carriage_type_group in external_carriage_type_groups)
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
                        foreach (ExternalSinglePassengerCarriageBookingsInfoDto single_carriage in carriages_list)
                        {
                            quality_class_free_places += single_carriage.Free_Places;
                            quality_class_total_places += single_carriage.Total_Places;
                            if (single_carriage.Ticket_Price < quality_class_min_price)
                            {
                                quality_class_min_price = single_carriage.Ticket_Price;
                            }
                            if (single_carriage.Ticket_Price > quality_class_max_price)
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
                        if (quality_class_min_price < type_group_min_price)
                        {
                            type_group_min_price = quality_class_min_price;
                        }
                        if (quality_class_max_price > type_group_max_price)
                        {
                            type_group_max_price = quality_class_max_price;
                        }
                    }
                    carriage_type_group.Value.Free_Places = type_group_free_places;
                    carriage_type_group.Value.Total_Places = type_group_total_places;
                    carriage_type_group.Value.Min_Price = type_group_min_price;
                    carriage_type_group.Value.Max_Price = type_group_max_price;

                    carriage_type_group.Value.Carriage_Quality_Class_Dictionary = carriage_quality_class_dictionary
                        .OrderBy(quality_class_group => quality_class_group.Key, new QualityClassComparer()).ToDictionary();
                }

                external_carriage_type_groups = external_carriage_type_groups.OrderBy(type_group => type_group.Key, new CarriageTypeComparer()).ToDictionary();


                /////////////////////////////ФОРМУВАННЯ РОЗКЛАДУ ДЛЯ ДАНОГО РЕЙСУ//////////////////////////// 


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
                    bool is_final_stop = false;
                    if (current_stop_index >= trip_start_stop_index && current_stop_index <= trip_end_stop_index)
                    {
                        is_part_of_trip = true;
                    }
                    if (current_stop_index == trip_end_stop_index)
                    {
                        is_final_stop = true;
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
                        Is_Final_Trip_Stop = is_final_stop,
                        Distance_From_Full_Route_Starting_Station = current_train_stop.Distance_From_Starting_Station
                    });
                }

                /////////////////////////////КОНСОЛІДУВАННЯ ІНФОРМАЦІЇ В ОДИН ЗОВНІШНІЙ ТРАНСФЕРНИЙ ОБ'ЄКТ//////////////////////////// 


                //Додаємо в список статистик по кожному поїзду статистику для даного поїзда
                total_train_routes_with_bookings_and_stations_info.Add(new ExternalTrainRaceWithBookingsInfoDto()
                {
                    Full_Train_Route_On_Date_Id = current_train_route_trip_info.Train_Route_On_Date.Id,
                    Train_Route_Id = current_train_route_trip_info.Train_Route_On_Date.Train_Route_Id,
                    Train_Route_Branded_Name = current_train_route_trip_info.Train_Route_On_Date.Train_Route.Branded_Name,
                    Train_Route_Class = TextEnumConvertationService.GetTrainQualityClassIntoString(current_train_route_trip_info.Train_Route_On_Date.Train_Route.Quality_Class),
                    Trip_Starting_Station_Title = current_train_route_trip_info.Desired_Starting_Station.Station.Title,
                    Trip_Starting_Station_Ukrainian_Title = current_train_route_trip_info.Desired_Starting_Station.Station.Ukrainian_Title,
                    Trip_Ending_Station_Title = current_train_route_trip_info.Desired_Ending_Station.Station.Title,
                    Trip_Ending_Station_Ukrainian_Title = current_train_route_trip_info.Desired_Ending_Station.Station.Ukrainian_Title,
                    Trip_Starting_Station_Departure_Time = current_train_route_trip_info.Departure_Time_From_Desired_Starting_Station,
                    Trip_Ending_Station_Arrival_Time = current_train_route_trip_info.Arrival_Time_For_Desired_Ending_Station,
                    Total_Trip_Duration = current_train_route_trip_info.Arrival_Time_For_Desired_Ending_Station - current_train_route_trip_info.Departure_Time_From_Desired_Starting_Station,
                    Full_Route_Starting_Station_Title = current_train_route_trip_info.Full_Route_Starting_Stop.Station.Title,
                    Full_Route_Starting_Station_Ukrainian_Title = current_train_route_trip_info.Full_Route_Starting_Stop.Station.Ukrainian_Title,
                    Full_Route_Ending_Station_Title = current_train_route_trip_info.Full_Route_Ending_Stop.Station.Title,
                    Full_Route_Ending_Station_Ukrainian_Title = current_train_route_trip_info.Full_Route_Ending_Stop.Station.Ukrainian_Title,
                    Carriage_Statistics_List = external_carriage_statistics_for_current_train_route_on_date,
                    Grouped_Carriage_Statistics_List = external_carriage_type_groups,
                    Train_Stops_List = external_train_stops,
                    Average_Speed_On_Trip = FullTrainAssignementService.FindSpeedOnSection(trip_starting_stop_km_point, trip_ending_stop_km_point, departure_time_from_trip_starting_station, arrival_time_to_trip_ending_station)

                });
            }
            return new SuccessQuery<List<ExternalTrainRaceWithBookingsInfoDto>>(total_train_routes_with_bookings_and_stations_info,
                new SuccessMessage($"Succesfully created ExternalTrainRaceWithBookingsInfoDto objects for train races:" +
                $" {ConsoleLogService.PrintRaces(appropriate_train_routes_on_date.Select(train_race => train_race.Train_Route_On_Date.Id).ToList())}",
                annotation: service_name, unit: ProgramUnit.ClientAPI));
        }
        [PartialLogicMethod]
        public (ExternalTrainRaceWithBookingsInfoDto? fastest_train_race, ExternalTrainRaceWithBookingsInfoDto? cheapest_train_race) _DefineAvailableTrainRacesTopChart
            (List<ExternalTrainRaceWithBookingsInfoDto> total_train_routes_with_bookings_and_stations_info)
        {
            ExternalTrainRaceWithBookingsInfoDto? fastest_train_race = total_train_routes_with_bookings_and_stations_info.MinBy(train_route_on_date =>
               train_route_on_date.Total_Trip_Duration);
            if (fastest_train_race is not null)
            {
                fastest_train_race.Is_Fastest = true;
            }
            ExternalTrainRaceWithBookingsInfoDto? cheapest_train_race = total_train_routes_with_bookings_and_stations_info.MinBy(train_route_on_date =>
               train_route_on_date.Carriage_Statistics_List.Count > 0 ? train_route_on_date.Carriage_Statistics_List.Min(carriage_assignment => carriage_assignment.Ticket_Price) : 0);
            if (cheapest_train_race is not null)
            {
                cheapest_train_race.Is_Cheapest = true;
            }
            return (fastest_train_race, cheapest_train_race);
        }

        /// <summary>
        /// Цей метод проводить пошук доступних рейсів поїздів між станціями в певну дату, а також вертає склад поїзда і інформацію
        /// про доступні місця в вагонах
        /// </summary>
        /// <param name="starting_station_title"></param>
        /// <param name="ending_station_title"></param>
        /// <param name="departure_date"></param>
        /// <param name="admin_mode"></param>
        /// <returns></returns>
        [ClientApiMethod]
        public async Task<QueryResult<List<ExternalTrainRaceWithBookingsInfoDto>>> SearchTrainRoutesBetweenStationsWithBookingsInfo
            (string starting_station_title, string ending_station_title, DateOnly departure_date, bool admin_mode = false)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("----------------------TRAIN TRIPS SEARCH PROCESS-------------------------");
            Console.ResetColor();
            Stopwatch sw = Stopwatch.StartNew();

            //////////////////////////////////ЧАСТИНА ПОШУКУ РЕЙСІВ ПОЇЗДІВ//////////////////////////////////////////////

            //Отримуємо список поїздів, які проходять через дані станції в потрібному порядку в потрібну дату
            QueryResult<List<InternalTrainRaceBetweenStationsDto>> appropriate_train_route_on_date_ids_get_result =
                await _GetAppropriateTrainRoutesBetweenStationsOnDate(starting_station_title, ending_station_title, departure_date);
            if(appropriate_train_route_on_date_ids_get_result.Fail)
            {
                return new FailQuery<List<ExternalTrainRaceWithBookingsInfoDto>>(appropriate_train_route_on_date_ids_get_result.Error);
            }
            List<InternalTrainRaceBetweenStationsDto> appropriate_train_routes_on_date = appropriate_train_route_on_date_ids_get_result.Value;
            
            //Беремо список айді знайдених поїздів(потрібно для функції ядра сервера, яка перевіряє бронювання для поїздів)
            List<string> appropriate_train_routes_on_date_ids =
                appropriate_train_routes_on_date.Select(train_route_on_date_info => train_route_on_date_info.Train_Route_On_Date.Id).ToList();


            //////////////////////////////////ЧАСТИНА ПОШУКУ ВСІХ ВІЛЬНИХ МІСЦЬ ДЛЯ ПОКУПКИ НА РЕЙСАХ//////////////////////////////////////////////

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
                return new FailQuery<List<ExternalTrainRaceWithBookingsInfoDto>>(train_routes_on_date_bookings_statistics_result.Error);
            }
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> ticket_bookings_info_for_appropriate_train_routes =
                train_routes_on_date_bookings_statistics_result.Value;

            //Ми вже отримали всю потрібну інформацію, яка реальна має бути передана пасажиру:

            // A) InternalTrainRaceBetweenStationsDto - інформацію про рейс поїзда в контексті його поїздки між двома станціями(які пасажир вказав
            //як бажані початкову та кінцеву(загальна інформація про даний рейс, його прибуття на початкову та кінцеву станцію подорожі, кілометраж і так далі,
            //а також розкладу руху цього поїзда на всьому маршруті(не тільки маршруті поїздки пасажира)

            // B) InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto - інформацію про склад рейсу, а також про всі вільні та заброньовані місця 
            //в кожному вагоні складу цього рейсу в контексті поїздки між початковою та кінцевою станцією, які обрав пасажир

            //Далі ми маємо скомбінувати ці дві порції внутрішньої інформації про рейс в один зовнішній трансферний клас:

            // AB) ExternalTrainRaceWithBookingsInfoDto - цей клас є зовнішнім трансферним об'єктом і є гібридом двох внутрішніх трансферних об'єктів (A + B),
            // тому що містить і інформацію про рейс поїзда в плані його розкладу руху на всьому маршруті та зокрема між станціями поїздки пасажира, 
            // а також містить інформацію про склад поїзда і всю інформацію про бронювання місць в вагонах на рейсі.


            QueryResult<List<ExternalTrainRaceWithBookingsInfoDto>> total_train_routes_with_bookings_and_station_info_get_result =
                _CreateListOfExternalTrainRaceWithBookingsInfoDto(appropriate_train_routes_on_date, ticket_bookings_info_for_appropriate_train_routes, 
                starting_station_title, ending_station_title);
            if(total_train_routes_with_bookings_and_station_info_get_result.Fail)
            {
                return new FailQuery<List<ExternalTrainRaceWithBookingsInfoDto>>(total_train_routes_with_bookings_and_station_info_get_result.Error);
            }
            //Формуємо список, де кожен елемент буде містити всю потрібну інформацію по одному конкретному поїзду
            List<ExternalTrainRaceWithBookingsInfoDto> total_train_routes_with_bookings_and_stations_info 
                = total_train_routes_with_bookings_and_station_info_get_result.Value;


            //Визначені рейсів-лідерів(найшвидші, найдешевші і т.д)
            (ExternalTrainRaceWithBookingsInfoDto? fastest_train_race, ExternalTrainRaceWithBookingsInfoDto? cheapest_train_race) = 
                _DefineAvailableTrainRacesTopChart(total_train_routes_with_bookings_and_stations_info);

            List<ExternalTrainRaceWithBookingsInfoDto> ordered_total_train_routes_with_bookings_and_stations_info =
                total_train_routes_with_bookings_and_stations_info.OrderBy(train_route_full_info => train_route_full_info.Trip_Starting_Station_Departure_Time).ToList();
            
            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"Train search time: ");
            Console.ResetColor();
            Console.WriteLine($"{sw.ElapsedMilliseconds / 1000.0} seconds");
            return new SuccessQuery<List<ExternalTrainRaceWithBookingsInfoDto>>(ordered_total_train_routes_with_bookings_and_stations_info, 
                new SuccessMessage($"Successfuly founded available train trips between {starting_station_title} and {ending_station_title} on date " +
                $"{departure_date}, schedule of these train trips(races), squads of these train races and available places in carriages", annotation: service_name,
                unit: ProgramUnit.ClientAPI));

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
        public List<ExternalSingleTrainStopDto> GetScheduleForSpecificTrainRouteOnDate(ExternalTrainRaceWithBookingsInfoDto train_route_on_date)
        {
            return train_route_on_date.Train_Stops_List;
        }
        public List<ExternalSingleTrainStopDto> GetScheduleForSpecificTrainRouteOnDateFromGeneralList
            (List<ExternalTrainRaceWithBookingsInfoDto> train_routes_on_date_statistics_list, string train_route_id)
        {
            ExternalTrainRaceWithBookingsInfoDto? desired_train_route_on_date = train_routes_on_date_statistics_list
                .FirstOrDefault(train_route_on_date_info => train_route_on_date_info.Train_Route_Id == train_route_id);
            return GetScheduleForSpecificTrainRouteOnDate(desired_train_route_on_date);
        }


        public List<ExternalSinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForAllPassengerCarriagesForSpecificTrainRouteOnDate(ExternalTrainRaceWithBookingsInfoDto train_route_on_date)
        {
            return train_route_on_date.Carriage_Statistics_List;
        }
        public List<ExternalSinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate
            (ExternalTrainRaceWithBookingsInfoDto train_route_on_date, PassengerCarriageType? carriage_type = null, PassengerCarriageQualityClass? quality_class = null)
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
    











  