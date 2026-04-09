using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;
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
    public async Task<Result<IReadOnlyList<EmployeeResponse>>> GetAllAsync()
    {
        var employees = await _unitOfWork.Employees.GetAllWithDepartmentAsync();

        var response = employees
            .Select(e => new EmployeeResponse
            {
                Id = e.Id,
                Name = e.Name,
                CurrentPosition = e.CurrentPosition,
                DepartmentName = e.Department.Name,
                Salary = e.Salary,
                YearlyBonus = e.CalculateYearlyBonus()
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<EmployeeResponse>>.Success(response);
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeDetailResponse>> GetByIdAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id);

        if (employee is null)
            return Result<EmployeeDetailResponse>.Failure($"Employee with ID {id} was not found.");

        return Result<EmployeeDetailResponse>.Success(MapToDetailResponse(employee));
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeDetailResponse>> CreateAsync(CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<EmployeeDetailResponse>.Failure("Employee name is required.");

        if (request.Salary <= 0)
            return Result<EmployeeDetailResponse>.Failure("Salary must be greater than zero.");

        var employee = new Employee
        {
            Name = request.Name,
            CurrentPosition = request.CurrentPosition,
            Salary = request.Salary,
            DepartmentId = request.DepartmentId
        };

        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        // Reload with related data for the response.
        var created = await _unitOfWork.Employees.GetByIdWithDetailsAsync(employee.Id);
        return Result<EmployeeDetailResponse>.Success(MapToDetailResponse(created!));
    }

    /// <inheritdoc/>
    public async Task<Result<EmployeeDetailResponse>> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee is null)
            return Result<EmployeeDetailResponse>.Failure($"Employee with ID {id} was not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<EmployeeDetailResponse>.Failure("Employee name is required.");

        if (request.Salary <= 0)
            return Result<EmployeeDetailResponse>.Failure("Salary must be greater than zero.");

        employee.Name = request.Name;
        employee.CurrentPosition = request.CurrentPosition;
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Employees.GetByIdWithDetailsAsync(employee.Id);
        return Result<EmployeeDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee is null)
            return Result.Failure($"Employee with ID {id} was not found.");

        _unitOfWork.Employees.Delete(employee);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Maps an <see cref="Employee"/> entity (with related data loaded) to a
    /// <see cref="EmployeeDetailResponse"/> DTO.
    /// </summary>
    /// <param name="employee">The entity to map.</param>
    /// <returns>The populated DTO.</returns>
    private static EmployeeDetailResponse MapToDetailResponse(Employee employee) => new()
    {
        Id = employee.Id,
        Name = employee.Name,
        CurrentPosition = employee.CurrentPosition,
        Salary = employee.Salary,
        YearlyBonus = employee.CalculateYearlyBonus(),
        DepartmentId = employee.DepartmentId,
        DepartmentName = employee.Department?.Name ?? string.Empty,
        PositionHistories = employee.PositionHistories
            .Select(ph => new PositionHistoryResponse
            {
                Position = ph.Position,
                StartDate = ph.StartDate,
                EndDate = ph.EndDate
            })
            .ToList(),
        Projects = employee.EmployeeProjects
            .Select(ep => ep.Project.Name)
            .ToList()
    };
}