using kudos.backend.domain.entities;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace kudos.backend.infrastructure.nhibernate.mappers;

/// <summary>
/// NHibernate mapping for the BookImage entity.
/// </summary>
public class BookImageMapper : ClassMapping<BookImage>
{
    private const string SchemaName = "public";

    public BookImageMapper()
    {
        Schema(SchemaName);
        Table("book_images");

        Id(x => x.Id, m =>
        {
            m.Column("id");
            m.Generator(Generators.Assigned);
            m.Type(NHibernateUtil.Guid);
        });

        Property(x => x.BookId, m =>
        {
            m.Column("book_id");
            m.NotNullable(true);
            m.Type(NHibernateUtil.Guid);
        });

        Property(x => x.ImageUrl, m =>
        {
            m.Column("image_url");
            m.NotNullable(true);
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
