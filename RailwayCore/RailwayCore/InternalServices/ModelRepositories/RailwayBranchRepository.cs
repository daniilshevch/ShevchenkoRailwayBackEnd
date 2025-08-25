using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ModelServices
{
    public class RailwayBranchRepository
    {
        private readonly AppDbContext context;
        public RailwayBranchRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<RailwayBranch?> CreateRailwayBranch(RailwayBranchDto input)
        {
            RailwayBranch? already_in_memory = await context.Railway_Branches.FirstOrDefaultAsync(railway_branch => railway_branch.Id == input.Id);
            if (already_in_memory is not null)
            {
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
            return railway_branch;
        }
        public async Task<RailwayBranch?> FindRailwayBranchById(int id)
        {
            RailwayBranch? railway_branch = await context.Railway_Branches.FirstOrDefaultAsync(railway_branch => railway_branch.Id == id);
            if (railway_branch == null)
            {
                return null;
            }
            return railway_branch;
        }
        public async Task<RailwayBranch?> FindRailwayBranchByTitle(string title)
        {
            RailwayBranch? railway_branch = await context.Railway_Branches.FirstOrDefaultAsync(railway_branch => railway_branch.Title == title);
            if (railway_branch == null)
            {
                return null;
            }
            return railway_branch;
        }

    }
}
