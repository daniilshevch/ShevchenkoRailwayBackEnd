using RailwayCore.Models;
using RailwayCore.Context;
using Microsoft.EntityFrameworkCore;
using System;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.SystemServices;
using System.Collections.Generic;
namespace RailwayCore.InternalServices.ModelServices
{
    public class TrainRouteRepository
    {
        private readonly AppDbContext context;
        private readonly RailwayBranchRepository railway_branch_repository;
        public TrainRouteRepository(AppDbContext context, RailwayBranchRepository railway_branch_repository)
        {
            this.context = context;
            this.railway_branch_repository = railway_branch_repository;
        }


        public async Task<QueryResult<TrainRoute>> CreateTrainRoute(TrainRouteDto input)
        {
            TrainRoute? already_in_memory = await context.Train_Routes.FirstOrDefaultAsync(train_route => train_route.Id == input.Id);
            if (already_in_memory is not null)
            {
                return new FailQuery<TrainRoute>(new Error(ErrorType.BadRequest, $"Train route with ID: {input.Id} already exists"));
            }
            RailwayBranch? railway_branch = await railway_branch_repository.FindRailwayBranchByTitle(input.Railway_Branch_Title);
            if (railway_branch == null)
            {
                return new FailQuery<TrainRoute>(new Error(ErrorType.BadRequest, $"Railway branch {input.Railway_Branch_Title} doesn't exist"));
            }
            TrainRoute train_route = new TrainRoute()
            {
                Id = input.Id,
                Is_Branded = input.Is_Branded,
                Branded_Name = input.Branded_Name,
                Quality_Class = input.Quality_Class,
                Speed_Type = input.Speed_Type,
                Trip_Type = input.Trip_Type,
                Frequency_Type = input.Frequency_Type,
                Assignement_Type = input.Assignement_Type,
                Railway_Branch = railway_branch,
                Train_Route_Coefficient = input.Train_Route_Coefficient,
            };
            context.Train_Routes.Add(train_route);
            await context.SaveChangesAsync();
            return new SuccessQuery<TrainRoute>(train_route);
        }
        public async Task<TrainRoute?> GetTrainRouteById(string id)
        {
            TrainRoute? train_route = await context.Train_Routes.Include(train_route => train_route.Railway_Branch).FirstOrDefaultAsync(train_route => train_route.Id == id);
            if (train_route == null)
            {
                return null;
            }
            return train_route;
        }
        public async Task<List<TrainRoute>> GetTrainRoutes()
        {
            return await context.Train_Routes.Include(train_route => train_route.Railway_Branch).ToListAsync();
        }
        public async Task<QueryResult<TrainRoute>> UpdateTrainRoute(TrainRouteDto input)
        {
            TrainRoute? train_route = await context.Train_Routes.FirstOrDefaultAsync(train_route => train_route.Id == input.Id);
            RailwayBranch? railway_branch = await context.Railway_Branches.FirstOrDefaultAsync(branch => branch.Title == input.Railway_Branch_Title);
            if(train_route == null)
            {
                return new FailQuery<TrainRoute>(new Error(ErrorType.NotFound, $"Can't find train route with ID: {input.Id}"));
            }
            if (railway_branch == null)
            {
                return new FailQuery<TrainRoute>(new Error(ErrorType.NotFound, $"Can't find railway branch {input.Railway_Branch_Title}"));
            }
            train_route.Trip_Type = input.Trip_Type;
            train_route.Quality_Class = input.Quality_Class;
            train_route.Railway_Branch = railway_branch;
            train_route.Is_Branded = input.Is_Branded;
            train_route.Assignement_Type = input.Assignement_Type;
            train_route.Speed_Type = input.Speed_Type;
            train_route.Frequency_Type = input.Frequency_Type;
            train_route.Branded_Name = input.Branded_Name;
            train_route.Train_Route_Coefficient = input.Train_Route_Coefficient;
            context.Train_Routes.Update(train_route);
            await context.SaveChangesAsync();
            return new SuccessQuery<TrainRoute>(train_route);
        }
        public async Task<bool> DeleteTrainRouteById(string id)
        {
            TrainRoute? train_route = await context.Train_Routes.FirstOrDefaultAsync(train_route => train_route.Id == id);
            if (train_route == null)
            {
                return false;
            }
            context.Train_Routes.Remove(train_route);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
