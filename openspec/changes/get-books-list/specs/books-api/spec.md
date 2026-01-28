# Spec: Books API

## Overview

Endpoint REST para obtener el listado paginado de libros. Utiliza el modelo de lectura (BookDao) para queries optimizadas y retorna los datos mapeados a DTOs.

## Endpoint

```
GET /api/books
```

### Authentication

- Requiere autenticación (cualquier usuario autenticado)
- No requiere política específica

---

## Request

### Query String Convention

Siguiendo la convención de FastEndpoints establecida en las guías:

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| search | string? | No | null | Busca en título, ISBN y nombre de autor |
| pageNumber | int | No | 1 | Número de página (mínimo 1) |
| pageSize | int | No | 10 | Elementos por página (1-100) |
| sortBy | string? | No | Title | Campo de ordenamiento |
| sortCriteria | string | No | asc | Dirección: asc o desc |

### Valid Sort Fields

- `Title` (default)
- `ISBN`
- `AuthorName`
- `CreationDate`

### Example Requests

```bash
# Listado básico (usa defaults)
GET /api/books

# Con búsqueda
GET /api/books?search=Garcia

# Con paginación
GET /api/books?pageNumber=2&pageSize=20

# Con ordenamiento
GET /api/books?sortBy=AuthorName&sortCriteria=desc

# Combinado
GET /api/books?search=novela&pageNumber=1&pageSize=10&sortBy=Title&sortCriteria=asc
```

---

## Response

### Success: 200 OK

```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Cien años de soledad",
      "isbn": "978-0060883287",
      "category": "Novela",
      "coverImageUrl": "https://example.com/covers/cien-anos.jpg",
      "authorId": "550e8400-e29b-41d4-a716-446655440001",
      "authorName": "Gabriel García Márquez",
      "creationDate": "2024-01-15T10:30:00Z"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440002",
      "title": "El Aleph",
      "isbn": "978-0142437889",
      "category": "Cuento",
      "coverImageUrl": null,
      "authorId": "550e8400-e29b-41d4-a716-446655440003",
      "authorName": "Jorge Luis Borges",
      "creationDate": "2024-01-16T14:20:00Z"
    }
  ],
  "count": 150,
  "pageNumber": 1,
  "pageSize": 10,
  "sortBy": "Title",
  "sortCriteria": "Ascending"
}
```

### Response Schema

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| items | BookDto[] | No | Array de libros |
| count | int | No | Total de registros que coinciden |
| pageNumber | int | No | Página actual |
| pageSize | int | No | Tamaño de página |
| sortBy | string | No | Campo de ordenamiento |
| sortCriteria | string | No | Dirección (Ascending/Descending) |

### BookDto Schema

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | guid | No | ID único del libro |
| title | string | No | Título |
| isbn | string | No | Código ISBN |
| category | string | Yes | Categoría/género |
| coverImageUrl | string | Yes | URL de imagen de portada |
| authorId | guid | No | ID del autor |
| authorName | string | No | Nombre del autor |
| creationDate | datetime | No | Fecha de creación UTC |

---

## Error Responses

### 400 Bad Request

Parámetros de query inválidos.

```json
{
  "statusCode": 400,
  "message": "Invalid request parameters",
  "errors": {
    "pageSize": ["PageSize must be between 1 and 100"]
  }
}
```

### 401 Unauthorized

Token de autenticación ausente o inválido.

```json
{
  "statusCode": 401,
  "message": "Unauthorized"
}
```

### 500 Internal Server Error

Error interno del servidor.

```json
{
  "statusCode": 500,
  "message": "An unexpected error occurred"
}
```

---

## Implementation Components

### Request/Response Model

**GetManyAndCountBooksModel.cs**

Siguiendo el patrón APSYS de las guías (FastEndpoints Basics, ejemplo 3: "GET Many con Query Parameters"):

**IMPORTANTE:** El Request está **vacío**. Los query params se obtienen del QueryString completo en el endpoint.

```csharp
namespace kudos.backend.webapi.features.books.models;

/// <summary>
/// Data model for retrieving many books with count
/// </summary>
public class GetManyAndCountBooksModel
{
    /// <summary>
    /// Represents the request data used to get many books with count.
    /// Empty - query params are obtained from HttpContext.Request.QueryString.Value
    /// </summary>
    public class Request
    {
        // Vacío - query params se obtienen del QueryString completo
    }

    /// <summary>
    /// Represents a paginated list of books along with the total count.
    /// </summary>
    public class Response : GetManyAndCountResultDto<BookDto>
    {
        // Hereda de GetManyAndCountResultDto:
        // - IEnumerable<BookDto> Items
        // - long Count
        // - int PageNumber
        // - int PageSize
        // - string SortBy
        // - string SortCriteria
    }
}
```

### DTO

**BookDto.cs**

```csharp
namespace kudos.backend.webapi.dtos;

/// <summary>
/// Data transfer object for Book.
/// </summary>
public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
}
```

### Use Case

**GetManyAndCountBooksUseCase.cs**

```csharp
namespace kudos.backend.application.usecases.books;

/// <summary>
/// Use case for retrieving a paginated list of books.
/// </summary>
public abstract class GetManyAndCountBooksUseCase
{
    public class Command : ICommand<GetManyAndCountResult<BookDao>>
    {
        public string? Query { get; set; }
    }

    public class Handler(IUnitOfWork uoW, ILogger<Handler> logger)
        : ICommandHandler<Command, GetManyAndCountResult<BookDao>>
    {
        private readonly IUnitOfWork _uoW = uoW;
        private readonly ILogger<Handler> _logger = logger;

        public async Task<GetManyAndCountResult<BookDao>> ExecuteAsync(
            Command command,
            CancellationToken ct)
        {
            try
            {
                _uoW.BeginTransaction();
                _logger.LogInformation(
                    "Executing GetManyAndCountBooksUseCase with query: {Query}",
                    command.Query);

                // Usa BookDaos (read-only repository)
                var books = await _uoW.BookDaos.GetManyAndCountAsync(
                    command.Query,
                    nameof(BookDao.Title),  // Default sorting
                    ct);

                _logger.LogInformation(
                    "End GetManyAndCountBooksUseCase with total: {Total}",
                    books.Count);

                _uoW.Commit();
                return books;
            }
            catch
            {
                _uoW.Rollback();
                throw;
            }
        }
    }
}
```

### Endpoint

**GetManyAndCountBooksEndpoint.cs**

```csharp
namespace kudos.backend.webapi.features.books.endpoint;

/// <summary>
/// Endpoint to get a paginated list of books.
/// </summary>
public class GetManyAndCountBooksEndpoint(AutoMapper.IMapper mapper)
    : Endpoint<GetManyAndCountBooksModel.Request, GetManyAndCountBooksModel.Response>
{
    private readonly AutoMapper.IMapper _mapper = mapper;

    public override void Configure()
    {
        Get("/api/books");
        // Sin Policies() - solo requiere autenticación (comportamiento por defecto)
        Description(d => d
            .WithTags("Books")
            .WithName("GetBooks")
            .WithDescription(@"Get a paginated list of books with filtering and sorting.

Query Parameters:
- search: Search in title, ISBN, author name
- pageNumber: Page number (default: 1)
- pageSize: Items per page (default: 10, max: 100)
- sortBy: Field to sort by (default: Title)
- sortCriteria: Sort direction 'asc' or 'desc' (default: asc)")
            .Produces<GetManyAndCountBooksModel.Response>(200, "application/json")
            .ProducesProblemDetails(StatusCodes.Status400BadRequest)
            .ProducesProblemDetails(StatusCodes.Status401Unauthorized)
            .ProducesProblemDetails(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(
        GetManyAndCountBooksModel.Request req,
        CancellationToken ct)
    {
        try
        {
            // Patrón APSYS: Query string completo se pasa al Use Case
            var command = new GetManyAndCountBooksUseCase.Command
            {
                Query = HttpContext.Request.QueryString.Value  // Query completo
            };

            var result = await command.ExecuteAsync(ct);
            var response = _mapper.Map<GetManyAndCountBooksModel.Response>(result);

            Logger.LogInformation("Successfully retrieved {Count} books", result.Count);
            await SendOkAsync(response, cancellation: ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve books");
            AddError($"An unexpected error occurred: {ex.Message}");
            await SendErrorsAsync(StatusCodes.Status500InternalServerError, cancellation: ct);
        }
    }
}
```

### Mapping Profile

**BookMappingProfile.cs**

```csharp
namespace kudos.backend.webapi.mappingprofiles;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        // DAO to DTO
        CreateMap<BookDao, BookDto>();

        // GetManyAndCount result mapping
        CreateMap<GetManyAndCountResult<BookDao>, GetManyAndCountResultDto<BookDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        // Nota: No se necesita mapeo Request → Command porque:
        // - El Request está vacío
        // - El Command se construye manualmente con el query string
    }
}
```

---

## File Structure

```
src/kudos.backend.application/
└── usecases/
    └── books/
        └── GetManyAndCountBooksUseCase.cs

src/kudos.backend.webapi/
├── dtos/
│   └── BookDto.cs
├── features/
│   └── books/
│       ├── models/
│       │   └── GetManyAndCountBooksModel.cs
│       └── endpoint/
│           └── GetManyAndCountBooksEndpoint.cs
└── mappingprofiles/
    └── BookMappingProfile.cs
```

---

## Testing

### Integration Tests

**GetManyAndCountBooksEndpointTests.cs**

Usando el escenario Sc030CreateBooks que crea 6 libros con autores.

```csharp
[TestFixture]
public class GetManyAndCountBooksEndpointTests : EndpointTestBase
{
    [Test]
    public async Task GetMany_WithoutFilters_ShouldReturnAllBooks()
    {
        // Arrange - LoadScenario(typeof(Sc030CreateBooks))
        // Act - GET /api/books
        // Assert - 200 OK, count = 6
    }

    [Test]
    public async Task GetMany_WithSearch_ShouldReturnFilteredResults()
    {
        // Arrange - LoadScenario
        // Act - GET /api/books?search=Garcia
        // Assert - Returns only García Márquez books
    }

    [Test]
    public async Task GetMany_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange - LoadScenario
        // Act - GET /api/books?pageNumber=1&pageSize=3
        // Assert - items.length = 3, count = 6
    }

    [Test]
    public async Task GetMany_WithSorting_ShouldReturnSortedResults()
    {
        // Arrange - LoadScenario
        // Act - GET /api/books?sortBy=AuthorName&sortCriteria=desc
        // Assert - First item is from author that comes last alphabetically
    }

    [Test]
    public async Task GetMany_WithoutAuth_ShouldReturn401()
    {
        // Act - GET /api/books sin autenticación
        // Assert - 401 Unauthorized (solo requiere estar autenticado)
    }
}
```

---

## Acceptance Criteria

- [ ] GET /api/books retorna 200 con estructura GetManyAndCountResultDto
- [ ] Búsqueda (`search`) filtra por título, ISBN y nombre de autor
- [ ] Paginación (`pageNumber`, `pageSize`) funciona correctamente
- [ ] Ordenamiento (`sortBy`, `sortCriteria`) funciona para campos válidos
- [ ] Campos opcionales (category, coverImageUrl) pueden ser null
- [ ] Retorna 401 si no está autenticado (no requiere política específica)
- [ ] Retorna 400 con parámetros inválidos (pageSize > 100)
- [ ] Use case usa repositorio de solo lectura BookDaos
- [ ] Request DTO está vacío (patrón APSYS)
- [ ] Query string se obtiene de HttpContext.Request.QueryString.Value
- [ ] Command se construye manualmente (no con AutoMapper)
- [ ] Mapping profile mapea BookDao → BookDto y Result → ResultDto
- [ ] Documentación Swagger con descripción de parámetros
- [ ] Tests de integración con escenario Sc030CreateBooks
