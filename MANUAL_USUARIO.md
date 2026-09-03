# Manual de Usuario – TecnoExpress

Sistema Web de Gestión y Trazabilidad de Pedidos

---

## 1. Inicio de Sesión

1. Abra el navegador y diríjase a la dirección del sistema.
2. Ingrese su **correo electrónico** y **contraseña** en el formulario de login.
3. Haga clic en **"Iniciar Sesión"**.
4. Si las credenciales son correctas, será redirigido al **Dashboard** correspondiente a su rol.
5. Si los datos son incorrectos, aparecerá un mensaje de error. Verifique sus credenciales e intente nuevamente.

> **Credenciales por defecto:**
> - Email: `admin@tecnoexpress.com`
> - Contraseña: `Admin123!`

---

## 2. Funcionalidades del Administrador

### 2.1 Dashboard

Al iniciar sesión, el administrador accede al **Dashboard** principal que muestra:

- Total de pedidos registrados
- Total de clientes activos
- Productos en inventario
- Despachos en curso
- Gráficos de resumen de actividad

### 2.2 Gestión de Usuarios

Acceda al menú **"Usuarios"** para administrar los usuarios del sistema.

**Crear usuario:**
1. Haga clic en **"Nuevo Usuario"**.
2. Complete los campos: nombre, apellido, correo electrónico, contraseña y rol.
3. Seleccione el rol (Administrador, Operador, Repartidor o Cliente).
4. Haga clic en **"Guardar"**.

**Editar usuario:**
1. Localice el usuario en la lista.
2. Haga clic en el ícono de **editar** (lápiz).
3. Modifique los campos deseados.
4. Haga clic en **"Actualizar"**.

**Eliminar usuario:**
1. Localice el usuario en la lista.
2. Haga clic en el ícono de **eliminar** (bote de basura).
3. Confirme la eliminación en el diálogo emergente.

**Buscar usuario:**
- Use el campo de búsqueda para filtrar por nombre o correo electrónico.

### 2.3 Gestión de Clientes

Acceda al menú **"Clientes"** para administrar la base de datos de clientes.

**Crear cliente:**
1. Haga clic en **"Nuevo Cliente"**.
2. Complete los datos: nombre, apellido, correo, teléfono, dirección.
3. Haga clic en **"Guardar"**.

**Editar cliente:**
1. Localice el cliente en la lista.
2. Haga clic en **editar**.
3. Modifique la información.
4. Haga clic en **"Actualizar"**.

**Eliminar cliente:**
1. Seleccione el cliente a eliminar.
2. Confirme la acción.

### 2.4 Gestión de Inventario

Acceda al menú **"Inventario"** para administrar productos, categorías y movimientos de stock.

**Productos:**

- **Crear producto:** Haga clic en "Nuevo Producto", complete nombre, código, categoría, precio, stock inicial, descripción e imagen (URL), luego guarde.
- **Editar producto:** Seleccione un producto, modifique los campos y actualice.
- **Eliminar producto:** Seleccione y confirme la eliminación.

**Categorías:**

- **Crear categoría:** Haga clic en "Nueva Categoría", ingrese el nombre y guarde.
- **Editar categoría:** Seleccione y modifique el nombre.
- **Eliminar categoría:** Seleccione y confirme.

**Movimientos de Inventario:**

- **Registrar movimiento:** Seleccione un producto, elija el tipo de movimiento (Entrada/Salida), indique la cantidad y una observación.
- **Consultar historial:** Visualice todos los movimientos registrados para cada producto.

### 2.5 Gestión de Despachos

Acceda al menú **"Despachos"** para administrar las entregas.

**Asignar repartidor:**
1. Seleccione un pedido pendiente de despacho.
2. Asigne un repartidor disponible del sistema.
3. Confirme la asignación.

**Cambiar estado del despacho:**
1. Seleccione el despacho.
2. Elija el nuevo estado (Pendiente, En Camino, Entregado, Fallido).
3. Confirme el cambio.

**Consultar despachos:**
- Filtre por fecha, estado o repartidor para encontrar despachos específicos.

### 2.6 Reportes

Acceda al menú **"Reportes"** para visualizar gráficos y estadísticas:

- **Gráfico de pedidos por período** – Visualice la cantidad de pedidos en un rango de fechas.
- **Gráfico de ventas** – Monitoree las ventas totales.
- **Distribución de estados** – Vea la proporción de pedidos por cada estado.
- **Productos más vendidos** – Identifique los productos con mayor demanda.

Use los filtros de fecha para personalizar los datos mostrados.

---

## 3. Funcionalidades del Operador

### 3.1 Dashboard

El operador accede a un Dashboard con métricas relevantes para su labor diaria.

### 3.2 Gestión de Clientes

Igual que el administrador, el operador puede:

- **Crear** nuevos clientes
- **Editar** información de clientes existentes
- **Eliminar** clientes del sistema
- **Buscar** clientes por nombre o correo

### 3.3 Gestión de Inventario

El operador puede administrar:

- **Productos:** Crear, editar, eliminar y consultar productos
- **Categorías:** Gestionar las categorías de productos
- **Movimientos de stock:** Registrar entradas y salidas de inventario

### 3.4 Gestión de Despachos

El operador puede:

- **Asignar repartidores** a pedidos pendientes
- **Cambiar estado** de despachos
- **Consultar historial** de despachos realizados
- **Filtrar despachos** por fecha, estado o repartidor

---

## 4. Funcionalidades del Cliente

### 4.1 Catálogo de Productos

1. Al iniciar sesión, el cliente accede al **Catálogo** de productos.
2. Navegue por las categorías disponibles.
3. Use el **buscador** para encontrar productos específicos.
4. Haga clic en un producto para ver su **detalle**: nombre, descripción, precio, disponibilidad.

### 4.2 Carrito de Compras

**Agregar productos al carrito:**
1. Encuentre el producto deseado en el catálogo.
2. Haga clic en **"Agregar al Carrito"**.
3. Indique la cantidad deseada.
4. El producto se añadirá a su carrito.

**Gestionar el carrito:**
1. Acceda al ícono del **carrito** en la navegación.
2. Modifique las cantidades de los productos.
3. Elimine productos que ya no desee.
4. Visualice el **total** de la compra.

**Confirmar compra:**
1. Revise los productos en su carrito.
2. Haga clic en **"Confirmar Compra"**.
3. Seleccione la dirección de entrega.
4. Confirme el pedido.
5. Recibirá un **código de seguimiento**.

### 4.3 Mis Pedidos

Acceda al menú **"Mis Pedidos"** para:

- **Ver historial** de todos sus pedidos
- **Consultar estado** de cada pedido en tiempo real
- **Visualizar timeline** con los cambios de estado del pedido:
  - Pendiente → Procesando → Enviado → En Camino → Entregado
- **Ver detalles** de cada pedido: productos, cantidades, total, fecha

### 4.4 Mi Perfil

Acceda al menú **"Mi Perfil"** para:

- **Editar nombre** y apellido
- **Actualizar correo** electrónico
- **Cambiar contraseña**
- **Modificar dirección** de entrega

1. Haga clic en **"Editar"**.
2. Modifique los campos deseados.
3. Haga clic en **"Guardar Cambios"**.

---

## 5. Funcionalidades del Repartidor

### 5.1 Mis Despachos

Al iniciar sesión, el repartidor accede a la lista de **despachos asignados**.

**Visualizar despachos:**
- Lista de todos los pedidos asignados con su estado actual.
- Información del cliente: nombre, dirección, teléfono.
- Detalle de los productos a entregar.

### 5.2 Iniciar Entrega

1. Seleccione un despacho **"Pendiente"**.
2. Haga clic en **"Iniciar Entrega"**.
3. El estado cambiará a **"En Camino"**.
4. El cliente recibirá la actualización de estado.

### 5.3 Confirmar Entrega

1. Una vez entregado el pedido al cliente, seleccione el despacho.
2. Haga clic en **"Confirmar Entrega"**.
3. El estado cambiará a **"Entregado"**.
4. Si no se pudo entregar, seleccione **"Entrega Fallida"** e indique el motivo.

---

## Atención al Cliente

Si tiene problemas o dudas, contacte al administrador del sistema o envíe un reporte a través del canal de soporte técnico de TecnoExpress.
