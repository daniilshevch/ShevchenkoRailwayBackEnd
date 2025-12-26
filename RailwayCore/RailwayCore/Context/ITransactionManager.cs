using Microsoft.EntityFrameworkCore.Storage;

namespace RailwayCore.Context
{
    public interface ITransactionManager
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}