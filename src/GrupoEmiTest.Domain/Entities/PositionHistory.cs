using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Enums;
using GrupoEmiTest.Domain.Errors;

namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Tracks the history of positions an employee has held during their tenure.
/// </summary>
public class PositionHistory
{
    /// <summary>Gets the unique identifier of the position-history record.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the identifier of the employee this record belongs to.</summary>
    public int EmployeeId { get; private set; }

    /// <summary>Gets the name of the position held.</summary>
    public PositionType Position { get; private set; }

    /// <summary>Gets the date on which the employee started this position.</summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Gets the date on which the employee left this position.
    /// A <c>null</c> value indicates that this is the employee's current position.
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>Gets or sets the associated employee navigation property.</summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>
    /// Factory method that creates a new <see cref="PositionHistory"/> record after validating the input.
    /// </summary>
    /// <param name="employeeId">The identifier of the employee this record belongs to.</param>
    /// <param name="position">The <see cref="PositionType"/> the employee is starting.</param>
    /// <param name="startDate">The date the employee started this position.</param>
    /// <param name="endDate">
    /// The date the employee left this position, or <see langword="null"/> for an active position.
    /// When provided, must be strictly after <paramref name="startDate"/>.
    /// </param>
    /// <returns>
    /// A successful <see cref="Result{PositionHistory}"/> with the new instance,
    /// or a failure result with a validation error.
    /// </returns>
    public static Result<PositionHistory> Create(
        int employeeId,
        PositionType position,
        DateTime startDate,
        DateTime? endDate = null)
    {
        if (endDate.HasValue && endDate.Value <= startDate)
            return PositionHistoryErrors.InvalidDateRange;

        return new PositionHistory
        {
            EmployeeId = employeeId,
            Position   = position,
            StartDate  = startDate,
            EndDate    = endDate
        };
    }

    /// <summary>
    /// Closes this position record by setting its end date.
    /// </summary>
    /// <param name="endDate">
    /// The date on which the employee left this position.
    /// Must be on or after <see cref="StartDate"/>.
    /// </param>
    /// <returns>
    /// <see cref="Result.Success()"/> when the record is closed,
    /// or a failure result if the record is already closed or the date is invalid.
    /// </returns>
    public Result Close(DateTime endDate)
    {
        if (EndDate.HasValue)
            return PositionHistoryErrors.AlreadyClosed;

        if (endDate < StartDate)
            return PositionHistoryErrors.InvalidDateRange;

        EndDate = endDate;
        return Result.Success();
    }
}