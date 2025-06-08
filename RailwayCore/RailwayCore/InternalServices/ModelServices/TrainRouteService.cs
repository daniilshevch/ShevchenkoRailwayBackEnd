using RailwayCore.Models;
using RailwayCore.Context;
using Microsoft.EntityFrameworkCore;
using System;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.SystemServices;
namespace RailwayCore.InternalServices.ModelServices
{
    public class TrainRouteService
    {
        private static TextService text_service = new TextService("TrainRouteService");
        private readonly AppDbContext context;
        private readonly RailwayBranchService railway_branch_service;
        public TrainRouteService(AppDbContext context, RailwayBranchService railway_branch_service)
        {
            this.context = context;
            this.railway_branch_service = railway_branch_service;
        }


        public async Task<TrainRoute?> AddTrainRoute(TrainRouteDto input)
        {
            TrainRoute? already_in_memory = await context.Train_Routes.FirstOrDefaultAsync(train_route => train_route.Id == input.Id);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform($"Train route with ID: {input.Id} already exists");
                return already_in_memory;
            }
            RailwayBranch? railway_branch = await railway_branch_service.FindRailwayBranchByTitle(input.Railway_Branch_Title);
            if (railway_branch == null)
            {
                text_service.FailPostInform("Fail in RailwayBranchService");
                return null;
            }
            TrainRoute train_route = new TrainRoute()
            {
                Id = input.Id,
                Is_Branded = input.Is_Branded,
                Branded_Name = input.Branded_Name,
                Quality_Class = input.Quality_Class,
                Speed_Type = input.Speed_Type,
                Frequency_Type = input.Frequency_Type,
                Assignement_Type = input.Assignement_Type,
                Railway_Branch = railway_branch
            };
            context.Train_Routes.Add(train_route);
            await context.SaveChangesAsync();
            text_service.SuccessPostInform("Succesfully created train route");
            return train_route;
        }
        public async Task<TrainRoute?> FindTrainRouteById(string id)
        {
            TrainRoute? train_route = await context.Train_Routes.FirstOrDefaultAsync(train_route => train_route.Id == id);
            if (train_route == null)
            {
                text_service.FailGetInform($"Can't find train_route with ID: {id}");
                return null;
            }
            text_service.SuccessGetInform($"Successfully got train route with ID: {id}");
            return train_route;
        }

    }
}
