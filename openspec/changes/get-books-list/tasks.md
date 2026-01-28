# Tasks: Get Books List

## 1. Migrations

### 1.1 M002CreateAuthorsTable
- [x] Crear archivo `M002CreateAuthorsTable.cs` en `kudos.backend.migrations`
- [x] Definir tabla `authors` con columnas: id, name, country, birth_date, creation_date
- [x] Crear índice `ix_authors_name` en columna name
- [x] Implementar método Down() para rollback

### 1.2 M003CreateBooksTable
- [x] Crear archivo `M003CreateBooksTable.cs` en `kudos.backend.migrations`
- [x] Definir tabla `books` con columnas: id, title, isbn, author_id, category, cover_image_url, creation_date
- [x] Agregar constraint UNIQUE en columna isbn
- [x] Crear FK `fk_books_author_id` a tabla authors
- [x] Crear índices: ix_books_title, ix_books_isbn, ix_books_author_id
- [x] Implementar método Down() para rollback

### 1.3 M004CreateBookImagesTable
- [x] Crear archivo `M004CreateBookImagesTable.cs` en `kudos.backend.migrations`
- [x] Definir tabla `book_images` con columnas: id, book_id, image_url, creation_date
- [x] Crear FK `fk_book_images_book_id` con ON DELETE CASCADE
- [x] Crear índice `ix_book_images_book_id`
- [x] Implementar método Down() para rollback

### 1.4 M005CreateBooksView
- [x] Crear archivo `M005CreateBooksView.cs` en `kudos.backend.migrations`
- [x] Crear vista `books_view` con JOIN a authors
- [x] Incluir campo calculado `search_all` (CONCAT de title, isbn, author_name)
- [x] Implementar método Down() con DROP VIEW

---

## 2. Domain Layer - Entities

### 2.1 Author Entity
- [x] Crear archivo `Author.cs` en `domain/entities`
- [x] Heredar de `AbstractDomainObject<Guid>`
- [x] Agregar propiedades: Name, Country, BirthDate
- [x] Implementar constructor con parámetro Name requerido
- [x] Implementar método `GetValidator()` retornando `AuthorValidator`

### 2.2 AuthorValidator
- [x] Crear archivo `AuthorValidator.cs` en `domain/validators`
- [x] Validar Name: NotEmpty, MaxLength(200)
- [x] Validar Country: MaxLength(100) cuando tiene valor
- [x] Validar BirthDate: LessThan(UtcNow) cuando tiene valor

### 2.3 Book Entity
- [x] Crear archivo `Book.cs` en `domain/entities`
- [x] Heredar de `AbstractDomainObject<Guid>`
- [x] Agregar propiedades: Title, ISBN, AuthorId, Category, CoverImageUrl
- [x] Implementar constructor con Title, ISBN, AuthorId requeridos
- [x] Implementar método `GetValidator()` retornando `BookValidator`

### 2.4 BookValidator
- [x] Crear archivo `BookValidator.cs` en `domain/validators`
- [x] Validar Title: NotEmpty, MaxLength(300)
- [x] Validar ISBN: NotEmpty, MaxLength(20)
- [x] Validar AuthorId: NotEmpty
- [x] Validar Category: MaxLength(100) cuando tiene valor
- [x] Validar CoverImageUrl: MaxLength(500) cuando tiene valor

### 2.5 BookImage Entity
- [x] Crear archivo `BookImage.cs` en `domain/entities`
- [x] Heredar de `AbstractDomainObject<Guid>`
- [x] Agregar propiedades: BookId, ImageUrl
- [x] Implementar constructor con BookId, ImageUrl requeridos
- [x] Implementar método `GetValidator()` retornando `BookImageValidator`

### 2.6 BookImageValidator
- [x] Crear archivo `BookImageValidator.cs` en `domain/validators`
- [x] Validar BookId: NotEmpty
- [x] Validar ImageUrl: NotEmpty, MaxLength(500)

---

## 3. Domain Layer - DAO

### 3.1 BookDao
- [x] Crear archivo `BookDao.cs` en `domain/daos`
- [x] NO heredar de AbstractDomainObject (es POCO simple)
- [x] Agregar propiedades virtuales: Id, Title, ISBN, Category, CoverImageUrl, AuthorId, AuthorName, CreationDate, SearchAll
- [x] Usar tipos nullable para Category y CoverImageUrl

---

## 4. Domain Layer - Repository Interfaces

### 4.1 IAuthorRepository
- [x] Crear archivo `IAuthorRepository.cs` en `domain/interfaces/repositories`
- [x] Heredar de `IRepository<Author, Guid>`
- [x] Agregar método `GetByNameAsync(string name, CancellationToken ct)`

### 4.2 IBookRepository
- [x] Crear archivo `IBookRepository.cs` en `domain/interfaces/repositories`
- [x] Heredar de `IRepository<Book, Guid>`
- [x] Agregar método `GetByIsbnAsync(string isbn, CancellationToken ct)`
- [x] Agregar método `ExistsIsbnAsync(string isbn, CancellationToken ct)`

### 4.3 IBookImageRepository
- [x] Crear archivo `IBookImageRepository.cs` en `domain/interfaces/repositories`
- [x] Heredar de `IRepository<BookImage, Guid>`
- [x] Agregar método `GetByBookIdAsync(Guid bookId, CancellationToken ct)`

### 4.4 IBookDaoRepository
- [x] Crear archivo `IBookDaoRepository.cs` en `domain/interfaces/repositories`
- [x] Heredar de `IReadOnlyRepository<BookDao, Guid>`

### 4.5 Update IUnitOfWork
- [x] Agregar propiedad `IAuthorRepository Authors` en región crud
- [x] Agregar propiedad `IBookRepository Books` en región crud
- [x] Agregar propiedad `IBookImageRepository BookImages` en región crud
- [x] Agregar propiedad `IBookDaoRepository BookDaos` en región read-only

---

## 5. Infrastructure Layer - Mappers

### 5.1 AuthorMapper
- [x] Crear archivo `AuthorMapper.cs` en `infrastructure/nhibernate/mappers`
- [x] Configurar Schema y Table("authors")
- [x] Mapear Id, Name, Country, BirthDate, CreationDate
- [x] Usar convención snake_case para columnas

### 5.2 BookMapper
- [x] Crear archivo `BookMapper.cs` en `infrastructure/nhibernate/mappers`
- [x] Configurar Schema y Table("books")
- [x] Mapear Id, Title, ISBN, AuthorId, Category, CoverImageUrl, CreationDate
- [x] Configurar ISBN como Unique
- [x] Usar convención snake_case para columnas

### 5.3 BookImageMapper
- [x] Crear archivo `BookImageMapper.cs` en `infrastructure/nhibernate/mappers`
- [x] Configurar Schema y Table("book_images")
- [x] Mapear Id, BookId, ImageUrl, CreationDate
- [x] Usar convención snake_case para columnas

### 5.4 BookDaoMapper
- [x] Crear archivo `BookDaoMapper.cs` en `infrastructure/nhibernate/mappers`
- [x] Configurar Schema y Table("books_view")
- [x] Configurar `Mutable(false)` para prevenir escrituras
- [x] Mapear todas las propiedades del BookDao
- [x] Usar convención snake_case para columnas

---

## 6. Infrastructure Layer - Repositories

### 6.1 NHAuthorRepository
- [x] Crear archivo `NHAuthorRepository.cs` en `infrastructure/nhibernate/repositories`
- [x] Heredar de `NHRepository<Author, Guid>`
- [x] Implementar `GetByNameAsync`

### 6.2 NHBookRepository
- [x] Crear archivo `NHBookRepository.cs` en `infrastructure/nhibernate/repositories`
- [x] Heredar de `NHRepository<Book, Guid>`
- [x] Implementar `GetByIsbnAsync`
- [x] Implementar `ExistsIsbnAsync`

### 6.3 NHBookImageRepository
- [x] Crear archivo `NHBookImageRepository.cs` en `infrastructure/nhibernate/repositories`
- [x] Heredar de `NHRepository<BookImage, Guid>`
- [x] Implementar `GetByBookIdAsync`

### 6.4 Update NHUnitOfWork
- [x] Agregar campo privado y propiedad `Authors` usando NHRepository
- [x] Agregar campo privado y propiedad `Books` usando NHRepository
- [x] Agregar campo privado y propiedad `BookImages` usando NHRepository
- [x] Agregar campo privado y propiedad `BookDaos` usando NHReadOnlyRepository

---

## 7. Test Scenarios

### 7.1 Sc020CreateAuthors
- [x] Crear archivo `Sc020CreateAuthors.cs` en `tests/kudos.backend.scenarios`
- [x] Implementar IScenario
- [x] Configurar PreloadScenario como null (sin dependencias)
- [x] Crear 3 autores: García Márquez, Borges, Allende
- [x] Manejar transacción con try/catch y rollback

### 7.2 Sc030CreateBooks
- [x] Crear archivo `Sc030CreateBooks.cs` en `tests/kudos.backend.scenarios`
- [x] Implementar IScenario
- [x] Configurar PreloadScenario como typeof(Sc020CreateAuthors)
- [x] Crear 6 libros (2 por autor)
- [x] Crear 2 BookImages para "Cien años de soledad"
- [x] Manejar transacción con try/catch y rollback

### 7.3 Update ScenarioBuilder
- [x] Registrar AuthorValidator en el contenedor DI
- [x] Registrar BookValidator en el contenedor DI
- [x] Registrar BookImageValidator en el contenedor DI

---

## 8. Application Layer

### 8.1 GetManyAndCountBooksUseCase
- [x] Crear directorio `usecases/books` en `kudos.backend.application`
- [x] Crear archivo `GetManyAndCountBooksUseCase.cs`
- [x] Definir clase Command con propiedad Query (string?)
- [x] Implementar Handler con IUnitOfWork y ILogger
- [x] Llamar `_uoW.BookDaos.GetManyAndCountAsync(query, nameof(BookDao.Title))`
- [x] Manejar transacción con try/catch y rollback

---

## 9. WebApi Layer

### 9.1 BookDto
- [x] Crear archivo `BookDto.cs` en `webapi/dtos`
- [x] Agregar propiedades: Id, Title, ISBN, Category, CoverImageUrl, AuthorId, AuthorName, CreationDate
- [x] Usar tipos nullable para Category y CoverImageUrl

### 9.2 GetManyAndCountBooksModel
- [x] Crear directorio `features/books/models` en webapi
- [x] Crear archivo `GetManyAndCountBooksModel.cs`
- [x] Definir clase Request vacía (patrón APSYS)
- [x] Definir clase Response heredando de `GetManyAndCountResultDto<BookDto>`

### 9.3 GetManyAndCountBooksEndpoint
- [x] Crear directorio `features/books/endpoint` en webapi
- [x] Crear archivo `GetManyAndCountBooksEndpoint.cs`
- [x] Configurar ruta `GET /api/books`
- [x] NO usar Policies() (solo requiere autenticación)
- [x] Configurar Description con tags, nombre y documentación de parámetros
- [x] En HandleAsync: extraer QueryString y construir Command manualmente
- [x] Mapear resultado a Response con AutoMapper
- [x] Manejar errores con try/catch

### 9.4 BookMappingProfile
- [x] Crear archivo `BookMappingProfile.cs` en `webapi/mappingprofiles`
- [x] Mapear BookDao → BookDto
- [x] Mapear GetManyAndCountResult<BookDao> → GetManyAndCountResultDto<BookDto>
