using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.SystemServices;
using Microsoft.EntityFrameworkCore.Query.Internal;
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
        [Refactored("22.07.2025", "v1")]
        public async Task<QueryResult<PassengerCarriageOnTrainRouteOnDate>> CreatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateDto input)
        {
            PassengerCarriageOnTrainRouteOnDate? already_in_memory = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .FirstOrDefaultAsync(carriage_assignement => carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && carriage_assignement.Passenger_Carriage_Id == input.Passenger_Carriage_Id);
            if (already_in_memory is not null)
            {
                return new FailQuery<PassengerCarriageOnTrainRouteOnDate>(new Error(ErrorType.BadRequest, $"This carriage is already in squad " +
                    $"by number {already_in_memory.Position_In_Squad}"));
            }
            PassengerCarriage? passenger_carriage = await passenger_carriage_repository.FindPassengerCarriageById(input.Passenger_Carriage_Id);
            if (passenger_carriage == null)
            {
                return new FailQuery<PassengerCarriageOnTrainRouteOnDate>(new Error(ErrorType.NotFound,
                    $"Can't find passenger carriage with ID: {input.Passenger_Carriage_Id}"));
            }
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_repository.GetTrainRouteOnDateById(input.Train_Route_On_Date_Id);
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

        [Refactored("22.07.2025", "v1")]
        public async Task<QueryResult<List<PassengerCarriageOnTrainRouteOnDate>>> GetPassengerCarriagesForTrainRouteOnDate(string train_route_on_date_id)
        {
            TrainRouteOnDate? train_route_on_date = await context.Train_Routes_On_Date
                .Include(train_route_on_date => train_route_on_date.Carriage_Assignements)
                .ThenInclude(carriage_assignment => carriage_assignment.Passenger_Carriage)
                .FirstOrDefaultAsync(train_route_on_date => train_route_on_date.Id == train_route_on_date_id);
            if (train_route_on_date is null)
            {
                return new FailQuery<List<PassengerCarriageOnTrainRouteOnDate>>(new Error(ErrorType.NotFound, $"Can't find train route on date with id" +
                    $"{train_route_on_date_id}"));
            }
            List<PassengerCarriageOnTrainRouteOnDate> carriage_assignments = train_route_on_date.Carriage_Assignements
                .OrderBy(carriage_assignment => carriage_assignment.Position_In_Squad).ToList();
            return new SuccessQuery<List<PassengerCarriageOnTrainRouteOnDate>>(carriage_assignments);
        }

        [Refactored("22.07.2025", "v1")]
        public async Task<QueryResult<PassengerCarriageOnTrainRouteOnDate>> UpdatePassengerCarriageOnTrainRouteOnDate(PassengerCarriageOnTrainRouteOnDateUpdateDto input)
        {
            PassengerCarriageOnTrainRouteOnDate? existing_carriage_assignment = await context.Passenger_Carriages_On_Train_Routes_On_Date
                .FirstOrDefaultAsync(carriage_assignement => carriage_assignement.Train_Route_On_Date_Id == input.Train_Route_On_Date_Id
                && carriage_assignement.Passenger_Carriage_Id == input.Passenger_Carriage_Id);
            if (existing_carriage_assignment is null)
            {
                return new FailQuery<PassengerCarriageOnTrainRouteOnDate>(new Error(ErrorType.NotFound, "Can't find this carriage assignment"));
            }
            if (input.Position_In_Squad is int position_in_squad)
            {
                existing_carriage_assignment.Position_In_Squad = position_in_squad;
            }
            if (input.Is_For_Children is bool is_for_children)
            {
                existing_carriage_assignment.Is_For_Children = is_for_children;
            }
            if (input.Is_For_Woman is bool is_for_woman)
            {
                existing_carriage_assignment.Is_For_Woman = is_for_woman;
            }
            if (input.Factual_Air_Conditioning is bool factual_air_conditioning)
            {
                existing_carriage_assignment.Factual_Air_Conditioning = factual_air_conditioning;
            }
            if (input.Factual_Shower_Availability is bool factual_shower_availability)
            {
                existing_carriage_assignment.Factual_Shower_Availability = factual_shower_availability;
            }
            if (input.Factual_Is_Inclusive is bool factual_is_inclusive)
            {
                existing_carriage_assignment.Factual_Is_Inclusive = factual_is_inclusive;
            }
            if (input.Food_Availability is bool food_availability)
            {
                existing_carriage_assignment.Food_Availability = food_availability;
            }
            context.Passenger_Carriages_On_Train_Routes_On_Date.Update(existing_carriage_assignment);
            await context.SaveChangesAsync();
            return new SuccessQuery<PassengerCarriageOnTrainRouteOnDate>(existing_carriage_assignment);
        }
        [Refactored("22.07.2025", "v1")]
        public async Task<bool> DeletePassengerCarriageOnTrainRouteOnDate(string passenger_carriage_id, string train_route_on_date_id)
        {
            PassengerCarriageOnTrainRouteOnDate? existing_carriage_assignment = await context.Passenger_Carriages_On_Train_Routes_On_Date
               .FirstOrDefaultAsync(carriage_assignement => carriage_assignement.Train_Route_On_Date_Id == train_route_on_date_id
               && carriage_assignement.Passenger_Carriage_Id == passenger_carriage_id);
            if(existing_carriage_assignment is null)
            {
                return false;
            }
            context.Passenger_Carriages_On_Train_Routes_On_Date.Remove(existing_carriage_assignment);
            await context.SaveChangesAsync();
            return true;
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
            TrainRouteOnDate? train_route_on_date = await train_route_on_date_repository.GetTrainRouteOnDateById(input.Train_Route_On_Date.Id);
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
