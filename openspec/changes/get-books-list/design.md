# Design: Get Books List

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                      WebApi Layer                                        │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │  GET /api/books                                                                      ││
│  │  GetManyAndCountBooksEndpoint → GetManyAndCountBooksModel.Request (vacío)           ││
│  │                              → HttpContext.Request.QueryString.Value                 ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
└────────────────────────────────────────────┬────────────────────────────────────────────┘
                                             │
                                             ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                   Application Layer                                      │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │  GetManyAndCountBooksUseCase                                                         ││
│  │  Command { Query: string } → IUnitOfWork.BookDaos.GetManyAndCountAsync()            ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
└────────────────────────────────────────────┬────────────────────────────────────────────┘
                                             │
                                             ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                     Domain Layer                                         │
│  ┌────────────────────────────────┐  ┌────────────────────────────────────────────────┐ │
│  │   Write Model (Entities)       │  │   Read Model (DAOs)                            │ │
│  │   ┌─────────┐ ┌─────────┐     │  │   ┌─────────────────────────────────────────┐  │ │
│  │   │ Author  │ │  Book   │     │  │   │              BookDao                     │  │ │
│  │   └────┬────┘ └────┬────┘     │  │   │  (mapea a books_view)                   │  │ │
│  │        │           │          │  │   │  Id, Title, ISBN, Category,             │  │ │
│  │        │     ┌─────┴────┐     │  │   │  CoverImageUrl, AuthorId, AuthorName,   │  │ │
│  │        │     │BookImage │     │  │   │  CreationDate, SearchAll                │  │ │
│  │        │     └──────────┘     │  │   └─────────────────────────────────────────┘  │ │
│  └────────┼─────────────────────-┘  └────────────────────────────────────────────────┘ │
└───────────┼─────────────────────────────────────────────────────────────────────────────┘
            │                                             │
            ▼                                             ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                  Infrastructure Layer                                    │
│  ┌────────────────────────────────┐  ┌────────────────────────────────────────────────┐ │
│  │   NHibernate Mappers           │  │   NHibernate Read-Only Repository             │ │
│  │   AuthorMapper → authors       │  │   NHReadOnlyRepository<BookDao>               │ │
│  │   BookMapper   → books         │  │   - QueryStringParser                         │ │
│  │   BookImageMapper → book_images│  │   - GetManyAndCountAsync()                    │ │
│  │   BookDaoMapper → books_view   │  │                                                │ │
│  └────────────────────────────────┘  └────────────────────────────────────────────────┘ │
└────────────────────────────────────────────┬────────────────────────────────────────────┘
                                             │
                                             ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                      Database                                            │
│  ┌─────────────┐  ┌─────────────┐  ┌───────────────┐  ┌─────────────────────────────┐  │
│  │   authors   │  │    books    │  │  book_images  │  │        books_view           │  │
│  │             │◄─│             │◄─│               │  │  (VIEW con JOIN a authors)  │  │
│  └─────────────┘  └─────────────┘  └───────────────┘  └─────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## Key Design Decisions

### 0. Schema en NHibernate Mappers (Limitación Técnica)

**Decisión:** Los mappers de NHibernate usan `"public"` hardcoded en lugar de `CustomVersionTableMetaData.SchemaNameValue`.

**Razón:**
- El proyecto `Infrastructure` no puede referenciar el proyecto `Migrations` (dependencia circular)
- `CustomVersionTableMetaData` está definido en el proyecto `Migrations`
- Se usa una constante local `private const string SchemaName = "public";` en cada mapper

**Impacto:**
- Si el schema cambia, hay que actualizar manualmente cada mapper
- En la práctica, el schema "public" de PostgreSQL rara vez cambia

```csharp
// En cada mapper:
private const string SchemaName = "public";

public BookDaoMapper()
{
    Schema(SchemaName);  // En lugar de CustomVersionTableMetaData.SchemaNameValue
    // ...
}
```

---

### 1. Separación Write Model / Read Model

**Decisión:** Usar entidades para escritura y DAOs para lectura.

**Razón:**
- Las operaciones de lectura requieren datos de múltiples tablas (Book + Author)
- El modelo de lectura (DAO) está optimizado para queries con datos pre-aplanados
- Evita N+1 queries al usar una vista SQL con JOIN pre-computado
- `Mutable(false)` en NHibernate previene escrituras accidentales en el DAO

```
Write Path: Endpoint → UseCase → Entity → Repository → Table
Read Path:  Endpoint → UseCase → DAO → ReadOnlyRepository → View
```

### 2. Vista SQL para el Modelo de Lectura

**Decisión:** Crear `books_view` que hace JOIN con `authors` y calcula `search_all`.

**Razón:**
- JOIN pre-computado evita múltiples queries
- Campo `search_all` permite búsqueda eficiente en múltiples campos con una sola condición
- La vista se actualiza automáticamente cuando cambian los datos subyacentes
- NHibernate trata la vista como una tabla de solo lectura

```sql
CREATE VIEW books_view AS
SELECT b.*, a.name AS author_name,
       CONCAT(b.title, ' ', b.isbn, ' ', a.name) AS search_all
FROM books b
INNER JOIN authors a ON b.author_id = a.id
```

### 3. Request DTO Vacío (Patrón APSYS)

**Decisión:** El Request del endpoint está vacío. El query string se extrae directamente de `HttpContext`.

**Razón:**
- El query string completo se pasa al Use Case para parsing centralizado
- `QueryStringParser` en la infraestructura extrae los parámetros estándar
- Evita duplicación de propiedades entre Request y Command
- Permite filtrado avanzado sin modificar el endpoint

**Implementación:** Se usa `EndpointWithoutRequest<T>` en lugar de `Endpoint<TRequest, TResponse>`:

```csharp
// Clase base cuando Request está vacío
public class GetManyAndCountBooksEndpoint
    : EndpointWithoutRequest<GetManyAndCountResultDto<BookDto>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new GetManyAndCountBooksUseCase.Command
        {
            Query = HttpContext.Request.QueryString.Value
        };
        // ...
        await Send.OkAsync(response, cancellation: ct);  // API: Send.XxxAsync
    }
}
```

### 4. Autor como Campos Planos en Respuesta

**Decisión:** Retornar `authorId` y `authorName` como campos planos, no como objeto anidado.

**Razón:**
- Simplifica el modelo de respuesta
- Coincide con la estructura del DAO (datos aplanados)
- Evita complejidad de mapeo adicional
- El cliente puede construir la relación si la necesita

```json
{
  "authorId": "...",
  "authorName": "Gabriel García Márquez"
}
// En lugar de:
{
  "author": { "id": "...", "name": "..." }
}
```

### 5. Categoría como String Simple

**Decisión:** `Category` es un campo string en Book, no una entidad separada.

**Razón:**
- Requisito explícito: no crear entidad de categoría
- Simplifica el modelo inicial
- Puede evolucionar a entidad en el futuro si se necesita
- Suficiente para el caso de uso actual (solo lectura)

### 6. BookImage con Cascade Delete

**Decisión:** FK de `book_images` a `books` tiene `ON DELETE CASCADE`.

**Razón:**
- Las imágenes no tienen sentido sin el libro
- Simplifica la eliminación de libros (futuro)
- Mantiene integridad referencial automáticamente

---

## Data Flow

### Request Flow

```
1. HTTP Request: GET /api/books?search=Garcia&pageNumber=1&pageSize=10

2. FastEndpoints routing → GetManyAndCountBooksEndpoint

3. Endpoint.HandleAsync():
   - Extrae query string: "?search=Garcia&pageNumber=1&pageSize=10"
   - Crea Command con Query

4. GetManyAndCountBooksUseCase.Handler:
   - Abre transacción
   - Llama BookDaos.GetManyAndCountAsync(query, defaultSort)

5. NHReadOnlyRepository.GetManyAndCountAsync():
   - QueryStringParser extrae: search="Garcia", pageNumber=1, pageSize=10
   - Construye query NHibernate con WHERE search_all ILIKE '%garcia%'
   - Ejecuta COUNT y SELECT con paginación

6. Retorna GetManyAndCountResult<BookDao>

7. AutoMapper: GetManyAndCountResult<BookDao> → GetManyAndCountResultDto<BookDto>

8. HTTP Response: 200 OK con JSON
```

### Entity Relationships

```
┌──────────────────┐
│      Author      │
├──────────────────┤
│ Id (PK)          │
│ Name             │──────────────┐
│ Country          │              │
│ BirthDate        │              │
│ CreationDate     │              │
└──────────────────┘              │
                                  │ 1:N
                                  │
┌──────────────────┐              │
│       Book       │◄─────────────┘
├──────────────────┤
│ Id (PK)          │
│ Title            │──────────────┐
│ ISBN (unique)    │              │
│ AuthorId (FK)    │              │
│ Category         │              │
│ CoverImageUrl    │              │
│ CreationDate     │              │
└──────────────────┘              │
                                  │ 1:N
                                  │
┌──────────────────┐              │
│    BookImage     │◄─────────────┘
├──────────────────┤
│ Id (PK)          │
│ BookId (FK)      │  CASCADE DELETE
│ ImageUrl         │
│ CreationDate     │
└──────────────────┘
```

---

## File Organization

```
src/
├── kudos.backend.domain/
│   ├── entities/
│   │   ├── Author.cs
│   │   ├── Book.cs
│   │   └── BookImage.cs
│   ├── validators/
│   │   ├── AuthorValidator.cs
│   │   ├── BookValidator.cs
│   │   └── BookImageValidator.cs
│   ├── daos/
│   │   └── BookDao.cs
│   └── interfaces/repositories/
│       ├── IAuthorRepository.cs
│       ├── IBookRepository.cs
│       ├── IBookImageRepository.cs
│       ├── IBookDaoRepository.cs
│       └── IUnitOfWork.cs          # Actualizar
│
├── kudos.backend.infrastructure/
│   └── nhibernate/
│       ├── mappers/
│       │   ├── AuthorMapper.cs
│       │   ├── BookMapper.cs
│       │   ├── BookImageMapper.cs
│       │   └── BookDaoMapper.cs
│       └── repositories/
│           ├── NHAuthorRepository.cs
│           ├── NHBookRepository.cs
│           ├── NHBookImageRepository.cs
│           └── NHUnitOfWork.cs     # Actualizar
│
├── kudos.backend.migrations/
│   ├── M002CreateAuthorsTable.cs
│   ├── M003CreateBooksTable.cs
│   ├── M004CreateBookImagesTable.cs
│   └── M005CreateBooksView.cs
│
├── kudos.backend.application/
│   └── usecases/
│       └── books/
│           └── GetManyAndCountBooksUseCase.cs
│
└── kudos.backend.webapi/
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

tests/
└── kudos.backend.scenarios/
    ├── Sc020CreateAuthors.cs
    └── Sc030CreateBooks.cs
```

---

## Implementation Order

La implementación debe seguir el orden de dependencias:

```
1. Migrations (base de datos)
   M002CreateAuthorsTable
   M003CreateBooksTable
   M004CreateBookImagesTable
   M005CreateBooksView
        │
        ▼
2. Domain Layer (entidades y contratos)
   Author, Book, BookImage (entities)
   AuthorValidator, BookValidator, BookImageValidator
   BookDao (DAO)
   IAuthorRepository, IBookRepository, IBookImageRepository, IBookDaoRepository
   IUnitOfWork (actualizar)
        │
        ▼
3. Infrastructure Layer (implementaciones)
   AuthorMapper, BookMapper, BookImageMapper, BookDaoMapper
   NHAuthorRepository, NHBookRepository, NHBookImageRepository
   NHUnitOfWork (actualizar)
        │
        ▼
4. Test Scenarios
   Sc020CreateAuthors
   Sc030CreateBooks
        │
        ▼
5. Application Layer
   GetManyAndCountBooksUseCase
        │
        ▼
6. WebApi Layer
   BookDto
   GetManyAndCountBooksModel
   GetManyAndCountBooksEndpoint
   BookMappingProfile
```

---

## Testing Strategy

### Scenarios para Carga de Datos

| Scenario | Dependencia | Datos |
|----------|-------------|-------|
| Sc020CreateAuthors | Ninguna | 3 autores latinoamericanos |
| Sc030CreateBooks | Sc020CreateAuthors | 6 libros + 2 BookImages |

### Tests de Integración

| Test | Escenario | Verifica |
|------|-----------|----------|
| GetMany_WithoutFilters | Sc030CreateBooks | Retorna 6 libros |
| GetMany_WithSearch | Sc030CreateBooks | Filtra por SearchAll |
| GetMany_WithPagination | Sc030CreateBooks | Paginación correcta |
| GetMany_WithSorting | Sc030CreateBooks | Ordenamiento funciona |
| GetMany_WithoutAuth | - | Retorna 401 |

---

## Security Considerations

- El endpoint es **público** (`AllowAnonymous`) - el catálogo de libros es información pública
- No requiere autenticación ni política específica
- No hay información sensible en la respuesta
- El query string se parsea de forma segura por QueryStringParser (previene SQL injection)

---

## Future Extensibility

Este diseño permite agregar fácilmente:

1. **Endpoint de detalle** (GET /api/books/{id})
   - Usa BookDao para datos básicos
   - Carga BookImages separadamente

2. **CRUD completo** (POST, PUT, DELETE)
   - Usa las entidades Author, Book, BookImage ya creadas
   - Usa los repositorios de escritura ya definidos

3. **Filtros avanzados**
   - QueryStringParser ya soporta filtros con operadores
   - Solo requiere documentar nuevos parámetros

4. **Categoría como entidad**
   - Agregar entidad Category
   - Modificar Book para FK a Category
   - Actualizar vista books_view
