using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrupoEmiTest.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPositionHistoryRepository"/>.
/// </summary>
public sealed class PositionHistoryRepository : Repository<PositionHistory>, IPositionHistoryRepository
{
    /// <summary>
    /// Initialises a new instance of <see cref="PositionHistoryRepository"/>.
    /// </summary>
    /// <param name="context">The shared database context.</param>
    public PositionHistoryRepository(GrupoEmiTestDBContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<PositionHistory>> GetByEmployeeIdAsync(int employeeId)
        => await _dbSet
            .Where(ph => ph.EmployeeId == employeeId)
            .OrderBy(ph => ph.StartDate)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<PositionHistory?> GetActiveByEmployeeIdAsync(int employeeId)
        => await _dbSet
            .FirstOrDefaultAsync(ph => ph.EmployeeId == employeeId && ph.EndDate == null);
}
