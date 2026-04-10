using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Enums;
using GrupoEmiTest.Domain.Errors;
using GrupoEmiTest.Domain.Extensions;

namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents an employee in the company.
/// Contains business logic for computing the yearly bonus
/// based on whether the employee holds a managerial position.
/// </summary>
public class Employee
{
    /// <summary>Gets the unique identifier of the employee.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the full name of the employee.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Gets the employee's current position type.</summary>
    public PositionType CurrentPosition { get; private set; }

    /// <summary>Gets the employee's monthly salary.</summary>
    public decimal Salary { get; private set; }

    /// <summary>Gets the identifier of the department the employee belongs to.</summary>
    public int DepartmentId { get; private set; }


    /// <summary>Gets or sets the department this employee belongs to.</summary>
    public Department Department { get; set; } = null!;

    /// <summary>Gets or sets the history of positions this employee has held.</summary>
    public ICollection<PositionHistory> PositionHistories { get; set; } = new List<PositionHistory>();

    /// <summary>Gets or sets the projects this employee is assigned to.</summary>
    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();


    /// <summary>
    /// Calculates the employee's yearly bonus.
    /// Managers (TeamLead and above) receive 20% of their salary.
    /// Regular employees receive 10% of their salary.
    /// </summary>
    /// <returns>The calculated yearly bonus amount.</returns>
    public decimal CalculateYearlyBonus()
        => CurrentPosition.IsManagerPosition() ? Salary * 0.20m : Salary * 0.10m;

    /// <summary>
    /// Factory method that creates a new <see cref="Employee"/> after validating the input.
    /// </summary>
    /// <param name="name">Full name of the employee.</param>
    /// <param name="position">Initial position type.</param>
    /// <param name="salary">Monthly salary — must be greater than zero.</param>
    /// <param name="departmentId">Identifier of the department the employee belongs to.</param>
    /// <returns>
    /// A successful <see cref="Result{Employee}"/> with the new instance,
    /// or a failure result with a validation error.
    /// </returns>
    public static Result<Employee> Create(string name, PositionType position, decimal salary, int departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return EmployeeErrors.NameEmpty;

        if (salary <= 0)
            return EmployeeErrors.InvalidSalary;

        if (departmentId <= 0)
            return EmployeeErrors.InvalidDepartmentId;

        return new Employee
        {
            Name = name.Trim(),
            CurrentPosition = position,
            Salary = salary,
            DepartmentId = departmentId
        };
    }

    /// <summary>
    /// Updates the employee's mutable fields after validating the input.
    /// </summary>
    /// <param name="name">New full name — cannot be empty.</param>
    /// <param name="position">New position type.</param>
    /// <param name="salary">New monthly salary — must be greater than zero.</param>
    /// <param name="departmentId">New department identifier.</param>
    /// <returns>
    /// <see cref="Result.Success()"/> when the update is applied,
    /// or a failure result with a validation error.
    /// </returns>
    public Result Update(string name, PositionType position, decimal salary, int departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return EmployeeErrors.NameEmpty;

        if (salary <= 0)
            return EmployeeErrors.InvalidSalary;

        if (departmentId <= 0)
            return EmployeeErrors.InvalidDepartmentId;

        Name = name.Trim();
        CurrentPosition = position;
        Salary = salary;
        DepartmentId = departmentId;

        return Result.Success();
    }
}