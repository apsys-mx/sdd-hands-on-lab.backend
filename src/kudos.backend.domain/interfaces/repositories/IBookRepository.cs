using kudos.backend.domain.entities;

namespace kudos.backend.domain.interfaces.repositories;

/// <summary>
/// Defines a repository for managing Book entities.
/// </summary>
public interface IBookRepository : IRepository<Book, Guid>
{
    /// <summary>
    /// Gets a book by its ISBN.
    /// </summary>
    /// <param name="isbn">The book's ISBN.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The book if found, null otherwise.</returns>
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default);

    /// <summary>
    /// Checks if a book with the specified ISBN exists.
    /// </summary>
    /// <param name="isbn">The ISBN to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if a book with the ISBN exists, false otherwise.</returns>
    Task<bool> ExistsIsbnAsync(string isbn, CancellationToken ct = default);
}
