using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.ModelRepositories.Implementations;
using RailwayCore.Models;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Interfaces;

namespace RailwayCore.InternalServices.ExecutiveServices.TrainAssignmentServices.Implementations
{
    public class TrainSquadCopyService : ITrainSquadCopyService
    {
        private readonly AppDbContext context;
        private readonly ITrainRouteOnDateRepository train_route_on_date_service;
        private readonly IPassengerCarriageOnTrainRouteOnDateRepository passenger_carriage_on_train_route_on_date_service;
        private readonly IPassengerCarriageRepository passenger_carriage_service;
        public TrainSquadCopyService(AppDbContext context, ITrainRouteOnDateRepository train_route_on_date_service,
            IPassengerCarriageOnTrainRouteOnDateRepository passenger_carriage_on_train_route_on_date_service,
            IPassengerCarriageRepository passenger_carriage_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.passenger_carriage_on_train_route_on_date_service = passenger_carriage_on_train_route_on_date_service;
            this.passenger_carriage_service = passenger_carriage_service;
        }
        public async Task<QueryResult> CopyTrainRouteOnDateWithSquad(string prototype_train_route_id, string new_train_route_id, DateOnly prototype_date, DateOnly new_date, bool creation_option = true)
        {
            string prototype_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(prototype_train_route_id, prototype_date);
            TrainRouteOnDate? prototype_train_route_on_date = await train_route_on_date_service.GetTrainRouteOnDateById(prototype_train_route_on_date_id);
            if (prototype_train_route_on_date is null)
            {
                return new FailQuery(new Error(ErrorType.NotFound, $"Can't find prototype with ID: {prototype_train_route_on_date_id}"));
            }
            TrainRouteOnDate? new_train_route_on_date = null;
            if (creation_option)
            {
                new_train_route_on_date = await train_route_on_date_service.CreateTrainRouteOnDate(new_train_route_id, new_date);
                if (new_train_route_on_date is null)
                {
                    return new FailQuery(new Error(ErrorType.BadRequest, "Problems while creating train race"));
                }
            }
            else
            {
                string new_train_route_on_date_id = train_route_on_date_service.BuildTrainRouteOnDateIdentificator(new_train_route_id, new_date);
                new_train_route_on_date = await train_route_on_date_service
                    .GetTrainRouteOnDateById(new_train_route_on_date_id);

                if (new_train_route_on_date == null)
                {
                    return new FailQuery(new Error(ErrorType.NotFound, $"Can't find train race with ID: {new_train_route_on_date_id}"));
                }
            }
            List<PassengerCarriageOnTrainRouteOnDate> passenger_carriages_on_prototype_train_route_on_date_on_station =
                context.Passenger_Carriages_On_Train_Routes_On_Date
                .Include(carriage_assignement => carriage_assignement.Train_Route_On_Date)
                .Include(carriage_assignement => carriage_assignement.Passenger_Carriage)
                .Where(carriage_assignement =>
                carriage_assignement.Train_Route_On_Date_Id == prototype_train_route_on_date_id).ToList();
            foreach (PassengerCarriageOnTrainRouteOnDate carriage_assignement in passenger_carriages_on_prototype_train_route_on_date_on_station)
            {
                PassengerCarriageOnTrainRouteOnDate new_carriage_assignement = new PassengerCarriageOnTrainRouteOnDate
                {
                    Train_Route_On_Date = new_train_route_on_date,
                    Passenger_Carriage = carriage_assignement.Passenger_Carriage,
                    Position_In_Squad = carriage_assignement.Position_In_Squad,
                    Is_For_Children = carriage_assignement.Is_For_Children,
                    Is_For_Woman = carriage_assignement.Is_For_Woman,
                    Factual_Air_Conditioning = carriage_assignement.Factual_Air_Conditioning,
                    Factual_Shower_Availability = carriage_assignement.Factual_Shower_Availability,
                    Factual_Is_Inclusive = carriage_assignement.Factual_Is_Inclusive,
                    Food_Availability = carriage_assignement.Food_Availability
                };
                await passenger_carriage_on_train_route_on_date_service.CreatePassengerCarriageOnTrainRouteOnDate(new_carriage_assignement);
            }
            await context.SaveChangesAsync();
            return new SuccessQuery();
        }
    }
}
