using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class PassengerCarriageOnTrainRouteOnDateDto
    {
        [JsonPropertyName("passenger_carriage_id")]
        public string Passenger_Carriage_Id { get; set; } = null!;
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("position_in_squad")]
        public int Position_In_Squad { get; set; }
        [JsonPropertyName("is_for_woman")]
        public bool Is_For_Woman { get; set; } = false;
        [JsonPropertyName("is_for_children")]
        public bool Is_For_Children { get; set; } = false;
        [JsonPropertyName("factual_air_conditioning")]
        public bool Factual_Air_Conditioning { get; set; } = false;
        [JsonPropertyName("factual_shower_availability")]
        public bool Factual_Shower_Availability { get; set; } = false;
        [JsonPropertyName("factual_is_inclusive")]
        public bool Factual_Is_Inclusive { get; set; } = false;
        [JsonPropertyName("food_availability")]
        public bool Food_Availability { get; set; } = false;
    }
    public class PassengerCarriageOnTrainRouteOnDateUpdateDto
    {
        [JsonPropertyName("passenger_carriage_id")]
        public string? Passenger_Carriage_Id { get; set; } = null!;
        [JsonPropertyName("train_route_on_date_id")]
        public string? Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("position_in_squad")]
        public int? Position_In_Squad { get; set; }
        [JsonPropertyName("is_for_woman")]
        public bool? Is_For_Woman { get; set; } = false;
        [JsonPropertyName("is_for_children")]
        public bool? Is_For_Children { get; set; } = false;
        [JsonPropertyName("factual_air_conditioning")]
        public bool? Factual_Air_Conditioning { get; set; } = false;
        [JsonPropertyName("factual_shower_availability")]
        public bool? Factual_Shower_Availability { get; set; } = false;
        [JsonPropertyName("factual_is_inclusive")]
        public bool? Factual_Is_Inclusive { get; set; } = false;
        [JsonPropertyName("food_availability")]
        public bool? Food_Availability { get; set; } = false;
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