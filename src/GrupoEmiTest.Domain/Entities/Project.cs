namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents a company project that employees can be assigned to.
/// </summary>
public class Project
{
    /// <summary>Gets or sets the unique identifier of the project.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the name of the project.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional description of the project.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the collection of employee-project assignments.</summary>
    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
}