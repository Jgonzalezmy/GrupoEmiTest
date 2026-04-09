using GrupoEmiTest.Domain.Enums;
using GrupoEmiTest.Domain.Extensions;

namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents an employee in the company.
/// Contains business logic for computing the yearly bonus
/// based on whether the employee holds a managerial position.
/// </summary>
public class Employee
{
    /// <summary>Gets or sets the unique identifier of the employee.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the full name of the employee.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the employee's current position type.</summary>
    public PositionType CurrentPosition { get; set; }

    /// <summary>Gets or sets the employee's monthly salary.</summary>
    public decimal Salary { get; set; }

    /// <summary>Gets or sets the identifier of the department the employee belongs to.</summary>
    public int DepartmentId { get; set; }


    // Navigation properties 
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
}