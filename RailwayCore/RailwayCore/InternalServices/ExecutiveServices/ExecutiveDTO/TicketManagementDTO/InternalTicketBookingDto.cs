using RailwayCore.Models.ModelEnums.TicketBookingEnums;

namespace RailwayCore.InternalServices.ExecutiveServices.ExecutiveDTO.TicketManagementDTO
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
}
