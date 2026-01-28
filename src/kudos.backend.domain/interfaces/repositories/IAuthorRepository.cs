using kudos.backend.domain.entities;

namespace kudos.backend.domain.interfaces.repositories;

/// <summary>
/// Defines a repository for managing Author entities.
/// </summary>
public interface IAuthorRepository : IRepository<Author, Guid>
{
    /// <summary>
    /// Gets an author by their name.
    /// </summary>
    /// <param name="name">The author's name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The author if found, null otherwise.</returns>
    Task<Author?> GetByNameAsync(string name, CancellationToken ct = default);
}
