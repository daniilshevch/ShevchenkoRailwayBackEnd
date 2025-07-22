using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.SystemServices;
namespace RailwayCore.InternalServices.ModelServices
{
    public class PassengerCarriageOnTrainRouteOnDateRepository
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateRepository train_route_on_date_repository;
        private readonly PassengerCarriageRepository passenger_carriage_repository;
        public PassengerCarriageOnTrainRouteOnDateRepository(AppDbContext context, TrainRouteOnDateRepository train_route_on_date_repository,
            PassengerCarriageRepository passenger_carriage_repository)
        {
            this.context = context;
            this.train_route_on_date_repository = train_route_on_date_repository;
            this.passenger_carriage_repository = passenger_carriage_repository;
        }
        public async Task<QueryResult<PassengerCarriageOnTrainRouteOnDate>> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input)
        {
            PassengerCarriageOnTrainRouteOnDate? already_in_memory = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .FirstOrDefaultAsync(carriage_assignement => carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && carriage_assignement.Passenger_Carriage_Id == input.Passenger_Carriage_Id);
            if (already_in_memory is not null)
            {
                return new SuccessQuery<PassengerCarriageOnTrainRouteOnDate>(already_in_memory);
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_repository.FindPassengerCarriageById(input.Passenger_Carriage_Id);
            if (passenger_carriage == null)
            {
                return new FailQuery<PassengerCarriageOnTrainRouteOnDate>(new Error(ErrorType.NotFound,
                    $"Can't find passenger carriage with ID: {input.Passenger_Carriage_Id}"));
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_repository.FindTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if (train_route_on_date == null)
            {
                return new FailQuery<PassengerCarriageOnTrainRouteOnDate>(new Error(ErrorType.NotFound,
    $"Can't find train race with ID: {input.Train_Route_On_Date_Id}"));
            }
            PassengerCarriageOnTrainRouteOnDate passenger_carriage_on_train_route_on_date = new PassengerCarriageOnTrainRouteOnDate
            {
                Passenger_Carriage = passenger_carriage,
                Train_Route_On_Date = train_route_on_date,
                Position_In_Squad = input.Position_In_Squad,
                Is_For_Children = input.Is_For_Children,
                Is_For_Woman = input.Is_For_Woman,
                Factual_Air_Conditioning = input.Factual_Air_Conditioning,
                Factual_Shower_Availability = input.Factual_Shower_Availability,
                Factual_Is_Inclusive = input.Factual_Is_Inclusive,
                Food_Availability = input.Food_Availability
            };
            context.Passenger_Carriages_On_Train_Routes_On_Date.Add(passenger_carriage_on_train_route_on_date);
            await context.SaveChangesAsync();
            return new SuccessQuery<PassengerCarriageOnTrainRouteOnDate>(passenger_carriage_on_train_route_on_date);
        }





        public async Task<PassengerCarriageOnTrainRouteOnDate?> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDate input)
        {
            PassengerCarriageOnTrainRouteOnDate? already_in_memory = await context.Passenger_Carriages_On_Train_Routes_On_Date
               .FirstOrDefaultAsync(carriage_assignement => carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date.Id
               && carriage_assignement.Passenger_Carriage_Id == input.Passenger_Carriage.Id);
            if (already_in_memory is not null)
            {
                return already_in_memory;
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_repository.FindPassengerCarriageById(input.Passenger_Carriage.Id);
            if (passenger_carriage == null)
            {
                return null;
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_repository.FindTrainRouteOnDateById(input.Train_Route_On_Date.Id);
            if (train_route_on_date == null)
            {
                return null;
            }
            context.Passenger_Carriages_On_Train_Routes_On_Date.Add(input);
            await context.SaveChangesAsync();
            return input;
        }

    }
}
