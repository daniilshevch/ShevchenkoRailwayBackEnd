using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.AdminDTO;
using System.Text.Json.Serialization;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Interfaces;
namespace RailwayManagementSystemAPI.ExternalServices.AdminServices.ModelRepositoryServices.Implementations
{
    public class ExternalTicketGroupByStartingStationDto
    {
        [JsonPropertyName("starting_station_title")]
        public string Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("ticket_groups_list")]
        public List<ExternalTicketGroupDto> Ticket_Group_List { get; set; } = new List<ExternalTicketGroupDto>();
    }
    public class ExternalTicketGroupByEndingStationDto
    {
        [JsonPropertyName("ending_station_title")]
        public string Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("ticket_groups_list")]
        public List<ExternalTicketGroupDto> Ticket_Group_List { get; set; } = new List<ExternalTicketGroupDto>();
    }
    public class ExternalTicketGroupDto
    {
        [JsonPropertyName("starting_station_title")]
        public string Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("ending_station_title")]
        public string Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("ticket_bookings_list")]
        public List<ExternalTicketBookingDto> Ticket_Bookings_List { get; set; } = new List<ExternalTicketBookingDto>();
    }
    public class TicketBookingRepositoryService : ITicketBookingRepositoryService
    {
        public readonly ITicketBookingRepository ticket_booking_repository;
        public TicketBookingRepositoryService(ITicketBookingRepository ticket_booking_repository)
        {
            this.ticket_booking_repository = ticket_booking_repository;
        }
        public async Task<QueryResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForTrainRouteOnDate(string train_route_on_date_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result =
                await ticket_booking_repository.GetTicketBookingsForTrainRouteOnDate(train_route_on_date_id);
            if (ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketBookingDto>>(ticket_bookings_get_result.Error);
            }
            List<ExternalTicketBookingDto> ticket_bookings = ticket_bookings_get_result.Value.Select(ticket_booking =>
                (ExternalTicketBookingDto)ticket_booking).ToList();
            return new SuccessQuery<List<ExternalTicketBookingDto>>(ticket_bookings);
        }
        public async Task<QueryResult<List<ExternalTicketBookingDto>>> GetTicketBookingsForCarriageAssignment(string train_route_on_date_id, string passenger_carriage_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result =
                await ticket_booking_repository.GetTicketBookingsForCarriageAssignment(train_route_on_date_id, passenger_carriage_id);
            if (ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketBookingDto>>(ticket_bookings_get_result.Error);
            }
            List<ExternalTicketBookingDto> ticket_bookings = ticket_bookings_get_result.Value.Select(ticket_booking =>
                (ExternalTicketBookingDto)ticket_booking).ToList();
            return new SuccessQuery<List<ExternalTicketBookingDto>>(ticket_bookings);
        }
        private List<ExternalTicketGroupByStartingStationDto> GroupTicketBookingsByStartingStation(List<TicketBooking> ticket_bookings)
        {
            return ticket_bookings.GroupBy(ticket_booking => ticket_booking.Starting_Station.Title)
                .Select(ticket_group_by_starting_station =>
                {
                    string starting_station_title = ticket_group_by_starting_station.Key;
                    IEnumerable<TicketBooking> ticket_bookings_in_current_group = ticket_group_by_starting_station;
                    List<ExternalTicketGroupDto> ticket_groups = ticket_bookings_in_current_group.GroupBy(ticket_booking => ticket_booking.Ending_Station.Title)
                        .Select(ticket_group_by_ending_station => new ExternalTicketGroupDto
                        {
                            Starting_Station_Title = starting_station_title,
                            Ending_Station_Title = ticket_group_by_ending_station.Key,
                            Ticket_Bookings_List = ticket_group_by_ending_station.Select(ticket_booking =>
                            (ExternalTicketBookingDto)ticket_booking).ToList()
                        }).ToList();
                    return new ExternalTicketGroupByStartingStationDto
                    {
                        Starting_Station_Title = starting_station_title,
                        Ticket_Group_List = ticket_groups
                    };
                }).ToList();
        }
        private List<ExternalTicketGroupByEndingStationDto> GroupTicketBookingsByEndingStation(List<TicketBooking> ticket_bookings)
        {
            return ticket_bookings.GroupBy(ticket_booking => ticket_booking.Ending_Station.Title)
                .Select(ticket_group_by_ending_station =>
                {
                    string ending_station_title = ticket_group_by_ending_station.Key;
                    IEnumerable<TicketBooking> ticket_bookings_in_current_group = ticket_group_by_ending_station;
                    List<ExternalTicketGroupDto> ticket_groups = ticket_bookings_in_current_group.GroupBy(ticket_booking => ticket_booking.Starting_Station.Title)
                        .Select(ticket_group_by_starting_station => new ExternalTicketGroupDto
                        {
                            Starting_Station_Title = ticket_group_by_starting_station.Key,
                            Ending_Station_Title = ending_station_title,
                            Ticket_Bookings_List = ticket_group_by_starting_station.Select(ticket_booking =>
                            (ExternalTicketBookingDto)ticket_booking).ToList()
                        }).ToList();
                    return new ExternalTicketGroupByEndingStationDto
                    {
                        Ending_Station_Title = ending_station_title,
                        Ticket_Group_List = ticket_groups
                    };
                }).ToList();
        }
        public async Task<QueryResult<List<ExternalTicketGroupByStartingStationDto>>> GetGroupedTicketBookingsForTrainRouteOnDateOrderedByStartingStation(string train_route_on_date_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result =
                await ticket_booking_repository.GetTicketBookingsForTrainRouteOnDate(train_route_on_date_id);
            if (ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketGroupByStartingStationDto>>(ticket_bookings_get_result.Error);
            }
            List<TicketBooking> ticket_bookings = ticket_bookings_get_result.Value;
            List<ExternalTicketGroupByStartingStationDto> grouped_ticket_bookings = GroupTicketBookingsByStartingStation(ticket_bookings);
            return new SuccessQuery<List<ExternalTicketGroupByStartingStationDto>>(grouped_ticket_bookings);
        }
        public async Task<QueryResult<List<ExternalTicketGroupByEndingStationDto>>> GetGroupedTicketBookingsForTrainRouteOnDateOrderedByEndingStation(string train_route_on_date_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result =
                await ticket_booking_repository.GetTicketBookingsForTrainRouteOnDate(train_route_on_date_id, inverted_sorting: true);
            if (ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketGroupByEndingStationDto>>(ticket_bookings_get_result.Error);
            }
            List<TicketBooking> ticket_bookings = ticket_bookings_get_result.Value;
            List<ExternalTicketGroupByEndingStationDto> grouped_ticket_bookings = GroupTicketBookingsByEndingStation(ticket_bookings);
            return new SuccessQuery<List<ExternalTicketGroupByEndingStationDto>>(grouped_ticket_bookings);
        }
        public async Task<QueryResult<List<ExternalTicketGroupByStartingStationDto>>>
            GetGroupedTicketBookingsForCarriageAssignmentOrderedByStartingStation(string train_route_on_date_id, string passenger_carriage_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result =
    await ticket_booking_repository.GetTicketBookingsForCarriageAssignment(train_route_on_date_id, passenger_carriage_id);
            if (ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketGroupByStartingStationDto>>(ticket_bookings_get_result.Error);
            }
            List<TicketBooking> ticket_bookings = ticket_bookings_get_result.Value;
            List<ExternalTicketGroupByStartingStationDto> grouped_ticket_bookings = GroupTicketBookingsByStartingStation(ticket_bookings);
            return new SuccessQuery<List<ExternalTicketGroupByStartingStationDto>>(grouped_ticket_bookings);
        }
        public async Task<QueryResult<List<ExternalTicketGroupByEndingStationDto>>>
    GetGroupedTicketBookingsForCarriageAssignmentOrderedByEndingStation(string train_route_on_date_id, string passenger_carriage_id)
        {
            QueryResult<List<TicketBooking>> ticket_bookings_get_result =
    await ticket_booking_repository.GetTicketBookingsForCarriageAssignment(train_route_on_date_id, passenger_carriage_id);
            if (ticket_bookings_get_result.Fail)
            {
                return new FailQuery<List<ExternalTicketGroupByEndingStationDto>>(ticket_bookings_get_result.Error);
            }
            List<TicketBooking> ticket_bookings = ticket_bookings_get_result.Value;
            List<ExternalTicketGroupByEndingStationDto> grouped_ticket_bookings = GroupTicketBookingsByEndingStation(ticket_bookings);
            return new SuccessQuery<List<ExternalTicketGroupByEndingStationDto>>(grouped_ticket_bookings);
        }
    }
}
