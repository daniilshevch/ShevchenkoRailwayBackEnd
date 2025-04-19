using RailwayCore.Models;
using RailwayCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RailwayCore.DTO
{
    public class PassengerTripInfoDto
    {
        public int? User_Id { get; set; }
        public string? Passenger_Name { get; set; }
        public string? Passenger_Surname { get; set; }
        public string? Trip_Starting_Station { get; set; }
        public string? Trip_Ending_Station { get; set; }
    }
    [Checked("18.04.2025")]
    public class InternalSinglePlaceDto //Представлення одного окремого місця в конкретному вагоні(номер місця, заброньованість місця, 
    {                                   //інформація про пасажира, якщо місце заброньоване)
        public int Place_In_Carriage { get; set; } //Номер місця в вагоні
        public bool Is_Free { get; set; } //Чи місце заброньоване
        public List<PassengerTripInfoDto>? Passenger_Trip_Info { get; set; } //Інформація про поїздки пасажирів, які забронювали місце(не обов'язковий запис)
    }
    [Checked("18.04.2025")]
    public class InternalCarriageAssignmentRepresentationDto //Інформація про один конкретний вагон в поїзді(про сам вагон, про бронювання місць, ціна і так далі)
    {
        public PassengerCarriageOnTrainRouteOnDate Carriage_Assignment { get; set; } = null!; //Інформація про вагон та його призначення на маршрут(місце в складі і так далі)
        public List<InternalSinglePlaceDto> Places_Availability { get; set; } = new List<InternalSinglePlaceDto>(); //Інформація про бронювання місць
        public int Price { get; set; } //Ціна квитка в вагон
        public int Free_Places { get; set; }  //Вільних місць в вагоні
        public int Total_Places { get; set; } //Всього місць в вагоні
    }
    [Checked("18.04.2025")]
    public class InternalTrainRouteOnDateAllCarriageAssignmentsRepresentationDto //Інформація про бронювання місць в усіх вагонах
    {
        public TrainRouteOnDate Train_Route_On_Date { get; set; } = null!; //Інформація про рейс поїзда(маршрут поїзда в дату)

        //Список об'єктів-трансферів, де кожний об'єкт представляє інформацію про бронювання в одному вагоні
        public List<InternalCarriageAssignmentRepresentationDto> Carriage_Statistics_List { get; set; } = new List<InternalCarriageAssignmentRepresentationDto>();
        public PlaceAccumulator Total_Place_Highlights { get; set; } = new PlaceAccumulator();


    }
    public class PlaceAccumulator
    {
        public int Platskart_Free { get; set; }
        public int Platskart_Total { get; set; }
        public int Coupe_Free { get; set; }
        public int Coupe_Total { get; set; }
        public int SV_Free { get; set; }
        public int SV_Total { get; set; }
        public int Min_Platskart_Price { get; set; }
        public int Min_Coupe_Price { get; set; }
        public int Min_SV_Price { get; set; }
        public void CountPlaces(PassengerCarriageType type, int free_places_amount, int total_places_amount)
        {
            switch (type)
            {
                case PassengerCarriageType.Platskart:
                    Platskart_Free += free_places_amount;
                    Platskart_Total += total_places_amount;
                    break;
                case PassengerCarriageType.Coupe:
                    Coupe_Free += free_places_amount;
                    Coupe_Total += total_places_amount;
                    break;
                case PassengerCarriageType.SV:
                    SV_Free += free_places_amount;
                    SV_Total += total_places_amount;
                    break;
            }
        }

    }
}
