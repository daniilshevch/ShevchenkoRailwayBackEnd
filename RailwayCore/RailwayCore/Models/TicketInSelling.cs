using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models
{
    public class TicketInSelling
    {
        public PassengerCarriage Passenger_Carriage { get; set; } = null!;
        public string Passenger_Carriage_Id { get; set; } = null!;
        public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!;
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public Station Starting_Station { get; set; } = null!;
        public int Starting_Station_Id { get; set; }
        public Station Ending_Station { get; set; } = null!;
        public int Ending_Station_Id { get; set; }
        public double Extra_Coefficient { get; set; } = 1;
    }
}
