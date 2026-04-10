using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.Domain.Interfaces;

/// <summary>
/// Coordinates the work of multiple repositories by sharing a single database context
/// and persisting all changes in a single transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Gets the employee repository.</summary>
    IEmployeeRepository Employees { get; }

    /// <summary>Gets the position-history repository.</summary>
    IPositionHistoryRepository PositionHistories { get; }

    /// <summary>Gets the generic repository for <see cref="ApplicationUser"/>.</summary>
    IRepository<ApplicationUser> Users { get; }

    /// <summary>
    /// Flushes all pending changes to the database.
    /// </summary>
    /// <returns>A task that resolves to the number of state entries written.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Executes <paramref name="operation"/> inside a database transaction.
    /// Commits on success and rolls back automatically on failure or exception,
    /// keeping transaction management out of the application layer.
    /// </summary>
    /// <typeparam name="T">The value type returned by the operation.</typeparam>
    /// <param name="operation">The transactional work to execute.</param>
    /// <returns>
    /// The <see cref="Result{T}"/> produced by <paramref name="operation"/>,
    /// or a rolled-back failure result.
    /// </returns>
    Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> operation);
}