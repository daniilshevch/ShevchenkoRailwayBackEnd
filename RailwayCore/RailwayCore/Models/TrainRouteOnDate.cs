using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models
{
    public class TrainRouteOnDate //Рейс похзду(конкретне призначення поїзда за певним маршрутом в певну дату)
    {
        [Key]
        [MaxLength(20)]
        public string Id { get; set; } = null!; //Ідентифікатор поїзду у форматі "{ідентифікатор машруту}_{рік}_{місяць}_{день}", де рік, місяць і день - дата відправлення рейсу
        public TrainRoute Train_Route { get; set; } = null!; //Маршрут, за яким проводиться рейс поїзда
        public string Train_Route_Id { get; set; } = null!; //вище
        public DateOnly Departure_Date { get; set; } //Дата відправлення рейсу
        public int? Train_Race_Coefficient { get; set; } = 1; //Коефіцієнт вартості рейсу
        public List<Station> Stations { get; set; } = new List<Station>(); //Станції, через які проходить рейс(без додаткової інформації)
        public List<TrainRouteOnDateOnStation> Train_Stops { get; set; } = new List<TrainRouteOnDateOnStation>(); //Зупинки, через які проходить рейс
        //(станції з конкретною інформацію про час прибуття і так далі)
        public List<PassengerCarriage> Passenger_Carriages { get; set; } = new List<PassengerCarriage>(); //Пасажирські вагони, які будуть курсувати в даному рейсі(без додаткової інформації)
        public List<PassengerCarriageOnTrainRouteOnDate> Carriage_Assignements { get; set; } = new List<PassengerCarriageOnTrainRouteOnDate>(); //Пасажирські вагони, які будуть
        //курсувати в рейсі з повною інформацію про призначення(місце в складі і так далі)
        public List<TicketBooking> Ticket_Bookings { get; set; } = new List<TicketBooking>(); //Квитки, куплені на даний рейс
        public List<TicketInSelling> Opened_Tickets { get; set; } = new List<TicketInSelling>();

    }
}
