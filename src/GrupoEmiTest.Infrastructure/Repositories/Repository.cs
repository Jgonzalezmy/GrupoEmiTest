using GrupoEmiTest.Domain.Interfaces;
using GrupoEmiTest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GrupoEmiTest.Infrastructure.Repositories;

/// <summary>
/// Generic EF Core implementation of <see cref="IRepository{T}"/>.
/// Provides standard CRUD operations for any entity type.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository. Must be a reference type.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    /// <summary>The shared database context used to perform all database operations.</summary>
    protected readonly GrupoEmiTestDBContext _context;

    /// <summary>
    /// The EF Core <see cref="DbSet{T}"/> that exposes the entity set
    /// for querying and persistence operations.
    /// </summary>
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initialises a new instance of <see cref="Repository{T}"/>.
    /// </summary>
    /// <param name="context">
    /// The database context shared within the unit of work.
    /// Must not be <see langword="null"/>.
    /// </param>
    public Repository(GrupoEmiTestDBContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Asynchronously retrieves all entities of type <typeparamref name="T"/> from the database.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to a read-only list containing
    /// all persisted entities, or an empty list if none exist.
    /// </returns>
    public async Task<IReadOnlyList<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    /// <summary>
    /// Asynchronously retrieves a single entity by its primary key.
    /// </summary>
    /// <param name="id">The integer primary key of the entity to locate.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to the matching entity,
    /// or <see langword="null"/> if no entity with the given <paramref name="id"/> exists.
    /// </returns>
    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    /// <summary>
    /// Asynchronously retrieves the first entity that satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">
    /// A lambda expression that defines the search condition.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to the first matching entity,
    /// or <see langword="null"/> if no entity satisfies the <paramref name="predicate"/>.
    /// </returns>
    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.FirstOrDefaultAsync(predicate);

    /// <summary>
    /// Asynchronously retrieves all entities that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">
    /// A lambda expression that defines the filter condition applied to the entity set.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to a read-only list of all entities
    /// matching the <paramref name="predicate"/>, or an empty list if none match.
    /// </returns>
    public async Task<IReadOnlyList<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    /// <summary>
    /// Asynchronously adds a new entity to the database context.
    /// The entity will be persisted on the next call to <c>SaveChangesAsync</c>.
    /// </summary>
    /// <param name="entity">The entity instance to add. Must not be <see langword="null"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous add operation.</returns>
    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    /// <summary>
    /// Marks an existing entity as modified in the database context.
    /// Changes will be persisted on the next call to <c>SaveChangesAsync</c>.
    /// </summary>
    /// <param name="entity">The entity instance with updated values. Must not be <see langword="null"/>.</param>
    public void Update(T entity)
        => _dbSet.Update(entity);

    /// <summary>
    /// Marks an entity for deletion from the database context.
    /// The entity will be removed on the next call to <c>SaveChangesAsync</c>.
    /// </summary>
    /// <param name="entity">The entity instance to delete. Must not be <see langword="null"/>.</param>
    public void Delete(T entity)
        => _dbSet.Remove(entity);

    /// <summary>
    /// Asynchronously determines whether any entity satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">
    /// A lambda expression that defines the condition to evaluate against the entity set.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that resolves to <see langword="true"/> if at least
    /// one entity matches the <paramref name="predicate"/>; otherwise <see langword="false"/>.
    /// </returns>
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AnyAsync(predicate);
}