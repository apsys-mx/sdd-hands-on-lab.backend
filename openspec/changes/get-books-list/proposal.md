# Proposal: Get Books List

## Why

Se necesita un endpoint que permita obtener el listado de libros del catálogo para mostrar en la interfaz de usuario. Este es el primer endpoint del módulo de libros y servirá como base para funcionalidades futuras (CRUD completo, detalle con imágenes, etc.).

El sistema actualmente no tiene entidades de dominio definidas. Este cambio establece el modelo de datos fundamental (Author, Book, BookImage) siguiendo el patrón de modelo de escritura + modelo de lectura establecido en las guías.

## What Changes

### Modelo de Escritura (Entidades CRUD)

Se crean las entidades de dominio con sus validadores, repositorios y mappers:

- **Author**: Representa un autor de libros
  - Name (requerido)
  - Country (opcional)
  - BirthDate (opcional)

- **Book**: Representa un libro del catálogo
  - Title (requerido)
  - ISBN (requerido, único)
  - AuthorId (requerido, FK a Author)
  - Category (opcional, string simple)
  - CoverImageUrl (opcional)

- **BookImage**: Imágenes adicionales de un libro
  - BookId (FK a Book)
  - ImageUrl (requerido)

### Modelo de Lectura (DAO + Vista SQL)

- **BookDao**: DAO que mapea a una vista SQL con datos aplanados
- **books_view**: Vista SQL que hace JOIN con Author para optimizar lecturas
- El DAO incluye: Title, ISBN, Category, CoverImageUrl, AuthorId, AuthorName, SearchAll

### Endpoint

- **GET /api/books**: Retorna listado paginado de libros con filtrado y ordenamiento
  - Soporta búsqueda por título, ISBN o nombre de autor (SearchAll)
  - Incluye datos del autor como campos planos (authorId, authorName)
  - NO incluye imágenes adicionales (solo en endpoint de detalle futuro)

### Carga de Datos

- Escenarios de prueba para poblar la base de datos con autores y libros de ejemplo

## Capabilities

- **write-model**: Modelo de escritura con entidades Author, Book y BookImage para operaciones CRUD
- **read-model**: Modelo de lectura con BookDao y vista SQL para queries optimizadas
- **books-api**: Endpoint GET /api/books para obtener listado paginado de libros

## Impact

### Por Capa

| Capa | Cambios |
|------|---------|
| Domain | 3 entidades, 3 validadores, 1 DAO, 4 interfaces de repositorio |
| Infrastructure | 4 mappers NHibernate, 3 repositorios, actualizar UnitOfWork |
| Migrations | 3 tablas + 1 vista SQL |
| Application | 1 use case (GetManyAndCountBooks) |
| WebApi | 1 endpoint, 1 DTO, 1 mapping profile |
| Scenarios | 2 escenarios (Authors, Books) |

### Relaciones

```
Author (1) ──────► (N) Book (1) ──────► (N) BookImage
```

## Non-Goals

- **NO** se implementan endpoints de creación, actualización o eliminación (CRUD se hará después)
- **NO** se implementa endpoint de detalle de libro (GET /books/{id})
- **NO** se incluyen imágenes adicionales en el listado (solo en detalle futuro)
- **NO** se valida formato de ISBN (solo unicidad)
- **NO** se implementa categoría como entidad separada (es string simple)
