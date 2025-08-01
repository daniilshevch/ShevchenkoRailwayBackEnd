using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class TrainRouteOnDateDto
    {
        [JsonPropertyName("train_route_id")]
        public string Train_Route_Id { get; set; } = null!;
        [JsonPropertyName("departure_date")]
        public DateOnly Departure_Date { get; set; }
        [JsonPropertyName("train_race_coefficient")]
        public double? Train_Race_Coefficient { get; set; } = 1;
    }
}
