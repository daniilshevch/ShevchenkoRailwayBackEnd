using RailwayCore.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RailwayCore.InternalDTO.ModelDTO;

public class PassengerCarriageDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("type_of")]
    public PassengerCarriageType Type_Of { get; set; }
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
    [JsonPropertyName("production_year")]
    public int? Production_Year { get; set; }
    [JsonPropertyName("manufacturer")]
    public PassengerCarriageManufacturer? Manufacturer { get; set; }
    [JsonPropertyName("quality_class")]
    public PassengerCarriageQualityClass? Quality_Class { get; set; }
    [JsonPropertyName("renewal_fact")]
    public bool Renewal_Fact { get; set; }
    [JsonPropertyName("renewal_year")]
    public int? Renewal_Year { get; set; }
    [JsonPropertyName("renewal_performer")]
    public PassengerCarriageManufacturer? Renewal_Performer { get; set; }
    [JsonPropertyName("renewal_info")]
    public string? Renewal_Info { get; set; }
    [JsonPropertyName("air_conditioning")]
    public bool Air_Conditioning { get; set; } = false;
    [JsonPropertyName("is_inclusive")]
    public bool Is_Inclusive { get; set; } = false;
    [JsonPropertyName("is_for_train_chief")]
    public bool Is_For_Train_Chief { get; set; } = false;
    [JsonPropertyName("shower_availability")]
    public bool Shower_Availability { get; set; } = false;
    [JsonPropertyName("in_current_use")]
    public bool In_Current_Use { get; set; } = true;
    [JsonPropertyName("appearance")]
    public string? Appearence { get; set; }
    [JsonPropertyName("station_depot_title")]
    public string? Station_Depot_Title { get; set; }
    public static explicit operator PassengerCarriageDto(PassengerCarriage input)
    {
        return new PassengerCarriageDto()
        {
            Id = input.Id,
            Type_Of = input.Type_Of,
            Capacity = input.Capacity,
            Manufacturer = input.Manufacturer,
            Production_Year = input.Production_Year,
            Quality_Class = input.Quality_Class,
            Renewal_Fact = input.Renewal_Fact,
            Renewal_Year = input.Renewal_Year,
            Renewal_Info = input.Renewal_Info,
            Renewal_Performer = input.Renewal_Performer,
            Air_Conditioning = input.Air_Conditioning,
            Is_Inclusive = input.Is_Inclusive,
            Is_For_Train_Chief = input.Is_For_Train_Chief,
            Shower_Availability = input.Shower_Availability,
            In_Current_Use = input.In_Current_Use,
            Appearence = input.Appearence
        };
    }
}
