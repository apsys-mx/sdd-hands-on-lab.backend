using kudos.backend.domain.entities;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace kudos.backend.infrastructure.nhibernate.mappers;

/// <summary>
/// NHibernate mapping for the Book entity.
/// </summary>
public class BookMapper : ClassMapping<Book>
{
    private const string SchemaName = "public";

    public BookMapper()
    {
        Schema(SchemaName);
        Table("books");

        Id(x => x.Id, m =>
        {
            m.Column("id");
            m.Generator(Generators.Assigned);
            m.Type(NHibernateUtil.Guid);
        });

        Property(x => x.Title, m =>
        {
            m.Column("title");
            m.NotNullable(true);
            m.Type(NHibernateUtil.String);
        });

        Property(x => x.ISBN, m =>
        {
            m.Column("isbn");
            m.NotNullable(true);
            m.Unique(true);
            m.Type(NHibernateUtil.String);
        });

        Property(x => x.AuthorId, m =>
        {
            m.Column("author_id");
            m.NotNullable(true);
            m.Type(NHibernateUtil.Guid);
        });

        Property(x => x.Category, m =>
        {
            m.Column("category");
            m.NotNullable(false);
            m.Type(NHibernateUtil.String);
        });

        Property(x => x.CoverImageUrl, m =>
        {
            m.Column("cover_image_url");
            m.NotNullable(false);
            m.Type(NHibernateUtil.String);
        });

        Property(x => x.CreationDate, m =>
        {
            m.Column("creation_date");
            m.NotNullable(true);
            m.Type(NHibernateUtil.DateTime);
        });
    }
}
