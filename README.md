# TPI Integrador 2026 — Grupo 7

**Módulo:** Presupuestos y Pedidos de Venta
**Modelo seguido:** proyecto de ejemplo del profesor *GestionPersonas* (C#, POO, N-capas, SQL Server con procedimientos almacenados).

## Qué hay en la carpeta

| Carpeta o archivo | Contenido |
|---|---|
| `SistemaVentas.sln` | La solución. Se abre con doble clic. |
| `Vista_usuario` | Módulo de Gestión de Usuarios (login, home, administración de usuarios, políticas y recuperación de contraseña), hecho en programación estructurada y pasado a .NET 8. Es el **proyecto de inicio**: desde el Home, el botón Ventas abre nuestro módulo con el usuario logueado. |
| `CapaEntidades` | Clases del módulo (`PresupuestoVenta`, `NotaPedido`, `AvisoMercaderia`, sus detalles y los 5 reportes) y las clases de consulta de otros grupos (`ClienteVentas`, `ProductoVentas`, `UsuarioVentas`). |
| `CapaDatos` | `Conexion`, `CD_EjecutarConsultasSP` y una clase `CD_` por entidad. Único proyecto con el paquete de SQL Server. |
| `CapaLogica` | Una clase `CN_` por entidad, con las reglas de negocio. |
| `CapaVista` | `FrmMenuVentas`, `FrmReportesVentas`, `Iconos` y tres pantallas por entidad: la lista (`FrmPresupuestosVenta`, `FrmNotasPedido`, `FrmAvisos`), la de carga y edición (`...Datos`) y la de detalle (`...Detalle`). También `appsettings.json`. |
| `BaseDeDatos` | Los tres scripts de abajo. |
| `Documentacion` | Diccionario de datos en Word (para el tablón). |

| Proyecto | Referencia a |
|---|---|
| `CapaEntidades` | — |
| `CapaDatos` | `CapaEntidades` |
| `CapaLogica` | `CapaDatos`, `CapaEntidades` |
| `CapaVista` | `CapaLogica`, `CapaEntidades` |

Los cuatro proyectos usan .NET 8 (las bibliotecas `net8.0`, la vista `net8.0-windows`).

## Scripts de la base

| Orden | Archivo | Para qué |
|---|---|---|
| 0 | `0_GestionUsuarios.sql` | Crea (o actualiza, sin borrar nada) la base `Sistema_Gestion` del módulo de Gestión de Usuarios: tablas, procedimientos, catálogos, los tres grupos de roles y dos usuarios de prueba (`admin` y `lsosa`). |
| 1 | `1_TablasDeOtrosGrupos_SoloParaProbar.sql` | Crea versiones reducidas de `Clientes` y `Productos` **solo si no existen**, y registra tres usuarios de prueba en Gestión de Usuarios (`jperez`, `mgomez`, `rlopez`; la clave está en el script). Nunca borra nada. |
| 2 | `2_ModuloGrupo7.sql` | Crea las 5 tablas del Grupo 7, las vistas y los 32 procedimientos. Solo borra y recrea las tablas propias. |
| 3 | `3_DatosYPruebas_Grupo7.sql` | Carga presupuestos, notas y avisos de ejemplo y ejecuta 8 pruebas (`PRUEBA 1 OK` … `PRUEBA 8 OK`). Correrlo justo después del 2. |

## Puesta en marcha (trabajando solos)

Todo vive en **una sola base**, `Sistema_Gestion`, la del módulo de Gestión de Usuarios.

1. En SSMS, ejecutar los scripts 0, 1, 2 y 3 en ese orden.
2. Para que el sistema mande correos (alta de usuarios, recuperación de contraseña y código de dos pasos), copiar `Vista_usuario\correo.ejemplo.json` como `Vista_usuario\correo.json` y completar la cuenta de Gmail. Ese archivo **no se sube a GitHub**.
3. Abrir `SistemaVentas.sln`.
4. Revisar el servidor en `CapaVista\appsettings.json` (viene con `.\\SQLEXPRESS`). Los dos módulos leen la conexión de ese mismo archivo. La guía completa para instalar desde cero está en `GUIA_DE_INSTALACION.txt`.
5. Compilar, Recompilar solución: 5 correctos, 0 erróneos.
6. Comprobar que `Vista_usuario` esté en negrita. Si no: clic derecho, Establecer como proyecto de inicio.
7. F5. Abre el login. Los usuarios de prueba (la clave está en los scripts 0 y 1) muestran los tres grupos que pidió el profesor:

| Grupo | Usuario de prueba | Qué ve en el Home |
|---|---|---|
| Administración | `admin` | Administrar usuarios y Políticas de seguridad |
| Ventas | `jperez`, `mgomez` (vendedores), `rlopez` (Depósito) | Presupuestos y pedidos de venta |
| Otros módulos | `lsosa` | Otros módulos (pantalla "en construcción") |

Todos ven además "Mi cuenta", para cambiar su contraseña o sus preguntas de seguridad.

## Integración con el resto del sistema

**Base de datos.** El módulo no lee nunca directamente las tablas de otros grupos: usa tres vistas adaptadoras.

| Vista | Lee la tabla | Columnas que necesita el módulo |
|---|---|---|
| `vw_Ventas_Clientes` | `Clientes` (Grupo 6) | `ID`, `Nombre`, `CUIT_DNI`, `Activo` |
| `vw_Ventas_Productos` | `Productos` (Grupo 1) | `ID`, `Codigo`, `Descripcion`, `PrecioActual`, `StockActual` |
| `vw_Ventas_Usuarios` | `Usuarios` (Seguridad) | `ID`, `NombreUsuario`, `NombreCompleto`, `Rol` |

En la base común:

1. **No** ejecutar el script 1.
2. En el script 2, cambiar el `USE` por el nombre de la base común.
3. Ajustar las tres vistas a los nombres reales. Por ejemplo, si `Productos` es como dice la consigna: `SELECT ID_Codigo AS ID, Nombre AS Descripcion, Precio_Venta AS PrecioActual, ...`.
4. Ajustar las `REFERENCES` de las 7 claves foráneas a la clave primaria real (por ejemplo `dbo.Productos (ID_Codigo)`).
5. Ejecutar el script 2.

Probado en una base de simulación armada como dice la consigna para los grupos 1 y 6: con solo los pasos 3 y 4 funcionó el circuito completo, sin tocar el C#.

**Código C#.** Las clases del módulo se copian a los proyectos comunes con el mismo nombre de capa. Los nombres se eligieron para no chocar con otros grupos:

- Presupuestos llevan `Venta` (`PresupuestoVenta`, `CD_PresupuestosVenta`, `FrmPresupuestosVenta`), porque el Grupo 3 tiene presupuestos de compra.
- Las clases de otros grupos que usa el módulo se llaman `ClienteVentas`, `ProductoVentas` y `UsuarioVentas`, y se leen con `CD_ListasVentas`. Conviven con las `Cliente`, `Producto` y `Usuario` reales sin conflicto.
- `Conexion` y `CD_EjecutarConsultasSP` son las del modelo del profesor. La nuestra agrega tres métodos `ValorONulo` a `CD_EjecutarConsultasSP`: al unificar, conservar esta versión.
- `Program.cs` y `FrmMenuVentas` son del módulo suelto: en el sistema integrado, el menú general abre `FrmMenuVentas` o directamente cada pantalla.
- El rol de Depósito está en `UsuarioVentas.ROL_DEPOSITO` (`"Deposito"`): ajustarlo al nombre que use Seguridad.

## Estado

- [x] Base de datos completa del módulo (5 entidades, 32 procedimientos, 7 vistas)
- [x] Diccionario de datos v1.4
- [x] Presupuestos: alta con líneas, modificación, baja lógica, aprobar, rechazar, consulta y listado
- [x] Notas de Pedido: alta directa, generar desde presupuesto aprobado, modificación, baja lógica, entregado, facturado, consulta y listado
- [x] Avisos de Mercadería Preparada: alta (el pedido pasa a Preparado), modificación, baja lógica (vuelve a Pendiente), consulta por período
- [x] Los 5 reportes de la consigna
- [x] Correcciones del profesor: grillas con lo principal y botón de detalles, acciones con íconos en cada fila, alta y edición en una ventana aparte, filtros que se aplican solos, clientes deudores en rojo al cargar una nota de pedido
- [x] Preparado para integrar (vistas adaptadoras, nombres sin conflicto, scripts que no borran tablas ajenas)
- [ ] Registro en `Auditoria_Cambios` (Grupo 1): depende de que exista un usuario logueado (Seguridad) y de la tabla definitiva del Grupo 1
