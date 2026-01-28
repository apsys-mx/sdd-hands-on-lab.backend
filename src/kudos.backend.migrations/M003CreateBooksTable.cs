using FluentMigrator;

namespace kudos.backend.migrations;

[Migration(3)]
public class M003CreateBooksTable : Migration
{
    private readonly string _tableName = "books";
    private readonly string _schemaName = CustomVersionTableMetaData.SchemaNameValue;

    public override void Up()
    {
        Create.Table(_tableName)
            .InSchema(_schemaName)
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("title").AsString(300).NotNullable()
            .WithColumn("isbn").AsString(20).NotNullable().Unique()
            .WithColumn("author_id").AsGuid().NotNullable()
            .WithColumn("category").AsString(100).Nullable()
            .WithColumn("cover_image_url").AsString(500).Nullable()
            .WithColumn("creation_date").AsDateTime().NotNullable();

        Create.ForeignKey("fk_books_author_id")
            .FromTable(_tableName).InSchema(_schemaName)
            .ForeignColumn("author_id")
            .ToTable("authors").InSchema(_schemaName)
            .PrimaryColumn("id");

        Create.Index("ix_books_title")
            .OnTable(_tableName).InSchema(_schemaName)
            .OnColumn("title").Ascending();

        Create.Index("ix_books_isbn")
            .OnTable(_tableName).InSchema(_schemaName)
            .OnColumn("isbn").Ascending();

        Create.Index("ix_books_author_id")
            .OnTable(_tableName).InSchema(_schemaName)
            .OnColumn("author_id").Ascending();
    }

    public override void Down()
    {
        Delete.Table(_tableName).InSchema(_schemaName);
    }
}
