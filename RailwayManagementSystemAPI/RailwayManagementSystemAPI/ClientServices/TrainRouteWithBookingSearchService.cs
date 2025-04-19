using RailwayManagementSystemAPI.API_DTO;
using RailwayCore.DTO;
using RailwayCore.Services;
using RailwayCore.Models;
using RailwayManagementSystemAPI.SystemServices;
namespace RailwayManagementSystemAPI.ClientServices
{
    public class TrainRouteWithBookingsSearchService
    {
        private readonly FullTrainRouteSearchService full_train_route_search_service;
        private readonly FullTicketBookingService ticket_booking_service;
        public TrainRouteWithBookingsSearchService(FullTrainRouteSearchService full_train_route_search_service, FullTicketBookingService ticket_booking_service)
        {
            this.full_train_route_search_service = full_train_route_search_service;
            this.ticket_booking_service = ticket_booking_service;
        }

        public async Task<List<TrainRouteWithBookingsInfoDto>?> SearchTrainRoutesBetweenStationsWithBookingsInfo
            (string starting_station_title, string ending_station_title, DateOnly departure_date)
        {
            DateTime first = DateTime.Now;
            Console.WriteLine(first);
            List<TrainRouteWithBookingsInfoDto> result_list = new List<TrainRouteWithBookingsInfoDto>();
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
          await ticket_booking_service.GetAllPassengerCarriagesPlaceBookingsForSeveralTrainRoutesOnDateWithPassengerInformationAnalytics
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



                List<SinglePassengerCarriageBookingsInfoDto> carriage_bookings_info = single_train_route.Value.Carriage_Statistics_List
                    .Select(single_carriage_info => new SinglePassengerCarriageBookingsInfoDto
                    {
                        Carriage_Position_In_Squad = single_carriage_info.Carriage_Assignment.Position_In_Squad,
                        Carriage_Type = GetCarriageTypeIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Type_Of),
                        Quality_Class = GetCarriageQualityClassIntoString(single_carriage_info.Carriage_Assignment.Passenger_Carriage.Quality_Class),
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
                List<SingleTrainStopDto> train_stops_dto = new List<SingleTrainStopDto>();
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
                    train_stops_dto.Add(new SingleTrainStopDto()
                    {
                        Station_Title = train_stop.Station.Title,
                        Arrival_Time = arrival_time,
                        Departure_Time = departure_time,
                        Stop_Duration = stop_duration,
                        Is_Part_Of_Trip = is_part_of_trip
                    });
                }

                result_list.Add(new TrainRouteWithBookingsInfoDto
                {
                    Train_Route_Id = single_train_route.Value.Train_Route_On_Date.Train_Route.Id,
                    Train_Route_Branded_Name = single_train_route.Value.Train_Route_On_Date.Train_Route.Branded_Name,
                    Train_Route_Class = GetTrainQualityClassIntoString(single_train_route.Value.Train_Route_On_Date.Train_Route.Quality_Class),
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
        public List<SingleTrainStopDto> GetScheduleForSpecificTrainRouteOnDate(TrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_on_date.Train_Stops_List;
        }
        public List<SinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForAllPassengerCarriagesForSpecificTrainRouteOnDate(TrainRouteWithBookingsInfoDto train_route_on_date)
        {
            return train_route_on_date.Carriage_Statistics_List;
        }
        public List<SinglePassengerCarriageBookingsInfoDto> GetBookingsInfoForPassengerCarriagesOfSpecificTypeForSpecificTrainRouteOnDate
            (TrainRouteWithBookingsInfoDto train_route_on_date, PassengerCarriageType? carriage_type = null, PassengerCarriageQualityClass? quality_class = null)
        {
            List<SinglePassengerCarriageBookingsInfoDto> output_result = train_route_on_date.Carriage_Statistics_List;
            if (carriage_type != null)
            {
                output_result = output_result
                    .Where(carriage_info => carriage_info.Carriage_Type == GetCarriageTypeIntoString((PassengerCarriageType)carriage_type)).ToList();
            }
            if (quality_class != null)
            {
                output_result = output_result
                    .Where(carriage_info => carriage_info.Quality_Class == GetCarriageQualityClassIntoString((PassengerCarriageQualityClass)quality_class)).ToList();
            }
            return output_result;
        }
        public static string GetCarriageTypeIntoString(PassengerCarriageType carriage_type)
        {
            switch (carriage_type)
            {
                case PassengerCarriageType.Platskart:
                    return "Platskart";
                case PassengerCarriageType.Coupe:
                    return "Coupe";
                case PassengerCarriageType.SV:
                    return "SV";
                default:
                    return "";
            }
        }
        public static string? GetCarriageQualityClassIntoString(PassengerCarriageQualityClass? quality_class)
        {
            if (quality_class == null)
            {
                return null;
            }
            switch (quality_class)
            {
                case PassengerCarriageQualityClass.S:
                    return "S";
                case PassengerCarriageQualityClass.A:
                    return "A";
                case PassengerCarriageQualityClass.B:
                    return "B";
                case PassengerCarriageQualityClass.C:
                    return "C";
                default:
                    return "";
            }
        }
        public static string? GetTrainQualityClassIntoString(TrainQualityClass? quality_class)
        {
            if (quality_class == null)
            {
                return null;
            }
            switch (quality_class)
            {
                case TrainQualityClass.S:
                    return "S";
                case TrainQualityClass.A:
                    return "A";
                case TrainQualityClass.B:
                    return "B";
                case TrainQualityClass.C:
                    return "C";
                default:
                    return "";
            }
        }
    }
}
