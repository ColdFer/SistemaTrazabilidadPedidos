# Prompts Utilizados en el Desarrollo del Proyecto

Sistema Web de Gestión y Trazabilidad de Pedidos – TecnoExpress

---

## 1. Prompt de Análisis Inicial

### Descripción del Prompt

Se solicitó a la IA realizar un análisis completo del proyecto solicitado, definiendo el alcance, los módulos necesarios, las tecnologías a utilizar y la arquitectura general del sistema.

### Contenido del Prompt

> "Necesito desarrollar un sistema web de gestión y trazabilidad de pedidos para una empresa llamada TecnoExpress. El sistema debe tener tres roles principales: Administrador, Operador y Repartidor, más una vista para Clientes. Necesito que analices y definas: la arquitectura del proyecto, las tecnologías ideales, los módulos necesarios por rol, la estructura de base de datos y el flujo de autenticación."

### Resultado Entregado

- Arquitectura multicapa definida (3 proyectos: Shared, Server, Client)
- Stack tecnológico seleccionado (.NET 9, Blazor WebAssembly, ASP.NET Core Web API, SQL Server, EF Core, JWT)
- Lista de módulos por rol con funcionalidades específicas
- Diagrama de entidades y relaciones de la base de datos
- Flujo de autenticación con JWT
- Estructura de carpetas del proyecto completa

---

## 2. Prompt de Desarrollo por Módulos

### Descripción del Prompt

Se desarrolló el sistema módulo por módulo, solicitando a la IA la creación completa de cada uno incluyendo backend, frontend y base de datos.

### Módulo: Autenticación

> "Crea el módulo de autenticación completo: controlador Auth con login y registro, servicio de autenticación con generación de tokens JWT, DTOs de LoginRequest, RegisterRequest y AuthResponse, y la página de Login en Blazor con manejo de errores."

**Entregado:** `AuthController.cs`, `AuthService.cs`, `IAuthService.cs`, `LoginRequest.cs`, `RegisterRequest.cs`, `AuthResponse.cs`, `Login.razor`

### Módulo: Gestión de Usuarios

> "Crea el módulo de gestión de usuarios administrados: controlador con CRUD completo, servicio y repository, DTOs para crear, editar y listar usuarios, y las páginas Razor de Index, Crear y Editar con formularios validados."

**Entregado:** `ManagedUsersController.cs`, `ManagedUserService.cs`, `ManagedUserRepository.cs`, `ManagedUserDto.cs`, `CreateManagedUserRequest.cs`, `UpdateManagedUserRequest.cs`, páginas `Usuarios/Index.razor`, `Crear.razor`, `Editar.razor`

### Módulo: Gestión de Clientes

> "Desarrolla el módulo de clientes con CRUD completo: controller, service, repository, DTOs y páginas Razor para listar, crear y editar clientes."

**Entregado:** `CustomersController.cs`, `CustomerService.cs`, `CustomerRepository.cs`, `CustomerDto.cs`, `CreateCustomerRequest.cs`, `UpdateCustomerRequest.cs`, `Clientes.razor`

### Módulo: Inventario

> "Crea el módulo de inventario que gestione productos, categorías y movimientos de stock. Necesito controlador con endpoints para cada operación, servicio con lógica de validación, repository con queries EF Core, DTOs y página Razor con tabs para productos, categorías e historial de movimientos."

**Entregado:** `InventoryController.cs`, `InventoryService.cs`, `InventoryRepository.cs`, `ProductDto.cs`, `CreateProductRequest.cs`, `UpdateProductRequest.cs`, `CategoryDto.cs`, `CreateCategoryRequest.cs`, `InventoryMovementDto.cs`, `CreateInventoryMovementRequest.cs`, `Inventario.razor`

### Módulo: Despachos

> "Desarrolla el módulo de despachos para asignar repartidores a pedidos, cambiar estados y seguimiento de entregas. Incluye controller, service, repository, DTOs y página Razor."

**Entregado:** `DeliveriesController.cs`, `DeliveryService.cs`, `DeliveryRepository.cs`, `DeliveryDto.cs`, `CreateDeliveryRequest.cs`, `UpdateDeliveryRequest.cs`, `ChangeDeliveryStatusRequest.cs`, `DeliveryDriverDto.cs`, `DeliveryOrderDto.cs`, `DeliveryAddressDto.cs`, `Despachos.razor`

### Módulo: Pedidos

> "Crea el módulo de pedidos con funcionalidad para crear pedidos desde el carrito, listar pedidos, ver detalle y historial de estados. Necesito controller, service, repository, DTOs y páginas Razor."

**Entregado:** `OrdersController.cs`, `OrderService.cs`, `OrderRepository.cs`, `OrderDto.cs`, `OrderDetailDto.cs`, `OrderStatusDto.cs`, `OrderStatusHistoryDto.cs`, `CreateOrderRequest.cs`, páginas `MisPedidos/Index.razor`, `Detalle.razor`

### Módulo: Carrito de Compras

> "Desarrolla el módulo de carrito de compras para clientes: agregar productos, modificar cantidades, eliminar items y confirmar compra. Incluye backend y frontend completo."

**Entregado:** `CartController.cs`, `CartService.cs`, `CartRepository.cs`, `CartDto.cs`, `CartItemDto.cs`, `AddToCartRequest.cs`, `UpdateCartItemRequest.cs`, `Carrito/Index.razor`

### Módulo: Catálogo

> "Crea el módulo de catálogo de productos para clientes: página de listado con filtros y página de detalle de producto."

**Entregado:** `Catalogo/Index.razor`, `Catalogo/Detalle.razor`

### Módulo: Reportes

> "Desarrolla el módulo de reportes con gráficos de ventas, pedidos y distribución de estados. Necesito controller, service y página Razor con visualización de datos."

**Entregado:** `ReportsController.cs`, `ReportService.cs`, `IReportService.cs`, `DashboardReportDto.cs`, `Reportes.razor`

### Módulo: Perfil de Usuario

> "Crea el módulo de perfil para que cada usuario pueda ver y editar sus datos personales."

**Entregado:** `ProfileController.cs`, `ProfileDto.cs`, `UpdateProfileRequest.cs`, `MiPerfil.razor`

---

## 3. Prompt de Creación del Backend

### Descripción del Prompt

Se solicitaron los componentes del backend de forma integral, incluyendo la configuración del proyecto servidor.

### Contenido del Prompt

> "Configura el proyecto ASP.NET Core Web API con: Program.cs con inyección de dependencias para todos los servicios y repositories, configuración de JWT, configuración de SQL Server con EF Core, CORS habilitado, y migración inicial de la base de datos con el esquema de entidades definido."

### Resultado Entregado

- `Program.cs` con todas las configuraciones
- `AppDbContext.cs` con el contexto de Entity Framework y configuración de entidades
- Migración inicial `20260824042721_InitialDB.cs`
- Snapshot de la base de datos `AppDbContextModelSnapshot.cs`
- Registro de todas las dependencias (DI) en el contenedor

---

## 4. Prompt de Creación del Frontend

### Descripción del Prompt

Se solicitó la creación completa del frontend Blazor WebAssembly.

### Contenido del Prompt

> "Crea el proyecto Blazor WebAssembly Hosted con: MainLayout con barra lateral de navegación, NavMenu con menús dinámicos según el rol del usuario, AuthLayout para páginas de autenticación, y todas las páginas Razor de cada módulo con formularios, tablas y validaciones."

### Resultado Entregado

- `MainLayout.razor` – Layout principal con sidebar
- `NavMenu.razor` – Navegación dinámica por rol
- `AuthLayout.razor` – Layout para login/registro
- `App.razor` – Router principal
- `_Imports.razor` – Imports globales
- Todas las páginas Razor de cada módulo

---

## 5. Prompt de Documentación

### Descripción del Prompt

Se solicitaron los 4 archivos de documentación del proyecto.

### Contenido del Prompt

> "Crea los siguientes archivos de documentación en español para el proyecto: 1) README.md profesional con descripción, tecnologías, estructura, arquitectura, funcionalidades por rol y pasos de ejecución. 2) MANUAL_USUARIO.md con instrucciones detalladas para cada rol. 3) MANUAL_TECNICO.md con arquitectura, esquema de BD, endpoints y guía para agregar módulos. 4) PROMPTS_UTILIZADOS.md describiendo los prompts usados en el desarrollo."

### Resultado Entregado

- `README.md` – Documentación principal del proyecto
- `MANUAL_USUARIO.md` – Manual de usuario completo
- `MANUAL_TECNICO.md` – Documentación técnica detallada
- `PROMPTS_UTILIZADOS.md` – Este archivo

---

## Resumen de Eficiencia del Desarrollo con IA

| Fase | Prompts | Archivos Generados |
|---|---|---|
| Análisis inicial | 1 | Arquitectura y diseño |
| Backend (API) | 3 | Controllers, Services, Repositories |
| Frontend (Blazor) | 2 | Pages, Layout, Components |
| Base de datos | 2 | Entities, DbContext, Migrations |
| Documentación | 1 | 4 archivos de documentación |
| **Total** | **9 prompts** | **~80+ archivos de código** |

El uso de IA permitió acelerar significativamente el desarrollo del sistema, generando código estructurado y funcional que sigue las mejores prácticas de arquitectura de software y convenciones de .NET.
