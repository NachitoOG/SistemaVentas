IF DB_ID('Sistema_Gestion') IS NULL
    CREATE DATABASE Sistema_Gestion;
GO

USE Sistema_Gestion;
GO

IF OBJECT_ID('dbo.Familia', 'U') IS NULL
    CREATE TABLE dbo.Familia
    (
        id_familia INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        familia    NVARCHAR(20)      NOT NULL
    );

IF OBJECT_ID('dbo.Roles', 'U') IS NULL
    CREATE TABLE dbo.Roles
    (
        id_rol INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Roles  NVARCHAR(20)      NOT NULL
    );

IF OBJECT_ID('dbo.Tipodoc', 'U') IS NULL
    CREATE TABLE dbo.Tipodoc
    (
        id_tipodoc  INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        descripcion NVARCHAR(20)      NOT NULL
    );

IF OBJECT_ID('dbo.Preguntas', 'U') IS NULL
    CREATE TABLE dbo.Preguntas
    (
        id_pregunta INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        pregunta    NVARCHAR(50)      NOT NULL
    );

IF OBJECT_ID('dbo.Persona', 'U') IS NULL
    CREATE TABLE dbo.Persona
    (
        id_persona INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        apellido   NVARCHAR(16)      NULL,
        nombre     NVARCHAR(16)      NULL,
        id_tipodoc INT               NULL REFERENCES dbo.Tipodoc (id_tipodoc),
        nrodoc     NVARCHAR(8)       NULL,
        correo     NVARCHAR(30)      NULL
    );

IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
    CREATE TABLE dbo.Usuario
    (
        id_usuario         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        usuario            NVARCHAR(20)      NOT NULL,
        estado             BIT               NULL,
        contraseña         NVARCHAR(256)     NOT NULL,
        id_persona         INT               NULL REFERENCES dbo.Persona (id_persona),
        debe_cambiar_clave BIT               NOT NULL CONSTRAINT DF_Usuario_DebeCambiarClave DEFAULT (0)
    );

IF OBJECT_ID('dbo.Historial_Pass', 'U') IS NULL
    CREATE TABLE dbo.Historial_Pass
    (
        id_Historial INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        fyh          DATETIME2(7)      NOT NULL,
        contraseña   NVARCHAR(256)     NOT NULL,
        id_usuario   INT               NULL REFERENCES dbo.Usuario (id_usuario)
    );

IF OBJECT_ID('dbo.Respuestas', 'U') IS NULL
    CREATE TABLE dbo.Respuestas
    (
        idpregunta INT          NOT NULL REFERENCES dbo.Preguntas (id_pregunta),
        id_usuario INT          NOT NULL REFERENCES dbo.Usuario (id_usuario),
        respuesta  NVARCHAR(30) NULL,
        PRIMARY KEY (idpregunta, id_usuario)
    );

IF OBJECT_ID('dbo.Roles_Familia', 'U') IS NULL
    CREATE TABLE dbo.Roles_Familia
    (
        id_rol     INT NOT NULL REFERENCES dbo.Roles (id_rol),
        id_familia INT NOT NULL REFERENCES dbo.Familia (id_familia),
        PRIMARY KEY (id_rol, id_familia)
    );

IF OBJECT_ID('dbo.usuario_roles', 'U') IS NULL
    CREATE TABLE dbo.usuario_roles
    (
        Id_usuario INT NOT NULL REFERENCES dbo.Usuario (id_usuario),
        id_rol     INT NOT NULL REFERENCES dbo.Roles (id_rol),
        PRIMARY KEY (Id_usuario, id_rol)
    );

IF OBJECT_ID('dbo.Permisos', 'U') IS NULL
    CREATE TABLE dbo.Permisos
    (
        id_permiso INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        id_rol     INT               NULL REFERENCES dbo.Roles (id_rol),
        permiso    NVARCHAR(20)      NULL
    );

IF OBJECT_ID('dbo.usuario_permisos', 'U') IS NULL
    CREATE TABLE dbo.usuario_permisos
    (
        id_usuario INT NOT NULL REFERENCES dbo.Usuario (id_usuario),
        id_permiso INT NOT NULL REFERENCES dbo.Permisos (id_permiso),
        PRIMARY KEY (id_usuario, id_permiso)
    );

IF OBJECT_ID('dbo.Politicas_Seguridad', 'U') IS NULL
    CREATE TABLE dbo.Politicas_Seguridad
    (
        id_politica                     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Politicas_Seguridad PRIMARY KEY,
        min_caracteres                  INT NOT NULL DEFAULT (8),
        cant_preguntas_validar          INT NOT NULL DEFAULT (3),
        requiere_mayus_minus            BIT NOT NULL DEFAULT (1),
        requiere_alfanumerico           BIT NOT NULL DEFAULT (1),
        requiere_caracter_especial      BIT NOT NULL DEFAULT (1),
        requiere_2fa_correo             BIT NOT NULL DEFAULT (0),
        bloquear_contrasenas_anteriores BIT NOT NULL DEFAULT (1),
        bloquear_datos_personales       BIT NOT NULL DEFAULT (1)
    );
GO

IF COL_LENGTH('dbo.Usuario', 'debe_cambiar_clave') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
        ADD debe_cambiar_clave BIT NOT NULL CONSTRAINT DF_Usuario_DebeCambiarClave DEFAULT (0) WITH VALUES;

    EXEC ('UPDATE dbo.Usuario SET debe_cambiar_clave = 1
           WHERE (SELECT COUNT(*) FROM dbo.Historial_Pass AS h WHERE h.id_usuario = Usuario.id_usuario) = 1;');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tipodoc WHERE descripcion = 'DNI')
    INSERT INTO dbo.Tipodoc (descripcion) VALUES ('DNI');
IF NOT EXISTS (SELECT 1 FROM dbo.Tipodoc WHERE descripcion = 'Pasaporte')
    INSERT INTO dbo.Tipodoc (descripcion) VALUES ('Pasaporte');
IF NOT EXISTS (SELECT 1 FROM dbo.Tipodoc WHERE descripcion = 'CUIL / CUIT')
    INSERT INTO dbo.Tipodoc (descripcion) VALUES ('CUIL / CUIT');

IF NOT EXISTS (SELECT 1 FROM dbo.Preguntas)
BEGIN
    INSERT INTO dbo.Preguntas (pregunta) VALUES (N'¿Cuál es tu sistema operativo preferido?');
    INSERT INTO dbo.Preguntas (pregunta) VALUES (N'¿Cuántos años de experiencia tienes?');
    INSERT INTO dbo.Preguntas (pregunta) VALUES (N'¿Cuál es tu lenguaje de programación favorito?');
    INSERT INTO dbo.Preguntas (pregunta) VALUES (N'¿Cuál es tu color favorito?');
    INSERT INTO dbo.Preguntas (pregunta) VALUES (N'¿Cuántos años tienes?');
    INSERT INTO dbo.Preguntas (pregunta) VALUES (N'¿En qué ciudad vives?');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Politicas_Seguridad)
    INSERT INTO dbo.Politicas_Seguridad (min_caracteres, cant_preguntas_validar, requiere_mayus_minus, requiere_alfanumerico,
                                         requiere_caracter_especial, requiere_2fa_correo, bloquear_contrasenas_anteriores,
                                         bloquear_datos_personales)
    VALUES (8, 3, 1, 1, 1, 0, 1, 1);
GO

DECLARE @Roles TABLE (rol NVARCHAR(20), familia NVARCHAR(20));
INSERT INTO @Roles VALUES (N'Administrador', N'Administración');
INSERT INTO @Roles VALUES (N'Vendedor', N'Ventas');
INSERT INTO @Roles VALUES (N'Depósito', N'Ventas');
INSERT INTO @Roles VALUES (N'Usuario general', N'Otros módulos');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Roles = N'Administrador')
    UPDATE dbo.Roles SET Roles = N'Administrador' WHERE Roles = N'Admin';

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Roles = N'Depósito')
    UPDATE dbo.Roles SET Roles = N'Depósito' WHERE Roles = N'Jefe de Depósito';

IF NOT EXISTS (SELECT 1 FROM dbo.Familia WHERE familia = N'Ventas')
   AND EXISTS (SELECT 1 FROM dbo.Familia WHERE familia = N'Comercial')
    UPDATE dbo.Familia SET familia = N'Ventas' WHERE familia = N'Comercial';

INSERT INTO dbo.Familia (familia)
SELECT DISTINCT r.familia FROM @Roles AS r
WHERE NOT EXISTS (SELECT 1 FROM dbo.Familia AS f WHERE f.familia = r.familia);

INSERT INTO dbo.Roles (Roles)
SELECT r.rol FROM @Roles AS r
WHERE NOT EXISTS (SELECT 1 FROM dbo.Roles AS x WHERE x.Roles = r.rol);

DELETE FROM dbo.Roles_Familia;

INSERT INTO dbo.Roles_Familia (id_rol, id_familia)
SELECT ro.id_rol, (SELECT MIN(f.id_familia) FROM dbo.Familia AS f WHERE f.familia = r.familia)
FROM @Roles AS r
INNER JOIN dbo.Roles AS ro ON ro.Roles = r.rol;

DELETE FROM dbo.Familia
WHERE id_familia NOT IN (SELECT id_familia FROM dbo.Roles_Familia);
GO

IF OBJECT_ID('dbo.sp_EliminarUsuarioCompleto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_EliminarUsuarioCompleto;
GO

IF OBJECT_ID('dbo.SP_validacion_contraseña', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_validacion_contraseña;
GO

CREATE PROCEDURE dbo.SP_validacion_contraseña
    @usuario            VARCHAR(20),
    @contraseña_escrita NVARCHAR(100),
    @coincide_pass      BIT OUTPUT,
    @coincide_user      BIT OUTPUT,
    @primer_login       BIT OUTPUT,
    @id_usuario         INT OUTPUT,
    @activo             BIT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @contraseña_real NVARCHAR(256);
    DECLARE @debe_cambiar BIT;

    SET @coincide_pass = 0;
    SET @coincide_user = 0;
    SET @primer_login = 0;
    SET @id_usuario = 0;
    SET @activo = 0;

    SELECT @contraseña_real = contraseña,
           @id_usuario = id_usuario,
           @activo = ISNULL(estado, 0),
           @debe_cambiar = debe_cambiar_clave
    FROM   dbo.Usuario
    WHERE  usuario = @usuario;

    IF @id_usuario > 0
    BEGIN
        SET @coincide_user = 1;

        IF @contraseña_escrita = @contraseña_real
        BEGIN
            SET @coincide_pass = 1;
            SET @primer_login = @debe_cambiar;
        END;
    END;
END;
GO

IF OBJECT_ID('dbo.sp_MigrarContraseña', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_MigrarContraseña;
GO

CREATE PROCEDURE dbo.sp_MigrarContraseña
    @id_usuario     INT,
    @nueva_contraseña NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Usuario
    SET    contraseña = @nueva_contraseña
    WHERE  id_usuario = @id_usuario;
END;
GO

IF OBJECT_ID('dbo.sp_CambiarContraseñaPrimerLogin', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CambiarContraseñaPrimerLogin;
GO

CREATE PROCEDURE dbo.sp_CambiarContraseñaPrimerLogin
    @id_usuario       INT,
    @nueva_contraseña NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE dbo.Usuario
        SET    contraseña = @nueva_contraseña,
               debe_cambiar_clave = 0
        WHERE  id_usuario = @id_usuario;

        INSERT INTO dbo.Historial_Pass (fyh, contraseña, id_usuario)
        VALUES (GETDATE(), @nueva_contraseña, @id_usuario);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('dbo.sp_RestablecerContraseña', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RestablecerContraseña;
GO

CREATE PROCEDURE dbo.sp_RestablecerContraseña
    @id_usuario       INT,
    @nueva_contraseña NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        UPDATE dbo.Usuario
        SET    contraseña = @nueva_contraseña,
               debe_cambiar_clave = 1
        WHERE  id_usuario = @id_usuario;

        INSERT INTO dbo.Historial_Pass (fyh, contraseña, id_usuario)
        VALUES (GETDATE(), @nueva_contraseña, @id_usuario);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('dbo.sp_RegistrarUsuario', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RegistrarUsuario;
GO

CREATE PROCEDURE dbo.sp_RegistrarUsuario
    @nombre     NVARCHAR(16),
    @apellido   NVARCHAR(16),
    @correo     NVARCHAR(30),
    @id_tipodoc INT,
    @nrodoc     NVARCHAR(8),
    @usuario    NVARCHAR(20),
    @contraseña NVARCHAR(256),
    @estado     BIT,
    @id_rol     INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @id_persona INT;
        DECLARE @id_usuario INT;

        INSERT INTO dbo.Persona (nombre, apellido, id_tipodoc, nrodoc, correo)
        VALUES (@nombre, @apellido, @id_tipodoc, @nrodoc, @correo);

        SET @id_persona = SCOPE_IDENTITY();

        INSERT INTO dbo.Usuario (usuario, estado, contraseña, id_persona, debe_cambiar_clave)
        VALUES (@usuario, @estado, @contraseña, @id_persona, 1);

        SET @id_usuario = SCOPE_IDENTITY();

        INSERT INTO dbo.Historial_Pass (fyh, contraseña, id_usuario)
        VALUES (GETDATE(), @contraseña, @id_usuario);

        IF @id_rol > 0
            INSERT INTO dbo.usuario_roles (Id_usuario, id_rol) VALUES (@id_usuario, @id_rol);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('dbo.sp_ModificarUsuarioCompleto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ModificarUsuarioCompleto;
GO

CREATE PROCEDURE dbo.sp_ModificarUsuarioCompleto
    @id_usuario INT,
    @nombre     NVARCHAR(16),
    @apellido   NVARCHAR(16),
    @correo     NVARCHAR(30),
    @id_tipodoc INT,
    @nrodoc     NVARCHAR(8),
    @estado     BIT,
    @id_rol     INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @id_persona INT;

        SELECT @id_persona = id_persona FROM dbo.Usuario WHERE id_usuario = @id_usuario;

        UPDATE dbo.Persona
        SET    nombre = @nombre,
               apellido = @apellido,
               correo = @correo,
               id_tipodoc = @id_tipodoc,
               nrodoc = @nrodoc
        WHERE  id_persona = @id_persona;

        UPDATE dbo.Usuario
        SET    estado = @estado
        WHERE  id_usuario = @id_usuario;

        DELETE FROM dbo.usuario_roles WHERE Id_usuario = @id_usuario;

        IF @id_rol > 0
            INSERT INTO dbo.usuario_roles (Id_usuario, id_rol) VALUES (@id_usuario, @id_rol);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

IF OBJECT_ID('dbo.sp_CambiarEstadoUsuario', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CambiarEstadoUsuario;
GO

CREATE PROCEDURE dbo.sp_CambiarEstadoUsuario
    @id_usuario INT,
    @estado     BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Usuario
    SET    estado = @estado
    WHERE  id_usuario = @id_usuario;
END;
GO

IF OBJECT_ID('dbo.sp_ListarUsuariosCompletos', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ListarUsuariosCompletos;
GO

CREATE PROCEDURE dbo.sp_ListarUsuariosCompletos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT U.id_usuario,
           P.nombre,
           P.apellido,
           P.correo,
           P.id_tipodoc,
           TD.descripcion AS tipo_documento,
           P.nrodoc,
           U.usuario,
           ISNULL(U.estado, 0) AS estado,
           UR.id_rol,
           R.Roles AS nombre_rol,
           F.id_familia,
           F.familia AS nombre_familia
    FROM   dbo.Usuario AS U
    INNER JOIN dbo.Persona AS P ON U.id_persona = P.id_persona
    INNER JOIN dbo.Tipodoc AS TD ON P.id_tipodoc = TD.id_tipodoc
    LEFT JOIN dbo.usuario_roles AS UR ON U.id_usuario = UR.Id_usuario
    LEFT JOIN dbo.Roles AS R ON UR.id_rol = R.id_rol
    LEFT JOIN dbo.Roles_Familia AS RF ON R.id_rol = RF.id_rol
    LEFT JOIN dbo.Familia AS F ON RF.id_familia = F.id_familia
    ORDER BY P.apellido, P.nombre;
END;
GO

IF OBJECT_ID('dbo.sp_ListarRoles', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ListarRoles;
GO

CREATE PROCEDURE dbo.sp_ListarRoles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.id_rol, r.Roles, f.familia
    FROM   dbo.Roles AS r
    LEFT JOIN dbo.Roles_Familia AS rf ON rf.id_rol = r.id_rol
    LEFT JOIN dbo.Familia AS f ON f.id_familia = rf.id_familia
    ORDER BY f.familia, r.Roles;
END;
GO

IF OBJECT_ID('dbo.sp_ListarTipoDoc', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ListarTipoDoc;
GO

CREATE PROCEDURE dbo.sp_ListarTipoDoc
AS
BEGIN
    SET NOCOUNT ON;

    SELECT id_tipodoc, descripcion FROM dbo.Tipodoc;
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerRolesUsuario', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerRolesUsuario;
GO

CREATE PROCEDURE dbo.sp_ObtenerRolesUsuario
    @id_usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT R.Roles AS NombreRol,
           ISNULL(F.familia, '') AS NombreFamilia
    FROM   dbo.usuario_roles AS UR
    INNER JOIN dbo.Roles AS R ON UR.id_rol = R.id_rol
    LEFT JOIN dbo.Roles_Familia AS RF ON RF.id_rol = R.id_rol
    LEFT JOIN dbo.Familia AS F ON F.id_familia = RF.id_familia
    WHERE  UR.Id_usuario = @id_usuario;
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerFamiliaPorRol', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerFamiliaPorRol;
GO

CREATE PROCEDURE dbo.sp_ObtenerFamiliaPorRol
    @id_rol INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT f.familia
    FROM   dbo.Familia AS f
    INNER JOIN dbo.Roles_Familia AS rf ON f.id_familia = rf.id_familia
    WHERE  rf.id_rol = @id_rol;
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerDatosPersonalesUsuario', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerDatosPersonalesUsuario;
GO

CREATE PROCEDURE dbo.sp_ObtenerDatosPersonalesUsuario
    @id_usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.nombre, p.apellido, p.nrodoc, p.correo
    FROM   dbo.Usuario AS u
    INNER JOIN dbo.Persona AS p ON u.id_persona = p.id_persona
    WHERE  u.id_usuario = @id_usuario;
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerPoliticasSeguridad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerPoliticasSeguridad;
GO

CREATE PROCEDURE dbo.sp_ObtenerPoliticasSeguridad
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 min_caracteres,
           cant_preguntas_validar,
           requiere_mayus_minus,
           requiere_alfanumerico,
           requiere_caracter_especial,
           requiere_2fa_correo,
           bloquear_contrasenas_anteriores,
           bloquear_datos_personales
    FROM   dbo.Politicas_Seguridad;
END;
GO

IF OBJECT_ID('dbo.sp_ActualizarPoliticasSeguridad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ActualizarPoliticasSeguridad;
GO

CREATE PROCEDURE dbo.sp_ActualizarPoliticasSeguridad
    @min_caracteres                  INT,
    @cant_preguntas_validar          INT,
    @requiere_mayus_minus            BIT,
    @requiere_alfanumerico           BIT,
    @requiere_caracter_especial      BIT,
    @requiere_2fa_correo             BIT,
    @bloquear_contrasenas_anteriores BIT,
    @bloquear_datos_personales       BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE TOP (1) dbo.Politicas_Seguridad
    SET    min_caracteres = @min_caracteres,
           cant_preguntas_validar = @cant_preguntas_validar,
           requiere_mayus_minus = @requiere_mayus_minus,
           requiere_alfanumerico = @requiere_alfanumerico,
           requiere_caracter_especial = @requiere_caracter_especial,
           requiere_2fa_correo = @requiere_2fa_correo,
           bloquear_contrasenas_anteriores = @bloquear_contrasenas_anteriores,
           bloquear_datos_personales = @bloquear_datos_personales;
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerPreguntaAlAzar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerPreguntaAlAzar;
GO

CREATE PROCEDURE dbo.sp_ObtenerPreguntaAlAzar
    @cantidad INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@cantidad) id_pregunta, pregunta
    FROM   dbo.Preguntas
    ORDER BY NEWID();
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerPreguntasRecuperacion', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerPreguntasRecuperacion;
GO

CREATE PROCEDURE dbo.sp_ObtenerPreguntasRecuperacion
    @usuario NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT U.id_usuario,
           P.correo,
           R.idpregunta,
           Pr.pregunta
    FROM   dbo.Usuario AS U
    INNER JOIN dbo.Persona AS P ON U.id_persona = P.id_persona
    INNER JOIN dbo.Respuestas AS R ON U.id_usuario = R.id_usuario
    INNER JOIN dbo.Preguntas AS Pr ON R.idpregunta = Pr.id_pregunta
    WHERE  U.usuario = @usuario;
END;
GO

IF OBJECT_ID('dbo.sp_GuardarRespuestaSeguridad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GuardarRespuestaSeguridad;
GO

CREATE PROCEDURE dbo.sp_GuardarRespuestaSeguridad
    @id_pregunta INT,
    @id_usuario  INT,
    @respuesta   NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Respuestas WHERE idpregunta = @id_pregunta AND id_usuario = @id_usuario)
        UPDATE dbo.Respuestas
        SET    respuesta = @respuesta
        WHERE  idpregunta = @id_pregunta AND id_usuario = @id_usuario;
    ELSE
        INSERT INTO dbo.Respuestas (idpregunta, id_usuario, respuesta)
        VALUES (@id_pregunta, @id_usuario, @respuesta);
END;
GO

IF OBJECT_ID('dbo.sp_ActualizarRespuestaSeguridad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ActualizarRespuestaSeguridad;
GO

CREATE PROCEDURE dbo.sp_ActualizarRespuestaSeguridad
    @id_usuario      INT,
    @id_pregunta     INT,
    @nueva_respuesta NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Respuestas
    SET    respuesta = TRIM(UPPER(@nueva_respuesta))
    WHERE  id_usuario = @id_usuario AND idpregunta = @id_pregunta;
END;
GO

IF OBJECT_ID('dbo.sp_ObtenerRespuestaSeguridad', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerRespuestaSeguridad;
GO

CREATE PROCEDURE dbo.sp_ObtenerRespuestaSeguridad
    @IdUsuario  INT,
    @IdPregunta INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT respuesta
    FROM   dbo.Respuestas
    WHERE  id_usuario = @IdUsuario AND idpregunta = @IdPregunta;
END;
GO

IF OBJECT_ID('dbo.sp_ValidarRespuesta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ValidarRespuesta;
GO

CREATE PROCEDURE dbo.sp_ValidarRespuesta
    @id_usuario  INT,
    @id_pregunta INT,
    @respuesta   NVARCHAR(30),
    @es_valido   BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1
               FROM   dbo.Respuestas
               WHERE  id_usuario = @id_usuario
                 AND  idpregunta = @id_pregunta
                 AND  TRIM(respuesta) = TRIM(@respuesta))
        SET @es_valido = 1;
    ELSE
        SET @es_valido = 0;
END;
GO

IF OBJECT_ID('dbo.sp_VerificarHistorialPass', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_VerificarHistorialPass;
GO

CREATE PROCEDURE dbo.sp_VerificarHistorialPass
    @id_usuario INT,
    @pass_hash  NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM   dbo.Historial_Pass
    WHERE  id_usuario = @id_usuario AND contraseña = @pass_hash;
END;
GO

IF OBJECT_ID('dbo.sp_VerificarPersonaDuplicada', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_VerificarPersonaDuplicada;
GO

CREATE PROCEDURE dbo.sp_VerificarPersonaDuplicada
    @nrodoc     VARCHAR(50),
    @correo     VARCHAR(100),
    @id_usuario INT = 0,
    @Existe     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @id_persona_ignorar INT = 0;

    IF @id_usuario > 0
        SELECT @id_persona_ignorar = id_persona FROM dbo.Usuario WHERE id_usuario = @id_usuario;

    IF EXISTS (SELECT 1
               FROM   dbo.Persona
               WHERE  (nrodoc = @nrodoc OR correo = @correo)
                 AND  (@id_persona_ignorar = 0 OR id_persona <> @id_persona_ignorar))
        SET @Existe = 1;
    ELSE
        SET @Existe = 0;
END;
GO

IF OBJECT_ID('dbo.sp_VerificarUsuarioExiste', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_VerificarUsuarioExiste;
GO

CREATE PROCEDURE dbo.sp_VerificarUsuarioExiste
    @usuario    VARCHAR(50),
    @id_usuario INT = 0,
    @Existe     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1
               FROM   dbo.Usuario
               WHERE  usuario = @usuario
                 AND  (@id_usuario = 0 OR id_usuario <> @id_usuario))
        SET @Existe = 1;
    ELSE
        SET @Existe = 0;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE usuario = 'admin')
BEGIN
    DECLARE @IdTipoDoc INT;
    DECLARE @IdRol INT;
    DECLARE @Clave NVARCHAR(256);

    SELECT @IdTipoDoc = id_tipodoc FROM dbo.Tipodoc WHERE descripcion = 'DNI';
    SELECT @IdRol = id_rol FROM dbo.Roles WHERE Roles = N'Administrador';
    SET @Clave = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'admin'), 2));

    EXEC dbo.sp_RegistrarUsuario 'Administrador', 'Sistema', 'admin@ejemplo.com', @IdTipoDoc, '30000001', 'admin', @Clave, 1, @IdRol;

    UPDATE dbo.Usuario SET debe_cambiar_clave = 0 WHERE usuario = 'admin';
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE usuario = 'lsosa')
BEGIN
    DECLARE @IdTipoDoc INT;
    DECLARE @IdRol INT;
    DECLARE @Clave NVARCHAR(256);

    SELECT @IdTipoDoc = id_tipodoc FROM dbo.Tipodoc WHERE descripcion = 'DNI';
    SELECT @IdRol = id_rol FROM dbo.Roles WHERE Roles = N'Usuario general';
    SET @Clave = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'Ventas.2026' + 'lsosa'), 2));

    EXEC dbo.sp_RegistrarUsuario 'Lucia', 'Sosa', 'lsosa@ejemplo.com', @IdTipoDoc, '30000002', 'lsosa', @Clave, 1, @IdRol;

    UPDATE dbo.Usuario SET debe_cambiar_clave = 0 WHERE usuario = 'lsosa';
END;
GO

PRINT 'Gestión de Usuarios lista.';
GO
