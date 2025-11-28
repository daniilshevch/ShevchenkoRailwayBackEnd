using Microsoft.EntityFrameworkCore;
using RailwayCore.Context;
using RailwayCore.InternalServices.ModelRepositories.Interfaces;
using RailwayCore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.InternalServices.ModelRepositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<User?> GetUserByEmailOrUsername(string email, string user_name)
        {
            return await context.Users.Include(user => user.User_Profile).FirstOrDefaultAsync(user => user.Email == email || user.User_Name == user_name);
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return await context.Users.Include(user => user.User_Profile).FirstOrDefaultAsync(user => user.Email == email);
        }
        public async Task AddUser(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
        public async Task AddUserProfile(UserProfile user_profile)
        {
            await context.User_Profiles.AddAsync(user_profile);
            await context.SaveChangesAsync();
        }
        public async Task UpdateUser(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();

        }
    }
}
