using kudos.backend.domain.daos;
using kudos.backend.domain.interfaces.repositories;
using NHibernate;

namespace kudos.backend.infrastructure.nhibernate.repositories;

/// <summary>
/// NHibernate implementation of IBookDaoRepository.
/// </summary>
public class NHBookDaoRepository : NHReadOnlyRepository<BookDao, Guid>, IBookDaoRepository
{
    public NHBookDaoRepository(ISession session)
        : base(session)
    {
    }
}
