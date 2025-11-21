using RailwayCore.Models;
namespace RailwayCore.InternalServices.ExecutiveServices.TicketManagementServices.Implementations
{
    public static class RouteAlgorithmService
    {
        /// <summary>
        /// Дана функція приймає в якості аргументу список зупинок одного рейсу та дві станції майбутньої поїздки(початкову та кінцеву), а далі
        /// ділить список всіх зупинок рейсу на 3 секції(ліву, центральну та праву).
        /// Ліва секція містить всі зупинки до початкової станції поїздки включно.
        /// Центральна секція містить всі станції майбутньої поїздки, НЕ включаючи початкову та кінцеву зупинку поїздки
        /// Права секція містить всі станції після кінцевої станції поїздки і включно з самою кінцевою станцією поїздки
        /// Дана функція потрібна для коректного визначення вільних місць між двома зупинками для певного рейсу поїзда.
        /// </summary>
        /// <param name="all_sorted_train_stops_of_train_route_on_date"></param>
        /// <param name="desired_starting_station_title"></param>
        /// <param name="desired_ending_station_title"></param>
        /// <returns></returns>
        [Crucial]
        [Algorithm("АЛГОРИТМ ТРЬОХ СЕКЦІЙ")]
        [Refactored("v1", "18.04.2025")]
        [Checked("11.05.2025")]
        [PartialLogicMethod]
        //На вхід в метод треба подавати відсортовані станції рейсу поїзда, інакше він буде працювати некоректно
        public static (List<int>, List<int>, List<int>) DivideSectionIntoThreeParts(List<TrainRouteOnDateOnStation> all_sorted_train_stops_of_train_route_on_date,
    string desired_starting_station_title, string desired_ending_station_title)
        {
            //Ініціалізовуємо 3 секції
            List<int> left_part = new List<int>(); //Ліва секція включає всі станції на маршруті поїзда до початкової станції подорожі включно з нею
            List<int> central_part = new List<int>(); //Центральна секція включає всі станції на маршруті поїзда між початковою та кінцевою станцією подорожі, обидві не включно
            List<int> right_part = new List<int>(); //Права секція включає всі станції на маршруті поїзда після кінцевої станції подорожі включно з нею
            int desired_starting_stop_index = all_sorted_train_stops_of_train_route_on_date
                .FindIndex(train_stop => train_stop.Station.Title == desired_starting_station_title); //Знаходимо номер за рахунком початкової станції подорожі 
            int desired_ending_stop_index = all_sorted_train_stops_of_train_route_on_date
                .FindIndex(train_stop => train_stop.Station.Title == desired_ending_station_title); //Знаходимо номер за рахунком кінцевої станції подорожі

            for (int stop_index = 0; stop_index <= desired_starting_stop_index; stop_index++) //Включаємо в ліву секцію всі станції до початкової станції подорожі включно з нею
            {
                left_part.Add(all_sorted_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            for (int stop_index = desired_starting_stop_index + 1; stop_index < desired_ending_stop_index; stop_index++) //Включаємо в центральну секцію всі станції між початковою та кінцевою станцією подорожі не включно
            {
                central_part.Add(all_sorted_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            for (int stop_index = desired_ending_stop_index; stop_index < all_sorted_train_stops_of_train_route_on_date.Count; stop_index++) //Включаємо в праву секції всі станції після кінцевої у подорожі включно
            {
                right_part.Add(all_sorted_train_stops_of_train_route_on_date[stop_index].Station_Id);
            }
            //Індекси в списку станцій можна брати до уваги, бо метод приймає посортований список станцій поїзда(це треба враховувати при виклику методу)
            return (left_part, central_part, right_part);
        }
        /// <summary>
        /// Даний метож є логічним продовженням попередньої функції, але працює одразу для декількох поїздів. Приймає на вхід список списків, де 
        /// кожен вкладений список є посортованим списком станцій одного рейсу поїзда. Ліва частина буде містити всі станції до початкової зупинки поїздки 
        /// включно з нею для кожного з поїздів списку(об'єднання лівих секцій для декількох поїздів), і аналогічно центральна частина - об'єднання центральних 
        /// секцій для кожного окремого поїзда, і аналогічно права частина - консолідація правих секцій для окремих поїздів.
        /// </summary>
        /// <param name="all_sorted_train_stops_of_several_train_routes_on_date"></param>
        /// <param name="desired_starting_station_title"></param>
        /// <param name="desired_ending_station_title"></param>
        /// <returns></returns>
        [Crucial]
        [Algorithm("МОДИФІКОВАНИЙ АЛГОРИТМ ТРЬОХ СЕКЦІЙ ДЛЯ ДЕКІЛЬКОХ ПОЇЗДІВ")]
        [Refactored("v1", "18.04.2025")]
        [Checked("11.05.2025")]
        [PartialLogicMethod]
        public static (List<int>, List<int>, List<int>) DivideSectionIntoThreePartsForSeveralTrains(List<List<TrainRouteOnDateOnStation>> all_sorted_train_stops_of_several_train_routes_on_date,
            string desired_starting_station_title, string desired_ending_station_title)
        {
            //Ініціалізація секцій аналогічно до базового алгоритму
            List<int> consolidated_left_part = new List<int>();
            List<int> consolidated_central_part = new List<int>();
            List<int> consolidated_right_part = new List<int>();
            //Перебираємо список станцій для кожного поїзду окремо
            foreach (List<TrainRouteOnDateOnStation> all_sorted_train_stops_for_current_train_route_on_date in all_sorted_train_stops_of_several_train_routes_on_date)
            {
                //Ділимо станції на 3 секцій для одного конкретного поїзда через базовий алгоритм
                (List<int> left_part_for_current_train, List<int> central_part_for_current_train, List<int> right_part_for_current_train) =
                    DivideSectionIntoThreeParts(all_sorted_train_stops_for_current_train_route_on_date, desired_starting_station_title, desired_ending_station_title);
                //Консолідуємо секцій для всіх поїздів(об'єднуємо ліву, центральну та праву секції між собою поокремо для кожного поїзда, створюючи консолідовані ліву, центральну та праву секції
                consolidated_left_part = consolidated_left_part.Union(left_part_for_current_train).ToList();
                consolidated_central_part = consolidated_central_part.Union(central_part_for_current_train).ToList();
                consolidated_right_part = consolidated_right_part.Union(right_part_for_current_train).ToList();
            }
            return (consolidated_left_part, consolidated_central_part, consolidated_right_part);
        }
    }
}
