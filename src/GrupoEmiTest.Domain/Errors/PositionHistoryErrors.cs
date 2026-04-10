using GrupoEmiTest.Domain.Common;

namespace GrupoEmiTest.Domain.Errors;

/// <summary>
/// Domain errors related to the <see cref="Entities.PositionHistory"/> entity.
/// </summary>
public static class PositionHistoryErrors
{
    /// <summary>No position-history record found with the given identifier.</summary>
    public static Error NotFound(int id) =>
        Error.NotFound("PositionHistory.NotFound", $"Position history with ID {id} was not found.");

    /// <summary>End date must be strictly after start date.</summary>
    public static readonly Error InvalidDateRange =
        Error.Validation("PositionHistory.InvalidDateRange", "End date must be after start date.");

    /// <summary>The position history record is already closed.</summary>
    public static readonly Error AlreadyClosed =
        Error.Conflict("PositionHistory.AlreadyClosed", "This position history record is already closed.");
}
