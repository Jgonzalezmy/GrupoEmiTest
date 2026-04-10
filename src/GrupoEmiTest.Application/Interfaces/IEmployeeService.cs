using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Domain.Common;

namespace GrupoEmiTest.Application.Interfaces;

/// <summary>
/// Defines the business-logic operations available for the Employee entity.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Returns a keyset-paginated page of employees with full related data.
    /// </summary>
    /// <param name="request">The pagination parameters (page size and optional last-seen ID cursor).</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing a <see cref="PagedResult{T}"/> with the current page and next-cursor.</returns>
    Task<Result<PagedResult<EmployeeResponse>>> GetAllAsync(PageRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single employee by ID including department, position history, and projects.
    /// </summary>
    /// <param name="id">The employee's primary key.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the <see cref="EmployeeResponse"/>, or a not-found failure.</returns>
    Task<Result<EmployeeResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new employee from the provided request data.
    /// </summary>
    /// <param name="request">The data required to create the employee.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the newly created <see cref="EmployeeResponse"/>.</returns>
    Task<Result<EmployeeResponse>> CreateAsync(EmployeeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing employee identified by <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The employee's primary key.</param>
    /// <param name="request">The updated employee data.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the updated <see cref="EmployeeResponse"/>, or a not-found failure.</returns>
    Task<Result<EmployeeResponse>> UpdateAsync(int id, EmployeeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an employee by ID.
    /// </summary>
    /// <param name="id">The employee's primary key.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result indicating success, or a not-found failure.</returns>
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a keyset-paginated page of employees that belong to the specified department
    /// and are assigned to at least one project.
    /// </summary>
    /// <param name="departmentId">The identifier of the department to filter by.</param>
    /// <param name="request">The pagination parameters (page size and optional last-seen ID cursor).</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A result containing a <see cref="PagedResult{T}"/> with the current page and next-cursor,
    /// or an empty page when no employees satisfy both conditions.
    /// </returns>
    Task<Result<PagedResult<EmployeeResponse>>> GetByDepartmentWithProjectsAsync(int departmentId, PageRequest request, CancellationToken cancellationToken);
}
