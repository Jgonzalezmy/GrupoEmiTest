using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.Application.DTOs.Request;

/// <summary>
/// Represents a request record containing the historical position information of an employee or entity.
/// </summary>
/// <param name="Position">The code of the position held.</param>
/// <param name="StartDate">The date when the position started.</param>
/// <param name="EndDate">The date when the position ended, or <see langword="null"/> if the position is currently active.</param>
public record PositionHistoryRequest(
    PositionType Position,
    DateTime StartDate,
    DateTime? EndDate
);
