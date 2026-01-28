using kudos.backend.domain.entities;

namespace kudos.backend.domain.interfaces.repositories;

/// <summary>
/// Defines a repository for managing BookImage entities.
/// </summary>
public interface IBookImageRepository : IRepository<BookImage, Guid>
{
    /// <summary>
    /// Gets all images for a specific book.
    /// </summary>
    /// <param name="bookId">The book's ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of images for the book.</returns>
    Task<IEnumerable<BookImage>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default);
}
