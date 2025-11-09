using RailwayCore.Models;
using RailwayCore.Models.ModelEnums.TicketBookingEnums;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.TicketBookingDTO.AdminDTO
{
    public class ExternalTicketBookingDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("full_ticket_id")]
        public Guid? Full_Ticket_Id { get; set; }
        [JsonPropertyName("user_id")]
        public int User_Id { get; set; }
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("passenger_carriage_id")]
        public string Passenger_Carriage_Id { get; set; } = null!;
        [JsonPropertyName("passenger_carriage_position_in_squad")]
        public int? Passenger_Carriage_Position_In_Squad { get; set; }
        [JsonPropertyName("starting_station_title")]
        public string Starting_Station_Title { get; set; } = null!;
        [JsonPropertyName("ending_station_title")]
        public string Ending_Station_Title { get; set; } = null!;
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; }
        [JsonPropertyName("passenger_name")] 
        public string Passenger_Name { get; set; } = null!;
        [JsonPropertyName("passenger_surname")]
        public string Passenger_Surname { get; set; } = null!;
        [JsonPropertyName("booking_time")]
        public DateTime Booking_Time { get; set; }
        [JsonPropertyName("booking_expiration_time")]
        public DateTime? Booking_Expiration_Time { get; set; }
        [JsonPropertyName("ticket_status")]
        public TicketStatus Ticket_Status { get; set; } = TicketStatus.Booked_And_Active;
        [JsonPropertyName("additional_services")]        
        public string? Additional_Services { get; set; } 
        public static explicit operator ExternalTicketBookingDto(TicketBooking input)
        {
            return new ExternalTicketBookingDto()
            {
                Id = input.Id,
                Full_Ticket_Id = input.Full_Ticket_Id,
                User_Id = input.User_Id,
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Passenger_Carriage_Id = input.Passenger_Carriage_Id,
                Passenger_Carriage_Position_In_Squad = input.Passenger_Carriage_Position_In_Squad,
                Starting_Station_Title = input.Starting_Station.Title,
                Ending_Station_Title = input.Ending_Station.Title,
                Place_In_Carriage = input.Place_In_Carriage,
                Passenger_Name = input.Passenger_Name,
                Passenger_Surname = input.Passenger_Surname,
                Booking_Time = input.Booking_Time,
                Booking_Expiration_Time = input.Booking_Expiration_Time,
                Ticket_Status = input.Ticket_Status,
                Additional_Services = input.Additional_Services
            };
        }
    }
}
