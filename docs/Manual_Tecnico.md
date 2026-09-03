# Manual Tecnico - TecnoExpress ERP

## 1. Requisitos Previos

- .NET 9 SDK
- SQL Server (Express o superior)
- Entity Framework Core tools (`dotnet tool install --global dotnet-ef`)
- Node.js (opcional, para desarrollo)

## 2. Estructura del Proyecto

### TrazabilidadPedidos.Shared
- **Entities/**: Modelos de dominio (Product, Order, Customer, etc.)
- **DTOs/**: Data Transfer Objects organizados por modulo
- **Enums/**: enumeraciones (PaymentMethod, DeliveryStatus, etc.)

### TrazabilidadPedidos.Server
- **Controllers/**: Endpoints REST con autorizacion JWT
- **Services/**: Logica de negocio (interfaces + implementaciones)
- **Repositories/**: Acceso a datos via EF Core
- **Data/**: AppDbContext con 22+ DbSets
- **Migrations/**: Migraciones de base de datos

### TrazabilidadPedidos.Client
- **Pages/**: Paginas Razor por modulo
- **Components/**: Componentes reutilizables (Maps, etc.)
- **Services/**: Client Services para llamadas HTTP
- **Layout/**: Layout principal y sidebar

## 3. Base de Datos

### Entidades Principales
```
User -> Customer -> Order -> OrderDetail -> Product
                       -> Payment
                       -> Delivery -> Address
                       -> OrderStatusHistory
                       -> Invoice -> InvoiceDetail

User -> DeliveryDriver -> Delivery
User -> Role -> Permission (via RolePermission)

Cart -> CartItem
Notification
AuditLog
```

### Migraciones
```bash
# Crear migracion
dotnet ef migrations add <NombreMigracion> --project TrazabilidadPedidos.Server

# Aplicar migraciones
dotnet ef database update --project TrazabilidadPedidos.Server

# Revertir ultima migracion
dotnet ef database update <MigrationAnterior> --project TrazabilidadPedidos.Server
```

## 4. Autenticacion y Autorizacion

### JWT
- Header: `Authorization: Bearer <token>`
- Payload: UserId, Email, Role
- Expiracion: 8 horas

### Roles
- Administrator: Acceso total
- Operator: Operaciones diarias
- DeliveryDriver: Despachos
- Customer: Compras propias

### Permisos
- Sistema de permisos granular por modulo
- Endpoint: `GET /api/Auth/my-permissions`
- Sidebar se filtra por permisos del usuario

## 5. Endpoints Principales

### Auth
- `POST /api/Auth/login` - Inicio de sesion
- `POST /api/Auth/register` - Registro de cliente
- `GET /api/Auth/my-permissions` - Permisos del usuario

### Orders
- `GET /api/Orders` - Todos los pedidos (Admin/Operator)
- `GET /api/Orders/my-orders` - Pedidos del cliente
- `POST /api/Orders/from-cart` - Crear desde carrito
- `PUT /api/Orders/{id}/status` - Cambiar estado
- `POST /api/Orders/{id}/accept` - Aceptar pedido
- `GET /api/Orders/{id}/history` - Historial real

### Payments
- `GET /api/Payments` - Todos los pagos
- `POST /api/Payments` - Crear pago
- `PUT /api/Payments/{id}/verify` - Verificar pago

### Reports
- `GET /api/Reports/dashboard` - Dashboard general
- `GET /api/Reports/sales-by-period` - Ventas por periodo
- `GET /api/Reports/top-products` - Productos top
- `GET /api/Reports/orders-by-status` - Pedidos por estado
- `GET /api/Reports/top-customers` - Clientes frecuentes
- `GET /api/Reports/driver-performance` - Rendimiento repartidores
- `GET /api/Reports/inventario` - Inventario

### Roles & Permissions
- `GET /api/Roles` - Listar roles
- `POST /api/Roles` - Crear rol
- `PUT /api/Roles/{id}/permissions` - Asignar permisos
- `GET /api/Permissions` - Listar permisos

### Audit
- `GET /api/Audit` - Registro de auditoria (Admin)

## 6. Notificaciones

Se crean automaticamente en:
- `OrderService.CreateAsync()` -> Notifica a operadores
- `OrderService.AcceptOrderAsync()` -> Notifica al cliente
- `OrderService.UpdateStatusAsync()` -> Notifica al cliente
- `PaymentService.VerifyAsync()` -> Notifica al cliente
- `DeliveryService.ChangeStatusAsync()` -> Notifica al cliente

## 7. Facturacion (QuestPDF)

- Se genera automaticamente al confirmar entrega
- Incluye: encabezado, datos cliente, tabla productos, IVA 13%, total
- Estilo boliviano con coloresinstitutionales

## 8. Mapas (Leaflet + OpenStreetMap)

### Componentes
- `OrderMap.razor`: Mapa de solo lectura para ver ubicacion
- `LocationPicker.razor`: Selector de ubicacion con geocoding

### Geocoding (Nominatim)
- Busqueda por texto en el selector de direccion
- API: `https://nominatim.openstreetmap.org/search`
- Restriccion: Bolivia (countrycodes=bo)

## 9. Reportes (Chart.js + CSV)

### Tipos
1. Dashboard: Barras + Doughnut
2. Ventas por periodo: Barras por dia
3. Productos top: Tabla rankeada
4. Pedidos por estado: Tabla porcentual
5. Clientes frecuentes: Tabla rankeada
6. Rendimiento repartidores: Tabla con tasa de exito
7. Inventario: Tabla con estados

### Exportacion CSV
- Descarga automatica via `downloadFile` JS interop
- UTF-8 con comillas para campos con comas

## 10. Auditoria

- Tabla `AuditLogs` con: UserId, Action, Entity, EntityId, OldValues, NewValues, IpAddress
- Registro automatico de acciones clave
- Consulta paginada con filtros

## 11. Responsive Design

### Breakpoints
- `>1024px`: Sidebar completo
- `768px-1024px`: Sidebar reducido
- `425px-768px`: Sidebar iconos
- `<425px`: Sidebar oculto

## 12. Troubleshooting

| Error | Causa | Solucion |
|-------|-------|----------|
| `CS1061` | Propiedad no existe en entidad | Verificar nombre real de la propiedad |
| `401 Unauthorized` | Token expirado | Iniciar sesion nuevamente |
| `500 Internal Error` | Error en servidor | Revisar logs del servidor |
| Migracion falla | DB lock | `sp_releaseapplock` en SQL Server |
