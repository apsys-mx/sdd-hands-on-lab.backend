using FluentMigrator;

namespace kudos.backend.migrations;

[Migration(4)]
public class M004CreateBookImagesTable : Migration
{
    private readonly string _tableName = "book_images";
    private readonly string _schemaName = CustomVersionTableMetaData.SchemaNameValue;

    public override void Up()
    {
        Create.Table(_tableName)
            .InSchema(_schemaName)
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("book_id").AsGuid().NotNullable()
            .WithColumn("image_url").AsString(500).NotNullable()
            .WithColumn("creation_date").AsDateTime().NotNullable();

        Create.ForeignKey("fk_book_images_book_id")
            .FromTable(_tableName).InSchema(_schemaName)
            .ForeignColumn("book_id")
            .ToTable("books").InSchema(_schemaName)
            .PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("ix_book_images_book_id")
            .OnTable(_tableName).InSchema(_schemaName)
            .OnColumn("book_id").Ascending();
    }

    public override void Down()
    {
        Delete.Table(_tableName).InSchema(_schemaName);
    }
}
