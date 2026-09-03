# TecnoExpress - Sistema de Gestion y Trazabilidad de Pedidos

Sistema ERP completo desarrollado en **.NET 9 Blazor WebAssembly Hosted** para la gestion integral de pedidos, inventario, pagos, facturacion y despachos de una empresa de tecnologia en Santa Cruz de la Sierra, Bolivia.

---

## Arquitectura

```
TrazabilidadPedidos/
├── TrazabilidadPedidos.Client/      # Blazor WASM Frontend
│   ├── Pages/                       # Paginas Razor por modulo
│   ├── Components/Maps/             # Componentes de mapa (Leaflet)
│   ├── Services/                    # Client Services (HTTP)
│   └── wwwroot/                     # CSS, JS, Leaflet, Chart.js, QR
├── TrazabilidadPedidos.Server/      # ASP.NET Core Web API
│   ├── Controllers/                 # Endpoints REST
│   ├── Services/                    # Logica de negocio
│   ├── Repositories/                # Acceso a datos (EF Core)
│   ├── Data/                        # DbContext
│   └── Migrations/                  # Migraciones EF Core
└── TrazabilidadPedidos.Shared/      # DTOs, Entidades, Enums
    ├── DTOs/                        # Data Transfer Objects
    ├── Entities/                    # Modelo de dominio
    └── Enums/                       # Enumeraciones
```

## Stack Tecnologico

| Capa | Tecnologia |
|------|-----------|
| Frontend | Blazor WebAssembly Hosted (.NET 9) |
| Backend | ASP.NET Core Web API |
| Base de datos | SQL Server (Entity Framework Core) |
| Autenticacion | JWT + BCrypt |
| Mapas | Leaflet + OpenStreetMap + Nominatim Geocoding |
| Graficos | Chart.js |
| QR | QRCode.js |
| Facturacion | QuestPDF |
| Framework CSS | Bootstrap 5 |

## Modulos del Sistema

| Modulo | Descripcion | Estado |
|--------|-------------|--------|
| **Dashboard** | Panel con estadisticas y graficos de pedidos y pagos | Completo |
| **Catalogo** | Productos con busqueda, filtros y stock visible | Completo |
| **Carrito** | Carrito de compras con cantidades y eliminacion | Completo |
| **Confirmar Pedido** | Seleccion de ubicacion (mapa + geocoding) y metodo de pago | Completo |
| **Pagos** | QR real con datos bancarios y transferencia con datos | Completo |
| **Bandeja de Pedidos** | Tabs, aceptar/rechazar con mapa, estado de pago | Completo |
| **Despachos** | Auto-creacion al preparar, mapa del repartidor | Completo |
| **Facturacion** | PDF QuestPDF con diseno boliviano, IVA 13% | Completo |
| **Inventario** | CRUD de productos con stock | Completo |
| **Usuarios** | CRUD de usuarios con roles | Completo |
| **Clientes** | Gestion de clientes | Completo |
| **Reportes** | 7 reportes con filtros y exportacion CSV | Completo |
| **Roles y Permisos** | CRUD de roles con asignacion de permisos | Completo |
| **Notificaciones** | Notificaciones en tiempo real para eventos clave | Completo |
| **Historial de Pedidos** | Timeline real con fechas y usuarios | Completo |
| **Auditoria** | Registro de acciones del sistema | Completo |
| **Responsive** | Diseno adaptable 1366/1024/768/425 px | Completo |

## Roles

| Rol | Permisos |
|-----|----------|
| **Administrator** | Acceso total al sistema |
| **Operator** | Pedidos, pagos, despachos, inventario, reportes |
| **DeliveryDriver** | Despachos asignados con mapa |
| **Customer** | Catalogo, carrito, pagos, pedidos propios |

## Flujo Principal del Pedido

```
1. Cliente agrega productos al carrito
2. Confirma pedido seleccionando ubicacion (mapa + geocoding) y pago
3. Se crea el pedido con estado "Pendiente" + pago registrado
4. Operador revisa y ACEPTA el pedido (descuenta inventario)
5. Operador prepara y marca como "ListoParaEntrega" (auto-crea despacho)
6. Repartidor inicia entrega, confirma en ruta
7. Al confirmar entrega: se genera factura automatica (QuestPDF)
8. Notificaciones se envian en cada paso clave
```

## Reportes Disponibles

1. **Dashboard** - Resumen general con metricas clave
2. **Ventas por Periodo** - Filtrado por fechas con ticket promedio
3. **Productos Top** - Productos mas vendidos (5/10/20)
4. **Pedidos por Estado** - Distribucion porcentual
5. **Clientes Frecuentes** - Clientes con mas pedidos/gasto
6. **Rendimiento de Repartidores** - Tasa de exito por repartidor
7. **Inventario** - Estado del stock (normal/bajo/agotado)

Todos los reportes permiten **exportar a CSV**.

## Credenciales de Prueba

| Usuario | Contrasena | Rol |
|---------|-----------|-----|
| admin@tecnoexpress.com | Admin123! | Administrator |
| operador@tecnoexpress.com | Oper123! | Operator |
| repartidor@tecnoexpress.com | Rep123! | DeliveryDriver |
| cliente@tecnoexpress.com | Cli123! | Customer |

## Ejecucion

```bash
# Restaurar paquetes
dotnet restore

# Aplicar migraciones
dotnet ef database update --project TrazabilidadPedidos.Server

# Ejecutar servidor
dotnet run --project TrazabilidadPedidos.Server

# Abrir en navegador
# https://localhost:5001
```

## Base de Datos

- Servidor: `.\SQLEXPRESS`
- Base de datos: `TrazabilidadPedidosDB`
- Migraciones EF Core: `dotnet ef database update --project TrazabilidadPedidos.Server`
