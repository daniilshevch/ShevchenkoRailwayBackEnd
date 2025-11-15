using RailwayCore.InternalDTO.ModelDTO;
using RailwayCore.Models;

namespace RailwayCore.InternalServices.ModelRepositories.Interfaces
{
    public interface IRailwayBranchRepository
    {
        Task<RailwayBranch?> CreateRailwayBranch(RailwayBranchDto input);
        Task<RailwayBranch?> FindRailwayBranchById(int id);
        Task<RailwayBranch?> FindRailwayBranchByTitle(string title);
    }
}