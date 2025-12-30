using RailwayCore.InternalServices.CoreServices.Interfaces;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainRaceDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalDTO.TrainStopDTO.ClientDTO;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.CarriageAssistantServices.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.ClientServices.Interfaces;
using System.Text.Json.Serialization;


public class AdminTicketsGroupByStartingStationDto
{
    [JsonPropertyName("trip_starting_station_title")]
    public string Trip_Starting_Station_Title { get; set; } = null!;
    [JsonPropertyName("destination_groups")]
    public List<AdminDestinationTicketGroupDto> Destination_Groups { get; set; } = new List<AdminDestinationTicketGroupDto>();
}
public class AdminDestinationTicketGroupDto
{
    [JsonPropertyName("trip_ending_station_title")]
    public string Trip_Ending_Station_Title { get; set; } = null!;
    [JsonPropertyName("passenger_trips_info")]
    public List<AdminPassengerTripInfoDto> Passenger_Trips_Info { get; set; } = new List<AdminPassengerTripInfoDto>();
}
public class AdminTicketsGroupByEndingStationDto
{
    [JsonPropertyName("trip_ending_station_title")]
    public string Trip_Ending_Station_Title { get; set; } = null!;

    [JsonPropertyName("starting_station_groups")]
    public List<AdminStartingStationTicketGroupDto> Starting_Station_Groups { get; set; } = new List<AdminStartingStationTicketGroupDto>();
}

public class AdminStartingStationTicketGroupDto
{
    [JsonPropertyName("trip_starting_station_title")]
    public string Trip_Starting_Station_Title { get; set; } = null!;

    [JsonPropertyName("passenger_trips_info")]
    public List<AdminPassengerTripInfoDto> Passenger_Trips_Info { get; set; } = new List<AdminPassengerTripInfoDto>();
}
public class AdminPassengerTripInfoDto
{
    [JsonPropertyName("place_in_carriage")]
    public int Place_In_Carriage { get; set; }
    [JsonPropertyName("passenger_trip_info")]
    public InternalPassengerTripInfoDto Passenger_Trip_Info { get; set; } = null!;
}
public class AdminFullTicketStatisticsInfoForPassengerCarriageDto
{
    [JsonPropertyName("passenger_carriage_bookings_info")]
    public ExternalSinglePassengerCarriageBookingsInfoDto Passenger_Carriage_Bookings_Info { get; set; } = null!;
    [JsonPropertyName("ticket_groups_by_starting_station")]
    public List<AdminTicketsGroupByStartingStationDto> Ticket_Groups_By_Starting_Station { get; set; } = new List<AdminTicketsGroupByStartingStationDto>();
    [JsonPropertyName("ticket_groups_by_ending_station")]
    public List<AdminTicketsGroupByEndingStationDto> Ticket_Groups_By_Ending_Station { get; set; } = new List<AdminTicketsGroupByEndingStationDto>();
}


namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.CarriageAssistantServices.Implementations
{
    public class CarriageAssistantTicketAggregationService : ICarriageAssistantTicketAggregationService
    {
        private readonly string service_name = "CarriageAssistantTicketAggregationService";
        private readonly ITrainRouteWithBookingsSearchService train_route_with_bookings_search_service;
        private readonly IFullTrainRouteSearchService full_train_route_search_service;
        public CarriageAssistantTicketAggregationService(ITrainRouteWithBookingsSearchService train_route_with_bookings_search_service,
            IFullTrainRouteSearchService full_train_route_search_service)
        {
            this.train_route_with_bookings_search_service = train_route_with_bookings_search_service;
            this.full_train_route_search_service = full_train_route_search_service;
        }
        public async Task<QueryResult<AdminFullTicketStatisticsInfoForPassengerCarriageDto>> GetGroupedTicketBookingsByStationsForTrainRace(
            string train_route_on_date_id,
            int carriage_position_in_squad,
            string? trip_starting_station_title,
            string? trip_ending_station_title,
            bool group_by_ending_station_first = false)
        {
            TrainRouteOnDateOnStation? full_route_starting_stop =
                await full_train_route_search_service.GetStartingTrainStopForTrainRouteOnDate(train_route_on_date_id);
            TrainRouteOnDateOnStation? full_route_ending_stop =
                await full_train_route_search_service.GetEndingTrainStopForTrainRouteOnDate(train_route_on_date_id);
            if (full_route_starting_stop is null || full_route_ending_stop is null)
            {
                return new FailQuery<AdminFullTicketStatisticsInfoForPassengerCarriageDto>(new Error(ErrorType.InternalServerError, $"Can't find starting or ending station " +
                    $"for train race {train_route_on_date_id}", annotation: service_name, unit: ProgramUnit.AdminAPI));
            }

            QueryResult<ExternalTrainRaceWithBookingsInfoDto> full_train_race_info_get_result =
                await train_route_with_bookings_search_service.GetCompleteInfoWithBookingsForTrainRaceOnDateBetweenStations(train_route_on_date_id,
                trip_starting_station_title ?? full_route_starting_stop.Station.Title, trip_ending_station_title ?? full_route_ending_stop.Station.Title,
                admin_mode: true, places_availability_info: true);
            if (full_train_race_info_get_result.Fail)
            {
                return new FailQuery<AdminFullTicketStatisticsInfoForPassengerCarriageDto>(full_train_race_info_get_result.Error);
            }

            ExternalTrainRaceWithBookingsInfoDto full_train_race_info = full_train_race_info_get_result.Value;
            List<ExternalSingleTrainStopDto> train_stops_for_train_race = full_train_race_info.Train_Stops_List;
            List<ExternalSinglePassengerCarriageBookingsInfoDto> bookings_info_for_carriages_list = full_train_race_info.Carriage_Statistics_List;

            ExternalSinglePassengerCarriageBookingsInfoDto? bookings_info_for_current_carriage = bookings_info_for_carriages_list
                .FirstOrDefault(carriage_bookings_info => carriage_bookings_info.Carriage_Position_In_Squad == carriage_position_in_squad);
            if (bookings_info_for_current_carriage is null)
            {
                return new FailQuery<AdminFullTicketStatisticsInfoForPassengerCarriageDto>(new Error(ErrorType.InternalServerError, $"Can't find bookings info for carriage with number " +
                    $"{carriage_position_in_squad} " +
                    $"for train race {train_route_on_date_id}", annotation: service_name, unit: ProgramUnit.AdminAPI));
            }

            var all_booked_tickets_for_current_carriage_with_trip_info = bookings_info_for_current_carriage.Places_Availability
                .Where(ticket_booking => ticket_booking.Is_Free == false && ticket_booking.Passenger_Trip_Info is not null)
                .SelectMany(ticket_booking => ticket_booking.Passenger_Trip_Info.Select(passenger_trip_info => new
                {
                    Place_Number = ticket_booking.Place_In_Carriage,
                    Passenger_Trip_Info = passenger_trip_info
                }));
            var stops_lookup_dictionary = train_stops_for_train_race.ToDictionary(stop => stop.Station_Title, stop => stop);
            AdminFullTicketStatisticsInfoForPassengerCarriageDto full_ticket_statistics_info = new AdminFullTicketStatisticsInfoForPassengerCarriageDto()
            {
                Passenger_Carriage_Bookings_Info = bookings_info_for_current_carriage
            };

            if (group_by_ending_station_first == false)
            {
                List<AdminTicketsGroupByStartingStationDto> grouped_ticket_bookings_by_starting_stop = all_booked_tickets_for_current_carriage_with_trip_info
                    .GroupBy(ticket_info => ticket_info.Passenger_Trip_Info.Trip_Starting_Station)
                    .Select(start_group => new AdminTicketsGroupByStartingStationDto
                    {
                        Trip_Starting_Station_Title = start_group.Key ?? "-",
                        Destination_Groups = start_group
                        .GroupBy(ticket_info => ticket_info.Passenger_Trip_Info.Trip_Ending_Station)
                        .Select(end_group => new AdminDestinationTicketGroupDto
                        {
                            Trip_Ending_Station_Title = end_group.Key ?? "-",
                            Passenger_Trips_Info = end_group.Select(ticket_info => new AdminPassengerTripInfoDto
                            {
                                Place_In_Carriage = ticket_info.Place_Number,
                                Passenger_Trip_Info = ticket_info.Passenger_Trip_Info
                            }).OrderBy(ticket_info => ticket_info.Place_In_Carriage).ToList()
                        }).OrderBy(end_group => stops_lookup_dictionary.TryGetValue(end_group.Trip_Ending_Station_Title, out ExternalSingleTrainStopDto? train_stop)
                        ? train_stop.Arrival_Time ?? DateTime.MaxValue : DateTime.MaxValue)
                        .ToList()
                    }).OrderBy(start_group => stops_lookup_dictionary.TryGetValue(start_group.Trip_Starting_Station_Title, out ExternalSingleTrainStopDto? train_stop)
                    ? train_stop.Departure_Time ?? DateTime.MinValue : DateTime.MinValue)
                    .ToList();
                full_ticket_statistics_info.Ticket_Groups_By_Starting_Station = grouped_ticket_bookings_by_starting_stop;
            }
            else
            {
                List<AdminTicketsGroupByEndingStationDto> grouped_ticket_bookings_by_destination = all_booked_tickets_for_current_carriage_with_trip_info
                    .GroupBy(ticket_info => ticket_info.Passenger_Trip_Info.Trip_Ending_Station)
                    .Select(end_group => new AdminTicketsGroupByEndingStationDto
                    {
                        Trip_Ending_Station_Title = end_group.Key ?? "-",
                        Starting_Station_Groups = end_group
                        .GroupBy(ticket_info => ticket_info.Passenger_Trip_Info.Trip_Starting_Station)
                        .Select(start_group => new AdminStartingStationTicketGroupDto
                        {
                            Trip_Starting_Station_Title = start_group.Key ?? "-",
                            Passenger_Trips_Info = start_group.Select(ticket_info => new AdminPassengerTripInfoDto
                            {
                                Place_In_Carriage = ticket_info.Place_Number,
                                Passenger_Trip_Info = ticket_info.Passenger_Trip_Info
                            })
                            .OrderBy(start_group => start_group.Place_In_Carriage)
                            .ToList()
                        }).OrderBy(start_group => stops_lookup_dictionary.TryGetValue(start_group.Trip_Starting_Station_Title, out ExternalSingleTrainStopDto? train_stop)
                        ? train_stop.Departure_Time ?? DateTime.MinValue : DateTime.MinValue)
                        .ToList()
                    }).OrderBy(end_group => stops_lookup_dictionary.TryGetValue(end_group.Trip_Ending_Station_Title, out ExternalSingleTrainStopDto? train_stop)
                    ? train_stop.Arrival_Time ?? DateTime.MinValue : DateTime.MinValue)
                    .ToList();
                full_ticket_statistics_info.Ticket_Groups_By_Ending_Station = grouped_ticket_bookings_by_destination;
            }


            return new SuccessQuery<AdminFullTicketStatisticsInfoForPassengerCarriageDto>(full_ticket_statistics_info,
                new SuccessMessage($"Successfully get all ticket bookings statistics info for passenger carriage with " +
                $"number {carriage_position_in_squad} in train race {train_route_on_date_id}"));
        }
    }
}
