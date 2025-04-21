using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.InternalServices.SystemServices;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Services
{
    public class RailwayBranchService
    {
        private static TextService text_service = new TextService("RailwayBranchService");
        private readonly AppDbContext context;
        public RailwayBranchService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<RailwayBranch?> CreateRailwayBranch(RailwayBranchDto input)
        {
            RailwayBranch? already_in_memory = await context.Railway_Branches.FirstOrDefaultAsync(railway_branch => railway_branch.Id == input.Id);
            if (already_in_memory is not null)
            {
                text_service.DuplicateGetInform($"Railway branch with ID: {input.Id} already exists");
                return already_in_memory;
            }
            RailwayBranch railway_branch = new RailwayBranch()
            {
                Id = input.Id,
                Title = input.Title,
                Office_Location = input.Office_Location
            };
            context.Railway_Branches.Add(railway_branch);
            await context.SaveChangesAsync();
            text_service.SuccessPostInform("Succesfully created railway branch");
            return railway_branch;
        }
        public async Task<RailwayBranch?> FindRailwayBranchById(int id)
        {
            RailwayBranch? railway_branch = await context.Railway_Branches.FirstOrDefaultAsync(railway_branch => railway_branch.Id == id);
            if (railway_branch == null)
            {
                text_service.FailGetInform($"Can't find railway branch with ID: {id}");
                return null;
            }
            text_service.SuccessGetInform($"Successfully got railway branch with ID: {id}");
            return railway_branch;
        }
        public async Task<RailwayBranch?> FindRailwayBranchByTitle(string title)
        {
            RailwayBranch? railway_branch = await context.Railway_Branches.FirstOrDefaultAsync(railway_branch => railway_branch.Title == title);
            if (railway_branch == null)
            {
                text_service.FailGetInform($"Can't find railway branch with Title: {title}");
                return null;
            }
            text_service.SuccessGetInform($"Successfully got railway branch with Title: {title}");
            return railway_branch;
        }

    }
}
