using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.Domain.Interfaces;

/// <summary>
/// Extends <see cref="IRepository{T}"/> with employee-specific query operations.
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>
    /// Retrieves an employee by ID including their department, position history, and projects.
    /// </summary>
    /// <param name="id">The employee's primary key.</param>
    /// <returns>A task that resolves to the employee with all related data, or <c>null</c>.</returns>
    Task<Employee?> GetByIdWithDetailsAsync(int id);

    /// <summary>
    /// Retrieves a page of employees including all related data using keyset pagination.
    /// Filters by <c>Id &gt; LastId</c> when a cursor is present, avoiding <c>OFFSET</c> entirely.
    /// </summary>
    /// <param name="request">The pagination parameters (page size and optional last-seen ID cursor).</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that resolves to a <see cref="PagedResult{T}"/> containing the employees for the
    /// current page and the cursor needed to fetch the next one.
    /// </returns>
    Task<PagedResult<Employee>> GetAllWithDetailsAsync(PageRequest request, CancellationToken cancellationToken);
}
