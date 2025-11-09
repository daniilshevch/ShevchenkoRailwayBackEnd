using System.ComponentModel.DataAnnotations;
using RailwayCore.Models.ModelEnums.PassengerCarriageEnums;
namespace RailwayCore.Models
{
    public class PassengerCarriage //Пасажирський вагон
    {
        [Key]
        [MaxLength(8)]
        public string Id { get; set; } = null!; //Числовий ідентифікатор вагона
        [MaxLength(30)]
        public PassengerCarriageType Type_Of { get; set; } //Тип вагона
        public int Capacity { get; set; } //Кількість пасажирських місць в вагоні
        public int? Production_Year { get; set; } //Рік виробництва вагона
        [MaxLength(30)]
        public PassengerCarriageManufacturer? Manufacturer { get; set; } //Завод-виробник вагона
        [MaxLength(30)]

        public PassengerCarriageQualityClass? Quality_Class { get; set; } //Підклас якості для типу вагону
        public bool Renewal_Fact { get; set; } = false; //Факт проходження капітально-відновлювального ремонту
        public int? Renewal_Year { get; set; } //Рік КВР(в разі якщо вагон його проходив)
        [MaxLength(30)]
        public PassengerCarriageManufacturer? Renewal_Performer { get; set; } //Завод,який проводив КВР(в разі якщо вагон його проходив)
        [MaxLength(60)]
        public string? Renewal_Info { get; set; } //Інформація про КВР
        public bool Wi_Fi { get; set; }
        public bool Air_Conditioning { get; set; } = false; //Наявність кондиціонеру
        public bool Is_Inclusive { get; set; } = false; //Здатність перевозити пасажирів з відхиленнями
        public bool Is_For_Train_Chief { get; set; } = false; //Здатність перевозити начальника поїзда
        public bool Shower_Availability { get; set; } = false; //Наявність душу
        public bool In_Current_Use { get; set; } = true; //Поточна експлуатація
        [MaxLength(30)]
        public string? Appearence { get; set; } //Інформація про зовнішній вигляд вагону
        public Station? Station_Depot { get; set; } //Станція, Депо приписки
        public int? Station_Depot_Id { get; set; } //вище
        public List<TrainRouteOnDate> Train_Routes_On_Date { get; set; } = new List<TrainRouteOnDate>(); //Рейси, в яких курсує вагон
        public List<PassengerCarriageOnTrainRouteOnDate> Carriage_Assignements { get; set; } = new List<PassengerCarriageOnTrainRouteOnDate>(); //Конкретна інформація про призначення вагона на маршрут в певну дату(рейс)
        public List<TicketBooking> Ticket_Bookings { get; set; } = new List<TicketBooking>(); // Квитки, куплені в даний вагон
        public List<TicketInSelling> Opened_Tickets { get; set; } = new List<TicketInSelling>();


    }
}
