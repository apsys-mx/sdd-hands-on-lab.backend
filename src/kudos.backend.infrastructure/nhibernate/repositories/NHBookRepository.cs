using kudos.backend.domain.entities;
using kudos.backend.domain.interfaces.repositories;
using NHibernate;
using NHibernate.Linq;

namespace kudos.backend.infrastructure.nhibernate.repositories;

/// <summary>
/// NHibernate implementation of IBookRepository.
/// </summary>
public class NHBookRepository : NHRepository<Book, Guid>, IBookRepository
{
    public NHBookRepository(ISession session, IServiceProvider serviceProvider)
        : base(session, serviceProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default)
    {
        return await _session.Query<Book>()
            .FirstOrDefaultAsync(b => b.ISBN == isbn, ct);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsIsbnAsync(string isbn, CancellationToken ct = default)
    {
        return await _session.Query<Book>()
            .AnyAsync(b => b.ISBN == isbn, ct);
    }
}
