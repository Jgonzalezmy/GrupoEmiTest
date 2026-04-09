namespace GrupoEmiTest.Application.DTOs.Response;

/// <summary>
/// Represents a response record containing the historical position information of an employee or entity.
/// </summary>
/// <param name="Position">The name or title of the position held.</param>
/// <param name="StartDate">The date when the position started.</param>
/// <param name="EndDate">The date when the position ended, or <see langword="null"/> if the position is currently active.</param>
public record PositionHistoryResponse(
    string Position,
    DateTime StartDate,
    DateTime? EndDate
);
