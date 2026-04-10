using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Interfaces;
using GrupoEmiTest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace GrupoEmiTest.Infrastructure.UnitOfWork;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// All repositories share the same <see cref="GrupoEmiTestDBContext"/> instance
/// so that changes are tracked together and persisted atomically.
/// </summary>
public sealed class UnitOfWork(
    GrupoEmiTestDBContext context,
    IEmployeeRepository employees,
    IPositionHistoryRepository positionHistories,
    IRepository<ApplicationUser> users) : IUnitOfWork
{
    /// <inheritdoc/>
    public IEmployeeRepository Employees { get; } = employees;

    /// <inheritdoc/>
    public IPositionHistoryRepository PositionHistories { get; } = positionHistories;

    /// <inheritdoc/>
    public IRepository<ApplicationUser> Users { get; } = users;

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync()
        => await context.SaveChangesAsync();

    /// <inheritdoc/>
    public async Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> operation)
    {
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await operation();

            if (result.IsFailure)
            {
                await transaction.RollbackAsync();
                return result;
            }

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public void Dispose() => context.Dispose();
}