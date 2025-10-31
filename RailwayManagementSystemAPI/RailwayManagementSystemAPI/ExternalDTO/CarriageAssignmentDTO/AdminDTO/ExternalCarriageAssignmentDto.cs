using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalDTO.CarriageAssignmentDTO.AdminDTO
{
    public class ExternalCarriageAssignmentDto
    {
        [JsonPropertyName("passenger_carriage_id")]
        public string Passenger_Carriage_Id { get; set; } = null!;
        [JsonPropertyName("passenger_carriage_info")]
        public PassengerCarriageDto? Passenger_Carriage_Info { get; set; } = null!;
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("position_in_squad")]
        public int Position_In_Squad { get; set; }
        [JsonPropertyName("is_for_woman")]
        public bool Is_For_Woman { get; set; } = false;
        [JsonPropertyName("is_for_children")]
        public bool Is_For_Children { get; set; } = false;
        [JsonPropertyName("factual_wi_fi")]
        public bool Factual_Wi_Fi { get; set; } = false;
        [JsonPropertyName("factual_air_conditioning")]
        public bool Factual_Air_Conditioning { get; set; } = false;
        [JsonPropertyName("factual_shower_availability")]
        public bool Factual_Shower_Availability { get; set; } = false;
        [JsonPropertyName("factual_is_inclusive")]
        public bool Factual_Is_Inclusive { get; set; } = false;
        [JsonPropertyName("food_availability")]
        public bool Food_Availability { get; set; } = false;
        public static explicit operator ExternalCarriageAssignmentDto(PassengerCarriageOnTrainRouteOnDate input)
        {
            return new ExternalCarriageAssignmentDto()
            {
                Passenger_Carriage_Id = input.Passenger_Carriage_Id,
                Passenger_Carriage_Info = input.Passenger_Carriage is not null ? (PassengerCarriageDto)input.Passenger_Carriage: null,
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Position_In_Squad = input.Position_In_Squad,
                Is_For_Woman = input.Is_For_Woman,
                Is_For_Children = input.Is_For_Children,
                Factual_Wi_Fi = input.Factual_Wi_Fi,
                Factual_Air_Conditioning = input.Factual_Air_Conditioning,
                Factual_Shower_Availability = input.Factual_Shower_Availability,
                Factual_Is_Inclusive = input.Factual_Is_Inclusive,
                Food_Availability = input.Food_Availability
            };
        }
    }
}
