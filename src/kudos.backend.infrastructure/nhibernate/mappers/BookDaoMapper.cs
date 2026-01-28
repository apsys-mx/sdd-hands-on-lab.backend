using kudos.backend.domain.daos;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace kudos.backend.infrastructure.nhibernate.mappers;

/// <summary>
/// NHibernate mapping for the BookDao (read-only view).
/// </summary>
public class BookDaoMapper : ClassMapping<BookDao>
{
    private const string SchemaName = "public";

    public BookDaoMapper()
    {
        Schema(SchemaName);
        Table("books_view");
        Mutable(false);  // Prevent INSERT/UPDATE operations

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
            m.Type(NHibernateUtil.String);
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

        Property(x => x.AuthorId, m =>
        {
            m.Column("author_id");
            m.NotNullable(true);
            m.Type(NHibernateUtil.Guid);
        });

        Property(x => x.AuthorName, m =>
        {
            m.Column("author_name");
            m.NotNullable(true);
            m.Type(NHibernateUtil.String);
        });

        Property(x => x.CreationDate, m =>
        {
            m.Column("creation_date");
            m.NotNullable(true);
            m.Type(NHibernateUtil.DateTime);
        });

        Property(x => x.SearchAll, m =>
        {
            m.Column("search_all");
            m.NotNullable(true);
            m.Type(NHibernateUtil.String);
        });
    }
}
