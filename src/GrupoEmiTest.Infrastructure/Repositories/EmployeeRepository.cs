using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrupoEmiTest.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IEmployeeRepository"/>.
/// Extends <see cref="Repository{T}"/> with employee-specific queries that
/// eager-load related data as required by the application layer.
/// </summary>
public sealed class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    /// <summary>
    /// Initialises a new instance of <see cref="EmployeeRepository"/>.
    /// </summary>
    /// <param name="context">The shared database context.</param>
    public EmployeeRepository(GrupoEmiTestDBContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<Employee?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.PositionHistories)
            .Include(e => e.EmployeeProjects)
                .ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<PagedResult<Employee>> GetAllWithDetailsAsync(
        PageRequest request,
        CancellationToken cancellationToken)
    {
        var items = await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(e => e.Department)
            .Include(e => e.PositionHistories)
            .Include(e => e.EmployeeProjects)
                .ThenInclude(ep => ep.Project)
            .Where(e => request.LastId == null || e.Id > request.LastId)
            .OrderBy(e => e.Id)
            .Take(request.PageSize + 1)
            .ToListAsync(cancellationToken);

        var hasNextPage = items.Count > request.PageSize;

        if (hasNextPage)
            items.RemoveAt(items.Count - 1);

        var nextCursor = hasNextPage ? items[^1].Id : (int?)null;

        return new PagedResult<Employee>(items, nextCursor, hasNextPage);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<Employee>> GetByDepartmentWithProjectsAsync(
        int departmentId,
        PageRequest request,
        CancellationToken cancellationToken)
    {
        var items = await _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(e => e.Department)
            .Include(e => e.PositionHistories)
            .Include(e => e.EmployeeProjects)
                .ThenInclude(ep => ep.Project)
            .Where(e => e.DepartmentId == departmentId
                     && e.EmployeeProjects.Any()
                     && (request.LastId == null || e.Id > request.LastId))
            .OrderBy(e => e.Id)
            .Take(request.PageSize + 1)
            .ToListAsync(cancellationToken);

        var hasNextPage = items.Count > request.PageSize;

        if (hasNextPage)
            items.RemoveAt(items.Count - 1);

        var nextCursor = hasNextPage ? items[^1].Id : (int?)null;

        return new PagedResult<Employee>(items, nextCursor, hasNextPage);
    }
}
