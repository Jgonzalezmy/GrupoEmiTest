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
    /// Retrieves all employees with their department information.
    /// </summary>
    /// <returns>A result containing a read-only list of <see cref="EmployeeResponse"/>.</returns>
    Task<Result<IReadOnlyList<EmployeeResponse>>> GetAllAsync();

    /// <summary>
    /// Retrieves a single employee by ID including position history and projects.
    /// </summary>
    /// <param name="id">The employee's primary key.</param>
    /// <returns>
    /// A result containing <see cref="EmployeeResponse"/> if found,
    /// or a failure if no employee with that ID exists.
    /// </returns>
    Task<Result<EmployeeResponse>> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new employee from the provided request data.
    /// </summary>
    /// <param name="request">The data required to create the employee.</param>
    /// <returns>
    /// A result containing the created <see cref="EmployeeResponse"/>,
    /// or a failure if validation fails.
    /// </returns>
    Task<Result<EmployeeResponse>> CreateAsync(EmployeeRequest request);

    /// <summary>
    /// Updates an existing employee with the provided request data.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="request">The updated employee data.</param>
    /// <returns>
    /// A result containing the updated <see cref="EmployeeResponse"/>,
    /// or a failure if the employee is not found.
    /// </returns>
    Task<Result<EmployeeResponse>> UpdateAsync(int id, EmployeeRequest request);

    /// <summary>
    /// Deletes the employee with the given ID.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>A successful result, or a failure if the employee is not found.</returns>
    Task<Result> DeleteAsync(int id);
}