using kudos.backend.domain.entities;
using kudos.backend.domain.interfaces.repositories;
using NHibernate;
using NHibernate.Linq;

namespace kudos.backend.infrastructure.nhibernate.repositories;

/// <summary>
/// NHibernate implementation of IAuthorRepository.
/// </summary>
public class NHAuthorRepository : NHRepository<Author, Guid>, IAuthorRepository
{
    public NHAuthorRepository(ISession session, IServiceProvider serviceProvider)
        : base(session, serviceProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Author?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _session.Query<Author>()
            .FirstOrDefaultAsync(a => a.Name == name, ct);
    }
}
