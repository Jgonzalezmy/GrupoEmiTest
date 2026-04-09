namespace GrupoEmiTest.Domain.Common;

/// <summary>
/// Encapsulates the parameters for a keyset (seek) pagination query.
/// </summary>
/// <param name="PageSize">The maximum number of items to return per page.</param>
/// <param name="LastId">
/// The primary key of the last item seen in the previous page.
/// Pass <see langword="null"/> to request the first page.
/// </param>
public record PageRequest(int PageSize, int? LastId);
