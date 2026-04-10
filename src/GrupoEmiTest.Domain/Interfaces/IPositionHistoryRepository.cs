using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.Domain.Interfaces;

/// <summary>
/// Extends <see cref="IRepository{T}"/> with position-history-specific query operations.
/// </summary>
public interface IPositionHistoryRepository : IRepository<PositionHistory>
{
    /// <summary>
    /// Retrieves all position-history records for the given employee, ordered by start date ascending.
    /// </summary>
    /// <param name="employeeId">The employee's primary key.</param>
    /// <returns>A task that resolves to a read-only list of position-history records.</returns>
    Task<IReadOnlyList<PositionHistory>> GetByEmployeeIdAsync(int employeeId);

    /// <summary>
    /// Retrieves the currently active position-history record for the given employee
    /// (the one with no <c>EndDate</c>), or <c>null</c> if none exists.
    /// </summary>
    /// <param name="employeeId">The employee's primary key.</param>
    /// <returns>A task that resolves to the active record, or <c>null</c>.</returns>
    Task<PositionHistory?> GetActiveByEmployeeIdAsync(int employeeId);
}
