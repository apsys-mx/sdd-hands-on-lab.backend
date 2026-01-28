using kudos.backend.domain.daos;

namespace kudos.backend.domain.interfaces.repositories;

/// <summary>
/// Defines a read-only repository for managing BookDao entities.
/// </summary>
public interface IBookDaoRepository : IReadOnlyRepository<BookDao, Guid>
{
    // Inherits from IReadOnlyRepository:
    // - GetAsync(Guid id, CancellationToken ct)
    // - GetAsync(CancellationToken ct)
    // - GetAsync(Expression<Func<BookDao, bool>> query, CancellationToken ct)
    // - CountAsync(CancellationToken ct)
    // - GetManyAndCountAsync(string? query, string defaultSorting, CancellationToken ct)
}
