using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.Domain.Interfaces;

/// <summary>
/// Coordinates the work of multiple repositories by sharing a single database context
/// and persisting all changes in a single transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Gets the employee repository.</summary>
    //IEmployeeRepository Employees { get; }

    /// <summary>Gets the generic repository for <see cref="ApplicationUser"/>.</summary>
    IRepository<ApplicationUser> Users { get; }

    /// <summary>
    /// Persists all pending changes tracked by the underlying context to the database.
    /// </summary>
    /// <returns>A task that resolves to the number of state entries written.</returns>
    Task<int> SaveChangesAsync();
}