# Manual Técnico – TecnoExpress

Sistema Web de Gestión y Trazabilidad de Pedidos

---

## 1. Visión General de la Arquitectura

El sistema utiliza una arquitectura **multicapa** con separación clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────────────┐
│                       PRESENTACIÓN                              │
│              Blazor WebAssembly (Client Project)                │
│  ┌─────────────┐  ┌──────────────┐  ┌────────────────────────┐ │
│  │ Razor Pages  │  │ Client Svc   │  │ Layout / NavMenu       │ │
│  └──────┬──────┘  └──────┬───────┘  └────────────────────────┘ │
└─────────┼────────────────┼──────────────────────────────────────┘
          │                │  HTTP/JSON
┌─────────▼────────────────▼──────────────────────────────────────┐
│                       API REST                                  │
│              ASP.NET Core Web API (Server Project)              │
│  ┌────────────┐  ┌────────────┐  ┌──────────────┐              │
│  │ Controllers │→ │ Services   │→ │ Repositories │              │
│  └────────────┘  └────────────┘  └──────┬───────┘              │
└──────────────────────────────────────────┼──────────────────────┘
                                           │
┌──────────────────────────────────────────▼──────────────────────┐
│                       DATOS                                     │
│              Entity Framework Core + SQL Server                 │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐      │
│  │   DbContext   │  │  Migrations  │  │  Base de Datos   │      │
│  └──────────────┘  └──────────────┘  └──────────────────┘      │
└─────────────────────────────────────────────────────────────────┘
```

### Capas

| Capa | Responsabilidad | Ubicación |
|---|---|---|
| **DTO** | Objetos de transferencia para comunicar capas | `TrazabilidadPedidos.Shared/DTOs/` |
| **Service** | Lógica de negocio, validaciones, reglas | `TrazabilidadPedidos.Server/Services/` |
| **Repository** | Acceso a datos, queries EF Core | `TrazabilidadPedidos.Server/Repositories/` |
| **Controller** | Endpoints HTTP, manejo de requests | `TrazabilidadPedidos.Server/Controllers/` |
| **Client Service** | Servicios HTTP en el frontend | `TrazabilidadPedidos.Client/Services/` |
| **Razor Page** | Interfaces de usuario y componentes | `TrazabilidadPedidos.Client/Pages/` |

---

## 2. Stack Tecnológico

| Componente | Tecnología | Versión |
|---|---|---|
| Runtime | .NET | 9.0 |
| Frontend | Blazor WebAssembly (Hosted) | - |
| Backend API | ASP.NET Core Web API | 9.0 |
| ORM | Entity Framework Core | 9.0 |
| Base de datos | SQL Server | - |
| Autenticación | JWT Bearer | - |
| API Docs | OpenAPI | - |
| CORS | Política AllowAllOrigins | - |

---

## 3. Estructura del Proyecto

```
TrazabilidadPedidos/
├── TrazabilidadPedidos.Shared/
│   ├── Entities/                    # Modelos de dominio (19 entidades)
│   │   ├── Address.cs
│   │   ├── Cart.cs
│   │   ├── CartItem.cs
│   │   ├── Category.cs
│   │   ├── Customer.cs
│   │   ├── Delivery.cs
│   │   ├── DeliveryDriver.cs
│   │   ├── Incident.cs
│   │   ├── InventoryMovement.cs
│   │   ├── Order.cs
│   │   ├── OrderDetail.cs
│   │   ├── OrderStatus.cs
│   │   ├── OrderStatusHistory.cs
│   │   ├── Payment.cs
│   │   ├── Permission.cs
│   │   ├── Product.cs
│   │   ├── Role.cs
│   │   ├── RolePermission.cs
│   │   └── User.cs
│   ├── DTOs/                       # Objetos de transferencia
│   │   ├── AuthResponse.cs
│   │   ├── LoginRequest.cs
│   │   ├── RegisterRequest.cs
│   │   ├── Cart/
│   │   ├── Customers/
│   │   ├── Dispatches/
│   │   ├── Inventory/
│   │   ├── Orders/
│   │   ├── Profile/
│   │   ├── Reports/
│   │   └── Users/
│   └── Enums/                      # Enumeraciones
│       ├── DeliveryStatus.cs
│       ├── MovementType.cs
│       ├── PaymentMethod.cs
│       └── PaymentStatus.cs
│
├── TrazabilidadPedidos.Server/
│   ├── Controllers/                 # Endpoints HTTP (10 controllers)
│   │   ├── AuthController.cs
│   │   ├── CartController.cs
│   │   ├── CustomersController.cs
│   │   ├── DeliveriesController.cs
│   │   ├── InventoryController.cs
│   │   ├── ManagedUsersController.cs
│   │   ├── OrdersController.cs
│   │   ├── ProfileController.cs
│   │   ├── ReportsController.cs
│   │   └── SystemController.cs
│   ├── Services/                    # Lógica de negocio
│   │   ├── AuthService.cs
│   │   ├── CartService.cs
│   │   ├── CustomerService.cs
│   │   ├── DeliveryService.cs
│   │   ├── InventoryService.cs
│   │   ├── ManagedUserService.cs
│   │   ├── OrderService.cs
│   │   ├── ReportService.cs
│   │   └── ApplicationInfoService.cs
│   ├── Repositories/                # Acceso a datos
│   │   ├── UserRepository.cs
│   │   ├── RoleRepository.cs
│   │   ├── CustomerRepository.cs
│   │   ├── DeliveryRepository.cs
│   │   ├── InventoryRepository.cs
│   │   ├── ManagedUserRepository.cs
│   │   ├── OrderRepository.cs
│   │   └── CartRepository.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   └── Migrations/
│       ├── AppDbContextModelSnapshot.cs
│       └── 20260824042721_InitialDB.cs
│
└── TrazabilidadPedidos.Client/
    ├── Layout/
    │   ├── MainLayout.razor
    │   ├── NavMenu.razor
    │   └── AuthLayout.razor
    ├── Pages/
    │   ├── Login.razor
    │   ├── Dashboard.razor
    │   ├── Home.razor
    │   ├── Usuarios/
    │   ├── Clientes.razor
    │   ├── Inventario.razor
    │   ├── Despachos.razor
    │   ├── Reportes.razor
    │   ├── Catalogo/
    │   ├── Carrito/
    │   ├── MisPedidos/
    │   ├── MisDespachos/
    │   ├── MiPerfil.razor
    │   ├── ClienteInicio.razor
    │   └── RepartidorInicio.razor
    ├── App.razor
    └── _Imports.razor
```

---

## 4. Esquema de Base de Datos

### Entidades Principales

| Entidad | Descripción | Relaciones |
|---|---|---|
| **User** | Usuarios del sistema | → Role |
| **Role** | Roles (Admin, Operator, DeliveryDriver, Customer) | ← User |
| **Permission** | Permisos del sistema | ← RolePermission |
| **RolePermission** | Relación Rol-Permiso | → Role, → Permission |
| **Customer** | Datos de clientes | ← User, → Order |
| **Product** | Productos del catálogo | → Category |
| **Category** | Categorías de productos | ← Product |
| **InventoryMovement** | Movimientos de stock | → Product |
| **Order** | Pedidos realizados | → Customer, → OrderDetail |
| **OrderDetail** | Detalle de productos en pedido | → Order, → Product |
| **OrderStatus** | Estados del pedido | ← Order |
| **OrderStatusHistory** | Historial de cambios de estado | → Order |
| **Delivery** | Despachos/entregas | → Order, → DeliveryDriver |
| **DeliveryDriver** | Datos de repartidores | ← Delivery |
| **Payment** | Pagos asociados a pedidos | → Order |
| **Cart** | Carrito de compras | → Customer |
| **CartItem** | Items del carrito | → Cart, → Product |
| **Address** | Direcciones de entrega | → Customer |
| **Incident** | Incidencias reportadas | → Delivery |

### Diagrama de Relaciones

```
User ──────── Role ──────── Permission
                  │
Customer ────────┘
  │
  ├── Order ──── OrderStatus
  │     │
  │     ├── OrderDetail ──── Product ──── Category
  │     │
  │     └── OrderStatusHistory
  │
  ├── Cart ──── CartItem ──── Product
  │
  └── Address

Delivery ──── Order
Delivery ──── DeliveryDriver

Payment ──── Order

Incident ──── Delivery
```

### Enums

| Enum | Valores |
|---|---|
| `DeliveryStatus` | Pendiente, EnCamino, Entregado, Fallido |
| `MovementType` | Entrada, Salida |
| `PaymentMethod` | Efectivo, Tarjeta, Transferencia |
| `PaymentStatus` | Pendiente, Completado, Fallido |

---

## 5. Endpoints de la API

### Autenticación

| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/auth/login` | Inicio de sesión |
| POST | `/api/auth/register` | Registro de usuario |

### Usuarios

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/managedusers` | Listar usuarios |
| GET | `/api/managedusers/{id}` | Obtener usuario por ID |
| POST | `/api/managedusers` | Crear usuario |
| PUT | `/api/managedusers/{id}` | Actualizar usuario |
| DELETE | `/api/managedusers/{id}` | Eliminar usuario |

### Clientes

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/customers` | Listar clientes |
| GET | `/api/customers/{id}` | Obtener cliente por ID |
| POST | `/api/customers` | Crear cliente |
| PUT | `/api/customers/{id}` | Actualizar cliente |
| DELETE | `/api/customers/{id}` | Eliminar cliente |

### Inventario

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/inventory/products` | Listar productos |
| GET | `/api/inventory/products/{id}` | Obtener producto |
| POST | `/api/inventory/products` | Crear producto |
| PUT | `/api/inventory/products/{id}` | Actualizar producto |
| DELETE | `/api/inventory/products/{id}` | Eliminar producto |
| GET | `/api/inventory/categories` | Listar categorías |
| POST | `/api/inventory/categories` | Crear categoría |
| PUT | `/api/inventory/categories/{id}` | Actualizar categoría |
| DELETE | `/api/inventory/categories/{id}` | Eliminar categoría |
| POST | `/api/inventory/movements` | Registrar movimiento |
| GET | `/api/inventory/movements` | Listar movimientos |

### Pedidos

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/orders` | Listar pedidos |
| GET | `/api/orders/{id}` | Obtener pedido |
| POST | `/api/orders` | Crear pedido |
| PUT | `/api/orders/{id}` | Actualizar pedido |
| DELETE | `/api/orders/{id}` | Eliminar pedido |
| GET | `/api/orders/{id}/history` | Historial de estados |

### Despachos

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/deliveries` | Listar despachos |
| GET | `/api/deliveries/{id}` | Obtener despacho |
| POST | `/api/deliveries` | Crear despacho |
| PUT | `/api/deliveries/{id}` | Actualizar despacho |
| DELETE | `/api/deliveries/{id}` | Eliminar despacho |
| PUT | `/api/deliveries/{id}/status` | Cambiar estado |

### Carrito

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/cart` | Obtener carrito del usuario |
| POST | `/api/cart/items` | Agregar item al carrito |
| PUT | `/api/cart/items/{id}` | Actualizar cantidad |
| DELETE | `/api/cart/items/{id}` | Eliminar item |

### Perfil

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/profile` | Obtener perfil |
| PUT | `/api/profile` | Actualizar perfil |

### Reportes

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/reports/dashboard` | Datos del dashboard |

### Sistema

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/system/info` | Información del sistema |

---

## 6. Flujo de Autenticación (JWT)

```
┌──────────┐         ┌──────────┐         ┌──────────┐
│  Client  │         │  Server  │         │   DB     │
└────┬─────┘         └────┬─────┘         └────┬─────┘
     │  POST /auth/login  │                    │
     │───────────────────→│                    │
     │                    │  Validar credenciales│
     │                    │───────────────────→│
     │                    │←───────────────────│
     │                    │  Generar JWT        │
     │  Token JWT         │                    │
     │←───────────────────│                    │
     │                    │                    │
     │  GET /api/*        │                    │
     │  Header: Bearer    │                    │
     │───────────────────→│                    │
     │                    │  Validar token      │
     │                    │  Verificar permisos  │
     │  Respuesta         │                    │
     │←───────────────────│                    │
```

### Configuración JWT

```json
{
  "JwtSettings": {
    "Issuer": "TecnoExpress",
    "Audience": "TecnoExpressUsers",
    "SecretKey": "TuClaveSecretaSuperSegura1234567890!"
  }
}
```

El token JWT se genera en `AuthService` y contiene:
- **Email** del usuario
- **Role** del usuario
- **Expiration** (tiempo de vida)

---

## 7. Cómo Agregar un Nuevo Módulo

### Paso 1: Crear la Entidad

Crear archivo en `TrazabilidadPedidos.Shared/Entities/`:

```csharp
namespace TrazabilidadPedidos.Shared.Entities
{
    public class NuevoModulo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        // Otros campos necesarios
    }
}
```

### Paso 2: Crear los DTOs

Crear carpeta en `TrazabilidadPedidos.Shared/DTOs/NuevoModulo/`:

```csharp
// NuevoModuloDto.cs
public class NuevoModuloDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

// CreateNuevoModuloRequest.cs
public class CreateNuevoModuloRequest
{
    public string Nombre { get; set; } = string.Empty;
}

// UpdateNuevoModuloRequest.cs
public class UpdateNuevoModuloRequest
{
    public string Nombre { get; set; } = string.Empty;
}
```

### Paso 3: Crear el Repository

Crear interfaz y implementación en `TrazabilidadPedidos.Server/Repositories/`:

```csharp
// INuevoModuloRepository.cs
public interface INuevoModuloRepository
{
    Task<IEnumerable<NuevoModulo>> GetAllAsync();
    Task<NuevoModulo?> GetByIdAsync(int id);
    Task<NuevoModulo> CreateAsync(NuevoModulo entity);
    Task<NuevoModulo?> UpdateAsync(int id, NuevoModulo entity);
    Task<bool> DeleteAsync(int id);
}

// NuevoModuloRepository.cs
public class NuevoModuloRepository : INuevoModuloRepository
{
    private readonly AppDbContext _context;
    // Implementación de métodos...
}
```

### Paso 4: Crear el Service

Crear interfaz y implementación en `TrazabilidadPedidos.Server/Services/`:

```csharp
// INuevoModuloService.cs
public interface INuevoModuloService
{
    Task<IEnumerable<NuevoModuloDto>> GetAllAsync();
    Task<NuevoModuloDto?> GetByIdAsync(int id);
    Task<NuevoModuloDto> CreateAsync(CreateNuevoModuloRequest request);
    Task<NuevoModuloDto?> UpdateAsync(int id, UpdateNuevoModuloRequest request);
    Task<bool> DeleteAsync(int id);
}

// NuevoModuloService.cs
public class NuevoModuloService : INuevoModuloService
{
    private readonly INuevoModuloRepository _repository;
    // Implementación de métodos...
}
```

### Paso 5: Crear el Controller

Crear en `TrazabilidadPedidos.Server/Controllers/`:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NuevoModuloController : ControllerBase
{
    private readonly INuevoModuloService _service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NuevoModuloDto>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NuevoModuloDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<NuevoModuloDto>> Create(CreateNuevoModuloRequest request)
    {
        var item = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NuevoModuloDto>> Update(int id, UpdateNuevoModuloRequest request)
    {
        var item = await _service.UpdateAsync(id, request);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
```

### Paso 6: Registrar en Program.cs

Agregar las dependencias en `Program.cs`:

```csharp
builder.Services.AddScoped<INuevoModuloRepository, NuevoModuloRepository>();
builder.Services.AddScoped<INuevoModuloService, NuevoModuloService>();
```

### Paso 7: Crear Migración

```bash
dotnet ef migrations add AddNuevoModulo --project TrazabilidadPedidos.Server
dotnet ef database update --project TrazabilidadPedidos.Server
```

### Paso 8: Crear Páginas en el Cliente

Crear en `TrazabilidadPedidos.Client/Pages/NuevoModulo/`:

- `Index.razor` – Lista principal
- `Crear.razor` – Formulario de creación
- `Editar.razor` – Formulario de edición

### Paso 9: Agregar Navegación

Actualizar `NavMenu.razor` con el enlace al nuevo módulo.

---

## 8. Convenciones de Nomenclatura de Archivos

| Tipo | Convención | Ejemplo |
|---|---|---|
| **Entidad** | PascalCase singular | `Product.cs`, `Order.cs` |
| **DTO** | PascalCase con sufijo | `ProductDto.cs`, `CreateProductRequest.cs` |
| **Enum** | PascalCase singular | `DeliveryStatus.cs`, `MovementType.cs` |
| **Interface** | Prefijo `I` + PascalCase | `IProductService.cs`, `IProductRepository.cs` |
| **Service** | PascalCase + sufijo `Service` | `ProductService.cs` |
| **Repository** | PascalCase + sufijo `Repository` | `ProductRepository.cs` |
| **Controller** | PascalCase + sufijo `Controller` | `ProductsController.cs` |
| **Razor Page** | PascalCase en inglés | `Inventario.razor`, `Despachos.razor` |
| **Carpeta** | PascalCase en inglés | `Entities/`, `DTOs/`, `Services/` |

### Convenciones de Código

- **Namespaces:** Siguen la estructura de carpetas (`TrazabilidadPedidos.Shared.Entities`)
- **Inyección de dependencias:** Constructor con interfaces (`private readonly IService _service`)
- **Acceso a datos:** Siempre a través de Repository, nunca directamente en Controllers
- **Endpoints HTTP:** `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`
- **Autenticación:** `[Authorize]` en controllers, `[AllowAnonymous]` en endpoints públicos
- **Async/Await:** Todos los métodos de acceso a datos y servicios son asíncronos
