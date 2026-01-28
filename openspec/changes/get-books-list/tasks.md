# Tasks: Get Books List

## 1. Migrations

### 1.1 M002CreateAuthorsTable
- [ ] Crear archivo `M002CreateAuthorsTable.cs` en `kudos.backend.migrations`
- [ ] Definir tabla `authors` con columnas: id, name, country, birth_date, creation_date
- [ ] Crear índice `ix_authors_name` en columna name
- [ ] Implementar método Down() para rollback

### 1.2 M003CreateBooksTable
- [ ] Crear archivo `M003CreateBooksTable.cs` en `kudos.backend.migrations`
- [ ] Definir tabla `books` con columnas: id, title, isbn, author_id, category, cover_image_url, creation_date
- [ ] Agregar constraint UNIQUE en columna isbn
- [ ] Crear FK `fk_books_author_id` a tabla authors
- [ ] Crear índices: ix_books_title, ix_books_isbn, ix_books_author_id
- [ ] Implementar método Down() para rollback

### 1.3 M004CreateBookImagesTable
- [ ] Crear archivo `M004CreateBookImagesTable.cs` en `kudos.backend.migrations`
- [ ] Definir tabla `book_images` con columnas: id, book_id, image_url, creation_date
- [ ] Crear FK `fk_book_images_book_id` con ON DELETE CASCADE
- [ ] Crear índice `ix_book_images_book_id`
- [ ] Implementar método Down() para rollback

### 1.4 M005CreateBooksView
- [ ] Crear archivo `M005CreateBooksView.cs` en `kudos.backend.migrations`
- [ ] Crear vista `books_view` con JOIN a authors
- [ ] Incluir campo calculado `search_all` (CONCAT de title, isbn, author_name)
- [ ] Implementar método Down() con DROP VIEW

---

## 2. Domain Layer - Entities

### 2.1 Author Entity
- [ ] Crear archivo `Author.cs` en `domain/entities`
- [ ] Heredar de `AbstractDomainObject<Guid>`
- [ ] Agregar propiedades: Name, Country, BirthDate
- [ ] Implementar constructor con parámetro Name requerido
- [ ] Implementar método `GetValidator()` retornando `AuthorValidator`

### 2.2 AuthorValidator
- [ ] Crear archivo `AuthorValidator.cs` en `domain/validators`
- [ ] Validar Name: NotEmpty, MaxLength(200)
- [ ] Validar Country: MaxLength(100) cuando tiene valor
- [ ] Validar BirthDate: LessThan(UtcNow) cuando tiene valor

### 2.3 Book Entity
- [ ] Crear archivo `Book.cs` en `domain/entities`
- [ ] Heredar de `AbstractDomainObject<Guid>`
- [ ] Agregar propiedades: Title, ISBN, AuthorId, Category, CoverImageUrl
- [ ] Implementar constructor con Title, ISBN, AuthorId requeridos
- [ ] Implementar método `GetValidator()` retornando `BookValidator`

### 2.4 BookValidator
- [ ] Crear archivo `BookValidator.cs` en `domain/validators`
- [ ] Validar Title: NotEmpty, MaxLength(300)
- [ ] Validar ISBN: NotEmpty, MaxLength(20)
- [ ] Validar AuthorId: NotEmpty
- [ ] Validar Category: MaxLength(100) cuando tiene valor
- [ ] Validar CoverImageUrl: MaxLength(500) cuando tiene valor

### 2.5 BookImage Entity
- [ ] Crear archivo `BookImage.cs` en `domain/entities`
- [ ] Heredar de `AbstractDomainObject<Guid>`
- [ ] Agregar propiedades: BookId, ImageUrl
- [ ] Implementar constructor con BookId, ImageUrl requeridos
- [ ] Implementar método `GetValidator()` retornando `BookImageValidator`

### 2.6 BookImageValidator
- [ ] Crear archivo `BookImageValidator.cs` en `domain/validators`
- [ ] Validar BookId: NotEmpty
- [ ] Validar ImageUrl: NotEmpty, MaxLength(500)

---

## 3. Domain Layer - DAO

### 3.1 BookDao
- [ ] Crear archivo `BookDao.cs` en `domain/daos`
- [ ] NO heredar de AbstractDomainObject (es POCO simple)
- [ ] Agregar propiedades virtuales: Id, Title, ISBN, Category, CoverImageUrl, AuthorId, AuthorName, CreationDate, SearchAll
- [ ] Usar tipos nullable para Category y CoverImageUrl

---

## 4. Domain Layer - Repository Interfaces

### 4.1 IAuthorRepository
- [ ] Crear archivo `IAuthorRepository.cs` en `domain/interfaces/repositories`
- [ ] Heredar de `IRepository<Author, Guid>`
- [ ] Agregar método `GetByNameAsync(string name, CancellationToken ct)`

### 4.2 IBookRepository
- [ ] Crear archivo `IBookRepository.cs` en `domain/interfaces/repositories`
- [ ] Heredar de `IRepository<Book, Guid>`
- [ ] Agregar método `GetByIsbnAsync(string isbn, CancellationToken ct)`
- [ ] Agregar método `ExistsIsbnAsync(string isbn, CancellationToken ct)`

### 4.3 IBookImageRepository
- [ ] Crear archivo `IBookImageRepository.cs` en `domain/interfaces/repositories`
- [ ] Heredar de `IRepository<BookImage, Guid>`
- [ ] Agregar método `GetByBookIdAsync(Guid bookId, CancellationToken ct)`

### 4.4 IBookDaoRepository
- [ ] Crear archivo `IBookDaoRepository.cs` en `domain/interfaces/repositories`
- [ ] Heredar de `IReadOnlyRepository<BookDao, Guid>`

### 4.5 Update IUnitOfWork
- [ ] Agregar propiedad `IAuthorRepository Authors` en región crud
- [ ] Agregar propiedad `IBookRepository Books` en región crud
- [ ] Agregar propiedad `IBookImageRepository BookImages` en región crud
- [ ] Agregar propiedad `IBookDaoRepository BookDaos` en región read-only

---

## 5. Infrastructure Layer - Mappers

### 5.1 AuthorMapper
- [ ] Crear archivo `AuthorMapper.cs` en `infrastructure/nhibernate/mappers`
- [ ] Configurar Schema y Table("authors")
- [ ] Mapear Id, Name, Country, BirthDate, CreationDate
- [ ] Usar convención snake_case para columnas

### 5.2 BookMapper
- [ ] Crear archivo `BookMapper.cs` en `infrastructure/nhibernate/mappers`
- [ ] Configurar Schema y Table("books")
- [ ] Mapear Id, Title, ISBN, AuthorId, Category, CoverImageUrl, CreationDate
- [ ] Configurar ISBN como Unique
- [ ] Usar convención snake_case para columnas

### 5.3 BookImageMapper
- [ ] Crear archivo `BookImageMapper.cs` en `infrastructure/nhibernate/mappers`
- [ ] Configurar Schema y Table("book_images")
- [ ] Mapear Id, BookId, ImageUrl, CreationDate
- [ ] Usar convención snake_case para columnas

### 5.4 BookDaoMapper
- [ ] Crear archivo `BookDaoMapper.cs` en `infrastructure/nhibernate/mappers`
- [ ] Configurar Schema y Table("books_view")
- [ ] Configurar `Mutable(false)` para prevenir escrituras
- [ ] Mapear todas las propiedades del BookDao
- [ ] Usar convención snake_case para columnas

---

## 6. Infrastructure Layer - Repositories

### 6.1 NHAuthorRepository
- [ ] Crear archivo `NHAuthorRepository.cs` en `infrastructure/nhibernate/repositories`
- [ ] Heredar de `NHRepository<Author, Guid>`
- [ ] Implementar `GetByNameAsync`

### 6.2 NHBookRepository
- [ ] Crear archivo `NHBookRepository.cs` en `infrastructure/nhibernate/repositories`
- [ ] Heredar de `NHRepository<Book, Guid>`
- [ ] Implementar `GetByIsbnAsync`
- [ ] Implementar `ExistsIsbnAsync`

### 6.3 NHBookImageRepository
- [ ] Crear archivo `NHBookImageRepository.cs` en `infrastructure/nhibernate/repositories`
- [ ] Heredar de `NHRepository<BookImage, Guid>`
- [ ] Implementar `GetByBookIdAsync`

### 6.4 Update NHUnitOfWork
- [ ] Agregar campo privado y propiedad `Authors` usando NHRepository
- [ ] Agregar campo privado y propiedad `Books` usando NHRepository
- [ ] Agregar campo privado y propiedad `BookImages` usando NHRepository
- [ ] Agregar campo privado y propiedad `BookDaos` usando NHReadOnlyRepository

---

## 7. Test Scenarios

### 7.1 Sc020CreateAuthors
- [ ] Crear archivo `Sc020CreateAuthors.cs` en `tests/kudos.backend.scenarios`
- [ ] Implementar IScenario
- [ ] Configurar PreloadScenario como null (sin dependencias)
- [ ] Crear 3 autores: García Márquez, Borges, Allende
- [ ] Manejar transacción con try/catch y rollback

### 7.2 Sc030CreateBooks
- [ ] Crear archivo `Sc030CreateBooks.cs` en `tests/kudos.backend.scenarios`
- [ ] Implementar IScenario
- [ ] Configurar PreloadScenario como typeof(Sc020CreateAuthors)
- [ ] Crear 6 libros (2 por autor)
- [ ] Crear 2 BookImages para "Cien años de soledad"
- [ ] Manejar transacción con try/catch y rollback

### 7.3 Update ScenarioBuilder
- [ ] Registrar AuthorValidator en el contenedor DI
- [ ] Registrar BookValidator en el contenedor DI
- [ ] Registrar BookImageValidator en el contenedor DI

---

## 8. Application Layer

### 8.1 GetManyAndCountBooksUseCase
- [ ] Crear directorio `usecases/books` en `kudos.backend.application`
- [ ] Crear archivo `GetManyAndCountBooksUseCase.cs`
- [ ] Definir clase Command con propiedad Query (string?)
- [ ] Implementar Handler con IUnitOfWork y ILogger
- [ ] Llamar `_uoW.BookDaos.GetManyAndCountAsync(query, nameof(BookDao.Title))`
- [ ] Manejar transacción con try/catch y rollback

---

## 9. WebApi Layer

### 9.1 BookDto
- [ ] Crear archivo `BookDto.cs` en `webapi/dtos`
- [ ] Agregar propiedades: Id, Title, ISBN, Category, CoverImageUrl, AuthorId, AuthorName, CreationDate
- [ ] Usar tipos nullable para Category y CoverImageUrl

### 9.2 GetManyAndCountBooksModel
- [ ] Crear directorio `features/books/models` en webapi
- [ ] Crear archivo `GetManyAndCountBooksModel.cs`
- [ ] Definir clase Request vacía (patrón APSYS)
- [ ] Definir clase Response heredando de `GetManyAndCountResultDto<BookDto>`

### 9.3 GetManyAndCountBooksEndpoint
- [ ] Crear directorio `features/books/endpoint` en webapi
- [ ] Crear archivo `GetManyAndCountBooksEndpoint.cs`
- [ ] Configurar ruta `GET /api/books`
- [ ] NO usar Policies() (solo requiere autenticación)
- [ ] Configurar Description con tags, nombre y documentación de parámetros
- [ ] En HandleAsync: extraer QueryString y construir Command manualmente
- [ ] Mapear resultado a Response con AutoMapper
- [ ] Manejar errores con try/catch

### 9.4 BookMappingProfile
- [ ] Crear archivo `BookMappingProfile.cs` en `webapi/mappingprofiles`
- [ ] Mapear BookDao → BookDto
- [ ] Mapear GetManyAndCountResult<BookDao> → GetManyAndCountResultDto<BookDto>

