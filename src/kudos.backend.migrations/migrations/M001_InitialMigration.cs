using FluentMigrator;

namespace kudos.backend.migrations.migrations;

/// <summary>
/// Initial migration - creates base schema.
/// </summary>
[Migration(1)]
public class M001_InitialMigration : Migration
{
    private const string SchemaName = "public";

    public override void Up()
    {
        // Validation query to verify migrations work
        Execute.Sql("SELECT 1;");
    }

    public override void Down()
    {
        // Nothing to revert
    }
}
