using RailwayCore.Models;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices
{
    public class PricingService
    {
        public static int DefineTicketPrice(double distance, double? _average_speed, double? _train_route_coefficient, double? _train_race_coefficient, PassengerCarriageType carriage_type, PassengerCarriageQualityClass? carriage_quality_class, DateOnly departure_date)
        {
            double train_route_coefficient;
            double average_speed_coefficient;
            double train_race_coefficient;
            double carriage_quality_class_coefficient;

            if (_average_speed is not null)
            {
                average_speed_coefficient = (double)_average_speed / 100;
            }
            else
            {
                average_speed_coefficient = 1.0;
            }

            if (_train_route_coefficient is not null)
            {
                train_route_coefficient = (double)_train_route_coefficient;
            }
            else
            {
                train_route_coefficient = 1.0;
            }

            if (_train_race_coefficient is not null)
            {
                train_race_coefficient = (double)_train_race_coefficient;
            }
            else
            {
                train_race_coefficient = 1.0;
            }
            double carriage_type_coefficient;
            switch (carriage_type)
            {
                case PassengerCarriageType.SV:
                    carriage_type_coefficient = 2;
                    break;
                case PassengerCarriageType.Coupe:
                    carriage_type_coefficient = 1;
                    break;
                case PassengerCarriageType.Platskart:
                    carriage_type_coefficient = 0.5;
                    break;
                default:
                    carriage_type_coefficient = 1;
                    break;
            }
            switch (carriage_quality_class)
            {
                case PassengerCarriageQualityClass.S:
                    carriage_quality_class_coefficient = 2;
                    break;
                case PassengerCarriageQualityClass.A:
                    carriage_quality_class_coefficient = 1.5;
                    break;
                case PassengerCarriageQualityClass.B:
                    carriage_quality_class_coefficient = 1.1;
                    break;
                case PassengerCarriageQualityClass.C:
                    carriage_quality_class_coefficient = 0.75;
                    break;
                default:
                    carriage_quality_class_coefficient = 1;
                    break;
            }
            return (int)Math.Floor(distance * average_speed_coefficient * train_route_coefficient * train_race_coefficient * carriage_type_coefficient * carriage_quality_class_coefficient);
        }
    }
}
