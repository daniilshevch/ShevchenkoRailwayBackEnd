using RailwayCore.Context;
using RailwayCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;

namespace RailwayCore.InternalServices.ModelRepositories.Implementations
{

    public class StationRepository : IStationRepository
    {
        private readonly AppDbContext context;
        private readonly RailwayBranchRepository railway_branch_repository;
        public StationRepository(AppDbContext context, RailwayBranchRepository railway_branch_repository)
        {
            this.context = context;
            this.railway_branch_repository = railway_branch_repository;
        }
        public async Task<QueryResult<Station>> CreateStation(StationDto input)
        {
            Station? already_in_memory = await context.Stations.FirstOrDefaultAsync(station => station.Id == input.Id);
            if (already_in_memory is not null)
            {
                return new FailQuery<Station>(new Error(ErrorType.BadRequest, $"Station with ID: {input.Id} already exists"));
            }
            RailwayBranch? railway_branch = await railway_branch_repository.FindRailwayBranchByTitle(input.Railway_Branch_Title);
            if (railway_branch is null)
            {
                return new FailQuery<Station>(new Error(ErrorType.NotFound, $"Can't find railway branch with title: {input.Railway_Branch_Title}"));
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
            return new SuccessQuery<Station>(station);
        }
        [Checked("18.04.2025")]
        public async Task<Station?> GetStationById(int id)
        {
            Station? station = await context.Stations.Include(station => station.Railway_Branch).FirstOrDefaultAsync(station => station.Id == id);
            if (station is null)
            {
                return null;
            }
            return station;
        }
        [Checked("18.04.2025")]
        public async Task<Station?> GetStationByTitle(string title)
        {
            Station? station = await context.Stations.Include(station => station.Railway_Branch).FirstOrDefaultAsync(station => station.Title == title);
            if (station == null)
            {
                return null;
            }
            return station;
        }
        public async Task<List<Station>> GetStations()
        {
            List<Station> stations = await context.Stations.Include(station => station.Railway_Branch).ToListAsync();
            return stations;
        }
        public async Task<QueryResult<Station>> UpdateStation(StationDto input)
        {
            Station? existing_station = await context.Stations.FirstOrDefaultAsync(station => station.Id == input.Id);
            if (existing_station is null)
            {
                return new FailQuery<Station>(new Error(ErrorType.NotFound, $"Can't find station with ID: {input.Id}"));
            }
            RailwayBranch? railway_branch = await railway_branch_repository.FindRailwayBranchByTitle(input.Railway_Branch_Title);
            if (railway_branch is null)
            {
                return new FailQuery<Station>(new Error(ErrorType.NotFound, $"Can't find railway branch with title: {input.Railway_Branch_Title}"));
            }
            existing_station.Register_Id = input.Register_Id;
            existing_station.Title = input.Title;
            existing_station.Location = input.Location;
            existing_station.Type_Of = input.Type_Of;
            existing_station.Region = input.Region;
            existing_station.Register_Id = input.Register_Id;
            existing_station.Railway_Branch = railway_branch;
            context.Stations.Update(existing_station);
            await context.SaveChangesAsync();
            return new SuccessQuery<Station>(existing_station);
        }
        public async Task<bool> DeleteStation(int id)
        {
            Station? station = await context.Stations.FirstOrDefaultAsync(station => station.Id == id);
            if (station is null)
            {
                return false;
            }
            context.Stations.Remove(station);
            await context.SaveChangesAsync();
            return true;

        }
    }
}
