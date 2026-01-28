# Spec: Read Model

## Overview

Modelo de lectura optimizado para queries. Define BookDao mapeado a una vista SQL que hace JOIN con Author para evitar N+1 queries y proporcionar datos aplanados para el listado.

## BookDao

Data Access Object para lectura de libros con información del autor aplanada.

### Fields

| Field | Type | Source | Description |
|-------|------|--------|-------------|
| Id | Guid | books.id | Identificador del libro |
| Title | string | books.title | Título del libro |
| ISBN | string | books.isbn | Código ISBN |
| Category | string? | books.category | Categoría (nullable) |
| CoverImageUrl | string? | books.cover_image_url | URL de portada (nullable) |
| AuthorId | Guid | books.author_id | ID del autor |
| AuthorName | string | authors.name | Nombre del autor (JOIN) |
| CreationDate | DateTime | books.creation_date | Fecha de creación |
| SearchAll | string | computed | Concatenación para búsqueda |

### Class Definition

```csharp
namespace kudos.backend.domain.daos;

/// <summary>
/// Data Access Object for Book entities, used for read-only database operations.
/// Maps to books_view which includes author information.
/// </summary>
public class BookDao
{
    public virtual Guid Id { get; set; }
    public virtual string Title { get; set; } = string.Empty;
    public virtual string ISBN { get; set; } = string.Empty;
    public virtual string? Category { get; set; }
    public virtual string? CoverImageUrl { get; set; }
    public virtual Guid AuthorId { get; set; }
    public virtual string AuthorName { get; set; } = string.Empty;
    public virtual DateTime CreationDate { get; set; }
    public virtual string SearchAll { get; set; } = string.Empty;
}
```

### Characteristics

- **No hereda de AbstractDomainObject** - Es un POCO simple
- **No tiene validador** - Es solo lectura
- **Propiedades virtuales** - Para compatibilidad con NHibernate
- **Mutable(false)** - Configuración en mapper para prevenir escrituras

---

## Database View Migration

### M005CreateBooksView

Siguiendo las convenciones de FluentMigrator para vistas SQL.

```csharp
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
```

### SearchAll Field

Campo calculado que concatena para búsqueda eficiente:
- Título del libro
- ISBN
- Nombre del autor

Permite buscar "García" y encontrar tanto libros con ese término en el título como libros de "García Márquez".

---

## Repository Interface

### IBookDaoRepository

```csharp
namespace kudos.backend.domain.interfaces.repositories;

/// <summary>
/// Defines a read-only repository for managing BookDao entities.
/// </summary>
public interface IBookDaoRepository : IReadOnlyRepository<BookDao, Guid>
{
    // Hereda de IReadOnlyRepository:
    // - GetAsync(Guid id, CancellationToken ct)
    // - GetAsync(CancellationToken ct)
    // - GetAsync(Expression<Func<BookDao, bool>> query, CancellationToken ct)
    // - CountAsync(CancellationToken ct)
    // - GetManyAndCountAsync(string? query, string defaultSorting, CancellationToken ct)
}
```

---

## NHibernate Mapper

### BookDaoMapper

```csharp
namespace kudos.backend.infrastructure.nhibernate.mappers;

public class BookDaoMapper : ClassMapping<BookDao>
{
    public BookDaoMapper()
    {
        Schema(CustomVersionTableMetaData.SchemaNameValue);
        Table("books_view");
        Mutable(false);  // CRÍTICO: Previene INSERT/UPDATE

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
```

---

## Query String Convention

Según las guías de FastEndpoints (sección "Query String Completo"), para endpoints `GetManyAndCount`:

### Patrón APSYS

1. **El Request DTO está vacío** o solo tiene una propiedad `Query` opcional
2. **El endpoint extrae el query string completo**: `HttpContext.Request.QueryString.Value`
3. **Se pasa al Use Case** que usa `QueryStringParser` para el parsing

```csharp
// En el Endpoint
public override async Task HandleAsync(GetManyAndCountModel.Request req, CancellationToken ct)
{
    var command = new GetManyAndCountBooksUseCase.Command
    {
        Query = HttpContext.Request.QueryString.Value  // Query completo
    };
    // ...
}
```

### Query String Parameters (documentación para consumidores)

Estos son los parámetros que el cliente puede enviar en el query string. El parsing lo realiza `QueryStringParser` en la capa de infraestructura.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `search` | string? | null | Término de búsqueda (busca en SearchAll) |
| `pageNumber` | int | 1 | Número de página (1-based) |
| `pageSize` | int | 10 | Elementos por página (máx 100) |
| `sortBy` | string? | "Title" | Campo de ordenamiento |
| `sortCriteria` | string | "asc" | Dirección: "asc" o "desc" |

### Example Query Strings

```
# Básico - usa defaults
GET /api/books

# Con búsqueda
GET /api/books?search=Garcia

# Con paginación
GET /api/books?pageNumber=2&pageSize=20

# Con ordenamiento
GET /api/books?sortBy=AuthorName&sortCriteria=desc

# Combinado completo
GET /api/books?search=novela&pageNumber=1&pageSize=10&sortBy=Title&sortCriteria=asc
```

### Query Parsing (via QueryStringParser)

El `NHReadOnlyRepository.GetManyAndCountAsync` recibe el query string completo y usa `QueryStringParser` para:

1. Extraer `search` y buscar en `SearchAll` con ILIKE
2. Extraer `pageNumber` y `pageSize` para paginación
3. Extraer `sortBy` y `sortCriteria` para ordenamiento
4. Construir la expresión de filtro y ejecutar la query

---

## Sortable Fields

| Field | Column | Description |
|-------|--------|-------------|
| Title | title | Título del libro (default) |
| ISBN | isbn | Código ISBN |
| AuthorName | author_name | Nombre del autor |
| CreationDate | creation_date | Fecha de creación |

---

## IUnitOfWork Integration

```csharp
public interface IUnitOfWork : IDisposable
{
    #region crud Repositories
    IAuthorRepository Authors { get; }
    IBookRepository Books { get; }
    IBookImageRepository BookImages { get; }
    #endregion

    #region read-only Repositories
    IBookDaoRepository BookDaos { get; }
    #endregion

    // ... transaction methods
}
```

---

## Performance Considerations

### Why a View?

- **Pre-computed JOIN**: Evita N+1 queries al cargar autores
- **SearchAll**: Búsqueda eficiente sin múltiples OR conditions
- **Campos aplanados**: No requiere deserialización de relaciones
- **Mutable(false)**: NHibernate no intenta actualizar

### Query Execution

```
Request: GET /api/books?search=Garcia&pageNumber=1&pageSize=10&sortBy=Title
                              ↓
QueryStringParser.ParseQuery() → quickSearch = "Garcia"
                              ↓
FilterExpressionParser → WHERE search_all ILIKE '%garcia%'
                              ↓
NHibernate Query → SELECT * FROM books_view WHERE ... ORDER BY title LIMIT 10 OFFSET 0
                              ↓
GetManyAndCountResult<BookDao> { Items, Count, PageNumber, PageSize, Sorting }
```

---

## Acceptance Criteria

- [ ] BookDao creado en `domain/daos/BookDao.cs` con propiedades virtuales
- [ ] BookDao NO hereda de AbstractDomainObject
- [ ] M005CreateBooksView crea vista SQL con JOIN y search_all
- [ ] BookDaoMapper configurado con `Mutable(false)` y Table("books_view")
- [ ] IBookDaoRepository hereda de IReadOnlyRepository
- [ ] IUnitOfWork incluye `BookDaos` en región read-only
- [ ] NHUnitOfWork implementa BookDaos usando NHReadOnlyRepository
- [ ] GetManyAndCountAsync busca correctamente en SearchAll
- [ ] QueryStringParser parsea: search, pageNumber, pageSize, sortBy, sortCriteria
- [ ] Query string se recibe completo desde el endpoint (patrón APSYS)
- [ ] Ordenamiento funciona para Title, ISBN, AuthorName, CreationDate
