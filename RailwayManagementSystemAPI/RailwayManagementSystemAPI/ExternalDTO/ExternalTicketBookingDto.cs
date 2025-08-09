using RailwayCore.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.ExternalDTO
{
    public class ExternalTicketBookingDto
    {
        public int Id { get; set; } 
        public Guid? Full_Ticket_Id { get; set; }
        public int User_Id { get; set; } 
        public string Train_Route_On_Date_Id { get; set; } = null!; 
        public string Passenger_Carriage_Id { get; set; } = null!; 
        public int? Passenger_Carriage_Position_In_Squad { get; set; }
        public string Starting_Station_Title { get; set; } = null!;
        public string Ending_Station_Title { get; set; } = null!;
        public int Place_In_Carriage { get; set; } 
        public string Passenger_Name { get; set; } = null!; 
        public string Passenger_Surname { get; set; } = null!; 
        public DateTime Booking_Time { get; set; } 
        public DateTime? Booking_Expiration_Time { get; set; } 
        public TicketStatus Ticket_Status { get; set; } = TicketStatus.Booked_And_Active; 
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
