using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Errors;

namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents an organisational department within the company.
/// </summary>
public class Department
{
    /// <summary>Gets the unique identifier of the department.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the name of the department.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Gets or sets the collection of employees belonging to this department.</summary>
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    /// <summary>
    /// Factory method that creates a new <see cref="Department"/> with the specified name.
    /// </summary>
    /// <param name="name">The name of the department.</param>
    /// <returns>A new <see cref="Department"/> instance.</returns>
    public static Result<Department> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return DepartmentErrors.InvalidName;
        return new Department { Name = name };
    }
}