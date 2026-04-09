using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Application.Extensions;
using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Enums;
using GrupoEmiTest.Domain.Interfaces;

namespace GrupoEmiTest.Application.Services;

/// <summary>
/// Implements the business-logic operations for the Employee entity.
/// Uses the Unit of Work and Repository patterns for data access.
/// All outcomes are returned via the Result pattern — no exceptions for business failures.
/// </summary>
public sealed class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initialises a new instance of <see cref="EmployeeService"/>.
    /// </summary>
    /// <param name="unitOfWork">The unit of work that coordinates repository access.</param>
    public EmployeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<EmployeeResponse>>> GetAllAsync(
        PageRequest request, CancellationToken cancellationToken)
    {
        var page = await _unitOfWork.Employees.GetAllWithDetailsAsync(request, cancellationToken);

        IReadOnlyList<EmployeeResponse> data = page.Data
            .Select(e => e.ToResponse())
            .ToList()
            .AsReadOnly();

        return Result.Success(new PagedResult<EmployeeResponse>(data, page.NextCursor, page.HasNextPage));
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeResponse>> GetByIdAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id);

        if (employee is null)
            return Result.Failure<EmployeeResponse>(
                new Error("Employee.NotFound", $"Employee with ID {id} was not found.", ErrorType.NotFound));

        return Result.Success(employee.ToResponse());
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeResponse>> CreateAsync(EmployeeRequest request)
    {
        var employee = new Employee
        {
            Name = request.Name,
            CurrentPosition = request.CurrentPosition,
            Salary = request.Salary,
            DepartmentId = request.DepartmentId
        };

        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(employee.ToResponse());
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeResponse>> UpdateAsync(int id, EmployeeRequest request)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id);

        if (employee is null)
            return Result.Failure<EmployeeResponse>(
                new Error("Employee.NotFound", $"Employee with ID {id} was not found.", ErrorType.NotFound));

        employee.Name = request.Name;
        employee.CurrentPosition = request.CurrentPosition;
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(employee.ToResponse());
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee is null)
            return Result.Failure(
                new Error("Employee.NotFound", $"Employee with ID {id} was not found.", ErrorType.NotFound));

        _unitOfWork.Employees.Delete(employee);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}