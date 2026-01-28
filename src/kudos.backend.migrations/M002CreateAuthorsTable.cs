using FluentMigrator;

namespace kudos.backend.migrations;

[Migration(2)]
public class M002CreateAuthorsTable : Migration
{
    private readonly string _tableName = "authors";
    private readonly string _schemaName = CustomVersionTableMetaData.SchemaNameValue;

    public override void Up()
    {
        Create.Table(_tableName)
            .InSchema(_schemaName)
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("country").AsString(100).Nullable()
            .WithColumn("birth_date").AsDateTime().Nullable()
            .WithColumn("creation_date").AsDateTime().NotNullable();

        Create.Index("ix_authors_name")
            .OnTable(_tableName)
            .InSchema(_schemaName)
            .OnColumn("name")
            .Ascending();
    }

    public override void Down()
    {
        Delete.Table(_tableName).InSchema(_schemaName);
    }
}
