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
        Express,
        Fast,
        General
    }
    public enum FrequencyType
    {
        Daily,
        One_To_One,
        Specific_Dates
    }
    public enum TripType
    {
        Night_Long_Distance,
        Day_Long_Distance,
        Night_Intercity,
        Day_Intercity,
        Night_Regional,
        Day_Regional,
        Local

    };
    public enum AssignementType
    {
        Whole_Year,
        Seasonal,
        Additional,
        Special
    }
    public enum TrainQualityClass
    {
        S,
        A,
        B,
        C
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
        public int? Train_Route_Coefficient { get; set; } = 1; //Коефіцієнт вартості маршруту
        public List<TrainRouteOnDate> Train_Assignements { get; set; } = new List<TrainRouteOnDate>(); //Конкретні рейси даного маршруту

    }
}
