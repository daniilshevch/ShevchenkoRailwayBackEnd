using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Models
{
    public class PassengerCarriageOnTrainRouteOnDate //Призначення вагону на певний рейс(певний маршрут в конкретну дату)
    {
        public PassengerCarriage Passenger_Carriage { get; set; } = null!; //Вагон
        public string Passenger_Carriage_Id { get; set; } = null!; //вище
        public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!; //Рейс поїзда(певний маршрут в конкретну дату)
        public string Train_Route_On_Date_Id { get; set; } = null!; //вище
        public int Position_In_Squad { get; set; } //Позиція в складі, на яку ставиться даний вагон, на даний рейс
        public bool Is_For_Woman { get; set; } = false; //Чи є тільки для жінок
        public bool Is_For_Children { get; set; } = false; //Чи є тільки для дітей
        public bool Factual_Wi_Fi { get; set; } = false; //Чи є в вагоні вай фай(чи він фактично доступний на рейсі) 
        public bool Factual_Air_Conditioning { get; set; } = false; //Чи фактично буде працювати кондиціонер на даному рейсі
        public bool Factual_Shower_Availability { get; set; } = false; //Чи фактично буде працювати душ на даному рейсі
        public bool Factual_Is_Inclusive { get; set; } = false; //Чи фактично вагон зможе перевозити людей з відхиленнями на даному рейсі
        public bool Factual_Wifi_Availability { get; set; } = false; //Чи фактично буде працювати вай-фай на даному рейсі (?? дублювання - вирішити)
        public bool Food_Availability { get; set; } = false; //Чи буде надаватись харчування на даному рейсі
        public override string ToString()
        {
            return $"{Passenger_Carriage.Id} - {Passenger_Carriage.Type_Of}({Passenger_Carriage.Manufacturer},{Passenger_Carriage.Capacity}) " +
                $" - {Position_In_Squad}";
        }
    }
}
