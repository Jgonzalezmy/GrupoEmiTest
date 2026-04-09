namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Join entity representing the many-to-many relationship between employees and projects.
/// </summary>
public class EmployeeProject
{
    /// <summary>Gets or sets the identifier of the employee.</summary>
    public int EmployeeId { get; set; }

    /// <summary>Gets or sets the identifier of the project.</summary>
    public int ProjectId { get; set; }

    /// <summary>Gets or sets the associated employee navigation property.</summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>Gets or sets the associated project navigation property.</summary>
    public Project Project { get; set; } = null!;
}