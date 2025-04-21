using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class InternalTicketBookingDto
    {
        public int User_Id { get; set; }
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public string Passenger_Carriage_Id { get; set; } = null!;
        public string Starting_Station_Title { get; set; } = null!;
        public string Ending_Station_Title { get; set; } = null!;
        public int Place_In_Carriage { get; set; }
        public string Passenger_Name { get; set; } = null!;
        public string Passenger_Surname { get; set; } = null!;
        public TicketStatus Ticket_Status { get; set; }
    }
    public class InternalTicketBookingDtoWithCarriagePosition
    {
        public int User_Id { get; set; }
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public int Passenger_Carriage_Position_In_Squad { get; set; }
        public string Starting_Station_Title { get; set; } = null!;
        public string Ending_Station_Title { get; set; } = null!;
        public int Place_In_Carriage { get; set; }
        public string Passenger_Name { get; set; } = null!;
        public string Passenger_Surname { get; set; } = null!;
        public TicketStatus Ticket_Status { get; set; }
    }
}
