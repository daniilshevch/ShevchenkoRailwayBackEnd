using RailwayCore.Models.ModelEnums.TrainRouteEnums;
using System.ComponentModel.DataAnnotations;

namespace RailwayCore.Models
{
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
