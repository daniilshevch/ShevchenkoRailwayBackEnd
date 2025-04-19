using RailwayCore.Context;
using RailwayCore.Models;
using RailwayCore.DTO;
using Microsoft.EntityFrameworkCore;
using System;
namespace RailwayCore.Services
{
    public class PassengerCarriageOnTrainRouteOnDateService
    {
        private readonly AppDbContext context;
        private readonly TrainRouteOnDateService train_route_on_date_service;
        private readonly PassengerCarriageService passenger_carriage_service;
        private TextService text_service = new TextService("PassengerCarriageOnTrainRouteOnDateService");
        public PassengerCarriageOnTrainRouteOnDateService(AppDbContext context, TrainRouteOnDateService train_route_on_date_service,
            PassengerCarriageService passenger_carriage_service)
        {
            this.context = context;
            this.train_route_on_date_service = train_route_on_date_service;
            this.passenger_carriage_service = passenger_carriage_service;
        }
        public async Task<PassengerCarriageOnTrainRouteOnDate?> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input)
        {
            PassengerCarriageOnTrainRouteOnDate? already_in_memory = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .FirstOrDefaultAsync(carriage_assignement => carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && carriage_assignement.Passenger_Carriage_Id == input.Passenger_Carriage_Id);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform("Carriage assignement with these parameters already exists");
                return already_in_memory;
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(input.Passenger_Carriage_Id);
            if (passenger_carriage == null)
            {
                text_service.FailPostInform("Fail in PassengerCarriageService");
                return null;
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date_Id);
            if (train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
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
            text_service.SuccessPostInform("Succesfully assigned carriage to train route on date");
            return passenger_carriage_on_train_route_on_date;
        }
        public async Task<PassengerCarriageOnTrainRouteOnDate?> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDate input)
        {
            PassengerCarriageOnTrainRouteOnDate? already_in_memory = await context.Passenger_Carriages_On_Train_Routes_On_Date
               .FirstOrDefaultAsync(carriage_assignement => carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date.Id
               && carriage_assignement.Passenger_Carriage_Id == input.Passenger_Carriage.Id);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform("Carriage assignement with these parameters already exists");
                return already_in_memory;
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_service.FindPassengerCarriageById(input.Passenger_Carriage.Id);
            if (passenger_carriage == null)
            {
                text_service.FailPostInform("Fail in PassengerCarriageService");
                return null;
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_service.FindTrainRouteOnDateById(input.Train_Route_On_Date.Id);
            if (train_route_on_date == null)
            {
                text_service.FailPostInform("Fail in TrainRouteOnDateService");
                return null;
            }
            context.Passenger_Carriages_On_Train_Routes_On_Date.Add(input);
            await context.SaveChangesAsync();
            text_service.SuccessPostInform("Succesfully assigned carriage to train route on date");
            return input;
        }

    }
}
