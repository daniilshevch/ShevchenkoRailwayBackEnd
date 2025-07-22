using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RailwayCore.InternalDTO.ModelDTO
{
    public class TrainRouteOnDateOnStationDto
    {
        [JsonPropertyName("train_route_on_date_id")]
        public string Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("station_title")]
        public string Station_Title { get; set; } = null!;
        [JsonPropertyName("arrival_time")]
        public DateTime? Arrival_Time { get; set; }
        [JsonPropertyName("departure_time")]
        public DateTime? Departure_Time { get; set; }
        [JsonPropertyName("stop_type")]
        public StopType Stop_Type { get; set; } = StopType.Boarding;
        [JsonPropertyName("distance_from_starting_station")]
        public double? Distance_From_Starting_Station { get; set; }
        [JsonPropertyName("speed_on_section")]
        public double? Speed_On_Section { get; set; }
        public static explicit operator TrainRouteOnDateOnStationDto(TrainRouteOnDateOnStation input)
        {
            return new TrainRouteOnDateOnStationDto()
            {
                Train_Route_On_Date_Id = input.Train_Route_On_Date_Id,
                Station_Title = input.Station.Title,
                Arrival_Time = input.Arrival_Time,
                Departure_Time = input.Departure_Time,
                Distance_From_Starting_Station = input.Distance_From_Starting_Station,
                Speed_On_Section = input.Speed_On_Section,
                Stop_Type = input.Stop_Type
            };
        }
    }
    public class TrainRouteOnDateOnStationUpdateDto
    {
        [JsonPropertyName("train_route_on_date_id")]
        public string? Train_Route_On_Date_Id { get; set; } = null!;
        [JsonPropertyName("station_title")]
        public string? Station_Title { get; set; } = null!;
        [JsonPropertyName("arrival_time")]
        public DateTime? Arrival_Time { get; set; }
        [JsonPropertyName("arrival_time_change")]
        public bool? Arrival_Time_Change { get; set; } = false; 
        [JsonPropertyName("departure_time")]
        public DateTime? Departure_Time { get; set; }
        [JsonPropertyName("departure_time_change")]
        public bool? Departure_Time_Change { get; set; } = false;

        [JsonPropertyName("stop_type")]
        public StopType? Stop_Type { get; set; } = StopType.Boarding;
        [JsonPropertyName("distance_from_starting_station")]
        public double? Distance_From_Starting_Station { get; set; }
        [JsonPropertyName("speed_on_section")]
        public double? Speed_On_Section { get; set; }
    }

    public class TrainStopWithoutRouteDto
    {
        public string Station_Title { get; set; } = null!;
        public DateTime? Arrival_Time { get; set; }
        public DateTime? Departure_Time { get; set; }
        public StopType Stop_Type { get; set; } = StopType.Boarding;
        public double? Distance_From_Starting_Station { get; set; }
    }
    public class TrainStopWithoutRouteWithSpeedDto
    {
        public string Station_Title { get; set; } = null!;
        public int Stop_Duration { get; set; }
        public StopType Stop_Type { get; set; } = StopType.Boarding;
        public double Speed_On_Section { get; set; }
        public double? Distance_From_Starting_Station { get; set; }
    }
}
