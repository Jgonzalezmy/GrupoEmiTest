using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Interfaces;
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
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task AddAndSaveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
        await context.Set<T>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAndSaveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAndSaveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> operation, CancellationToken cancellationToken = default)
    {
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await operation();

            if (result.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return result;
            }

            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc/>
    public void Dispose() => context.Dispose();
}