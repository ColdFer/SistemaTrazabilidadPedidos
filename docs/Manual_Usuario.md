# Manual de Usuario - TecnoExpress ERP

## 1. Inicio de Sesion

1. Abrir la aplicacion en el navegador
2. Ingresar correo electronico y contrasena
3. Hacer clic en "Iniciar Sesion"
4. El sistema redirige al dashboard segun el rol

## 2. Roles y Funcionalidades

### Cliente
- **Catalogo**: Ver productos con precios, stock y busqueda
- **Carrito**: Agregar productos, modificar cantidades, eliminar
- **Confirmar Pedido**: Seleccionar direccion en mapa (geocoding disponible), metodo de pago
- **Mis Pedidos**: Ver historial con timeline real, descargar factura PDF
- **Pagos**: Verificar estado de pagos, subir comprobante

### Operador
- **Bandeja de Pedidos**: Ver todos los pedidos, aceptar/rechazar, cambiar estado
- **Pagos**: Verificar y aprobar pagos con comprobante
- **Despachos**: Gestionar programacion y estado de entregas
- **Inventario**: CRUD de productos, categorias, movimientos
- **Reportes**: Dashboard, ventas, productos, inventario (con exportacion CSV)

### Administrador
- **Todo lo del Operador** mas:
- **Usuarios**: CRUD de usuarios con roles
- **Roles**: Gestionar roles y asignar permisos
- **Reportes**: Todos los reportes del sistema
- **Auditoria**: Registro de acciones del sistema

### Repartidor
- **Despachos Asignados**: Ver despachos con mapa y navegacion
- **Estado de Entrega**: Cambiar estado (En ruta, Entregado, Fallido)

## 3. Funcionalidades Comunes

### Notificaciones
- Las notificaciones aparecen en el icono de campana en la barra superior
- Se reciben al: crear pedido, aceptar pedido, cambios de estado, pagos verificados

### Mapas
- **Seleccionar direccion**: Clic en el mapa o buscar por texto (geocoding Nominatim)
- **Ver ubicacion**: Mapa de solo lectura con marcador
- **Google Maps**: Boton para abrir en Google Maps (navegacion)

### Facturacion
- Las facturas se generan automaticamente al confirmar entrega
- Formato PDF con diseno boliviano (QuestPDF)
- Incluye IVA 13% y desglose de productos

### Reportes
- Cada reporte tiene filtros (fechas, limites)
- Boton "Exportar CSV" para descargar datos
- Datos reales de la base de datos

## 4. Atajos de Teclado

- `Tab`: Navegar entre campos
- `Enter`: Confirmar accion
- `Esc`: Cerrar modales

## 5. Solucion de Problemas

| Problema | Solucion |
|----------|----------|
| No carga el mapa | Verificar conexion a internet (OpenStreetMap) |
| Factura no genera | Verificar que el pedido este en estado "Entregado" |
| No aparecen reportes | Verificar rol de Administrador |
| Pagos no se verifican | Verificar rol de Operador |
