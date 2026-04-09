namespace GrupoEmiTest.Domain.Common;

/// <summary>
/// Represents a page of results produced by a keyset (seek) pagination query.
/// Avoids <c>OFFSET/SKIP</c> entirely — always resolves in O(1) via primary-key index seek.
/// </summary>
/// <typeparam name="T">The type of items contained in the page.</typeparam>
/// <param name="Data">The items on the current page.</param>
/// <param name="NextCursor">
/// The primary key of the last item on this page, used as the <c>LastId</c> for the next request.
/// <see langword="null"/> when <see cref="HasNextPage"/> is <see langword="false"/>.
/// </param>
/// <param name="HasNextPage">
/// <see langword="true"/> if more items exist beyond the current page; otherwise <see langword="false"/>.
/// </param>
public record PagedResult<T>(
    IReadOnlyList<T> Data,
    int? NextCursor,
    bool HasNextPage);
