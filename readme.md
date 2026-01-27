# Catálogo de Libros - Backend

## Spec Driven Design (SDD)

Este proyecto es parte de un hands-on-lab diseñado para demostrar el proceso de **Spec Driven Design** - un enfoque donde las especificaciones guían el desarrollo desde el inicio.

## ¿Qué es Spec Driven Design?

SDD es una metodología donde:

1. **Primero se define la especificación** (contratos, esquemas, comportamientos esperados)
2. **Luego se implementa** siguiendo esa especificación como guía
3. **Se valida** que la implementación cumple con la especificación

### Beneficios

- Claridad en los requisitos desde el inicio
- Contratos claros entre frontend y backend
- Documentación viva del sistema
- Desarrollo paralelo de equipos
- Tests derivados de las especificaciones

---

## El Proyecto: Catálogo de Libros

Un sistema para gestionar un catálogo de libros. Permite buscar, visualizar y administrar información de libros con sus autores, categorías y disponibilidad.

### Funcionalidades

- Buscar libros por título, autor o ISBN
- Ver detalle de un libro
- Listar libros por categoría
- Agregar nuevos libros al catálogo
- Editar información de libros existentes
- Marcar libros como favoritos

---

## Stack Tecnológico

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| .NET | 9.0 | Runtime & SDK |
| C# | 13.x | Lenguaje |
| FastEndpoints | 5.x | API Framework |
| NHibernate | 5.x | ORM |
| PostgreSQL | 16.x | Base de Datos |
| AutoMapper | 13.x | Object Mapping |
| FluentResults | 3.x | Result Pattern |
| JWT Bearer | 9.x | Authentication |
| Swagger | 6.x | API Documentation |

---

## Setup del Proyecto

### Prerrequisitos

- .NET SDK >= 9.0
- PostgreSQL >= 16.0
- Git
- Node.js (para OpenSpec CLI)

### 1. Clonar el repositorio

```bash
git clone https://github.com/apsys-mx/sdd-hands-on-lab.backend.git
cd sdd-hands-on-lab.backend
```

### 2. Configurar variables de entorno

Crear archivo `.env` en la raíz del proyecto:

```env
# Database Configuration
DATABASE_CONNECTION_STRING=Host=localhost;Port=5432;Database=books_catalog_db;Username=postgres;Password=your_password

# Auth0 Configuration (Hashira Stone Testing)
AUTH0_AUTHORITY=https://hashira-stone-testing.us.auth0.com/
AUTH0_AUDIENCE=https://hashira-stone-backend.online/api
```

> **Nota:** Estos valores corresponden al tenant de Auth0 configurado para el hands-on lab. Si usas tu propio tenant de Auth0, reemplaza los valores con los de tu aplicación.

### 3. Restaurar dependencias

```bash
dotnet restore
```

### 4. Ejecutar migraciones

```bash
dotnet run --project src/kudos.backend.migrations
```

### 5. Iniciar el servidor de desarrollo

```bash
dotnet run --project src/kudos.backend.webapi
```

La API estará disponible en `https://localhost:8080`

La documentación Swagger estará en `https://localhost:8080/swagger`

---

## Comandos Disponibles

| Comando | Descripción |
|---------|-------------|
| `dotnet restore` | Restaura las dependencias |
| `dotnet build` | Compila la solución |
| `dotnet run --project src/kudos.backend.webapi` | Inicia el servidor |
| `dotnet test` | Ejecuta todos los tests |
| `dotnet test --filter "Category=Unit"` | Ejecuta solo tests unitarios |
| `dotnet test --filter "Category=Integration"` | Ejecuta tests de integración |

---

## Estructura del Proyecto

```
backend/
├── src/
│   ├── kudos.backend.domain/         # Entidades y reglas de negocio
│   ├── kudos.backend.application/    # Casos de uso y servicios
│   ├── kudos.backend.infrastructure/ # Implementaciones (DB, externos)
│   ├── kudos.backend.webapi/         # Endpoints y configuración API
│   └── kudos.backend.migrations/     # Migraciones de base de datos
├── tests/
│   ├── kudos.backend.domain.tests/         # Tests del dominio
│   ├── kudos.backend.application.tests/    # Tests de aplicación
│   ├── kudos.backend.infrastructure.tests/ # Tests de infraestructura
│   ├── kudos.backend.webapi.tests/         # Tests de API
│   ├── kudos.backend.scenarios/            # Tests de escenarios/BDD
│   ├── kudos.backend.common.tests/         # Utilidades compartidas
│   └── kudos.backend.ndbunit/              # Fixtures de base de datos
└── kudos.backend.sln
```

---

## Arquitectura

El proyecto sigue **Clean Architecture** con las siguientes capas:

### Capas

- **Domain**: Entidades, value objects, interfaces de repositorios y reglas de negocio
- **Application**: Casos de uso, DTOs, validaciones y orquestación
- **Infrastructure**: Implementación de repositorios con NHibernate, servicios externos
- **WebAPI**: Endpoints con FastEndpoints, configuración de autenticación y Swagger

### Principios

- Dependency Inversion: Las capas internas no conocen las externas
- Single Responsibility: Cada clase tiene una única razón para cambiar
- Interface Segregation: Interfaces pequeñas y específicas
- Open/Closed: Abierto para extensión, cerrado para modificación

### Flujo de una petición

```
Request → FastEndpoint → Application Service → Domain → Repository → Database
                ↓
Response ← DTO Mapping ← Result ← Domain Entity ← NHibernate ←
```

---

## Testing

El proyecto incluye múltiples niveles de testing:

| Tipo | Proyecto | Propósito |
|------|----------|-----------|
| Unit | `*.domain.tests` | Reglas de negocio |
| Unit | `*.application.tests` | Casos de uso |
| Integration | `*.infrastructure.tests` | Repositorios y DB |
| API | `*.webapi.tests` | Endpoints |
| BDD | `*.scenarios` | Escenarios de negocio |

### Ejecutar tests

```bash
# Todos los tests
dotnet test

# Solo un proyecto
dotnet test tests/kudos.backend.domain.tests

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## Recursos

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [FastEndpoints Documentation](https://fast-endpoints.com)
- [NHibernate Documentation](https://nhibernate.info)
- [Auth0 .NET SDK](https://auth0.com/docs/libraries/auth0-aspnet-core-authentication)

---

## OpenSpec - Spec Driven Design

### Instalar OpenSpec CLI

OpenSpec es la herramienta de línea de comandos para Spec Driven Design:

```bash
npm install -g @fission-ai/openspec@latest
```

Verificar la instalación:

```bash
openspec --version
```

### Inicializar OpenSpec en el proyecto

Ejecutar en la raíz del proyecto:

```bash
openspec init
```

Cuando se solicite seleccionar la herramienta de IA, elegir **Claude Code**.
