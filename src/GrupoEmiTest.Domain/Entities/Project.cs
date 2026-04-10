namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents a company project that employees can be assigned to.
/// </summary>
public class Project
{
    /// <summary>Gets the unique identifier of the project.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the name of the project.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Gets an optional description of the project.</summary>
    public string? Description { get; private set; }

    /// <summary>Gets or sets the collection of employee-project assignments.</summary>
    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();

    /// <summary>
    /// Factory method that creates a new <see cref="Project"/> with the specified name and optional description.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="description">An optional description of the project.</param>
    /// <returns>A new <see cref="Project"/> instance.</returns>
    public static Project Create(string name, string? description = null) =>
        new() { Name = name, Description = description };
}