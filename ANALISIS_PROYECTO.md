# ANALISIS DEL PROYECTO - Sistema Web de Gestión y Trazabilidad de Pedidos TecnoExpress

## 1. ESTADO ACTUAL DEL PROYECTO

### Estructura de la Solución
- **TrazabilidadPedidos.Shared** — Entidades, DTOs, Enums
- **TrazabilidadPedidos.Server** — API REST, EF Core, JWT Auth
- **TrazabilidadPedidos.Client** — Blazor WebAssembly

### COMPILACIÓN: OK (0 errores, 0 warnings críticos)

---

## 2. LO QUE EXISTE

### Entidades (Shared/Entities)
| Entidad | Estado |
|---------|--------|
| User | Completa |
| Role | Completa (semilla: Admin, Operator, DeliveryDriver, Customer) |
| Permission | Completa (15 permisos semilla) |
| RolePermission | Completa |
| Customer | Completa (UserId FK a User) |
| Address | Completa (CustomerId FK) |
| DeliveryDriver | Completa (UserId FK) |
| Product | Completa (CategoryId FK) |
| Category | Completa |
| Order | Completa (CustomerId FK, CurrentStatusId FK) |
| OrderDetail | Completa (OrderId FK, ProductId FK) |
| OrderStatus | Completa (sin datos semilla) |
| OrderStatusHistory | Completa |
| InventoryMovement | Completa (ProductId FK, UserId FK) |
| Delivery | Completa (OrderId FK, AddressId FK, DeliveryDriverId FK) |
| Incident | Completa |
| Payment | Completa |

### DTOs (Shared/DTOs)
| DTO | Estado |
|-----|--------|
| AuthResponse, LoginRequest, RegisterRequest | Completos |
| CustomerDto, CreateCustomerRequest, UpdateCustomerRequest | Completos |
| ProductDto, CreateProductRequest, UpdateProductRequest | Completos |
| CategoryDto, CreateCategoryRequest | Completos |
| InventoryMovementDto, CreateInventoryMovementRequest | Completos |
| DeliveryDto, CreateDeliveryRequest, UpdateDeliveryRequest | Completos |
| ChangeDeliveryStatusRequest, DeliveryAddressDto, DeliveryDriverDto, DeliveryOrderDto | Completos |
| ManagedUserDto, CreateManagedUserRequest, UpdateManagedUserRequest | Completos |

### Repositories (Server/Repositories)
| Repository | Estado |
|------------|--------|
| UserRepository | Completo |
| RoleRepository | Completo |
| CustomerRepository | Completo |
| InventoryRepository | Completo |
| DeliveryRepository | Completo |
| ManagedUserRepository | Completo |

### Services (Server/Services)
| Service | Estado |
|---------|--------|
| AuthService | Completo (Login + Register) |
| CustomerService | Completo (CRUD) |
| InventoryService | Completo (CRUD + movimientos) |
| DeliveryService | Completo (CRUD + cambio estado) |
| ManagedUserService | Completo (CRUD usuarios internos) |

### Controllers (Server/Controllers)
| Controller | Ruta | Estado |
|------------|------|--------|
| AuthController | api/Auth | Completo (login + register) |
| CustomersController | api/Customers | Completo (CRUD) |
| InventoryController | api/Inventory | Completo (categories, products, movements) |
| DeliveriesController | api/Deliveries | Completo (CRUD + selectors) |
| ManagedUsersController | api/ManagedUsers | Completo (solo Admin) |
| SystemController | api/System | Info del sistema |

### Client Services
| Service | Estado |
|---------|--------|
| AuthClientService | Completo |
| CustomerClientService | Completo |
| InventoryClientService | Completo |
| DeliveryClientService | Completo |
| ManagedUserClientService | Completo |

### Páginas Razor (Client/Pages)
| Página | Ruta | Estado |
|--------|------|--------|
| Login.razor | /login | Completo |
| Dashboard.razor | /dashboard | Básico (cards placeholder) |
| Clientes.razor | /clientes | CRUD inline completo |
| Usuarios/Index.razor | /usuarios | Lista completa |
| Usuarios/Crear.razor | /usuarios/crear | Formulario completo |
| Usuarios/Editar.razor | /usuarios/editar/{id} | Formulario completo |
| Inventario.razor | /inventario | Tabs (productos/movimientos/categorías) |
| Despachos.razor | /despachos | CRUD inline completo |
| ClienteInicio.razor | /cliente/inicio | Placeholder (sin funcionalidad) |
| RepartidorInicio.razor | /repartidor/inicio | Placeholder (solo texto) |

### Layout
| Componente | Estado |
|------------|--------|
| MainLayout.razor | Completo (sidebar + topbar) |
| NavMenu.razor | Completo (roles: Admin, Operator, Customer, DeliveryDriver) |
| AuthLayout.razor | Completo |

---

## 3. LO QUE FALTA

### Módulos No Implementados

| # | Módulo | Descripción | Prioridad |
|---|--------|-------------|-----------|
| 1 | **Mi Perfil** | Customer edita su nombre, teléfono, dirección | ALTA |
| 2 | **Catálogo** | Vista pública de productos para Customer | ALTA |
| 3 | **Carrito** | Carrito de compras persistente (backend) | ALTA |
| 4 | **Pedidos** | Creación de pedidos con transacción EF Core | ALTA |
| 5 | **Mis Pedidos** | Customer ve sus pedidos con timeline de estados | ALTA |
| 6 | **Mis Despachos** | DeliveryDriver ve sus asignaciones y cambia estado | ALTA |
| 7 | **Reportes** | Dashboard con gráficas (ventas, productos, stock) | MEDIA |

### Backend No Implementado

| # | Componente | Descripción |
|---|------------|-------------|
| 1 | OrderService / OrdersController | Crear pedidos, listar por cliente |
| 2 | CartService / CartController | CRUD del carrito |
| 3 | CatalogService / CatalogController | Productos públicos |
| 4 | ProfileService endpoint | Obtener/actualizar perfil propio |
| 5 | ReportService / ReportController | Datos para gráficas |
| 6 | OrderStatus semilla | Pendiente, Preparando, En camino, Entregado |
| 7 | OrderStatusHistory al cambiar estado | Historial automático |

### Client Services No Implementados

| # | Service | Descripción |
|---|---------|-------------|
| 1 | OrderClientService | Crear pedidos, listar |
| 2 | CartClientService | CRUD carrito |
| 3 | CatalogClientService | Productos públicos |
| 4 | ProfileClientService | Perfil propio |

### Páginas No Implementadas

| # | Página | Ruta |
|---|--------|------|
| 1 | MiPerfil.razor | /mi-perfil |
| 2 | Catalogo/Index.razor | /catalogo |
| 3 | Catalogo/Detalle.razor | /catalogo/{id} |
| 4 | Carrito/Index.razor | /carrito |
| 5 | MisPedidos/Index.razor | /mis-pedidos |
| 6 | MisPedidos/Detalle.razor | /mis-pedidos/{id} |
| 7 | MisDespachos/Index.razor | /mis-despachos |

### Base de Datos

| Item | Estado |
|------|--------|
| OrderStatus semilla | FALTA (Pendiente, Preparando, En camino, Entregado) |
| Tabla Cart/CartItem | FALTA (entidades + migración) |
| Admin semilla (user) | FALTA en DB (solo tiene Register por código) |

---

## 4. MÓDULOS COMPLETOS vs INCOMPLETOS

### COMPLETOS (funcionales)
- Autenticación (Login, Register, JWT, BCrypt)
- Gestión de Usuarios (Admin CRUD)
- Clientes (CRUD con search/filtros)
- Inventario (Productos, Categorías, Movimientos)
- Despachos (CRUD con cambio de estado)
- Layout y Sidebar por roles

### INCOMPLETOS
- ClienteInicio.razor → Solo placeholder, sin funcionalidad real
- RepartidorInicio.razor → Solo texto
- Dashboard → Sin gráficas ni datos reales
- No existe flujo de compra (catálogo → carrito → pedido)
- No existe vista "Mis Pedidos" para Customer
- No existe vista "Mis Despachos" funcional para DeliveryDriver
- No existe Mi Perfil para Customer
- No hay OrderStatus semilla en DB

---

## 5. ARCHIVOS A MODIFICAR

| Archivo | Cambio |
|---------|--------|
| AppDbContext.cs | Agregar DbSet Cart, CartItem. Agregar seed OrderStatus. Configurar tablas Cart/CartItem |
| Program.cs (Server) | Registrar nuevos servicios y repositorios |
| Program.cs (Client) | Registrar nuevos client services |
| NavMenu.razor | Actualizar menú por rol (Catálogo, Carrito, MisPedidos, MiPerfil, MisDespachos, Reportes) |
| Login.razor | Guardar userId en localStorage |
| _Imports.razor | Agregar namespaces necesarios |
| ClienteInicio.razor | Actualizar con cards que enlacen a funcionalidad real |

---

## 6. ARCHIVOS A CREAR

### Shared/DTOs
- Orders/CreateOrderRequest.cs
- Orders/OrderDto.cs
- Orders/OrderDetailDto.cs
- Cart/CartDto.cs
- Cart/CartItemDto.cs
- Cart/AddToCartRequest.cs
- Cart/UpdateCartItemRequest.cs
- Catalog/PublicProductDto.cs
- Profile/UpdateProfileRequest.cs
- Profile/ProfileDto.cs
- Reports/DashboardReportDto.cs
- Reports/SalesByMonthDto.cs
- Reports/TopProductDto.cs

### Shared/Entities
- Cart.cs
- CartItem.cs

### Server/Repositories
- IOrderRepository.cs + OrderRepository.cs
- ICartRepository.cs + CartRepository.cs

### Server/Services
- IOrderService.cs + OrderService.cs
- ICartService.cs + CartService.cs
- IReportService.cs + ReportService.cs

### Server/Controllers
- OrdersController.cs
- CartController.cs
- CatalogController.cs (o extender InventoryController)
- ProfileController.cs
- ReportsController.cs

### Client/Services
- OrderClientService.cs
- CartClientService.cs
- CatalogClientService.cs
- ProfileClientService.cs
- ReportClientService.cs

### Client/Pages
- Catalogo/Index.razor + .razor.css
- Catalogo/Detalle.razor + .razor.css
- Carrito/Index.razor + .razor.css
- MisPedidos/Index.razor + .razor.css
- MisPedidos/Detalle.razor + .razor.css
- MisDespachos/Index.razor + .razor.css
- MiPerfil.razor + .razor.css
- Reportes.razor + .razor.css

---

## 7. RESUMEN DE TRABAJO

- **Total archivos a crear:** ~45
- **Total archivos a modificar:** ~10
- **Módulos nuevos:** 7
- **Compilación actual:** OK
- **Base de datos:** Requiere migración nueva para Cart, CartItem, OrderStatus seed
