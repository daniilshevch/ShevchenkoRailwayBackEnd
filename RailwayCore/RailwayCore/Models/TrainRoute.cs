using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models
{
    public enum SpeedType
    {
        Express = 0,
        Fast = 1,
        General = 2
    }
    public enum FrequencyType
    {
        Daily = 0,
        One_To_One = 1,
        Specific_Dates = 2
    }
    public enum TripType
    {
        Night_Long_Distance = 0,
        Day_Long_Distance = 1,
        Night_Intercity = 2,
        Day_Intercity = 3,
        Night_Regional = 4,
        Day_Regional = 5,
        Local = 6

    };
    public enum AssignementType
    {
        Whole_Year = 0,
        Seasonal = 1,
        Additional = 2,
        Special = 3
    }
    public enum TrainQualityClass
    {
        S = 0,
        A = 1,
        B = 2,
        C = 3
    }
    public class TrainRoute //Маршрут поїзда
    {
        [Key]
        [MaxLength(8)]
        public string Id { get; set; } = null!; //Ідентифікатор маршруту
        public bool Is_Branded { get; set; } = false; //Фірмовість поїзду
        public TrainQualityClass? Quality_Class { get; set; } //Клас якості-престижу поїзду
        [MaxLength(30)]
        public string? Branded_Name { get; set; } //Фірмова назва(якщо є)
        public TripType? Trip_Type { get; set; } //Тип маршруту
        public SpeedType? Speed_Type { get; set; } //Швидкісний тип маршруту
        public FrequencyType? Frequency_Type { get; set; } //Частота курсування маршруту
        public AssignementType? Assignement_Type { get; set; } //Тип призначення маршруту
        public RailwayBranch? Railway_Branch { get; set; } //Філія, яка обслуговує маршрут
        public int? Railway_Branch_Id { get; set; } //вище
        public double? Train_Route_Coefficient { get; set; } = 1; //Коефіцієнт вартості маршруту
        public List<TrainRouteOnDate> Train_Assignements { get; set; } = new List<TrainRouteOnDate>(); //Конкретні рейси даного маршруту

    }
}
