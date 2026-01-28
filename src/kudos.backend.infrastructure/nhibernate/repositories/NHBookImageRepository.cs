using kudos.backend.domain.entities;
using kudos.backend.domain.interfaces.repositories;
using NHibernate;
using NHibernate.Linq;

namespace kudos.backend.infrastructure.nhibernate.repositories;

/// <summary>
/// NHibernate implementation of IBookImageRepository.
/// </summary>
public class NHBookImageRepository : NHRepository<BookImage, Guid>, IBookImageRepository
{
    public NHBookImageRepository(ISession session, IServiceProvider serviceProvider)
        : base(session, serviceProvider)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BookImage>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default)
    {
        return await _session.Query<BookImage>()
            .Where(bi => bi.BookId == bookId)
            .ToListAsync(ct);
    }
}
