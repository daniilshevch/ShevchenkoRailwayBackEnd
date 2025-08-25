using RailwayCore.Models;
using RailwayCore.Context;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Globalization;
using System;
using RailwayCore.InternalDTO.ModelDTO;
namespace RailwayCore.InternalServices.ModelServices
{
    public class TrainRouteOnDateRepository
    {

        private AppDbContext context;
        private TrainRouteRepository train_route_repository;
        public TrainRouteOnDateRepository(AppDbContext context, TrainRouteRepository train_route_repository)
        {
            this.context = context;
            this.train_route_repository = train_route_repository;
        }

        public async Task<TrainRouteOnDate?> CreateTrainRouteOnDate(TrainRouteOnDateDto input)
        {
            TrainRouteOnDate? already_in_memory = await context.Train_Routes_On_Date
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Train_Route_Id == input.Train_Route_Id
                && train_route_on_date.Departure_Date == input.Departure_Date);
            if (already_in_memory is not null)
            {
                return already_in_memory;
            }
            TrainRoute? train_route
            = await train_route_repository.GetTrainRouteById(input.Train_Route_Id);
            if (train_route == null)
            {
                return null;
            }
            DateOnly departure_date = input.Departure_Date;
            string train_route_id = train_route.Id;
            string train_route_on_date_id =
                BuildTrainRouteOnDateIdentificator(train_route_id, departure_date);
            TrainRouteOnDate train_route_on_date = new TrainRouteOnDate
            {
                Id = train_route_on_date_id,
                Departure_Date = departure_date,
                Train_Route = train_route,
                Train_Race_Coefficient = input.Train_Race_Coefficient
            };

            context.Train_Routes_On_Date.Add(train_route_on_date);
            await context.SaveChangesAsync();
            return train_route_on_date;
        }
        public async Task<TrainRouteOnDate?> CreateTrainRouteOnDate(string train_route_id, DateOnly departure_date)
        {
            TrainRouteOnDateDto input = new TrainRouteOnDateDto { Train_Route_Id = train_route_id, Departure_Date = departure_date };
            return await CreateTrainRouteOnDate(input);

        }
        public async Task<TrainRouteOnDate?> GetTrainRouteOnDateById(string id)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == id);
            if (train_route_on_date == null)
            {
                return null;
            }
            return train_route_on_date;
        }
        public async Task<List<TrainRouteOnDate>> GetTrainRoutesOnDateForTrainRoute(string train_route_id)
        {
            List<TrainRouteOnDate> train_routes_on_date = await context.Train_Routes_On_Date.Where(train_route_on_date =>
            train_route_on_date.Train_Route_Id == train_route_id).ToListAsync();
            return train_routes_on_date;
        }
        public async Task<TrainRouteOnDate?> ChangeTrainRaceCoefficientForTrainRouteOnDate(string train_route_on_date_id, double train_race_coefficient)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date.FirstOrDefaultAsync(train_route_on_date =>
            train_route_on_date.Id == train_route_on_date_id);
            if(train_route_on_date is null)
            {
                return null;
            }
            train_route_on_date.Train_Race_Coefficient = train_race_coefficient;
            context.Train_Routes_On_Date.Update(train_route_on_date);
            await context.SaveChangesAsync();
            return train_route_on_date;
        }
        public async Task<bool> DeleteTrainRouteOnDate(string train_route_on_date_id)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date.FirstOrDefaultAsync(train_route_on_date =>
            train_route_on_date.Id == train_route_on_date_id);
            if(train_route_on_date is null)
            {
                return false;
            }
            context.Train_Routes_On_Date.Remove(train_route_on_date);
            await context.SaveChangesAsync();
            return true;
        }


        public string BuildTrainRouteOnDateIdentificator(string train_route_id, DateOnly departure_date)
        {
            string departure_day = departure_date.Day.ToString("D2");
            string departure_month = departure_date.Month.ToString("D2");
            string departure_year = departure_date.Year.ToString();
            string train_route_on_date_id =
                string.Join("_", new string[] { train_route_id, departure_year, departure_month, departure_day });
            return train_route_on_date_id;
        }
        public (string, DateOnly) GetTrainRouteIdAndDateFromTrainRouteOnDate(string train_route_on_date_id)
        {
            string[] attributes = train_route_on_date_id.Split('_');
            string train_route_id = attributes[0];
            int year = Convert.ToInt32(attributes[1]);
            int month = Convert.ToInt32(attributes[2]);
            int day = Convert.ToInt32(attributes[3]);
            return (train_route_id, new DateOnly(year, month, day));
        }

    }
}
