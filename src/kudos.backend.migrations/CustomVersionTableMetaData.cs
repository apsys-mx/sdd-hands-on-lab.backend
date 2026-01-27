using FluentMigrator.Runner.VersionTableInfo;

namespace kudos.backend.migrations;

/// <summary>
/// Configures the table that FluentMigrator uses to track applied migrations.
/// </summary>
public class CustomVersionTableMetaData : IVersionTableMetaData
{
    public static string SchemaNameValue => "public";

    public required object ApplicationContext { get; set; }

    public bool OwnsSchema => true;

    public string SchemaName => SchemaNameValue;

    public string TableName => "versioninfo";

    public string ColumnName => "version";

    public string UniqueIndexName => "uc_version";

    public string AppliedOnColumnName => "appliedon";

    public string DescriptionColumnName => "description";

    public bool CreateWithPrimaryKey => false;
}
