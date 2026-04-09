namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Tracks the history of positions an employee has held during their tenure.
/// </summary>
public class PositionHistory
{
    /// <summary>Gets or sets the unique identifier of the position-history record.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the identifier of the employee this record belongs to.</summary>
    public int EmployeeId { get; set; }

    /// <summary>Gets or sets the name of the position held.</summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>Gets or sets the date on which the employee started this position.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date on which the employee left this position.
    /// A <c>null</c> value indicates that this is the employee's current position.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>Gets or sets the associated employee navigation property.</summary>
    public Employee Employee { get; set; } = null!;
}