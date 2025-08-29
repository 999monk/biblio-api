## BiblioAPI

API RESTful diseñada para la gestión de una biblioteca. Permite administrar libros, socios y préstamos, ofreciendo un sistema completo y ordenado para el control de inventario y membresías.

### Características Principales

-   **Gestión de libros**: Operaciones CRUD completas para el catálogo de libros.
-   **Gestión de socios**: Operaciones CRUD para los socios de la biblioteca.
-   **Control de préstamos**: Registro, renovación y devolución de préstamos.
-   **Autenticación segura**: Sistema de autenticación basado en JSON Web Tokens (JWT).
-   **Tareas en segundo plano**: Hangfire para procesos automatizados (ej. aviso de préstamo registrado vía mail).

### Tecnologías utilizadas

-   **Backend**: C#, .NET 9
-   **Base de datos**: SQL Server, Entity Framework Core
-   **Autenticación**: JWT (JSON Web Tokens)
-   **Hashing de contraseñas**: BCrypt.Net-Next
-   **Tareas en segundo plano**: Hangfire
-   **Notifiación vía mail**: servidor SMTP con MailKit

### Endpoints

#### Autenticación

-   `POST /api/login`: Autentica a un usuario y devuelve un token JWT.

#### Libros (`/api/libros`)

-   `GET /`: Obtiene un listado de todos los libros.
-   `GET /{id}`: Obtiene un libro específico por su ID.
-   `POST /`: Crea un nuevo libro (requiere autenticación).
-   `PUT /{id}`: Actualiza un libro existente (requiere autenticación).
-   `DELETE /{id}`: Elimina un libro (requiere autenticación).

#### Socios (`/api/socios`)

-   `GET /`: Obtiene un listado de todos los socios.
-   `GET /{id}`: Obtiene un socio específico por su ID.
-   `POST /`: Crea un nuevo socio (requiere autenticación).
-   `PUT /{id}`: Actualiza un socio existente (requiere autenticación).
-   `DELETE /{id}`: Elimina un socio (requiere autenticación).

#### Préstamos (`/api/prestamos`)

-   `POST /registrar`: Registra un nuevo préstamo (requiere autenticación).
-   `PUT /renovar/{id}`: Renueva un préstamo existente (requiere autenticación).
-   `PUT /devolver/{id}`: Marca un préstamo como devuelto (requiere autenticación).
-   `GET /vencidos`: Obtiene una lista de los préstamos vencidos (requiere autenticación).

### Balance final

#### Lo que creo que está relativamente bien:
- → Estructura y separación de responsabilidades: arquitectura en capas clásica.
      * Controllers: se dedican exclusivamente a manejar las peticiones y respuestas HTTP.
      * Services: contienen toda la lógica de negocio (las "reglas").
      * Data (DataContext): gestiona la persistencia de datos. 
- → Uso correcto de inyección de dependencias.
- → Manejo asíncrono con Hangfire.
- → Uso de DTOs para modelar los datos que entran y salen.
- → Test unitarios simples.

#### Posibles mejoras a futuro:
- → Validación de datos de entrada.
- → Manejo de errores más específico.
- → Mapear las entidades y DTOs con Automapper.
- → Agregar test que aborden más caminos y funcionalidades.
