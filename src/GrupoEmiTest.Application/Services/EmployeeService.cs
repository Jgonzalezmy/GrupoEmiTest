using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Application.Extensions;
using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Errors;
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
    public async Task<Result<EmployeeResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id, cancellationToken);

        if (employee is null)
            return EmployeeErrors.NotFound();

        return employee.ToResponse();
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeResponse>> CreateAsync(EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employeeResult = Employee.Create(
            name: request.Name,
            position: request.CurrentPosition,
            salary: request.Salary,
            departmentId: request.DepartmentId);

        if (employeeResult.IsFailure)
            return employeeResult.Error;

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.Employees.AddAsync(employeeResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var historyResult = PositionHistory.Create(
                employeeId: employeeResult.Value.Id,
                position: request.CurrentPosition,
                startDate: DateTime.UtcNow);

            if (historyResult.IsFailure)
                return historyResult.Error;

            await _unitOfWork.PositionHistories.AddAsync(historyResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(employeeResult.Value.ToResponse());
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeResponse>> UpdateAsync(int id, EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id, cancellationToken);

        if (employee is null)
            return EmployeeErrors.NotFound(id);

        bool positionChanged = employee.CurrentPosition != request.CurrentPosition;

        var updateResult = employee.Update(
            name: request.Name,
            position: request.CurrentPosition,
            salary: request.Salary,
            departmentId: request.DepartmentId);

        if (updateResult.IsFailure)
            return updateResult.Error;

        if (!positionChanged)
        {
            await _unitOfWork.UpdateAndSaveAsync(employee, cancellationToken);
            return Result.Success(employee.ToResponse());
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var activeHistory = await _unitOfWork.PositionHistories.GetActiveByEmployeeIdAsync(id, cancellationToken);
            if (activeHistory is not null)
            {
                var closeResult = activeHistory.Close(DateTime.UtcNow);
                if (closeResult.IsFailure)
                    return closeResult.Error;

                _unitOfWork.PositionHistories.Update(activeHistory);
            }

            var newHistoryResult = PositionHistory.Create(
                employeeId: id,
                position: request.CurrentPosition,
                startDate: DateTime.UtcNow);

            if (newHistoryResult.IsFailure)
                return newHistoryResult.Error;

            await _unitOfWork.PositionHistories.AddAsync(newHistoryResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(employee.ToResponse());
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken);

        if (employee is null)
            return EmployeeErrors.NotFound(id);

        await _unitOfWork.DeleteAndSaveAsync(employee, cancellationToken);

        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<EmployeeResponse>>> GetByDepartmentWithProjectsAsync(
        int departmentId,
        PageRequest request,
        CancellationToken cancellationToken)
    {
        var page = await _unitOfWork.Employees
            .GetByDepartmentWithProjectsAsync(departmentId, request, cancellationToken);

        IReadOnlyList<EmployeeResponse> data = page.Data
            .Select(e => e.ToResponse())
            .ToList()
            .AsReadOnly();

        return Result.Success(new PagedResult<EmployeeResponse>(data, page.NextCursor, page.HasNextPage));
    }
}