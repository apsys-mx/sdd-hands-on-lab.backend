# Spec: Write Model

## Overview

Modelo de escritura con las entidades de dominio para operaciones CRUD. Define Author, Book y BookImage siguiendo los patrones de Clean Architecture y las guías de desarrollo.

## Entities

### Author

Representa un autor de libros.

#### Fields

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| Id | Guid | Yes | PK, auto-generated | Identificador único |
| Name | string | Yes | Max 200 chars, not empty | Nombre completo del autor |
| Country | string | No | Max 100 chars | País de origen |
| BirthDate | DateTime? | No | - | Fecha de nacimiento |
| CreationDate | DateTime | Yes | Auto-set UTC | Fecha de creación del registro |

#### Validation Rules (AuthorValidator)

```csharp
RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Name is required")
    .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

RuleFor(x => x.Country)
    .MaximumLength(100).WithMessage("Country cannot exceed 100 characters")
    .When(x => !string.IsNullOrEmpty(x.Country));

RuleFor(x => x.BirthDate)
    .LessThan(DateTime.UtcNow).WithMessage("BirthDate must be in the past")
    .When(x => x.BirthDate.HasValue);
```

---

### Book

Representa un libro del catálogo.

#### Fields

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| Id | Guid | Yes | PK, auto-generated | Identificador único |
| Title | string | Yes | Max 300 chars, not empty | Título del libro |
| ISBN | string | Yes | Max 20 chars, unique | Código ISBN |
| AuthorId | Guid | Yes | FK to Author | Referencia al autor |
| Category | string | No | Max 100 chars | Categoría/género |
| CoverImageUrl | string | No | Max 500 chars | URL de imagen de portada |
| CreationDate | DateTime | Yes | Auto-set UTC | Fecha de creación |

#### Validation Rules (BookValidator)

```csharp
RuleFor(x => x.Title)
    .NotEmpty().WithMessage("Title is required")
    .MaximumLength(300).WithMessage("Title cannot exceed 300 characters");

RuleFor(x => x.ISBN)
    .NotEmpty().WithMessage("ISBN is required")
    .MaximumLength(20).WithMessage("ISBN cannot exceed 20 characters");

RuleFor(x => x.AuthorId)
    .NotEmpty().WithMessage("AuthorId is required");

RuleFor(x => x.Category)
    .MaximumLength(100).WithMessage("Category cannot exceed 100 characters")
    .When(x => !string.IsNullOrEmpty(x.Category));

RuleFor(x => x.CoverImageUrl)
    .MaximumLength(500).WithMessage("CoverImageUrl cannot exceed 500 characters")
    .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));
```

---

### BookImage

Imágenes adicionales de un libro (no la portada).

#### Fields

| Field | Type | Required | Constraints | Description |
|-------|------|----------|-------------|-------------|
| Id | Guid | Yes | PK, auto-generated | Identificador único |
| BookId | Guid | Yes | FK to Book | Referencia al libro |
| ImageUrl | string | Yes | Max 500 chars, not empty | URL de la imagen |
| CreationDate | DateTime | Yes | Auto-set UTC | Fecha de creación |

#### Validation Rules (BookImageValidator)

```csharp
RuleFor(x => x.BookId)
    .NotEmpty().WithMessage("BookId is required");

RuleFor(x => x.ImageUrl)
    .NotEmpty().WithMessage("ImageUrl is required")
    .MaximumLength(500).WithMessage("ImageUrl cannot exceed 500 characters");
```

---

## Database Migrations

Siguiendo las convenciones de FluentMigrator establecidas en las guías.

### M002CreateAuthorsTable

```csharp
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
```

### M003CreateBooksTable

```csharp
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
```

### M004CreateBookImagesTable

```csharp
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
```

---

## Repository Interfaces

### IAuthorRepository

```csharp
public interface IAuthorRepository : IRepository<Author, Guid>
{
    Task<Author?> GetByNameAsync(string name, CancellationToken ct = default);
}
```

### IBookRepository

```csharp
public interface IBookRepository : IRepository<Book, Guid>
{
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default);
    Task<bool> ExistsIsbnAsync(string isbn, CancellationToken ct = default);
}
```

### IBookImageRepository

```csharp
public interface IBookImageRepository : IRepository<BookImage, Guid>
{
    Task<IEnumerable<BookImage>> GetByBookIdAsync(Guid bookId, CancellationToken ct = default);
}
```

---

## Test Scenarios

Siguiendo la convención `Sc###Create{Entity}` de las guías de testing.

### Sc020CreateAuthors

**Archivo:** `tests/kudos.backend.scenarios/Sc020CreateAuthors.cs`

```csharp
/// <summary>
/// Scenario to create authors for testing.
/// Used in: Book-related tests that require authors.
/// </summary>
public class Sc020CreateAuthors(IUnitOfWork uoW) : IScenario
{
    private readonly IUnitOfWork _uoW = uoW;

    public string ScenarioFileName => "CreateAuthors";
    public Type? PreloadScenario => null;  // Sin dependencias

    public async Task SeedData()
    {
        var authors = new List<(string Name, string? Country, DateTime? BirthDate)>
        {
            ("Gabriel García Márquez", "Colombia", new DateTime(1927, 3, 6)),
            ("Jorge Luis Borges", "Argentina", new DateTime(1899, 8, 24)),
            ("Isabel Allende", "Chile", new DateTime(1942, 8, 2))
        };

        try
        {
            _uoW.BeginTransaction();
            foreach (var (name, country, birthDate) in authors)
            {
                var author = new Author(name) { Country = country, BirthDate = birthDate };
                await _uoW.Authors.CreateAsync(author);
            }
            _uoW.Commit();
        }
        catch
        {
            _uoW.Rollback();
            throw;
        }
    }
}
```

### Sc030CreateBooks

**Archivo:** `tests/kudos.backend.scenarios/Sc030CreateBooks.cs`

```csharp
/// <summary>
/// Scenario to create books with authors for testing.
/// Used in: GetManyAndCountBooks tests, search and pagination tests.
/// </summary>
public class Sc030CreateBooks(IUnitOfWork uoW) : IScenario
{
    private readonly IUnitOfWork _uoW = uoW;

    public string ScenarioFileName => "CreateBooks";
    public Type? PreloadScenario => typeof(Sc020CreateAuthors);  // Requiere autores

    public async Task SeedData()
    {
        try
        {
            _uoW.BeginTransaction();

            // Obtener autores del escenario anterior
            var authors = (await _uoW.Authors.GetAsync()).ToList();
            var garciaMarquez = authors.First(a => a.Name.Contains("García Márquez"));
            var borges = authors.First(a => a.Name.Contains("Borges"));
            var allende = authors.First(a => a.Name.Contains("Allende"));

            // Crear libros
            var books = new List<Book>
            {
                new("Cien años de soledad", "978-0060883287", garciaMarquez.Id)
                    { Category = "Novela", CoverImageUrl = "https://example.com/cien-anos.jpg" },
                new("El amor en los tiempos del cólera", "978-0307389732", garciaMarquez.Id)
                    { Category = "Novela" },
                new("El Aleph", "978-0142437889", borges.Id)
                    { Category = "Cuento" },
                new("Ficciones", "978-0802130303", borges.Id)
                    { Category = "Cuento" },
                new("La casa de los espíritus", "978-1501117015", allende.Id)
                    { Category = "Novela" },
                new("Eva Luna", "978-0553383829", allende.Id)
                    { Category = "Novela" }
            };

            foreach (var book in books)
                await _uoW.Books.CreateAsync(book);

            // Agregar imágenes a algunos libros
            var cienAnos = books[0];
            await _uoW.BookImages.CreateAsync(new BookImage(cienAnos.Id, "https://example.com/cien-anos-back.jpg"));
            await _uoW.BookImages.CreateAsync(new BookImage(cienAnos.Id, "https://example.com/cien-anos-spine.jpg"));

            _uoW.Commit();
        }
        catch
        {
            _uoW.Rollback();
            throw;
        }
    }
}
```

---

## Acceptance Criteria

- [ ] Entidad Author hereda de AbstractDomainObject con GetValidator()
- [ ] Entidad Book hereda de AbstractDomainObject con GetValidator()
- [ ] Entidad BookImage hereda de AbstractDomainObject con GetValidator()
- [ ] AuthorValidator valida Name requerido y longitudes máximas
- [ ] BookValidator valida Title, ISBN, AuthorId requeridos
- [ ] BookImageValidator valida BookId, ImageUrl requeridos
- [ ] M002CreateAuthorsTable crea tabla con índice en name
- [ ] M003CreateBooksTable crea tabla con FK a authors e índice unique en isbn
- [ ] M004CreateBookImagesTable crea tabla con FK cascade a books
- [ ] IAuthorRepository, IBookRepository, IBookImageRepository definidos
- [ ] IUnitOfWork actualizado con Authors, Books, BookImages
- [ ] Mappers NHibernate para las 3 entidades (schema hardcoded `"public"`)
- [ ] NHUnitOfWork implementa las 3 propiedades de repositorio
- [ ] Sc020CreateAuthors crea 3 autores de prueba
- [ ] Sc030CreateBooks crea 6 libros con imágenes (depende de Sc020)
- [ ] ScenarioBuilder registra AuthorValidator, BookValidator, BookImageValidator
