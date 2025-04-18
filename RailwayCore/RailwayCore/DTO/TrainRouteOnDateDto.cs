using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.DTO
{
    public class TrainRouteOnDateDto
    {
        public string Train_Route_Id { get; set; } = null!;
        public DateOnly Departure_Date { get; set; }
    }
}
