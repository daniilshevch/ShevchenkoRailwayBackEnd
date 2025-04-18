using RailwayCore.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayCore.DTO
{
    public class PassengerCarriageDto
    {
        public string Id { get; set; } = null!;
        public PassengerCarriageType Type_Of { get; set; }
        public int Capacity { get; set; }
        public int? Production_Year { get; set; }
        public PassengerCarriageManufacturer? Manufacturer { get; set; }
        public PassengerCarriageQualityClass? Quality_Class { get; set; }
        public bool Renewal_Fact { get; set; }
        public int? Renewal_Year { get; set; }
        public PassengerCarriageManufacturer? Renewal_Performer { get; set; }
        public string? Renewal_Info { get; set; }
        public bool Air_Conditioning { get; set; } = false;
        public bool Is_Inclusive { get; set; } = false;
        public bool Is_For_Women { get; set; } = false;
        public bool Is_For_Children { get; set; } = false;
        public bool Is_For_Train_Chief { get; set; } = false;
        public bool Shower_Availability { get; set; } = false;
        public bool In_Current_Use { get; set; } = true;
        public string? Appearence { get; set; }
        public int? Station_Depot_Id { get; set; }
    }


}
