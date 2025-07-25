using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class TrainRouteOnDateDto
    {
        public string Train_Route_Id { get; set; } = null!;
        public DateOnly Departure_Date { get; set; }
        public double? Train_Race_Coefficient { get; set; } = 1;
    }
}
