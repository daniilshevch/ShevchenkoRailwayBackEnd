using RailwayCore.InternalServices.CoreServices.Interfaces;
using RailwayCore.Models;
using RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.ClientDTO.UserTicketManagement;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.CodeBaseServices;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Implementations
{
    public class TicketProfileBuilder : ITicketProfileBuilder
    {
        private readonly IFullTrainRouteSearchService full_train_route_search_service;
        private readonly IQRCodeGeneratorService qr_code_generator_service;

        public TicketProfileBuilder(IFullTrainRouteSearchService full_train_route_search_service, IQRCodeGeneratorService qr_code_generator_service)
        {
            this.full_train_route_search_service = full_train_route_search_service;
            this.qr_code_generator_service = qr_code_generator_service;
        }
        public async Task<ExternalProfileTicketBookingDto> CreateProfileDtoForTicketBooking(TicketBooking ticket_booking)
        {
            string ticket_status = TextEnumConvertationService.GetTicketBookingStatusIntoString(ticket_booking.Ticket_Status);
            string carriage_type = TextEnumConvertationService.GetCarriageTypeIntoString(ticket_booking.Passenger_Carriage.Type_Of);
            string? carriage_quality_class = TextEnumConvertationService.GetCarriageQualityClassIntoString(ticket_booking.Passenger_Carriage.Quality_Class);
            string full_route_starting_station_title = (await full_train_route_search_service
                .GetStartingTrainStopForTrainRouteOnDate(ticket_booking.Train_Route_On_Date_Id))!.Station.Title;
            string full_route_ending_station_title = (await full_train_route_search_service
                .GetEndingTrainStopForTrainRouteOnDate(ticket_booking.Train_Route_On_Date_Id))!.Station.Title;
            DateTime? departure_time_from_trip_starting_station = (await full_train_route_search_service
                .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(ticket_booking.Train_Route_On_Date_Id, ticket_booking.Starting_Station_Id))!.Departure_Time;
            DateTime? arrival_time_to_trip_ending_station = (await full_train_route_search_service
                .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(ticket_booking.Train_Route_On_Date_Id, ticket_booking.Ending_Station_Id))!.Arrival_Time;
            TimeSpan? trip_duration = arrival_time_to_trip_ending_station - departure_time_from_trip_starting_station;
            string qr_code_base_64 = qr_code_generator_service.GenerateQrCodeBase64(ticket_booking.Full_Ticket_Id.ToString()); //!!!!Вирішити

            TrainRouteOnDateOnStation? starting_trip_stop = await full_train_route_search_service
                .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(ticket_booking.Train_Route_On_Date_Id, ticket_booking.Starting_Station_Id);
            TrainRouteOnDateOnStation? ending_trip_stop = await full_train_route_search_service
                .GetTrainStopInfoByTrainRouteOnDateIdAndStationId(ticket_booking.Train_Route_On_Date_Id, ticket_booking.Ending_Station_Id);
            double? km_point_of_starting_trip_stop = starting_trip_stop?.Distance_From_Starting_Station;
            double? km_point_of_ending_trip_stop = ending_trip_stop?.Distance_From_Starting_Station;

            double? speed_on_trip = null;
            if (km_point_of_starting_trip_stop is not null && km_point_of_ending_trip_stop is not null && trip_duration is not null)
            {
                speed_on_trip = (km_point_of_ending_trip_stop - km_point_of_starting_trip_stop) / trip_duration.Value.TotalHours;
            }
            ExternalProfileTicketBookingDto output_ticket = new ExternalProfileTicketBookingDto()
            {
                Full_Ticket_Id = ticket_booking.Full_Ticket_Id,
                Ticket_Status = ticket_status,
                Train_Route_On_Date_Id = ticket_booking.Train_Route_On_Date_Id,
                Train_Route_Id = ticket_booking.Train_Route_On_Date.Train_Route_Id,
                Passenger_Carriage_Position_In_Squad = ticket_booking.Passenger_Carriage_Position_In_Squad,
                Place_In_Carriage = ticket_booking.Place_In_Carriage,
                Carriage_Type = carriage_type,
                Carriage_Quality_Class = carriage_quality_class,
                Full_Route_Starting_Station_Title = full_route_starting_station_title,
                Full_Route_Ending_Station_Title = full_route_ending_station_title,
                Trip_Starting_Station_Title = ticket_booking.Starting_Station.Title,
                Trip_Ending_Station_Title = ticket_booking.Ending_Station.Title,
                Departure_Time_From_Trip_Starting_Station = departure_time_from_trip_starting_station,
                Arrival_Time_To_Trip_Ending_Station = arrival_time_to_trip_ending_station,
                Trip_Duration = trip_duration,
                Passenger_Name = ticket_booking.Passenger_Name,
                Passenger_Surname = ticket_booking.Passenger_Surname,
                Speed_On_Trip = speed_on_trip,
                Qr_Code = qr_code_base_64
            };
            return output_ticket;
        }
    }
}
