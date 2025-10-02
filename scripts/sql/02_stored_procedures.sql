-- Stored procedures for ReyRecords platform

IF OBJECT_ID('seg.procCatUsuariosIns', 'P') IS NOT NULL
    DROP PROCEDURE seg.procCatUsuariosIns;
GO

IF OBJECT_ID('seg.procCatRolesConPorNombre', 'P') IS NOT NULL
    DROP PROCEDURE seg.procCatRolesConPorNombre;
GO

CREATE PROCEDURE seg.procCatRolesConPorNombre
    @pNombre NVARCHAR(100),
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM seg.CatRoles WHERE Nombre = @pNombre AND Activo = 1)
    BEGIN
        SET @pResultado = 1;
        SET @pMsg = 'Rol localizado correctamente.';

        SELECT TOP 1 RolId, Nombre, Descripcion, EsAdmin
        FROM seg.CatRoles
        WHERE Nombre = @pNombre AND Activo = 1;
    END
    ELSE
    BEGIN
        SET @pResultado = 0;
        SET @pMsg = 'El rol solicitado no existe o está inactivo.';
    END;
END;
GO

CREATE PROCEDURE seg.procCatUsuariosIns
    @pRolId INT,
    @pNombre NVARCHAR(120),
    @pApellidos NVARCHAR(160) = NULL,
    @pCorreo NVARCHAR(256),
    @pPasswordHash VARBINARY(512),
    @pPasswordSalt VARBINARY(256),
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT,
    @pUsuarioId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF EXISTS (SELECT 1 FROM seg.CatUsuarios WHERE Correo = @pCorreo)
        BEGIN
            SET @pResultado = 0;
            SET @pMsg = 'El correo electrónico ya se encuentra registrado.';
            ROLLBACK TRAN;
            RETURN;
        END;

        INSERT INTO seg.CatUsuarios (RolId, Nombre, Apellidos, Correo, PasswordHash, PasswordSalt)
        VALUES (@pRolId, @pNombre, @pApellidos, @pCorreo, @pPasswordHash, @pPasswordSalt);

        SET @pUsuarioId = CAST(SCOPE_IDENTITY() AS INT);

        COMMIT TRAN;

        SET @pResultado = 1;
        SET @pMsg = 'Usuario registrado correctamente.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        SET @pResultado = 0;
        SET @pMsg = ERROR_MESSAGE();
    END CATCH;
END;
GO

IF OBJECT_ID('seg.procCatUsuariosConLogin', 'P') IS NOT NULL
    DROP PROCEDURE seg.procCatUsuariosConLogin;
GO

IF OBJECT_ID('seg.procCatUsuariosActUltimoAcceso', 'P') IS NOT NULL
    DROP PROCEDURE seg.procCatUsuariosActUltimoAcceso;
GO

CREATE PROCEDURE seg.procCatUsuariosActUltimoAcceso
    @pUsuarioId INT,
    @pFecha DATETIME2,
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        UPDATE seg.CatUsuarios
        SET UltimoAcceso = @pFecha
        WHERE UsuarioId = @pUsuarioId;

        IF @@ROWCOUNT = 0
        BEGIN
            SET @pResultado = 0;
            SET @pMsg = 'No fue posible actualizar el acceso del usuario indicado.';
            ROLLBACK TRAN;
            RETURN;
        END;

        COMMIT TRAN;

        SET @pResultado = 1;
        SET @pMsg = 'Último acceso actualizado correctamente.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        SET @pResultado = 0;
        SET @pMsg = ERROR_MESSAGE();
    END CATCH;
END;
GO

CREATE PROCEDURE seg.procCatUsuariosConLogin
    @pCorreo NVARCHAR(256),
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM seg.CatUsuarios WHERE Correo = @pCorreo AND Activo = 1)
    BEGIN
        SET @pResultado = 1;
        SET @pMsg = 'Usuario localizado correctamente.';

        SELECT TOP 1
            u.UsuarioId,
            u.Nombre,
            u.Apellidos,
            u.Correo,
            u.PasswordHash,
            u.PasswordSalt,
            u.RolId,
            r.Nombre AS RolNombre,
            r.EsAdmin,
            u.Activo,
            u.FechaRegistro,
            u.UltimoAcceso
        FROM seg.CatUsuarios u
        INNER JOIN seg.CatRoles r ON u.RolId = r.RolId
        WHERE u.Correo = @pCorreo AND u.Activo = 1;
    END
    ELSE
    BEGIN
        SET @pResultado = 0;
        SET @pMsg = 'No se encontró el usuario solicitado o está inactivo.';
    END;
END;
GO

IF OBJECT_ID('seg.procCatUsuariosConPorId', 'P') IS NOT NULL
    DROP PROCEDURE seg.procCatUsuariosConPorId;
GO

CREATE PROCEDURE seg.procCatUsuariosConPorId
    @pUsuarioId INT,
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM seg.CatUsuarios WHERE UsuarioId = @pUsuarioId AND Activo = 1)
    BEGIN
        SET @pResultado = 1;
        SET @pMsg = 'Usuario localizado correctamente.';

        SELECT TOP 1
            u.UsuarioId,
            u.Nombre,
            u.Apellidos,
            u.Correo,
            u.RolId,
            r.Nombre AS RolNombre,
            r.EsAdmin,
            u.FechaRegistro,
            u.UltimoAcceso
        FROM seg.CatUsuarios u
        INNER JOIN seg.CatRoles r ON u.RolId = r.RolId
        WHERE u.UsuarioId = @pUsuarioId;
    END
    ELSE
    BEGIN
        SET @pResultado = 0;
        SET @pMsg = 'No se encontró el usuario solicitado o está inactivo.';
    END;
END;
GO

IF OBJECT_ID('seg.procOpCancionesConPorUsuario', 'P') IS NOT NULL
    DROP PROCEDURE seg.procOpCancionesConPorUsuario;
GO

CREATE PROCEDURE seg.procOpCancionesConPorUsuario
    @pUsuarioId INT,
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM seg.CatUsuarios WHERE UsuarioId = @pUsuarioId)
    BEGIN
        SET @pResultado = 1;
        SET @pMsg = 'Canciones obtenidas correctamente.';

        SELECT
            c.CancionId,
            c.UsuarioId,
            c.Titulo,
            c.Descripcion,
            c.TotalReproducciones,
            c.MontoGanado,
            c.FechaPublicacion,
            c.Activo
        FROM seg.OpCanciones c
        WHERE c.UsuarioId = @pUsuarioId
        ORDER BY c.FechaPublicacion DESC;
    END
    ELSE
    BEGIN
        SET @pResultado = 0;
        SET @pMsg = 'El usuario indicado no existe.';
    END;
END;
GO

IF OBJECT_ID('seg.procOpCancionesConResumen', 'P') IS NOT NULL
    DROP PROCEDURE seg.procOpCancionesConResumen;
GO

CREATE PROCEDURE seg.procOpCancionesConResumen
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM seg.OpCanciones)
    BEGIN
        SET @pResultado = 1;
        SET @pMsg = 'Información global obtenida correctamente.';

        SELECT
            u.UsuarioId,
            u.Nombre,
            u.Apellidos,
            u.Correo,
            r.Nombre AS RolNombre,
            c.CancionId,
            c.Titulo,
            c.TotalReproducciones,
            c.MontoGanado,
            c.FechaPublicacion
        FROM seg.OpCanciones c
        INNER JOIN seg.CatUsuarios u ON c.UsuarioId = u.UsuarioId
        INNER JOIN seg.CatRoles r ON u.RolId = r.RolId
        ORDER BY u.Nombre, c.Titulo;
    END
    ELSE
    BEGIN
        SET @pResultado = 1;
        SET @pMsg = 'No existen canciones registradas.';
        SELECT TOP 0
            CAST(NULL AS INT) AS UsuarioId,
            CAST(NULL AS NVARCHAR(120)) AS Nombre,
            CAST(NULL AS NVARCHAR(160)) AS Apellidos,
            CAST(NULL AS NVARCHAR(256)) AS Correo,
            CAST(NULL AS NVARCHAR(100)) AS RolNombre,
            CAST(NULL AS INT) AS CancionId,
            CAST(NULL AS NVARCHAR(200)) AS Titulo,
            CAST(NULL AS BIGINT) AS TotalReproducciones,
            CAST(NULL AS DECIMAL(18,4)) AS MontoGanado,
            CAST(NULL AS DATETIME2) AS FechaPublicacion;
    END;
END;
GO

IF OBJECT_ID('seg.procLogReproduccionesIns', 'P') IS NOT NULL
    DROP PROCEDURE seg.procLogReproduccionesIns;
GO

CREATE PROCEDURE seg.procLogReproduccionesIns
    @pCancionId INT,
    @pPlataforma NVARCHAR(120) = NULL,
    @pImporte DECIMAL(18, 6),
    @pResultado BIT OUTPUT,
    @pMsg VARCHAR(500) OUTPUT,
    @pReproduccionId BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM seg.OpCanciones WHERE CancionId = @pCancionId AND Activo = 1)
        BEGIN
            SET @pResultado = 0;
            SET @pMsg = 'La canción indicada no existe o está inactiva.';
            ROLLBACK TRAN;
            RETURN;
        END;

        INSERT INTO seg.LogReproducciones (CancionId, Plataforma, Importe)
        VALUES (@pCancionId, @pPlataforma, @pImporte);

        SET @pReproduccionId = CAST(SCOPE_IDENTITY() AS BIGINT);

        COMMIT TRAN;

        SET @pResultado = 1;
        SET @pMsg = 'Reproducción registrada correctamente.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        SET @pResultado = 0;
        SET @pMsg = ERROR_MESSAGE();
    END CATCH;
END;
GO
