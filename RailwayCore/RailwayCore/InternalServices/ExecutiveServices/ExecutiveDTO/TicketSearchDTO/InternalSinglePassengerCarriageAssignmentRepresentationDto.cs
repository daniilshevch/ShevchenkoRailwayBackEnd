﻿using RailwayCore.Models;
using System.Text.Json.Serialization;
/// <summary>
/// Даний трансферний об'єкт містить інформацію про один конкретний вагон в складі деякого рейсу, а конкретно про заброньовані місця в ньому
/// (в контексті поїздки між двома станціями)
/// </summary>
[Checked("18.04.2025")]
public class InternalSinglePassengerCarriageAssignmentRepresentationDto //Інформація про один конкретний вагон в поїзді(про сам вагон, про бронювання місць, ціна і так далі)
{
    [JsonPropertyName("carriage_assignment")]
    public PassengerCarriageOnTrainRouteOnDate Carriage_Assignment { get; set; } = null!; //Інформація про вагон та його призначення на маршрут(місце в складі і так далі)
    [JsonPropertyName("places_availability")]
    public List<InternalSinglePlaceDto> Places_Availability { get; set; } = new List<InternalSinglePlaceDto>(); //Інформація про бронювання місць
    [JsonPropertyName("free_places")]
    public int Free_Places { get; set; }  //Вільних місць в вагоні
    [JsonPropertyName("total_places")]
    public int Total_Places { get; set; } //Всього місць в вагоні
}