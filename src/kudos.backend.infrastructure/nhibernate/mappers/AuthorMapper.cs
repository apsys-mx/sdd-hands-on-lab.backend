using kudos.backend.domain.entities;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace kudos.backend.infrastructure.nhibernate.mappers;

/// <summary>
/// NHibernate mapping for the Author entity.
/// </summary>
public class AuthorMapper : ClassMapping<Author>
{
    private const string SchemaName = "public";

    public AuthorMapper()
    {
        Schema(SchemaName);
        Table("authors");

        Id(x => x.Id, m =>
        {
            m.Column("id");
            m.Generator(Generators.Assigned);
            m.Type(NHibernateUtil.Guid);
        });

        Property(x => x.Name, m =>
        {
            m.Column("name");
            m.NotNullable(true);
            m.Type(NHibernateUtil.String);
        });

        Property(x => x.Country, m =>
        {
            m.Column("country");
            m.NotNullable(false);
            m.Type(NHibernateUtil.String);
        });

        Property(x => x.BirthDate, m =>
        {
            m.Column("birth_date");
            m.NotNullable(false);
            m.Type(NHibernateUtil.DateTime);
        });

        Property(x => x.CreationDate, m =>
        {
            m.Column("creation_date");
            m.NotNullable(true);
            m.Type(NHibernateUtil.DateTime);
        });
    }
}
