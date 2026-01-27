# Hands-On Lab: Spec Driven Design con OpenSpec

Este documento contiene los pasos para seguir el hands-on lab de Spec Driven Design.

---

## Paso 1: Instalar OpenSpec CLI

OpenSpec es la herramienta de línea de comandos para Spec Driven Design:

```bash
npm install -g @fission-ai/openspec@latest
```

Verificar la instalación:

```bash
openspec --version
```

---

## Paso 2: Inicializar OpenSpec en el proyecto

Ejecutar en la raíz del proyecto:

```bash
openspec init
```

Cuando se solicite seleccionar la herramienta de IA, elegir **Claude Code**.

---

## Referencia: Comandos OpenSpec (OPSX)

Los comandos OPSX están disponibles en `.claude/commands/opsx/` y permiten gestionar cambios siguiendo el enfoque Spec Driven Design.

| Comando | Descripción |
|---------|-------------|
| `/opsx:explore` | Modo exploración para pensar ideas, investigar problemas y clarificar requisitos. No implementa código, solo analiza y visualiza. |
| `/opsx:new` | Inicia un nuevo cambio creando la estructura de artefactos (proposal, specs, design, tasks). Proceso paso a paso. |
| `/opsx:continue` | Continúa trabajando en un cambio existente, creando el siguiente artefacto en la secuencia. |
| `/opsx:ff` | Fast-forward: crea todos los artefactos necesarios de una sola vez para pasar directo a implementación. |
| `/opsx:apply` | Implementa las tareas definidas en un cambio, marcándolas como completadas conforme avanza. |
| `/opsx:verify` | Verifica que la implementación coincide con los artefactos (specs, tasks, design) antes de archivar. |
| `/opsx:sync` | Sincroniza delta specs de un cambio hacia las specs principales del proyecto. |
| `/opsx:archive` | Archiva un cambio completado, moviéndolo a `openspec/changes/archive/` con fecha. |
| `/opsx:bulk-archive` | Archiva múltiples cambios a la vez, resolviendo conflictos de specs inteligentemente. |
| `/opsx:onboard` | Tutorial guiado que recorre un ciclo completo de OpenSpec con trabajo real en el codebase. |

### Flujo típico de trabajo

```
/opsx:explore → /opsx:new → /opsx:continue (x N) → /opsx:apply → /opsx:verify → /opsx:archive
```

O de forma acelerada:

```
/opsx:ff → /opsx:apply → /opsx:archive
```

### Ejemplo: Implementar búsqueda de libros por ISBN

Supongamos que queremos agregar la funcionalidad de buscar libros por ISBN.

**1. Explorar el problema (opcional)**
```
/opsx:explore buscar libros por ISBN
```
Claude analiza el codebase, identifica dónde encajaría la funcionalidad y hace preguntas para clarificar requisitos.

**2. Crear el cambio**
```
/opsx:new add-search-by-isbn
```
Esto crea la estructura en `openspec/changes/add-search-by-isbn/`.

**3. Crear artefactos uno por uno**
```
/opsx:continue
```
Claude genera el **proposal.md** explicando el "por qué" del cambio.

```
/opsx:continue
```
Claude genera **specs/search-by-isbn/spec.md** con los requisitos detallados:
```markdown
### Requirement: Búsqueda por ISBN
El sistema DEBE permitir buscar un libro por su código ISBN.

#### Scenario: ISBN válido encontrado
- **WHEN** el usuario busca con ISBN "978-3-16-148410-0"
- **THEN** el sistema retorna el libro correspondiente

#### Scenario: ISBN no encontrado
- **WHEN** el usuario busca con ISBN inexistente
- **THEN** el sistema retorna un mensaje indicando que no se encontró
```

```
/opsx:continue
```
Claude genera **design.md** con decisiones técnicas (endpoint, capa de servicio, etc.)

```
/opsx:continue
```
Claude genera **tasks.md** con las tareas de implementación:
```markdown
- [ ] Crear endpoint GET /api/books/isbn/{isbn}
- [ ] Agregar método FindByIsbn en IBookRepository
- [ ] Implementar FindByIsbn en BookRepository
- [ ] Agregar caso de uso SearchBookByIsbnService
- [ ] Agregar tests unitarios
```

**4. Implementar las tareas**
```
/opsx:apply
```
Claude implementa cada tarea, modificando el código y marcándolas como completadas.

**5. Verificar la implementación**
```
/opsx:verify
```
Claude verifica que todo esté completo y coincida con los specs.

**6. Archivar el cambio**
```
/opsx:archive
```
Mueve el cambio a `openspec/changes/archive/2026-01-27-add-search-by-isbn/`.

---

## Siguientes pasos

(Los siguientes pasos del hands-on lab se agregarán aquí)
