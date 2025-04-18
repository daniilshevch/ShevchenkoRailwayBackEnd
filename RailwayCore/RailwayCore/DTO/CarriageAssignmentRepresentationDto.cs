using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RailwayCore.DTO
{
    public class SinglePlace
    {
        [JsonPropertyName("place_in_carriage")]
        public int Place_In_Carriage { get; set; }
        [JsonPropertyName("is_free")]
        public bool Is_Free { get; set; }
    }
    public class CarriageAssignmentRepresentationDto
    {
        public PassengerCarriageOnTrainRouteOnDate Carriage_Assignment { get; set; } = null!;
        public List<SinglePlace> Places_Availability { get; set; } = new List<SinglePlace>();
        public int Price { get; set; }
        public int Free_Places { get; set; }
        public int Total_Places { get; set; }
    }
    public class TrainRouteOnDateCarriageAssignmentsRepresentationDto
    {
        public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!;
        public List<CarriageAssignmentRepresentationDto> Carriage_Statistics_List { get; set; } = new List<CarriageAssignmentRepresentationDto>();
        public int Platskart_Free { get; set; }
        public int Platskart_Total { get; set; }
        public int Coupe_Free { get; set; }
        public int Coupe_Total { get; set; }
        public int SV_Free { get; set; }
        public int SV_Total { get; set; }
        public int Min_Platskart_Price { get; set; }
        public int Min_Coupe_Price { get; set; }
        public int Min_SV_Price { get; set; }
    }
}
