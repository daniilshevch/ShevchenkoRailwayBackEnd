using RailwayCore.Models;
using System.ComponentModel.DataAnnotations;

namespace RailwayManagementSystemAPI.ExternalDTO
{
    public class ExternalSimpleTrainRaceDto
    {
        public string Id { get; set; } = null!;
        public string Train_Route_Id { get; set; } = null!; 
        public DateOnly Departure_Date { get; set; } 
        public double? Train_Race_Coefficient { get; set; } = 1; 
        public static explicit operator ExternalSimpleTrainRaceDto(TrainRouteOnDate train_race)
        {
            return new ExternalSimpleTrainRaceDto()
            {
                Id = train_race.Id,
                Train_Route_Id = train_race.Train_Route_Id,
                Departure_Date = train_race.Departure_Date,
                Train_Race_Coefficient = train_race.Train_Race_Coefficient
            };
        }
    }
}
    