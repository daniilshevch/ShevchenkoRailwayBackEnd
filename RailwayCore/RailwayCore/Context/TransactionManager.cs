using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayCore.Context
{
    public class TransactionManager : ITransactionManager
    {
        private readonly AppDbContext context;
        public TransactionManager(AppDbContext context)
        {
            this.context = context;
        }
        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return context.Database.BeginTransactionAsync();
        }
    }
}
