using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Interfaces;
using GrupoEmiTest.Infrastructure.Data;
using GrupoEmiTest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace GrupoEmiTest.Infrastructure.UnitOfWork;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// All repositories share the same <see cref="GrupoEmiTestDBContext"/> instance
/// so that changes are tracked together and persisted atomically.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly GrupoEmiTestDBContext _context;
    private IDbContextTransaction? _transaction;
    private IEmployeeRepository? _employees;
    private IRepository<ApplicationUser>? _users;

    /// <summary>
    /// Initialises a new instance of <see cref="UnitOfWork"/>.
    /// </summary>
    /// <param name="context">The database context to coordinate.</param>
    public UnitOfWork(GrupoEmiTestDBContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    /// <remarks>Repository instances are created lazily on first access.</remarks>
    public IEmployeeRepository Employees
        => _employees ??= new EmployeeRepository(_context);

    /// <inheritdoc/>
    public IRepository<ApplicationUser> Users
        => _users ??= new Repository<ApplicationUser>(_context);

    /// <inheritdoc/>
    public async Task BeginTransactionAsync()
        => _transaction = await _context.Database.BeginTransactionAsync();

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    /// <inheritdoc/>
    public async Task CommitAsync()
    {
        ArgumentNullException.ThrowIfNull(_transaction);
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <inheritdoc/>
    public async Task RollbackAsync()
    {
        if (_transaction is null) 
            return;

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}