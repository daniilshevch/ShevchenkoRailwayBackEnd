using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ExecutiveServices
{
    public static class RouteAlgorithmService
    {
        [Crucial]
        [Algorithm("АЛГОРИТМ ТРЬОХ СЕКЦІЙ")]
        [Refactored("v1", "18.04.2025")]
        [Checked("11.05.2025")]
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

        [Crucial]
        [Algorithm("МОДИФІКОВАНИЙ АЛГОРИТМ ТРЬОХ СЕКЦІЙ ДЛЯ ДЕКІЛЬКОХ ПОЇЗДІВ")]
        [Refactored("v1", "18.04.2025")]
        [Checked("11.05.2025")]
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
