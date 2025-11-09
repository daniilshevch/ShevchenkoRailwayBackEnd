using RailwayCore.InternalServices.ModelServices;
using RailwayCore.Models;
using RailwayCore.Context;
using Microsoft.EntityFrameworkCore;
using RailwayCore.InternalServices.ExecutiveServices.TrainRouteSearchServices;
using RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models.ModelEnums.PassengerCarriageEnums;
using RailwayCore.Models.ModelEnums.TicketBookingEnums;

/// <summary>
/// Допоміжній клас, який містить докладну інформацію про пасажира та характеристики його поїздки
/// </summary>
[Checked("19.04.2025")]
public class BookingInfo
{
    public string Passenger_Name { get; set; } = null!;
    public string Passenger_Surname { get; set; } = null!;
    public User User_Booker { get; set; } = null!;
    public string Starting_Station_Title { get; set; } = null!;
    public string Ending_Station_Title { get; set; } = null!;
}

/// <summary>
/// Допомоміжний клас, який містить інформацію про рейс, вагон та місце в цьому вагоні, 
/// а також може містити інформацію про пасажира та докладну інформацію про його поїздку, 
/// в тому числі початкову та кінцеву зупинки поїздки
/// </summary>
[Checked("19.04.2025")]
public class TrainRouteOnDateCarriageBookedPlace
{
    public string Train_Route_On_Date_Id { get; set; } = null!;
    public string Carriage_Id { get; set; } = null!;
    public int Place { get; set; }
    public BookingInfo Booking_Info { get; set; } = null!;
}
public class PlaceAccumulator
{
    public int Platskart_Free { get; set; }
    public int Platskart_Total { get; set; }
    public int Coupe_Free { get; set; }
    public int Coupe_Total { get; set; }
    public int SV_Free { get; set; }
    public int SV_Total { get; set; }
    public int Min_Platskart_Price { get; set; }
    public int Min_Coupe_Price { get; set; }
    public int Min_SV_Price { get; set; }
    public void CountPlaces(PassengerCarriageType type, int free_places_amount, int total_places_amount)
    {
        switch (type)
        {
            case PassengerCarriageType.Platskart:
                Platskart_Free += free_places_amount;
                Platskart_Total += total_places_amount;
                break;
            case PassengerCarriageType.Coupe:
                Coupe_Free += free_places_amount;
                Coupe_Total += total_places_amount;
                break;
            case PassengerCarriageType.SV:
                SV_Free += free_places_amount;
                SV_Total += total_places_amount;
                break;
        }
    }

}
namespace RailwayCore.InternalServices.ExecutiveServices
{
    /// <summary>
    /// Даний сервіс займається пошуком бронювань в вагонах(тобто займається пошуком квитків для рейсів(між станціями))
    /// </summary>
    [ExecutiveService]
    public class TicketAvailabilityCheckService
    {
        private readonly string service_name = "TicketAvailabilityCheckService";
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateRepository train_route_on_date_service;
        private readonly PassengerCarriageRepository passenger_carriage_service;
        private readonly StationRepository station_service;
        private readonly TrainScheduleSearchService train_schedule_search_service;
        private readonly TrainSquadSearchService train_squad_search_service;
        public TicketAvailabilityCheckService(AppDbContext context, 
            TrainRouteOnDateRepository train_route_on_date_service, 
            PassengerCarriageRepository passenger_carriage_service, 
            StationRepository station_service, 
            TrainScheduleSearchService train_schedule_search_service,
            TrainSquadSearchService train_squad_search_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.passenger_carriage_service = passenger_carriage_service;
            this.station_service = station_service;
            this.train_schedule_search_service = train_schedule_search_service;
            this.train_squad_search_service = train_squad_search_service;
            
        }

        /// <summary>
        /// Даний метод перевіряє, чи є вільним або заброньованим одне конкретне місце, в одному конкретному вагоні в одному конкретному рейсі поїзда між двома станціями
        /// </summary>
        /// <param name="train_route_on_date_id"></param>
        /// <param name="desired_starting_station_title"></param>
        /// <param name="desired_ending_station_title"></param>
        /// <param name="passenger_carriage_id"></param>
        /// <param name="place_in_carrriage"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<QueryResult<bool>> CheckTicketAvailabilityBetweenStationsForTrainRouteOnDate(string train_route_on_date_id, string desired_starting_station_title,
           string desired_ending_station_title, string passenger_carriage_id, int place_in_carrriage)
        {
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.GetTrainRouteOnDateById(train_route_on_date_id);
            if (train_route_on_date is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.NotFound, $"Can't find train route on date with ID: {train_route_on_date_id}", annotation: service_name, unit: ProgramUnit.Core));
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.GetPassengerCarriageById(passenger_carriage_id);
            if (passenger_carriage is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.NotFound, $"Can't find passenger carriage with ID: {passenger_carriage_id}", annotation: service_name, unit: ProgramUnit.Core));
            }
            Station? desired_starting_station = await station_service.GetStationByTitle(desired_starting_station_title);
            Station? desired_ending_station = await station_service.GetStationByTitle(desired_ending_station_title);
            if (desired_starting_station is null || desired_ending_station is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.NotFound, $"Can't find one of the stations", annotation: service_name, unit: ProgramUnit.Core));
            }

            //Знаходимо всі зупинки для поїзда, в якому ми потенційно хочемо купити квиток(в порядку слідування поїзда)
            List<TrainRouteOnDateOnStation>? all_train_stops_of_train_route_on_date = await train_schedule_search_service.GetTrainStopsForTrainRouteOnDate(train_route_on_date_id, order_mode: true);
            if (all_train_stops_of_train_route_on_date is null)
            {
                return new FailQuery<bool>(new Error(ErrorType.BadRequest, $"Fail while searching stations for train route on date", annotation: service_name, unit: ProgramUnit.Core));
            }
            //Ділимо зупинки на 3 секції згідно з алгоритмом
            (List<int> left_part, List<int> central_part, List<int> right_part) =
                RouteAlgorithmService.DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, desired_starting_station_title, desired_ending_station_title);

            //Перевірка, чи існує квиток(або квитки), який блокує придбання квитку на це місце пасажиром(фактично перевіряє, чи місце в даному вагоні в даному поїзді між цими станціями вільне)
            bool is_available = !await context.Ticket_Bookings.AnyAsync(ticket => ticket.Train_Route_On_Date_Id == train_route_on_date_id &&
            ticket.Passenger_Carriage_Id == passenger_carriage_id && ticket.Place_In_Carriage == place_in_carrriage &&
            (left_part.Contains(ticket.Starting_Station_Id) && right_part.Contains(ticket.Ending_Station_Id) ||
            central_part.Contains(ticket.Starting_Station_Id) || central_part.Contains(ticket.Ending_Station_Id))
                && ticket.Ticket_Status != TicketStatus.Returned);

            return new SuccessQuery<bool>(is_available, new SuccessMessage($"Successfully checked status for place # {place_in_carrriage} in carriage with " +
                $"id: {passenger_carriage_id} in race {train_route_on_date_id} between stations {desired_starting_station_title} and {desired_ending_station_title}. " +
                $"Status: {(is_available ? "FREE" : "RESERVED")}", annotation: service_name, unit: ProgramUnit.Core));
        }




        /// <summary>
        /// Даний метод знаходить всі заброньовані місця в усіх вагонах декількох рейсів різних поїздів в контексті поїздки між деякими двома станціями
        /// </summary>
        /// <param name="train_route_on_date_ids"></param>
        /// <param name="starting_station_title"></param>
        /// <param name="ending_station_title"></param>
        /// <returns></returns>
        [ExecutiveMethod] 
        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDate
           (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {

            /////////////////////////////////////////////////ПІДГОТОВЧА ЧАСТИНА//////////////////////////////////////////////////////////

            //Знаходимо список тих поїздів, айді яких надано в списку
            List<TrainRouteOnDate> train_routes_on_date_list = await context.Train_Routes_On_Date.Where(train_route_on_date =>
            train_route_on_date_ids.Contains(train_route_on_date.Id)).ToListAsync();

            //За один запит знаходимо всі зупинки для всіх поїздів зі списку(вони будуть посортовані спочатку за поїздом, а потім за порядком слідування на маршруті,
            //тобто спочатку всі зупинки першого поїзду в порядку слідування, потім всі зупинки другого поїзда в порядку слідування і так далі)
            List<TrainRouteOnDateOnStation> train_stops_from_all_train_routes_on_date = await train_schedule_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Ініціалізуємо список списків, де кожен вкладений список представляє собою посортований список станцій для одного конкретного поїзда в дату(потрібно для алгоритму трьох секцій)
            List<List<TrainRouteOnDateOnStation>> train_stops_for_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Для кожного поїзду зі списку бажаних поїздів знаходимо його посортовані станції
                List<TrainRouteOnDateOnStation> train_stops_for_current_train_route_on_date =
                    train_stops_from_all_train_routes_on_date.Where(train_stop => train_stop.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();
                train_stops_for_train_route_on_date_list.Add(train_stops_for_current_train_route_on_date);
            }

            //Збираємо станції всіх поїздів зі списку і ділимо їх на 3 зони(ліва зона, центральна зона, права зона) - подробиці в Алгоритмі Трьох Секцій
            (List<int> consolidated_left_part, List<int> consolidated_central_part, List<int> consolidated_right_part)
                = RouteAlgorithmService.DivideSectionIntoThreePartsForSeveralTrains(train_stops_for_train_route_on_date_list, starting_station_title, ending_station_title);


            /////////////////////////////////////////////////ОТРИМАННЯ КВИТКІВ В РЕЙСІ МІЖ СТАНЦІЯМИ//////////////////////////////////////////////////////////

            //Збираємо всі бронювання квитків в усіх вагонах в усіх поїздах зі списку між обраними станціями поїздки
            //Тут ми шукаємо в базі всі квитки, які грають роль при пошуку вільних місць між поїздкою саме між початковою та кінцевою станцією, які ми задавали
            //при пошуку. Для того, щоб квиток перекривав якесь місце в вагоні, мають виконуватись одна з двох умов:
            
            //1) Початкова або кінцева станція знаходяться в центральній зоні поїздки(з алгоритму 3 секцій, всі зупинки між початковою та
            //кінцевою станцією поїздки, обидві не включно)
            //2) Початкова станція поїздки знаходиться в лівій зоні(з алгоритму 3 секцій, всі зупинки до початкової станції поїздки, включно з нею) і 
            //кінцева станція поїздки знаходиться в правій зоні(з алгоритму 3 секцій, кінцева зупинка поїздки і всі зупинки після неї)

            //При чому ми проводимо цей пошук для всіх поїздів з того списку, який ми отримали під час пошуку доступних поїздів, між станціями, і для 
            //всіх вагонів цих поїздів. Нам важливо просто отримати всі квитки, які були викуплені в цих поїздах і які перекриваються з бажаною поїздкою.

            List<TicketBooking> booked_tickets = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.User)
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Where(ticket_booking =>
            (consolidated_left_part.Contains(ticket_booking.Starting_Station_Id) && consolidated_right_part.Contains(ticket_booking.Ending_Station_Id)
            || consolidated_central_part.Contains(ticket_booking.Starting_Station_Id) || consolidated_central_part.Contains(ticket_booking.Ending_Station_Id))
            && train_route_on_date_ids.Contains(ticket_booking.Train_Route_On_Date_Id) && ticket_booking.Ticket_Status != TicketStatus.Returned).ToListAsync();

            //Збираємо з квитків потрібну інформацію про заброньоване місце(поїзд, номер вагона, місце в вагоні)
            List<TrainRouteOnDateCarriageBookedPlace> booked_places_for_carriage_in_train_route_on_date =
                booked_tickets.Select(ticket => new TrainRouteOnDateCarriageBookedPlace
                {
                    Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id,
                    Carriage_Id = ticket.Passenger_Carriage_Id,
                    Place = ticket.Place_In_Carriage,
                }).ToList();


            /////////////////////////////////////////КОНСОЛІДАЦІЯ ЗІБРАНОЇ ІНФОРМАЦІЇ В СТАТИСТИЧНІ ОБ'ЄКТИ//////////////////////////////////////////////


            //Словник статистичних-трансферів для всіх поїздів зі списку, де кожен окремий трансфер містить статистику по одному поїзду, включаючи бронювання в усіх його вагонах
            //ключ - ідентифікатор рейсу поїзда в дату, значення - об'єкт трансферу для конкретного рейсу
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> train_route_on_date_carriage_booking_statistics_dictionary
                = new Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>();

            //За один запит отримуємо інформацію про всі призначення вагонів на всі рейси поїздів(вони будуть посортовані спочатку за поїздом, а потім за номерами,
            //тобто спочатку всі вагони першого поїзду, посортовані за номерами в складі, потім другого і так далі)
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments_for_all_train_routes_on_date =
                await train_squad_search_service.GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Перебираємо кожен поїзд зі списку і збираємо статистику бронювань для нього
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Ініціалізуємо об'єкт-трансфер, який буде мати інформацію про один конкретний поїзд зі статистикою бронювань по всіх його вагонах
                train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id] =
                    new InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto()
                    {
                        Train_Route_On_Date = train_routes_on_date_list.FirstOrDefault(train_route_on_date => train_route_on_date.Id == current_train_route_on_date_id)!,
                    };

                //Отримуємо всі вагони, що призначені в склад конкретного поїзда(фільтруємо попередньо отриманий список вагонів зі всіх поїздів)
                List<PassengerCarriageOnTrainRouteOnDate> carriage_assignment_for_current_train_route_on_date =
                    carriage_assignments_for_all_train_routes_on_date.Where(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();

                carriage_assignment_for_current_train_route_on_date = carriage_assignment_for_current_train_route_on_date
                    .OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();  //Загалом-то це сортування надлишкове

                //Перебираємо кожен вагон в складі поїзда для отримання статистики бронювань по ньому
                foreach (PassengerCarriageOnTrainRouteOnDate current_carriage_assignment in carriage_assignment_for_current_train_route_on_date)
                {
                    //Отримуємо всі заброньовані місця в даному конкретному вагоні в даному конкретному рейсі поїзда 
                    //Зберігаються через HashSet, тому не буде дублікатів і буде дуже швидкий пошук
                    HashSet<int> booked_places_in_current_carriage = booked_places_for_carriage_in_train_route_on_date
                        .Where(ticket => ticket.Train_Route_On_Date_Id == current_train_route_on_date_id
                        && ticket.Carriage_Id == current_carriage_assignment.Passenger_Carriage_Id).Select(ticket => ticket.Place).ToHashSet();

                    //Отримуємо докладну інформацію про вагон
                    PassengerCarriage passenger_carriage = current_carriage_assignment.Passenger_Carriage;

                    //Отримуємо загальну кількість місць в вагоні
                    int capacity = passenger_carriage.Capacity;

                    //Ініціалізуємо список, де кожен об'єкт представляє одне місце в вагоні
                    List<InternalSinglePlaceDto> places_availability = new List<InternalSinglePlaceDto>();

                    int booked_places_amount = 0;
                    //Перебираємо всі місця в даному вагоні і, якщо місце є в списку зайнятих, помічаємо його, як зайняте
                    for (int current_place_in_carriage = 1; current_place_in_carriage <= capacity; current_place_in_carriage++)
                    {
                        //Перевірка, чи є дане місце в списку зайнятих для даного вагону
                        if (booked_places_in_current_carriage.Contains(current_place_in_carriage))
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = false
                            });
                            booked_places_amount++;
                        }
                        else
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = true
                            });
                        }
                    }
                    int free_places_amount = capacity - booked_places_amount;

                    //Додаємо в загальну статистику поїзда статистику одного конкретного вагона
                    train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id].Carriage_Statistics_List.Add(new InternalSinglePassengerCarriageAssignmentRepresentationDto
                    {
                        Carriage_Assignment = current_carriage_assignment,
                        Free_Places = free_places_amount,
                        Places_Availability = places_availability,
                        Total_Places = capacity,
                    });
                }
            }
            return new SuccessQuery<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>(train_route_on_date_carriage_booking_statistics_dictionary,
                new SuccessMessage($"Successfully got information about all booked places for races: {ConsoleLogService.PrintRaces(train_route_on_date_ids)} " +
                $"between {starting_station_title} and {ending_station_title}", annotation: service_name, unit: ProgramUnit.Core));
        }

        /// <summary>
        /// Даний метод знаходить всі заброньовані місця в усіх вагонах декількох рейсів різних поїздів в контексті поїздки між деякими двома станціями, а також
        /// отримує інформацію про те, хто бронював ці місця і між якими станціями ці пасажири здійснюватимуть поїздку
        /// </summary>
        /// <param name="train_route_on_date_ids"></param>
        /// <param name="starting_station_title"></param>
        /// <param name="ending_station_title"></param>
        /// <returns></returns>
        [ExecutiveMethod]
        public async Task<QueryResult<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>> GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics
  (List<string> train_route_on_date_ids, string starting_station_title, string ending_station_title)
        {
            //Знаходимо список тих поїздів, айді яких надано в списку
            List<TrainRouteOnDate> train_routes_on_date_list = await context.Train_Routes_On_Date.Where(train_route_on_date =>
            train_route_on_date_ids.Contains(train_route_on_date.Id)).ToListAsync();

            //За один запит знаходимо всі зупинки для всіх поїзів зі списку(вони будуть посортовані спочатку за поїздом, а потім за порядком слідування на маршруті,
            //тобто спочатку всі зупинки першого поїзду в порядку слідування, потім всі зупинки другого поїзда в порядку слідування і так далі)
            List<TrainRouteOnDateOnStation> train_stops_from_all_train_routes_on_date = await train_schedule_search_service.GetTrainStopsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Ініціалізуємо список списків, де кожен вкладений список представляє собою посортований список станцій для одного конкретного поїзда в дату(потрібно для алгоритму трьох секцій)
            List<List<TrainRouteOnDateOnStation>> train_stops_for_train_route_on_date_list = new List<List<TrainRouteOnDateOnStation>>();
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Для кожного поїзду зі списку бажаних поїздів знаходимо його посортовані станції
                List<TrainRouteOnDateOnStation> train_stops_for_current_train_route_on_date =
                    train_stops_from_all_train_routes_on_date.Where(train_stop => train_stop.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();
                train_stops_for_train_route_on_date_list.Add(train_stops_for_current_train_route_on_date);
            }
            //Збираємо станції всіх поїздів зі списку і ділимо їх на 3 зони(ліва зона, центральна зона, права зона) - подробиці в Алгоритмі Трьох Секцій
            (List<int> consolidated_left_part, List<int> consolidated_central_part, List<int> consolidated_right_part)
                = RouteAlgorithmService.DivideSectionIntoThreePartsForSeveralTrains(train_stops_for_train_route_on_date_list, starting_station_title, ending_station_title);

            //Збираємо всі бронювання квитків в усіх вагонах в усіх поїздах зі списку між обраними станціями поїздки
            List<TicketBooking> booked_tickets = await context.Ticket_Bookings
                .Include(ticket_booking => ticket_booking.User)
                .Include(ticket_booking => ticket_booking.Passenger_Carriage)
                .Include(ticket_booking => ticket_booking.Train_Route_On_Date)
                .Include(ticket_booking => ticket_booking.Starting_Station)
                .Include(ticket_booking => ticket_booking.Ending_Station)
                .Where(ticket_booking =>
            (consolidated_left_part.Contains(ticket_booking.Starting_Station_Id) && consolidated_right_part.Contains(ticket_booking.Ending_Station_Id)
            || consolidated_central_part.Contains(ticket_booking.Starting_Station_Id) || consolidated_central_part.Contains(ticket_booking.Ending_Station_Id))
            && train_route_on_date_ids.Contains(ticket_booking.Train_Route_On_Date_Id) && ticket_booking.Ticket_Status != TicketStatus.Returned).ToListAsync();

            //Збираємо з квитків потрібну інформацію про заброньоване місце(поїзд, номер вагона, місце в вагоні)
            List<TrainRouteOnDateCarriageBookedPlace> booked_places_for_carriage_in_train_route_on_date =
                booked_tickets.Select(ticket => new TrainRouteOnDateCarriageBookedPlace
                {
                    Train_Route_On_Date_Id = ticket.Train_Route_On_Date_Id,
                    Carriage_Id = ticket.Passenger_Carriage_Id,
                    Place = ticket.Place_In_Carriage,
                    Booking_Info = new BookingInfo //Інформація про пасажира + початкову та кінцеву станції його поїздки
                    {
                        Passenger_Name = ticket.Passenger_Name,
                        Passenger_Surname = ticket.Passenger_Surname,
                        Starting_Station_Title = ticket.Starting_Station.Title,
                        Ending_Station_Title = ticket.Ending_Station.Title,
                        User_Booker = ticket.User
                    }
                }).ToList();

            //Словник статистичних-трансферів для всіх поїздів зі списку, де кожен окремий трансфер містить статистику по одному поїзду, включаючи бронювання в усіх його вагонах
            //ключ - ідентифікатор рейсу поїзда в дату, значення - об'єкт трансферу для конкретного рейсу
            Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto> train_route_on_date_carriage_booking_statistics_dictionary
                = new Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>();

            //За один запит отримуємо інформацію про всі призначення вагонів на всі рейси поїздів(вони будуть посортовані спочатку за поїздом, а потім за номерами,
            //тобто спочатку всі вагони першого поїзду, посортовані за номерами в складі, потім другого і так далі)
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments_for_all_train_routes_on_date =
                await train_squad_search_service.GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(train_route_on_date_ids);

            //Перебираємо кожен поїзд зі списку і збираємо статистику бронювань для нього
            foreach (string current_train_route_on_date_id in train_route_on_date_ids)
            {
                //Ініціалізуємо об'єкт-трансфер, який буде мати інформацію про один конкретний поїзд зі статистикою бронювань по всіх його вагонах
                train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id] =
                    new InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto()
                    {
                        Train_Route_On_Date = train_routes_on_date_list.FirstOrDefault(train_route_on_date => train_route_on_date.Id == current_train_route_on_date_id)!,
                    };

                //Отримуємо всі вагони, що призначені в склад конкретного поїзда(фільтруємо попередньо отриманий список вагонів зі всіх поїздів)
                List<PassengerCarriageOnTrainRouteOnDate>? carriage_assignment_for_current_train_route_on_date =
                    carriage_assignments_for_all_train_routes_on_date.Where(carriage_assignment => carriage_assignment.Train_Route_On_Date_Id == current_train_route_on_date_id).ToList();

                carriage_assignment_for_current_train_route_on_date = carriage_assignment_for_current_train_route_on_date
                    .OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();  //Загалом-то це сортування надлишкове

                //Перебираємо кожен вагон в складі поїзда для отримання статистики бронювань по ньому
                foreach (PassengerCarriageOnTrainRouteOnDate current_carriage_assignment in carriage_assignment_for_current_train_route_on_date)
                {
                    //Отримуємо всі заброньовані місця в даному конкретному вагоні в даному конкретному рейсі поїзда 
                    //Зберігаються через HashSet, тому не буде дублікатів і буде дуже швидкий пошук
                    HashSet<int> booked_places_in_current_carriage = booked_places_for_carriage_in_train_route_on_date
                        .Where(ticket => ticket.Train_Route_On_Date_Id == current_train_route_on_date_id
                        && ticket.Carriage_Id == current_carriage_assignment.Passenger_Carriage_Id).Select(ticket => ticket.Place).ToHashSet();

                    //Отримуємо докладну інформацію про вагон
                    PassengerCarriage passenger_carriage = current_carriage_assignment.Passenger_Carriage;

                    //Отримуємо загальну кількість місць в вагоні
                    int capacity = passenger_carriage.Capacity;

                    //Ініціалізуємо список, де кожен об'єкт представляє одне місце в вагоні
                    List<InternalSinglePlaceDto> places_availability = new List<InternalSinglePlaceDto>();

                    int booked_places_amount = 0;
                    //Перебираємо всі місця в даному вагоні і, якщо місце є в списку зайнятих, помічаємо його, як зайняте
                    for (int current_place_in_carriage = 1; current_place_in_carriage <= capacity; current_place_in_carriage++)
                    {
                        List<BookingInfo> bookers_for_current_place = booked_places_for_carriage_in_train_route_on_date.
                            Where(ticket => ticket.Train_Route_On_Date_Id == current_train_route_on_date_id && ticket.Carriage_Id == current_carriage_assignment.Passenger_Carriage_Id
                            && ticket.Place == current_place_in_carriage).Select(ticket => ticket.Booking_Info).ToList();
                        List<InternalPassengerTripInfoDto> trip_info_for_current_place = bookers_for_current_place.Select(booker => new InternalPassengerTripInfoDto
                        {
                            User_Id = booker.User_Booker.Id,
                            Trip_Starting_Station = booker.Starting_Station_Title,
                            Trip_Ending_Station = booker.Ending_Station_Title,
                            Passenger_Name = booker.Passenger_Name,
                            Passenger_Surname = booker.Passenger_Surname
                        }).ToList();

                        //Перевірка, чи є дане місце в списку зайнятих для даного вагону
                        if (booked_places_in_current_carriage.Contains(current_place_in_carriage))
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = false,
                                Passenger_Trip_Info = trip_info_for_current_place
                            });
                            booked_places_amount++;
                        }
                        else
                        {
                            places_availability.Add(new InternalSinglePlaceDto
                            {
                                Place_In_Carriage = current_place_in_carriage,
                                Is_Free = true
                            });
                        }
                    }
                    int free_places_amount = capacity - booked_places_amount;

                    //Додаємо в загальну статистику поїзда статистику одного конкретного вагона
                    train_route_on_date_carriage_booking_statistics_dictionary[current_train_route_on_date_id].Carriage_Statistics_List.Add(new InternalSinglePassengerCarriageAssignmentRepresentationDto
                    {
                        Carriage_Assignment = current_carriage_assignment,
                        Free_Places = free_places_amount,
                        Places_Availability = places_availability,
                        Total_Places = capacity,
                    });
                }
            }
            return new SuccessQuery<Dictionary<string, InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto>>(train_route_on_date_carriage_booking_statistics_dictionary,
                 new SuccessMessage($"Successfully got information about all booked places for races: {ConsoleLogService.PrintRaces(train_route_on_date_ids)} " +
                $"between {starting_station_title} and {ending_station_title}", annotation: service_name, unit: ProgramUnit.Core));
        }
    }
}
