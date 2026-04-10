using GrupoEmiTest.Domain.Common;
using System.Linq.Expressions;

namespace GrupoEmiTest.Domain.Interfaces;

/// <summary>
/// Generic repository interface that defines the standard data-access operations
/// for any entity type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A task that resolves to a read-only collection of all entities.</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single entity by its integer primary key.
    /// </summary>
    /// <param name="id">The primary key value.</param>
    /// <returns>A task that resolves to the entity, or <c>null</c> if not found.</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity that satisfies the given predicate, or <c>null</c> if none found.
    /// </summary>
    /// <param name="predicate">A filter expression.</param>
    /// <returns>A task that resolves to the matching entity or <c>null</c>.</returns>
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities that satisfy the given predicate.
    /// </summary>
    /// <param name="predicate">A filter expression.</param>
    /// <returns>A task that resolves to a read-only collection of matching entities.</returns>
    Task<IReadOnlyList<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the repository.
    /// Changes are not persisted until <c>IUnitOfWork.SaveChangesAsync</c> is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Propagated when the caller is cancelled.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing entity as modified.
    /// Changes are not persisted until <c>IUnitOfWork.SaveChangesAsync</c> is called.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Marks an entity for deletion.
    /// Changes are not persisted until <c>IUnitOfWork.SaveChangesAsync</c> is called.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(T entity);

    /// <summary>
    /// Checks whether any entity satisfies the given predicate.
    /// </summary>
    /// <param name="predicate">A filter expression.</param>
    /// <returns><c>true</c> if at least one entity matches; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a keyset-paginated page of entities ordered and filtered by
    /// <paramref name="keySelector"/>.
    /// Avoids <c>OFFSET/SKIP</c> — resolves in O(1) via primary-key index seek.
    /// </summary>
    /// <param name="request">
    /// Pagination parameters: page size and the cursor (last seen key) from the previous page.
    /// Pass <c>null</c> as <see cref="PageRequest.LastId"/> for the first page.
    /// </param>
    /// <param name="keySelector">
    /// Expression that selects the integer key used for ordering and cursor filtering
    /// (typically the primary key, e.g. <c>e => e.Id</c>).
    /// </param>
    /// <param name="cancellationToken">Propagated when the caller is cancelled.</param>
    /// <returns>
    /// A <see cref="PagedResult{T}"/> containing the page data, the next cursor value,
    /// and a flag indicating whether more pages exist.
    /// </returns>
    Task<PagedResult<T>> GetPageAsync(
        PageRequest request,
        Expression<Func<T, int>> keySelector,
        CancellationToken cancellationToken = default);
}