using RailwayCore.Context;
using RailwayCore.DTO;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace RailwayCore.Services
{
    public class StationService
    {
        private static TextService text_service = new TextService("StationService");
        private readonly AppDbContext context;
        private readonly RailwayBranchService railway_branch_service;
        public StationService(AppDbContext context, RailwayBranchService railway_branch_service)
        {
            this.context = context;
            this.railway_branch_service = railway_branch_service;
        }
        public async Task<Station?> CreateStation(StationDto input)
        {
            Station? already_in_memory = await context.Stations.FirstOrDefaultAsync(station => station.Id == input.Id);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform($"Station with ID: {input.Id} already exists");
                return already_in_memory;
            }
            RailwayBranch? railway_branch = await railway_branch_service.FindRailwayBranchByTitle(input.Railway_Branch_Title);
            if (railway_branch == null)
            {
                text_service.FailPostInform($"Fail in RailwayBranchService");
                return null;
            }
            Station station = new Station()
            {
                Id = input.Id,
                Register_Id = input.Register_Id,
                Location = input.Location,
                Title = input.Title,
                Type_Of = input.Type_Of,
                Locomotive_Depot = input.Locomotive_Depot,
                Carriage_Depot = input.Carriage_Depot,
                Railway_Branch = railway_branch,
                Region = input.Region,
            };
            context.Stations.Add(station);
            await context.SaveChangesAsync();
            text_service.SuccessPostInform($"Station {station.Title}({station.Id}) has been added successfully");
            return station;
        }
        [Checked("18.04.2025")]
        public async Task<Station?> FindStationById(int id)
        {
            Station? station = await context.Stations.FirstOrDefaultAsync(station => station.Id == id);
            if (station == null)
            {
                text_service.FailGetInform($"Can't find station with ID: {id}");
                return null;
            }
            text_service.SuccessGetInform($"Successfully got station with ID: {id}");
            return station;
        }
        [Checked("18.04.2025")]
        public async Task<Station?> FindStationByTitle(string title)
        {
            Station? station = await context.Stations.FirstOrDefaultAsync(station => station.Title == title);
            if (station == null)
            {
                text_service.FailGetInform($"Can't find station {title}");
                return null;
            }
            text_service.SuccessGetInform($"Successfully got station with title: {title}");
            return station;
        }

    }
}
