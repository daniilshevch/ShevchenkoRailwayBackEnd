using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Archieve
{
    internal class RewrittenMethods
    {
        //public async Task<List<TrainRouteOnDateOnStation>?> GetTrainStopsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids, bool order_mode = true)
        //{
        //    List<TrainRouteOnDateOnStation> train_stops_for_several_train_routes = await context.Train_Routes_On_Date_On_Stations
        //        .Include(train_stop => train_stop.Train_Route_On_Date)
        //        .Include(train_stop => train_stop.Station)
        //        .Where(train_stop => train_route_on_date_ids.Contains(train_stop.Train_Route_On_Date_Id))
        //        .OrderBy(train_stop => train_stop.Train_Route_On_Date_Id).ThenBy(train_stop => train_stop.Arrival_Time)
        //        .ToListAsync();
        //    return train_stops_for_several_train_routes;
        //}

        //public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignementsForTrainRouteOnDate(string train_route_on_date_id)
        //{
        //    //TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
        //    TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
        //        .Include(train_route_on_date => train_route_on_date.Carriage_Assignements)
        //        .ThenInclude(carriage_assignement => carriage_assignement.Passenger_Carriage)
        //        .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == train_route_on_date_id);
        //    if (train_route_on_date is null)
        //    {
        //        text_service.FailPostInform("Fail in TrainRouteOnDateService");
        //        return null;
        //    }
        //    List<PassengerCarriageOnTrainRouteOnDate> passenger_carriages = train_route_on_date.Carriage_Assignements.OrderBy(carriage_assignement => carriage_assignement.Position_In_Squad).ToList();
        //    return passenger_carriages;
        //}

        //public async Task<List<PassengerCarriageOnTrainRouteOnDate>?> GetPassengerCarriageAssignmentsForSeveralTrainRoutesOnDate(List<string> train_route_on_date_ids_list)
        //{
        //    //TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(train_route_on_date_id);
        //    List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments = await
        //        context.Passenger_Carriages_On_Train_Routes_On_Date
        //        .Include(carriage_assignment => carriage_assignment.Passenger_Carriage)
        //        .Include(carriage_assignment => carriage_assignment.Train_Route_On_Date)
        //        .Where(carriage_assignment => train_route_on_date_ids_list.Contains(carriage_assignment.Train_Route_On_Date_Id)).ToListAsync();
        //    return carriage_assignments;
        //}

        //[Crucial]
        //[Algorithm("АЛГОРИТМ ТРЬОХ СЕКЦІЙ")]
        //public static (List<int>, List<int>, List<int>) DivideSectionIntoThreeParts
        //    (List<TrainRouteOnDateOnStation> all_train_stops_of_train_route_on_date, string desired_starting_station_title, string desired_ending_station_title)
        //{

        //    int desired_starting_stop_index = all_train_stops_of_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == desired_starting_station_title);
        //    int desired_ending_stop_index = all_train_stops_of_train_route_on_date.FindIndex(train_stop => train_stop.Station.Title == desired_ending_station_title);
        //    int total_stops_amount = all_train_stops_of_train_route_on_date.Count;
        //    List<int> left_part = new List<int>();
        //    List<int> central_part = new List<int>();
        //    List<int> right_part = new List<int>();
        //    for (int stop_index = 0; stop_index <= desired_starting_stop_index; stop_index++)
        //    {
        //        left_part.Add(all_train_stops_of_train_route_on_date[stop_index].Station_Id);
        //    }
        //    for (int stop_index = desired_starting_stop_index + 1; stop_index < desired_ending_stop_index; stop_index++)
        //    {
        //        central_part.Add(all_train_stops_of_train_route_on_date[stop_index].Station_Id);
        //    }
        //    for (int stop_index = desired_ending_stop_index; stop_index < total_stops_amount; stop_index++)
        //    {
        //        right_part.Add(all_train_stops_of_train_route_on_date[stop_index].Station_Id);
        //    }
        //    return (left_part, central_part, right_part);
        //}



        //[Crucial]
        //[Algorithm("МОДИФІКОВАНИЙ АЛГОРИТМ ТРЬОХ СЕКЦІЙ(ДЛЯ ДЕКІЛЬКОХ ПОЇЗДІВ)")]
        //public static (List<int>, List<int>, List<int>) DivideSectionIntoThreePartsForSeveralTrains(List<List<TrainRouteOnDateOnStation>> all_train_stops_of_train_route_on_date_list,
        //    string desired_starting_station_title, string desired_ending_station_title)
        //{
        //    List<int> consolidated_left_part = new List<int>();
        //    List<int> consolidated_central_part = new List<int>();
        //    List<int> consolidated_right_part = new List<int>();
        //    foreach (List<TrainRouteOnDateOnStation> all_train_stops_of_train_route_on_date in all_train_stops_of_train_route_on_date_list)
        //    {
        //        (List<int> left_part_for_single_train, List<int> central_part_for_single_train, List<int> right_part_for_single_train) =
        //            DivideSectionIntoThreeParts(all_train_stops_of_train_route_on_date, desired_starting_station_title, desired_ending_station_title);
        //        consolidated_left_part = consolidated_left_part.Union(left_part_for_single_train).ToList();
        //        consolidated_central_part = consolidated_central_part.Union(central_part_for_single_train).ToList();
        //        consolidated_right_part = consolidated_right_part.Union(right_part_for_single_train).ToList();

        //    }
        //    return (consolidated_left_part, consolidated_central_part, consolidated_right_part);
        //}
    }
}
