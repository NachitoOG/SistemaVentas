USE Sistema_Gestion;
GO

IF OBJECT_ID('dbo.Clientes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes
    (
        ID        INT IDENTITY(1,1) NOT NULL,
        Nombre    NVARCHAR(200)     NOT NULL,
        CUIT_DNI  VARCHAR(20)       NULL,
        Activo    BIT               NOT NULL
            CONSTRAINT DF_Clientes_Activo DEFAULT (1),

        CONSTRAINT PK_Clientes PRIMARY KEY (ID)
    );
END;
GO

IF OBJECT_ID('dbo.Productos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos
    (
        ID            INT IDENTITY(1,1) NOT NULL,
        Codigo        VARCHAR(50)       NOT NULL,
        Descripcion   NVARCHAR(300)     NOT NULL,
        PrecioActual  DECIMAL(18,2)     NOT NULL
            CONSTRAINT DF_Productos_Precio DEFAULT (0),
        StockActual   INT               NOT NULL
            CONSTRAINT DF_Productos_Stock DEFAULT (0),

        CONSTRAINT PK_Productos PRIMARY KEY (ID)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE usuario = 'jperez')
BEGIN
    DECLARE @IdTipoDoc INT;
    DECLARE @IdRol INT;
    DECLARE @Clave NVARCHAR(256);

    SELECT @IdTipoDoc = id_tipodoc FROM dbo.Tipodoc WHERE descripcion = 'DNI';
    SELECT @IdRol = id_rol FROM dbo.Roles WHERE Roles = N'Vendedor';
    SET @Clave = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'jperez'), 2));

    EXEC dbo.sp_RegistrarUsuario 'Juan', 'Perez', 'jperez@ejemplo.com', @IdTipoDoc, '30111222',
                                 'jperez', @Clave, 1, @IdRol;

END;

UPDATE dbo.Usuario
SET    contraseña = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'jperez'), 2)),
       debe_cambiar_clave = 0
WHERE  usuario = 'jperez';

INSERT INTO dbo.Historial_Pass (fyh, contraseña, id_usuario)
SELECT GETDATE(), u.contraseña, u.id_usuario
FROM   dbo.Usuario AS u
WHERE  u.usuario = 'jperez'
  AND  NOT EXISTS (SELECT 1 FROM dbo.Historial_Pass AS h
                   WHERE h.id_usuario = u.id_usuario AND h.contraseña = u.contraseña);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE usuario = 'mgomez')
BEGIN
    DECLARE @IdTipoDoc INT;
    DECLARE @IdRol INT;
    DECLARE @Clave NVARCHAR(256);

    SELECT @IdTipoDoc = id_tipodoc FROM dbo.Tipodoc WHERE descripcion = 'DNI';
    SELECT @IdRol = id_rol FROM dbo.Roles WHERE Roles = N'Vendedor';
    SET @Clave = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'mgomez'), 2));

    EXEC dbo.sp_RegistrarUsuario 'Maria', 'Gomez', 'mgomez@ejemplo.com', @IdTipoDoc, '30333444',
                                 'mgomez', @Clave, 1, @IdRol;

END;

UPDATE dbo.Usuario
SET    contraseña = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'mgomez'), 2)),
       debe_cambiar_clave = 0
WHERE  usuario = 'mgomez';

INSERT INTO dbo.Historial_Pass (fyh, contraseña, id_usuario)
SELECT GETDATE(), u.contraseña, u.id_usuario
FROM   dbo.Usuario AS u
WHERE  u.usuario = 'mgomez'
  AND  NOT EXISTS (SELECT 1 FROM dbo.Historial_Pass AS h
                   WHERE h.id_usuario = u.id_usuario AND h.contraseña = u.contraseña);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE usuario = 'rlopez')
BEGIN
    DECLARE @IdTipoDoc INT;
    DECLARE @IdRol INT;
    DECLARE @Clave NVARCHAR(256);

    SELECT @IdTipoDoc = id_tipodoc FROM dbo.Tipodoc WHERE descripcion = 'DNI';
    SELECT @IdRol = id_rol FROM dbo.Roles WHERE Roles = N'Depósito';
    SET @Clave = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'rlopez'), 2));

    EXEC dbo.sp_RegistrarUsuario 'Roberto', 'Lopez', 'rlopez@ejemplo.com', @IdTipoDoc, '30555666',
                                 'rlopez', @Clave, 1, @IdRol;

END;

UPDATE dbo.Usuario
SET    contraseña = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'rlopez'), 2)),
       debe_cambiar_clave = 0
WHERE  usuario = 'rlopez';

INSERT INTO dbo.Historial_Pass (fyh, contraseña, id_usuario)
SELECT GETDATE(), u.contraseña, u.id_usuario
FROM   dbo.Usuario AS u
WHERE  u.usuario = 'rlopez'
  AND  NOT EXISTS (SELECT 1 FROM dbo.Historial_Pass AS h
                   WHERE h.id_usuario = u.id_usuario AND h.contraseña = u.contraseña);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Clientes)
BEGIN
    INSERT INTO dbo.Clientes (Nombre, CUIT_DNI, Activo) VALUES ('Muebleria El Roble S.A.', '30-71234567-9', 1);
    INSERT INTO dbo.Clientes (Nombre, CUIT_DNI, Activo) VALUES ('Ana Rodriguez', '27-28456789-4', 1);
    INSERT INTO dbo.Clientes (Nombre, CUIT_DNI, Activo) VALUES ('Distribuidora del Sur SRL', '30-69876543-1', 1);
    INSERT INTO dbo.Clientes (Nombre, CUIT_DNI, Activo) VALUES ('Carlos Fernandez', '20-33445566-7', 1);
    INSERT INTO dbo.Clientes (Nombre, CUIT_DNI, Activo) VALUES ('Comercial Antigua S.A.', '30-60001112-3', 0);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Productos)
BEGIN
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('SIL-001', 'Silla de madera reforzada', 45000.00, 120);
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('MES-001', 'Mesa comedor 6 personas', 320000.00, 25);
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('MES-002', 'Mesa ratona vidrio templado', 150000.00, 40);
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('ARM-001', 'Armario 4 puertas melamina', 480000.00, 15);
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('BIB-001', 'Biblioteca 5 estantes', 210000.00, 30);
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('ESC-001', 'Escritorio con cajonera', 260000.00, 22);
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('SOF-001', 'Sofa 3 cuerpos tapizado', 890000.00, 8);
    INSERT INTO dbo.Productos (Codigo, Descripcion, PrecioActual, StockActual) VALUES ('BAN-001', 'Banqueta alta regulable', 68000.00, 60);
END;
GO
