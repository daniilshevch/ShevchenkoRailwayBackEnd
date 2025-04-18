using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models
{
    public enum StopType
    {
        Boarding,
        Technical
    }
    public class TrainRouteOnDateOnStation  //Зупинка рейса поїзда
    {
        public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!; //Рейс поїзда, який робить зупинку
        public string Train_Route_On_Date_Id { get; set; } = null!; //вище
        public Station Station { get; set; } = null!; //Станція, на якій робить зупинку рейс поїзда
        public int Station_Id { get; set; } //вище

        public DateTime? Arrival_Time { get; set; } //Час прибуття(якщо станція стартова, то null)
        public DateTime? Departure_Time { get; set; }  //Час відправлення(якщо станція кінцева, то null)
        [MaxLength(20)]
        public StopType Stop_Type { get; set; } = StopType.Boarding; //Тип зупинки
        public double? Distance_From_Starting_Station { get; set; } //Відстань до даної станції від початку маршруту рейсу
        public double? Speed_On_Section { get; set; } //Швидкість на наступній секції(від даної зупинки до наступної)
        public override string ToString()
        {
            string? arrival_time;
            string? departure_time;
            if (Arrival_Time is not null)
            {
                arrival_time = Arrival_Time.ToString();
            }
            else
            {
                arrival_time = "#################";
            }
            if (Departure_Time is not null)
            {
                departure_time = Departure_Time.ToString();
            }
            else
            {
                departure_time = "#################";
            }

            return $"{Station.Title}: {arrival_time} - {departure_time}";
        }


    }
}
