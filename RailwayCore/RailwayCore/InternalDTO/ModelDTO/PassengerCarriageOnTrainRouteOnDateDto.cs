using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class PassengerCarriageOnTrainRouteOnDateDto
    {
        public string Passenger_Carriage_Id { get; set; } = null!;
        public string Train_Route_On_Date_Id { get; set; } = null!;
        public int Position_In_Squad { get; set; }
        public bool Is_For_Woman { get; set; } = false;
        public bool Is_For_Children { get; set; } = false;
        public bool Factual_Air_Conditioning { get; set; } = false;
        public bool Factual_Shower_Availability { get; set; } = false;
        public bool Factual_Is_Inclusive { get; set; } = false;
        public bool Food_Availability { get; set; } = false;
    }
    public class CarriageAssignementWithoutRouteDTO
    {
        public string Passenger_Carriage_Id { get; set; } = null!;
        public int Position_In_Squad { get; set; }
        public bool Is_For_Woman { get; set; } = false;
        public bool Is_For_Children { get; set; } = false;
        public bool Factual_Air_Conditioning { get; set; } = false;
        public bool Factual_Shower_Availability { get; set; } = false;
        public bool Factual_Is_Inclusive { get; set; } = false;
        public bool Food_Availability { get; set; } = false;
    }
}