using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models
{
    public enum StationType
    {
        Mixed = 0,
        Passenger = 1,
        Cargo = 2
    }
    public enum Region
    {
        Lvivska = 0,
        Odeska = 1,
        Ternopilska = 2,
        Khmelnytska = 3,
        Zakarpatska = 4,
        Vinnytska = 5,
        Rivnenska = 6,
        Volynska = 7,
        Ivano_Frankivska = 8,
        Chernivetska = 9,
        Zhytomyrska = 10,
        Kyivska = 11,
        Mykolaivska = 12,
        Khersonska = 13,
        Kyrovogradska = 14,
        Dnipropetrovska = 15,
        Kharkivska = 16,
        Chernihivska = 17,
        Sumska = 18,
        Donetska = 19,
        Zaporizka = 20,
        Luhanska = 21,
        Poltavska = 22,
        Cherkaska = 23
    }
    public class Station //Станція
    {
        [Key]
        public int Id { get; set; } //Ідентифікатор станції
        public string? Register_Id { get; set; } //Інший ідентифікатор станції
        [MaxLength(30)]
        public string Title { get; set; } = null!; //Назва станції
        [MaxLength(30)]
        public string? Ukrainian_Title { get; set; } //Українська назва станції(можливо, буде прибрано)
        [MaxLength(30)]
        public string? Location { get; set; } //Фактичне місцезнаходження станції(в деяких випадках не збігається)
        [MaxLength(20)]
        public StationType Type_Of { get; set; } = StationType.Mixed; //Тип станції за призначенням
        public Region? Region { get; set; } //Область, в якій знаходиться станція
        [MaxLength(20)]
        public string? Locomotive_Depot { get; set; } //Локомотивне депо(якщо наявне)
        [MaxLength(20)]
        public string? Carriage_Depot { get; set; } //Вагонне депо(якщо наявне)
        public RailwayBranch Railway_Branch { get; set; } = null!; //Філія, до якої приписана станція
        public int Railway_Branch_Id { get; set; } //вище
        public List<PassengerCarriage> Carriages_In_Depot { get; set; } = new List<PassengerCarriage>(); //Вагони, які приписані до вагонного депо станції(якщо воно є)
        public List<TrainRouteOnDate> Train_Routes_On_Date { get; set; } = new List<TrainRouteOnDate>(); //Рейси поїздів, які проходять через станцію
        public List<TrainRouteOnDateOnStation> Train_Stops { get; set; } = new List<TrainRouteOnDateOnStation>(); //Зупинки поїздів на цій станції(з ідентифікатором рейсу і часом прибуття-відправлення)
        public List<TicketBooking> Ticket_Bookings_With_Station_As_Starting { get; set; } = new List<TicketBooking>(); //Квитки, де дана станція є стартовою
        public List<TicketBooking> Ticket_Bookings_With_Station_As_Ending { get; set; } = new List<TicketBooking>(); //Квитки, де дана станція є фінальною
        public List<TicketInSelling> Opened_Tickets_With_Station_As_Starting { get; set; } = new List<TicketInSelling>();
        public List<TicketInSelling> Opened_Tickets_With_Station_As_Ending { get; set; } = new List<TicketInSelling>();
    }
}
