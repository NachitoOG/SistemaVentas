USE Sistema_Gestion;
GO

IF OBJECT_ID('dbo.Avisos_Mercaderia_Preparada', 'U') IS NOT NULL
    DROP TABLE dbo.Avisos_Mercaderia_Preparada;
IF OBJECT_ID('dbo.Detalle_Notas_Pedido', 'U') IS NOT NULL
    DROP TABLE dbo.Detalle_Notas_Pedido;
IF OBJECT_ID('dbo.Notas_Pedido', 'U') IS NOT NULL
    DROP TABLE dbo.Notas_Pedido;
IF OBJECT_ID('dbo.Detalle_Presupuestos_Venta', 'U') IS NOT NULL
    DROP TABLE dbo.Detalle_Presupuestos_Venta;
IF OBJECT_ID('dbo.Presupuestos_Venta', 'U') IS NOT NULL
    DROP TABLE dbo.Presupuestos_Venta;
GO

CREATE TABLE dbo.Presupuestos_Venta
(
    ID             INT IDENTITY(1,1) NOT NULL,
    FK_Cliente     INT               NOT NULL,
    FK_Usuario     INT               NOT NULL,
    Fecha_Emision  DATETIME2(0)      NOT NULL
        CONSTRAINT DF_Presupuestos_Fecha DEFAULT (GETDATE()),
    Estado         VARCHAR(30)       NOT NULL
        CONSTRAINT DF_Presupuestos_Estado DEFAULT ('Pendiente'),
    Activo         BIT               NOT NULL
        CONSTRAINT DF_Presupuestos_Activo DEFAULT (1),

    CONSTRAINT PK_Presupuestos_Venta PRIMARY KEY (ID),
    CONSTRAINT FK_Presupuestos_Cliente FOREIGN KEY (FK_Cliente) REFERENCES dbo.Clientes (ID),
    CONSTRAINT FK_Presupuestos_Usuario FOREIGN KEY (FK_Usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Presupuestos_Estado CHECK (Estado IN ('Pendiente', 'Aprobado', 'Rechazado'))
);
GO

CREATE TABLE dbo.Detalle_Presupuestos_Venta
(
    ID                    INT IDENTITY(1,1) NOT NULL,
    FK_Presupuesto_Venta  INT               NOT NULL,
    FK_Producto           INT               NOT NULL,
    Cantidad              INT               NOT NULL,
    Precio_Unitario       DECIMAL(18,2)     NOT NULL,

    CONSTRAINT PK_Detalle_Presupuestos_Venta PRIMARY KEY (ID),
    CONSTRAINT FK_DetallePresupuesto_Cabecera FOREIGN KEY (FK_Presupuesto_Venta) REFERENCES dbo.Presupuestos_Venta (ID),
    CONSTRAINT FK_DetallePresupuesto_Producto FOREIGN KEY (FK_Producto) REFERENCES dbo.Productos (ID),
    CONSTRAINT CK_DetallePresupuesto_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_DetallePresupuesto_Precio CHECK (Precio_Unitario >= 0)
);
GO

CREATE TABLE dbo.Notas_Pedido
(
    ID                    INT IDENTITY(1,1) NOT NULL,
    FK_Cliente            INT               NOT NULL,
    FK_Usuario            INT               NOT NULL,
    FK_Presupuesto_Venta  INT               NULL,
    Fecha_Emision         DATETIME2(0)      NOT NULL
        CONSTRAINT DF_NotasPedido_Fecha DEFAULT (GETDATE()),
    Estado                VARCHAR(30)       NOT NULL
        CONSTRAINT DF_NotasPedido_Estado DEFAULT ('Pendiente'),
    Activo                BIT               NOT NULL
        CONSTRAINT DF_NotasPedido_Activo DEFAULT (1),

    CONSTRAINT PK_Notas_Pedido PRIMARY KEY (ID),
    CONSTRAINT FK_NotasPedido_Cliente FOREIGN KEY (FK_Cliente) REFERENCES dbo.Clientes (ID),
    CONSTRAINT FK_NotasPedido_Usuario FOREIGN KEY (FK_Usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_NotasPedido_Presupuesto FOREIGN KEY (FK_Presupuesto_Venta) REFERENCES dbo.Presupuestos_Venta (ID),
    CONSTRAINT CK_NotasPedido_Estado CHECK (Estado IN ('Pendiente', 'Preparado', 'Entregado', 'Facturado'))
);
GO

CREATE TABLE dbo.Detalle_Notas_Pedido
(
    ID               INT IDENTITY(1,1) NOT NULL,
    FK_Nota_Pedido   INT               NOT NULL,
    FK_Producto      INT               NOT NULL,
    Cantidad         INT               NOT NULL,
    Precio_Unitario  DECIMAL(18,2)     NOT NULL,

    CONSTRAINT PK_Detalle_Notas_Pedido PRIMARY KEY (ID),
    CONSTRAINT FK_DetalleNotasPedido_Cabecera FOREIGN KEY (FK_Nota_Pedido) REFERENCES dbo.Notas_Pedido (ID),
    CONSTRAINT FK_DetalleNotasPedido_Producto FOREIGN KEY (FK_Producto) REFERENCES dbo.Productos (ID),
    CONSTRAINT CK_DetalleNotasPedido_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_DetalleNotasPedido_Precio CHECK (Precio_Unitario >= 0)
);
GO

CREATE TABLE dbo.Avisos_Mercaderia_Preparada
(
    ID              INT IDENTITY(1,1) NOT NULL,
    FK_Nota_Pedido  INT               NOT NULL,
    FK_Usuario      INT               NOT NULL,
    Fecha_Emision   DATETIME2(0)      NOT NULL
        CONSTRAINT DF_Avisos_Fecha DEFAULT (GETDATE()),
    Activo          BIT               NOT NULL
        CONSTRAINT DF_Avisos_Activo DEFAULT (1),

    CONSTRAINT PK_Avisos_Mercaderia_Preparada PRIMARY KEY (ID),
    CONSTRAINT FK_AvisoMercaderia_NotaPedido FOREIGN KEY (FK_Nota_Pedido) REFERENCES dbo.Notas_Pedido (ID),
    CONSTRAINT FK_AvisoMercaderia_Usuario FOREIGN KEY (FK_Usuario) REFERENCES dbo.Usuario (id_usuario)
);
GO

IF OBJECT_ID('dbo.vw_Ventas_Clientes', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Ventas_Clientes;
GO

CREATE VIEW dbo.vw_Ventas_Clientes
AS
SELECT ID       AS ID,
       Nombre   AS Nombre,
       CUIT_DNI AS CUIT_DNI,
       Activo   AS Activo
FROM   dbo.Clientes;
GO

IF OBJECT_ID('dbo.vw_Ventas_Productos', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Ventas_Productos;
GO

CREATE VIEW dbo.vw_Ventas_Productos
AS
SELECT ID           AS ID,
       Codigo       AS Codigo,
       Descripcion  AS Descripcion,
       PrecioActual AS PrecioActual,
       StockActual  AS StockActual
FROM   dbo.Productos;
GO

IF OBJECT_ID('dbo.vw_Ventas_Usuarios', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Ventas_Usuarios;
GO

CREATE VIEW dbo.vw_Ventas_Usuarios
AS
SELECT u.id_usuario                AS ID,
       u.usuario                   AS NombreUsuario,
       p.nombre + ' ' + p.apellido AS NombreCompleto,
       CASE
           WHEN EXISTS (SELECT 1
                        FROM   dbo.usuario_roles AS ur
                        INNER JOIN dbo.Roles AS r ON r.id_rol = ur.id_rol
                        WHERE  ur.Id_usuario = u.id_usuario
                          AND  r.Roles = N'Depósito')
               THEN 'Deposito'
           ELSE ISNULL((SELECT TOP 1 r.Roles
                        FROM   dbo.usuario_roles AS ur
                        INNER JOIN dbo.Roles AS r ON r.id_rol = ur.id_rol
                        WHERE  ur.Id_usuario = u.id_usuario
                        ORDER BY r.id_rol), 'Sin rol')
       END                         AS Rol,
       u.estado                    AS Activo
FROM   dbo.Usuario AS u
INNER JOIN dbo.Persona AS p ON p.id_persona = u.id_persona;
GO

IF OBJECT_ID('dbo.vw_Presupuesto_Venta', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Presupuesto_Venta;
GO

CREATE VIEW dbo.vw_Presupuesto_Venta
AS
SELECT
    p.ID,
    p.FK_Cliente,
    c.Nombre AS Cliente,
    p.FK_Usuario,
    u.NombreCompleto AS Usuario,
    p.Fecha_Emision,
    p.Estado,
    p.Activo,
    ISNULL((SELECT SUM(d.Cantidad * d.Precio_Unitario)
              FROM dbo.Detalle_Presupuestos_Venta AS d
             WHERE d.FK_Presupuesto_Venta = p.ID), 0) AS Total
FROM dbo.Presupuestos_Venta AS p
INNER JOIN dbo.vw_Ventas_Clientes AS c ON c.ID = p.FK_Cliente
INNER JOIN dbo.vw_Ventas_Usuarios AS u ON u.ID = p.FK_Usuario;
GO

IF OBJECT_ID('dbo.vw_Nota_Pedido', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Nota_Pedido;
GO

CREATE VIEW dbo.vw_Nota_Pedido
AS
SELECT
    n.ID,
    n.FK_Cliente,
    c.Nombre AS Cliente,
    n.FK_Usuario,
    u.NombreCompleto AS Usuario,
    n.FK_Presupuesto_Venta,
    n.Fecha_Emision,
    n.Estado,
    n.Activo,
    ISNULL((SELECT SUM(d.Cantidad * d.Precio_Unitario)
              FROM dbo.Detalle_Notas_Pedido AS d
             WHERE d.FK_Nota_Pedido = n.ID), 0) AS Total
FROM dbo.Notas_Pedido AS n
INNER JOIN dbo.vw_Ventas_Clientes AS c ON c.ID = n.FK_Cliente
INNER JOIN dbo.vw_Ventas_Usuarios AS u ON u.ID = n.FK_Usuario;
GO

IF OBJECT_ID('dbo.vw_Aviso_Mercaderia', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Aviso_Mercaderia;
GO

CREATE VIEW dbo.vw_Aviso_Mercaderia
AS
SELECT
    a.ID,
    a.FK_Nota_Pedido,
    a.FK_Usuario,
    u.NombreCompleto AS Usuario,
    c.Nombre AS Cliente,
    n.Fecha_Emision AS Fecha_Pedido,
    a.Fecha_Emision,
    n.Estado AS Estado_Pedido,
    a.Activo
FROM dbo.Avisos_Mercaderia_Preparada AS a
INNER JOIN dbo.Notas_Pedido AS n ON n.ID = a.FK_Nota_Pedido
INNER JOIN dbo.vw_Ventas_Clientes AS c ON c.ID = n.FK_Cliente
INNER JOIN dbo.vw_Ventas_Usuarios AS u ON u.ID = a.FK_Usuario;
GO

IF OBJECT_ID('dbo.vw_Ventas_DeudaClientes', 'V') IS NOT NULL
    DROP VIEW dbo.vw_Ventas_DeudaClientes;
GO

CREATE VIEW dbo.vw_Ventas_DeudaClientes
AS
SELECT FK_Cliente,
       SUM(Total) AS Deuda
FROM   dbo.vw_Nota_Pedido
WHERE  Activo = 1
  AND  Estado = 'Entregado'
GROUP BY FK_Cliente;
GO

IF OBJECT_ID('dbo.sp_Ventas_ListarClientes', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ventas_ListarClientes;
GO

CREATE PROCEDURE dbo.sp_Ventas_ListarClientes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.ID, c.Nombre, c.CUIT_DNI, ISNULL(d.Deuda, 0) AS Deuda
    FROM   dbo.vw_Ventas_Clientes AS c
    LEFT JOIN dbo.vw_Ventas_DeudaClientes AS d ON d.FK_Cliente = c.ID
    WHERE  c.Activo = 1
    ORDER BY c.Nombre;
END;
GO

IF OBJECT_ID('dbo.sp_Ventas_ListarUsuarios', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ventas_ListarUsuarios;
GO

CREATE PROCEDURE dbo.sp_Ventas_ListarUsuarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ID, NombreUsuario, NombreCompleto, Rol
    FROM   dbo.vw_Ventas_Usuarios
    WHERE  Activo = 1
    ORDER BY NombreCompleto;
END;
GO

IF OBJECT_ID('dbo.sp_Ventas_ListarProductos', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Ventas_ListarProductos;
GO

CREATE PROCEDURE dbo.sp_Ventas_ListarProductos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ID, Codigo, Descripcion, PrecioActual, StockActual
    FROM   dbo.vw_Ventas_Productos
    ORDER BY Descripcion;
END;
GO

IF OBJECT_ID('dbo.sp_PresupuestoVenta_Alta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_PresupuestoVenta_Alta;
GO

CREATE PROCEDURE dbo.sp_PresupuestoVenta_Alta
    @FK_Cliente     INT,
    @FK_Usuario     INT,
    @Fecha_Emision  DATETIME2(0),
    @ID             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Clientes WHERE ID = @FK_Cliente AND Activo = 1)
    BEGIN
        RAISERROR('El cliente no existe o está dado de baja.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Usuarios WHERE ID = @FK_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision IS NULL OR CAST(@Fecha_Emision AS DATE) > CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha de emisión es obligatoria y no puede ser futura.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO dbo.Presupuestos_Venta (FK_Cliente, FK_Usuario, Fecha_Emision, Estado, Activo)
        VALUES (@FK_Cliente, @FK_Usuario, @Fecha_Emision, 'Pendiente', 1);

        SET @ID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_PresupuestoVenta_Modificar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_PresupuestoVenta_Modificar;
GO

CREATE PROCEDURE dbo.sp_PresupuestoVenta_Modificar
    @ID             INT,
    @FK_Cliente     INT,
    @FK_Usuario     INT,
    @Fecha_Emision  DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Presupuestos_Venta
                   WHERE ID = @ID AND Activo = 1 AND Estado = 'Pendiente')
    BEGIN
        RAISERROR('Solo se puede modificar un presupuesto Pendiente que no esté dado de baja.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Clientes WHERE ID = @FK_Cliente AND Activo = 1)
    BEGIN
        RAISERROR('El cliente no existe o está dado de baja.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Usuarios WHERE ID = @FK_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision IS NULL OR CAST(@Fecha_Emision AS DATE) > CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha de emisión es obligatoria y no puede ser futura.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE dbo.Presupuestos_Venta
        SET    FK_Cliente    = @FK_Cliente,
               FK_Usuario    = @FK_Usuario,
               Fecha_Emision = @Fecha_Emision
        WHERE  ID = @ID;
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_PresupuestoVenta_CambiarEstado', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_PresupuestoVenta_CambiarEstado;
GO

CREATE PROCEDURE dbo.sp_PresupuestoVenta_CambiarEstado
    @ID      INT,
    @Estado  VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoActual VARCHAR(30);

    IF @Estado IS NULL OR @Estado NOT IN ('Aprobado', 'Rechazado')
    BEGIN
        RAISERROR('El nuevo estado tiene que ser Aprobado o Rechazado.', 16, 1);
        RETURN;
    END;

    SELECT @EstadoActual = Estado
    FROM   dbo.Presupuestos_Venta
    WHERE  ID = @ID AND Activo = 1;

    IF @EstadoActual IS NULL
    BEGIN
        RAISERROR('El presupuesto no existe o está dado de baja.', 16, 1);
        RETURN;
    END;

    IF @EstadoActual <> 'Pendiente'
    BEGIN
        RAISERROR('El presupuesto ya está %s. Solo se puede aprobar o rechazar uno Pendiente.', 16, 1, @EstadoActual);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Detalle_Presupuestos_Venta WHERE FK_Presupuesto_Venta = @ID)
    BEGIN
        RAISERROR('El presupuesto no tiene líneas.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE dbo.Presupuestos_Venta
        SET    Estado = @Estado
        WHERE  ID = @ID;
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_PresupuestoVenta_Baja', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_PresupuestoVenta_Baja;
GO

CREATE PROCEDURE dbo.sp_PresupuestoVenta_Baja
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Presupuestos_Venta WHERE ID = @ID)
    BEGIN
        RAISERROR('El presupuesto indicado no existe.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Presupuestos_Venta WHERE ID = @ID AND Activo = 0)
    BEGIN
        RAISERROR('El presupuesto ya se encuentra dado de baja.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Presupuestos_Venta WHERE ID = @ID AND Estado = 'Aprobado')
    BEGIN
        RAISERROR('No se puede dar de baja un presupuesto aprobado.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE dbo.Presupuestos_Venta
        SET    Activo = 0
        WHERE  ID = @ID;
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_PresupuestoVenta_Consultar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_PresupuestoVenta_Consultar;
GO

CREATE PROCEDURE dbo.sp_PresupuestoVenta_Consultar
    @ID             INT         = NULL,
    @FK_Cliente     INT         = NULL,
    @Estado         VARCHAR(30) = NULL,
    @Incluir_Bajas  BIT         = 0
AS
BEGIN
    SET NOCOUNT ON;

    SET @Estado = NULLIF(LTRIM(RTRIM(@Estado)), '');

    SELECT  ID,
            FK_Cliente,
            Cliente,
            FK_Usuario,
            Usuario,
            Fecha_Emision,
            Estado,
            Activo,
            Total
    FROM    dbo.vw_Presupuesto_Venta
    WHERE   (@ID         IS NULL OR ID = @ID)
      AND   (@FK_Cliente IS NULL OR FK_Cliente = @FK_Cliente)
      AND   (@Estado     IS NULL OR Estado = @Estado)
      AND   (@Incluir_Bajas = 1 OR Activo = 1)
    ORDER BY Fecha_Emision DESC, ID DESC;
END;
GO

IF OBJECT_ID('dbo.sp_PresupuestoVenta_Listar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_PresupuestoVenta_Listar;
GO

CREATE PROCEDURE dbo.sp_PresupuestoVenta_Listar
    @Incluir_Bajas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    EXEC dbo.sp_PresupuestoVenta_Consultar @Incluir_Bajas = @Incluir_Bajas;
END;
GO

IF OBJECT_ID('dbo.sp_DetallePresupuestoVenta_Alta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DetallePresupuestoVenta_Alta;
GO

CREATE PROCEDURE dbo.sp_DetallePresupuestoVenta_Alta
    @FK_Presupuesto_Venta  INT,
    @FK_Producto           INT,
    @Cantidad              INT,
    @Precio_Unitario       DECIMAL(18,2),
    @ID                    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Presupuestos_Venta
                   WHERE ID = @FK_Presupuesto_Venta AND Activo = 1 AND Estado = 'Pendiente')
    BEGIN
        RAISERROR('Solo se pueden cargar líneas en un presupuesto Pendiente.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Productos WHERE ID = @FK_Producto)
    BEGIN
        RAISERROR('El producto no existe.', 16, 1);
        RETURN;
    END;

    IF @Cantidad IS NULL OR @Cantidad <= 0
    BEGIN
        RAISERROR('La cantidad tiene que ser mayor a cero.', 16, 1);
        RETURN;
    END;

    IF @Precio_Unitario IS NULL OR @Precio_Unitario < 0
    BEGIN
        RAISERROR('El precio no puede ser negativo.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO dbo.Detalle_Presupuestos_Venta (FK_Presupuesto_Venta, FK_Producto, Cantidad, Precio_Unitario)
        VALUES (@FK_Presupuesto_Venta, @FK_Producto, @Cantidad, @Precio_Unitario);

        SET @ID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_DetallePresupuestoVenta_BorrarPorPresupuesto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DetallePresupuestoVenta_BorrarPorPresupuesto;
GO

CREATE PROCEDURE dbo.sp_DetallePresupuestoVenta_BorrarPorPresupuesto
    @FK_Presupuesto_Venta INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Presupuestos_Venta
                   WHERE ID = @FK_Presupuesto_Venta AND Activo = 1 AND Estado = 'Pendiente')
    BEGIN
        RAISERROR('Solo se pueden cambiar las líneas de un presupuesto Pendiente.', 16, 1);
        RETURN;
    END;

    DELETE FROM dbo.Detalle_Presupuestos_Venta
    WHERE  FK_Presupuesto_Venta = @FK_Presupuesto_Venta;
END;
GO

IF OBJECT_ID('dbo.sp_DetallePresupuestoVenta_Listar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DetallePresupuestoVenta_Listar;
GO

CREATE PROCEDURE dbo.sp_DetallePresupuestoVenta_Listar
    @FK_Presupuesto_Venta INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  d.ID,
            d.FK_Presupuesto_Venta,
            d.FK_Producto,
            p.Descripcion AS Producto,
            d.Cantidad,
            d.Precio_Unitario
    FROM    dbo.Detalle_Presupuestos_Venta AS d
    INNER JOIN dbo.vw_Ventas_Productos AS p ON p.ID = d.FK_Producto
    WHERE   d.FK_Presupuesto_Venta = @FK_Presupuesto_Venta
    ORDER BY d.ID;
END;
GO

IF OBJECT_ID('dbo.sp_NotaPedido_Alta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotaPedido_Alta;
GO

CREATE PROCEDURE dbo.sp_NotaPedido_Alta
    @FK_Cliente     INT,
    @FK_Usuario     INT,
    @Fecha_Emision  DATETIME2(0),
    @ID             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Clientes WHERE ID = @FK_Cliente AND Activo = 1)
    BEGIN
        RAISERROR('El cliente no existe o está dado de baja.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Usuarios WHERE ID = @FK_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision IS NULL OR CAST(@Fecha_Emision AS DATE) > CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha de emisión es obligatoria y no puede ser futura.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO dbo.Notas_Pedido (FK_Cliente, FK_Usuario, FK_Presupuesto_Venta, Fecha_Emision, Estado, Activo)
        VALUES (@FK_Cliente, @FK_Usuario, NULL, @Fecha_Emision, 'Pendiente', 1);

        SET @ID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_NotaPedido_GenerarDesdePresupuesto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotaPedido_GenerarDesdePresupuesto;
GO

CREATE PROCEDURE dbo.sp_NotaPedido_GenerarDesdePresupuesto
    @FK_Presupuesto_Venta  INT,
    @FK_Usuario            INT,
    @ID                    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FK_Cliente INT;
    DECLARE @EstadoPresupuesto VARCHAR(30);

    SELECT @FK_Cliente = FK_Cliente,
           @EstadoPresupuesto = Estado
    FROM   dbo.Presupuestos_Venta
    WHERE  ID = @FK_Presupuesto_Venta AND Activo = 1;

    IF @FK_Cliente IS NULL
    BEGIN
        RAISERROR('El presupuesto no existe o está dado de baja.', 16, 1);
        RETURN;
    END;

    IF @EstadoPresupuesto <> 'Aprobado'
    BEGIN
        RAISERROR('Solo se puede generar una nota de pedido desde un presupuesto Aprobado. Este está %s.', 16, 1, @EstadoPresupuesto);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Notas_Pedido
               WHERE FK_Presupuesto_Venta = @FK_Presupuesto_Venta AND Activo = 1)
    BEGIN
        RAISERROR('Ya existe una nota de pedido para el presupuesto %d.', 16, 1, @FK_Presupuesto_Venta);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Usuarios WHERE ID = @FK_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Notas_Pedido (FK_Cliente, FK_Usuario, FK_Presupuesto_Venta, Fecha_Emision, Estado, Activo)
        VALUES (@FK_Cliente, @FK_Usuario, @FK_Presupuesto_Venta, GETDATE(), 'Pendiente', 1);

        SET @ID = SCOPE_IDENTITY();

        INSERT INTO dbo.Detalle_Notas_Pedido (FK_Nota_Pedido, FK_Producto, Cantidad, Precio_Unitario)
        SELECT @ID, FK_Producto, Cantidad, Precio_Unitario
        FROM   dbo.Detalle_Presupuestos_Venta
        WHERE  FK_Presupuesto_Venta = @FK_Presupuesto_Venta;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_NotaPedido_Modificar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotaPedido_Modificar;
GO

CREATE PROCEDURE dbo.sp_NotaPedido_Modificar
    @ID             INT,
    @FK_Cliente     INT,
    @FK_Usuario     INT,
    @Fecha_Emision  DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Notas_Pedido
                   WHERE ID = @ID AND Activo = 1 AND Estado = 'Pendiente')
    BEGIN
        RAISERROR('Solo se puede modificar una nota de pedido Pendiente que no esté dada de baja.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Notas_Pedido
               WHERE ID = @ID AND FK_Presupuesto_Venta IS NOT NULL AND FK_Cliente <> @FK_Cliente)
    BEGIN
        RAISERROR('El pedido viene de un presupuesto: no se le puede cambiar el cliente.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Clientes WHERE ID = @FK_Cliente AND Activo = 1)
    BEGIN
        RAISERROR('El cliente no existe o está dado de baja.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Usuarios WHERE ID = @FK_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision IS NULL OR CAST(@Fecha_Emision AS DATE) > CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha de emisión es obligatoria y no puede ser futura.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE dbo.Notas_Pedido
        SET    FK_Cliente    = @FK_Cliente,
               FK_Usuario    = @FK_Usuario,
               Fecha_Emision = @Fecha_Emision
        WHERE  ID = @ID;
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_NotaPedido_CambiarEstado', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotaPedido_CambiarEstado;
GO

CREATE PROCEDURE dbo.sp_NotaPedido_CambiarEstado
    @ID      INT,
    @Estado  VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoActual VARCHAR(30);

    SELECT @EstadoActual = Estado
    FROM   dbo.Notas_Pedido
    WHERE  ID = @ID AND Activo = 1;

    IF @EstadoActual IS NULL
    BEGIN
        RAISERROR('La nota de pedido no existe o está dada de baja.', 16, 1);
        RETURN;
    END;

    IF @Estado = 'Preparado'
    BEGIN
        RAISERROR('Un pedido pasa a Preparado cuando Depósito carga el aviso de mercadería preparada.', 16, 1);
        RETURN;
    END;

    IF NOT ((@EstadoActual = 'Preparado' AND @Estado = 'Entregado')
         OR (@EstadoActual = 'Entregado' AND @Estado = 'Facturado'))
    BEGIN
        RAISERROR('No se puede pasar de %s a %s. El orden es Pendiente, Preparado, Entregado, Facturado.', 16, 1, @EstadoActual, @Estado);
        RETURN;
    END;

    BEGIN TRY
        UPDATE dbo.Notas_Pedido
        SET    Estado = @Estado
        WHERE  ID = @ID;
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_NotaPedido_Baja', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotaPedido_Baja;
GO

CREATE PROCEDURE dbo.sp_NotaPedido_Baja
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoActual VARCHAR(30);

    IF NOT EXISTS (SELECT 1 FROM dbo.Notas_Pedido WHERE ID = @ID)
    BEGIN
        RAISERROR('La nota de pedido indicada no existe.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Notas_Pedido WHERE ID = @ID AND Activo = 0)
    BEGIN
        RAISERROR('La nota de pedido ya se encuentra dada de baja.', 16, 1);
        RETURN;
    END;

    SELECT @EstadoActual = Estado FROM dbo.Notas_Pedido WHERE ID = @ID;

    IF @EstadoActual <> 'Pendiente'
    BEGIN
        RAISERROR('Solo se puede dar de baja una nota de pedido Pendiente. Esta está %s.', 16, 1, @EstadoActual);
        RETURN;
    END;

    BEGIN TRY
        UPDATE dbo.Notas_Pedido
        SET    Activo = 0
        WHERE  ID = @ID;
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_NotaPedido_Consultar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotaPedido_Consultar;
GO

CREATE PROCEDURE dbo.sp_NotaPedido_Consultar
    @ID             INT         = NULL,
    @FK_Cliente     INT         = NULL,
    @Estado         VARCHAR(30) = NULL,
    @Incluir_Bajas  BIT         = 0
AS
BEGIN
    SET NOCOUNT ON;

    SET @Estado = NULLIF(LTRIM(RTRIM(@Estado)), '');

    SELECT  ID,
            FK_Cliente,
            Cliente,
            FK_Usuario,
            Usuario,
            FK_Presupuesto_Venta,
            Fecha_Emision,
            Estado,
            Activo,
            Total
    FROM    dbo.vw_Nota_Pedido
    WHERE   (@ID         IS NULL OR ID = @ID)
      AND   (@FK_Cliente IS NULL OR FK_Cliente = @FK_Cliente)
      AND   (@Estado     IS NULL OR Estado = @Estado)
      AND   (@Incluir_Bajas = 1 OR Activo = 1)
    ORDER BY Fecha_Emision DESC, ID DESC;
END;
GO

IF OBJECT_ID('dbo.sp_NotaPedido_Listar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotaPedido_Listar;
GO

CREATE PROCEDURE dbo.sp_NotaPedido_Listar
    @Incluir_Bajas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    EXEC dbo.sp_NotaPedido_Consultar @Incluir_Bajas = @Incluir_Bajas;
END;
GO

IF OBJECT_ID('dbo.sp_DetalleNotaPedido_Alta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DetalleNotaPedido_Alta;
GO

CREATE PROCEDURE dbo.sp_DetalleNotaPedido_Alta
    @FK_Nota_Pedido   INT,
    @FK_Producto      INT,
    @Cantidad         INT,
    @Precio_Unitario  DECIMAL(18,2),
    @ID               INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Notas_Pedido
                   WHERE ID = @FK_Nota_Pedido AND Activo = 1 AND Estado = 'Pendiente')
    BEGIN
        RAISERROR('Solo se pueden cargar líneas en una nota de pedido Pendiente.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Productos WHERE ID = @FK_Producto)
    BEGIN
        RAISERROR('El producto no existe.', 16, 1);
        RETURN;
    END;

    IF @Cantidad IS NULL OR @Cantidad <= 0
    BEGIN
        RAISERROR('La cantidad tiene que ser mayor a cero.', 16, 1);
        RETURN;
    END;

    IF @Precio_Unitario IS NULL OR @Precio_Unitario < 0
    BEGIN
        RAISERROR('El precio no puede ser negativo.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO dbo.Detalle_Notas_Pedido (FK_Nota_Pedido, FK_Producto, Cantidad, Precio_Unitario)
        VALUES (@FK_Nota_Pedido, @FK_Producto, @Cantidad, @Precio_Unitario);

        SET @ID = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_DetalleNotaPedido_BorrarPorNotaPedido', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DetalleNotaPedido_BorrarPorNotaPedido;
GO

CREATE PROCEDURE dbo.sp_DetalleNotaPedido_BorrarPorNotaPedido
    @FK_Nota_Pedido INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Notas_Pedido
                   WHERE ID = @FK_Nota_Pedido AND Activo = 1 AND Estado = 'Pendiente')
    BEGIN
        RAISERROR('Solo se pueden cambiar las líneas de una nota de pedido Pendiente.', 16, 1);
        RETURN;
    END;

    DELETE FROM dbo.Detalle_Notas_Pedido
    WHERE  FK_Nota_Pedido = @FK_Nota_Pedido;
END;
GO

IF OBJECT_ID('dbo.sp_DetalleNotaPedido_Listar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DetalleNotaPedido_Listar;
GO

CREATE PROCEDURE dbo.sp_DetalleNotaPedido_Listar
    @FK_Nota_Pedido INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  d.ID,
            d.FK_Nota_Pedido,
            d.FK_Producto,
            p.Descripcion AS Producto,
            d.Cantidad,
            d.Precio_Unitario
    FROM    dbo.Detalle_Notas_Pedido AS d
    INNER JOIN dbo.vw_Ventas_Productos AS p ON p.ID = d.FK_Producto
    WHERE   d.FK_Nota_Pedido = @FK_Nota_Pedido
    ORDER BY d.ID;
END;
GO

IF OBJECT_ID('dbo.sp_Aviso_Alta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Aviso_Alta;
GO

CREATE PROCEDURE dbo.sp_Aviso_Alta
    @FK_Nota_Pedido  INT,
    @FK_Usuario      INT,
    @Fecha_Emision   DATETIME2(0),
    @ID              INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoNota VARCHAR(30);
    DECLARE @FechaNota DATETIME2(0);

    SELECT @EstadoNota = Estado,
           @FechaNota = Fecha_Emision
    FROM   dbo.Notas_Pedido
    WHERE  ID = @FK_Nota_Pedido AND Activo = 1;

    IF @EstadoNota IS NULL
    BEGIN
        RAISERROR('La nota de pedido no existe o está dada de baja.', 16, 1);
        RETURN;
    END;

    IF @EstadoNota <> 'Pendiente'
    BEGIN
        RAISERROR('Solo se puede avisar la preparación de un pedido Pendiente. Este está %s.', 16, 1, @EstadoNota);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Detalle_Notas_Pedido WHERE FK_Nota_Pedido = @FK_Nota_Pedido)
    BEGIN
        RAISERROR('La nota de pedido no tiene líneas para preparar.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Usuarios WHERE ID = @FK_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision IS NULL OR CAST(@Fecha_Emision AS DATE) > CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha del aviso es obligatoria y no puede ser futura.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision < DATEADD(SECOND, -1, @FechaNota)
    BEGIN
        RAISERROR('La fecha del aviso no puede ser anterior a la del pedido.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Avisos_Mercaderia_Preparada (FK_Nota_Pedido, FK_Usuario, Fecha_Emision, Activo)
        VALUES (@FK_Nota_Pedido, @FK_Usuario, @Fecha_Emision, 1);

        SET @ID = SCOPE_IDENTITY();

        UPDATE dbo.Notas_Pedido
        SET    Estado = 'Preparado'
        WHERE  ID = @FK_Nota_Pedido;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_Aviso_Modificar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Aviso_Modificar;
GO

CREATE PROCEDURE dbo.sp_Aviso_Modificar
    @ID             INT,
    @FK_Usuario     INT,
    @Fecha_Emision  DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FechaNota DATETIME2(0);

    SELECT @FechaNota = n.Fecha_Emision
    FROM   dbo.Avisos_Mercaderia_Preparada AS a
    INNER JOIN dbo.Notas_Pedido AS n ON n.ID = a.FK_Nota_Pedido
    WHERE  a.ID = @ID AND a.Activo = 1;

    IF @FechaNota IS NULL
    BEGIN
        RAISERROR('El aviso no existe o está dado de baja.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Ventas_Usuarios WHERE ID = @FK_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision IS NULL OR CAST(@Fecha_Emision AS DATE) > CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('La fecha del aviso es obligatoria y no puede ser futura.', 16, 1);
        RETURN;
    END;

    IF @Fecha_Emision < DATEADD(SECOND, -1, @FechaNota)
    BEGIN
        RAISERROR('La fecha del aviso no puede ser anterior a la del pedido.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        UPDATE dbo.Avisos_Mercaderia_Preparada
        SET    FK_Usuario    = @FK_Usuario,
               Fecha_Emision = @Fecha_Emision
        WHERE  ID = @ID;
    END TRY
    BEGIN CATCH
        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_Aviso_Baja', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Aviso_Baja;
GO

CREATE PROCEDURE dbo.sp_Aviso_Baja
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FK_Nota_Pedido INT;
    DECLARE @EstadoNota VARCHAR(30);

    IF NOT EXISTS (SELECT 1 FROM dbo.Avisos_Mercaderia_Preparada WHERE ID = @ID)
    BEGIN
        RAISERROR('El aviso indicado no existe.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Avisos_Mercaderia_Preparada WHERE ID = @ID AND Activo = 0)
    BEGIN
        RAISERROR('El aviso ya se encuentra dado de baja.', 16, 1);
        RETURN;
    END;

    SELECT @FK_Nota_Pedido = a.FK_Nota_Pedido,
           @EstadoNota = n.Estado
    FROM   dbo.Avisos_Mercaderia_Preparada AS a
    INNER JOIN dbo.Notas_Pedido AS n ON n.ID = a.FK_Nota_Pedido
    WHERE  a.ID = @ID;

    IF @EstadoNota <> 'Preparado'
    BEGIN
        RAISERROR('No se puede dar de baja el aviso: el pedido ya está %s.', 16, 1, @EstadoNota);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Avisos_Mercaderia_Preparada
        SET    Activo = 0
        WHERE  ID = @ID;

        UPDATE dbo.Notas_Pedido
        SET    Estado = 'Pendiente'
        WHERE  ID = @FK_Nota_Pedido;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @Msg NVARCHAR(4000);
        SET @Msg = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH;
END;
GO

IF OBJECT_ID('dbo.sp_Aviso_Consultar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Aviso_Consultar;
GO

CREATE PROCEDURE dbo.sp_Aviso_Consultar
    @ID              INT  = NULL,
    @FK_Nota_Pedido  INT  = NULL,
    @Fecha_Desde     DATE = NULL,
    @Fecha_Hasta     DATE = NULL,
    @Incluir_Bajas   BIT  = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  ID,
            FK_Nota_Pedido,
            FK_Usuario,
            Usuario,
            Cliente,
            Fecha_Pedido,
            Fecha_Emision,
            Estado_Pedido,
            Activo
    FROM    dbo.vw_Aviso_Mercaderia
    WHERE   (@ID             IS NULL OR ID = @ID)
      AND   (@FK_Nota_Pedido IS NULL OR FK_Nota_Pedido = @FK_Nota_Pedido)
      AND   (@Fecha_Desde    IS NULL OR CAST(Fecha_Emision AS DATE) >= @Fecha_Desde)
      AND   (@Fecha_Hasta    IS NULL OR CAST(Fecha_Emision AS DATE) <= @Fecha_Hasta)
      AND   (@Incluir_Bajas = 1 OR Activo = 1)
    ORDER BY Fecha_Emision DESC, ID DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Aviso_Listar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Aviso_Listar;
GO

CREATE PROCEDURE dbo.sp_Aviso_Listar
    @Incluir_Bajas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    EXEC dbo.sp_Aviso_Consultar @Incluir_Bajas = @Incluir_Bajas;
END;
GO

IF OBJECT_ID('dbo.sp_Rpt_PresupuestosVentaPorClienteEstado', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Rpt_PresupuestosVentaPorClienteEstado;
GO

CREATE PROCEDURE dbo.sp_Rpt_PresupuestosVentaPorClienteEstado
    @FK_Cliente   INT         = NULL,
    @Estado       VARCHAR(30) = NULL,
    @Fecha_Desde  DATE        = NULL,
    @Fecha_Hasta  DATE        = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Estado = NULLIF(LTRIM(RTRIM(@Estado)), '');

    SELECT  Cliente,
            Estado,
            COUNT(*)   AS Cantidad,
            SUM(Total) AS Monto_Total
    FROM    dbo.vw_Presupuesto_Venta
    WHERE   Activo = 1
      AND   (@FK_Cliente  IS NULL OR FK_Cliente = @FK_Cliente)
      AND   (@Estado      IS NULL OR Estado = @Estado)
      AND   (@Fecha_Desde IS NULL OR CAST(Fecha_Emision AS DATE) >= @Fecha_Desde)
      AND   (@Fecha_Hasta IS NULL OR CAST(Fecha_Emision AS DATE) <= @Fecha_Hasta)
    GROUP BY Cliente, Estado
    ORDER BY Cliente, Estado;
END;
GO

IF OBJECT_ID('dbo.sp_Rpt_NotasPedidoPorEstado', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Rpt_NotasPedidoPorEstado;
GO

CREATE PROCEDURE dbo.sp_Rpt_NotasPedidoPorEstado
    @Fecha_Desde  DATE = NULL,
    @Fecha_Hasta  DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  e.Estado,
            COUNT(n.ID)            AS Cantidad,
            ISNULL(SUM(n.Total), 0) AS Monto_Total
    FROM    (SELECT 1 AS Orden, 'Pendiente' AS Estado
             UNION ALL SELECT 2, 'Preparado'
             UNION ALL SELECT 3, 'Entregado'
             UNION ALL SELECT 4, 'Facturado') AS e
    LEFT JOIN dbo.vw_Nota_Pedido AS n
           ON  n.Estado = e.Estado
          AND  n.Activo = 1
          AND  (@Fecha_Desde IS NULL OR CAST(n.Fecha_Emision AS DATE) >= @Fecha_Desde)
          AND  (@Fecha_Hasta IS NULL OR CAST(n.Fecha_Emision AS DATE) <= @Fecha_Hasta)
    GROUP BY e.Orden, e.Estado
    ORDER BY e.Orden;
END;
GO

IF OBJECT_ID('dbo.sp_Rpt_PedidosPendientesPreparar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Rpt_PedidosPendientesPreparar;
GO

CREATE PROCEDURE dbo.sp_Rpt_PedidosPendientesPreparar
    @FK_Cliente INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  n.ID AS Numero_Pedido,
            n.Fecha_Emision AS Fecha_Pedido,
            n.Cliente,
            n.Usuario,
            DATEDIFF(HOUR, n.Fecha_Emision, GETDATE()) AS Horas_Esperando,
            (SELECT COUNT(*) FROM dbo.Detalle_Notas_Pedido AS d
              WHERE d.FK_Nota_Pedido = n.ID) AS Lineas,
            ISNULL((SELECT SUM(d.Cantidad) FROM dbo.Detalle_Notas_Pedido AS d
              WHERE d.FK_Nota_Pedido = n.ID), 0) AS Unidades
    FROM    dbo.vw_Nota_Pedido AS n
    WHERE   n.Activo = 1
      AND   n.Estado = 'Pendiente'
      AND   (@FK_Cliente IS NULL OR n.FK_Cliente = @FK_Cliente)
    ORDER BY n.Fecha_Emision;
END;
GO

IF OBJECT_ID('dbo.sp_Rpt_AvisosPorPeriodo', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Rpt_AvisosPorPeriodo;
GO

CREATE PROCEDURE dbo.sp_Rpt_AvisosPorPeriodo
    @Fecha_Desde  DATE = NULL,
    @Fecha_Hasta  DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  ID AS Numero_Aviso,
            Fecha_Emision AS Fecha_Aviso,
            FK_Nota_Pedido AS Numero_Pedido,
            Fecha_Pedido,
            Cliente,
            Usuario,
            Estado_Pedido,
            CAST(DATEDIFF(MINUTE, Fecha_Pedido, Fecha_Emision) / 60.0 AS DECIMAL(10,2)) AS Horas_Preparacion
    FROM    dbo.vw_Aviso_Mercaderia
    WHERE   Activo = 1
      AND   (@Fecha_Desde IS NULL OR CAST(Fecha_Emision AS DATE) >= @Fecha_Desde)
      AND   (@Fecha_Hasta IS NULL OR CAST(Fecha_Emision AS DATE) <= @Fecha_Hasta)
    ORDER BY Fecha_Emision DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Rpt_TiempoPromedioPreparacion', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Rpt_TiempoPromedioPreparacion;
GO

CREATE PROCEDURE dbo.sp_Rpt_TiempoPromedioPreparacion
    @Fecha_Desde  DATE = NULL,
    @Fecha_Hasta  DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  COUNT(*) AS Pedidos_Preparados,
            CAST(AVG(CAST(DATEDIFF(MINUTE, Fecha_Pedido, Fecha_Emision) AS DECIMAL(18,2))) / 60.0 AS DECIMAL(10,2)) AS Promedio_Horas,
            CAST(MIN(DATEDIFF(MINUTE, Fecha_Pedido, Fecha_Emision)) / 60.0 AS DECIMAL(10,2)) AS Minimo_Horas,
            CAST(MAX(DATEDIFF(MINUTE, Fecha_Pedido, Fecha_Emision)) / 60.0 AS DECIMAL(10,2)) AS Maximo_Horas
    FROM    dbo.vw_Aviso_Mercaderia
    WHERE   Activo = 1
      AND   (@Fecha_Desde IS NULL OR CAST(Fecha_Emision AS DATE) >= @Fecha_Desde)
      AND   (@Fecha_Hasta IS NULL OR CAST(Fecha_Emision AS DATE) <= @Fecha_Hasta);
END;
GO
