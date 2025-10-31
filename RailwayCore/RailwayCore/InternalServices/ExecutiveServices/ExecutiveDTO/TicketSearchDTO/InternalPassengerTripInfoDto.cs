﻿using System.Text.Json.Serialization;
/// <summary>
/// Даний клас містить докладну характеристику про пасажира та його поїздку на певному рейсі між певними станціями
/// </summary>
public class InternalPassengerTripInfoDto
{
    [JsonPropertyName("user_id")]
    public int? User_Id { get; set; }
    [JsonPropertyName("passenger_name")]
    public string? Passenger_Name { get; set; }
    [JsonPropertyName("passenger_surname")]
    public string? Passenger_Surname { get; set; }
    [JsonPropertyName("trip_starting_station")]
    public string? Trip_Starting_Station { get; set; }
    [JsonPropertyName("trip_ending_station")]
    public string? Trip_Ending_Station { get; set; }
}
