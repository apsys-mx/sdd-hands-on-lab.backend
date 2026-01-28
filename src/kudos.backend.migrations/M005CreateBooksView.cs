using FluentMigrator;

namespace kudos.backend.migrations;

[Migration(5)]
public class M005CreateBooksView : Migration
{
    private readonly string _viewName = "books_view";
    private readonly string _schemaName = CustomVersionTableMetaData.SchemaNameValue;

    public override void Up()
    {
        var fullViewName = $"{_schemaName}.{_viewName}";
        var sql = $@"
            CREATE OR REPLACE VIEW {fullViewName} AS
            SELECT
                b.id,
                b.title,
                b.isbn,
                b.category,
                b.cover_image_url,
                b.author_id,
                a.name AS author_name,
                b.creation_date,
                CONCAT(b.title, ' ', b.isbn, ' ', a.name) AS search_all
            FROM {_schemaName}.books b
            INNER JOIN {_schemaName}.authors a ON b.author_id = a.id;
        ";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        var fullViewName = $"{_schemaName}.{_viewName}";
        Execute.Sql($"DROP VIEW IF EXISTS {fullViewName};");
    }
}
