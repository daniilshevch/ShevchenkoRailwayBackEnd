using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.DTO;
using RailwayCore.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace RailwayCore.Services
{
    public class PassengerCarriageService
    {
        private readonly AppDbContext context;
        private readonly StationService station_service;
        private TextService text_service = new TextService("PassengerCarriageService");
        public PassengerCarriageService(AppDbContext context, StationService station_service)
        {
            this.context = context;
            this.station_service = station_service;
        }
        public async Task<PassengerCarriage?> CreatePassengerCarriage(PassengerCarriageDto input)
        {
            PassengerCarriage? already_in_memory = await context.Passenger_Carriages
                .FirstOrDefaultAsync(carriage => carriage.Id == input.Id);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform($"Passenger carriage with ID: {input.Id} already exists");
                return already_in_memory;
            }
            Station? station_depot = null;
            if (input.Station_Depot_Id is int depot_id)
            {
                station_depot = await station_service.FindStationById(depot_id);
                if (station_depot == null)
                {
                    text_service.FailPostInform("Fail in StationService");
                    return null;
                }
            }
            PassengerCarriage passenger_carriage = new PassengerCarriage
            {
                Id = input.Id,
                Type_Of = input.Type_Of,
                Capacity = input.Capacity,
                Production_Year = input.Production_Year,
                Manufacturer = input.Manufacturer,
                Quality_Class = input.Quality_Class,
                Renewal_Fact = input.Renewal_Fact,
                Renewal_Year = input.Renewal_Year,
                Renewal_Performer = input.Renewal_Performer,
                Renewal_Info = input.Renewal_Info,
                Air_Conditioning = input.Air_Conditioning,
                Is_Inclusive = input.Is_Inclusive,
                Is_For_Train_Chief = input.Is_For_Train_Chief,
                Shower_Availability = input.Shower_Availability,
                In_Current_Use = input.In_Current_Use,
                Appearence = input.Appearence,
                Station_Depot = station_depot
            };
            context.Passenger_Carriages.Add(passenger_carriage);
            await context.SaveChangesAsync();
            text_service.SuccessPostInform($"Passenger carriage {input.Id} has been successfully created");
            return passenger_carriage;
        }
        public async Task<PassengerCarriage?> FindPassengerCarriageById(string id)
        {
            PassengerCarriage? passenger_carriage = await context.Passenger_Carriages.FirstOrDefaultAsync(carriage => carriage.Id == id);
            if (passenger_carriage == null)
            {
                text_service.FailGetInform($"Can't find passenger carriage with ID: {id}");
                return null;
            }
            text_service.SuccessGetInform($"Succesfully got passenger carriage with ID: {id}");
            return passenger_carriage;
        }
    }
}
