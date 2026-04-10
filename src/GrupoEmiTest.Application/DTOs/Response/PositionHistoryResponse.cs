using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.Application.DTOs.Response;

/// <summary>
/// Represents the historical position record of an employee.
/// </summary>
/// <param name="Position">The <see cref="PositionType"/> enum value that identifies the position held.</param>
/// <param name="PositionDescription">The human-readable display name of the position.</param>
/// <param name="StartDate">The date on which the employee started this position.</param>
/// <param name="EndDate">
/// The date on which the employee left this position,
/// or <see langword="null"/> if this is the employee's current position.
/// </param>
public record PositionHistoryResponse(
    PositionType Position,
    string PositionDescription,
    DateTime StartDate,
    DateTime? EndDate
);
