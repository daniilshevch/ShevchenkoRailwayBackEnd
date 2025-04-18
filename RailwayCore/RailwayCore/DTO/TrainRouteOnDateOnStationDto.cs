using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.DTO
{
    public class TrainRouteOnDateOnStationDto
    {
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public string Station_Title { get; set; } = null!;
        public DateTime? Arrival_Time { get; set; }
        public DateTime? Departure_Time { get; set; }
        public StopType Stop_Type { get; set; } = StopType.Boarding;
        public double? Distance_From_Starting_Station { get; set; }
        public double? Speed_On_Section { get; set; }
    }
    public class TrainStopWithoutRouteDto
    {
        public string Station_Title { get; set; } = null!;
        public DateTime? Arrival_Time { get; set; }
        public DateTime? Departure_Time { get; set; }
        public StopType Stop_Type { get; set; } = StopType.Boarding;
        public double? Distance_From_Starting_Station { get; set; }
    }
    public class TrainStopWithoutRouteWithSpeedDto
    {
        public string Station_Title { get; set; } = null!;
        public int Stop_Duration { get; set; }
        public StopType Stop_Type { get; set; } = StopType.Boarding;
        public double Speed_On_Section { get; set; }
        public double? Distance_From_Starting_Station { get; set; }
    }
}
