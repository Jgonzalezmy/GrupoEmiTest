namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents an organisational department within the company.
/// </summary>
public class Department
{
    /// <summary>Gets or sets the unique identifier of the department.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the name of the department.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the collection of employees belonging to this department.</summary>
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}