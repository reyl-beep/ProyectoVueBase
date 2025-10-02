-- Database schema for ReyRecords platform
-- Ensure execution within the desired database context (e.g., USE ReyRecordsDB;)

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'seg')
BEGIN
    EXEC('CREATE SCHEMA seg AUTHORIZATION dbo;');
END;
GO

IF OBJECT_ID('seg.CatRoles', 'U') IS NOT NULL
    DROP TABLE seg.CatRoles;
GO

CREATE TABLE seg.CatRoles
(
    RolId INT IDENTITY(1, 1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    EsAdmin BIT NOT NULL DEFAULT(0),
    Activo BIT NOT NULL DEFAULT(1),
    FechaCreacion DATETIME2 NOT NULL DEFAULT(SYSDATETIME())
);
GO

IF OBJECT_ID('seg.CatUsuarios', 'U') IS NOT NULL
    DROP TABLE seg.CatUsuarios;
GO

CREATE TABLE seg.CatUsuarios
(
    UsuarioId INT IDENTITY(1, 1) PRIMARY KEY,
    RolId INT NOT NULL,
    Nombre NVARCHAR(120) NOT NULL,
    Apellidos NVARCHAR(160) NULL,
    Correo NVARCHAR(256) NOT NULL,
    PasswordHash VARBINARY(512) NOT NULL,
    PasswordSalt VARBINARY(256) NOT NULL,
    FechaRegistro DATETIME2 NOT NULL DEFAULT(SYSDATETIME()),
    Activo BIT NOT NULL DEFAULT(1),
    UltimoAcceso DATETIME2 NULL,
    CONSTRAINT UK_CatUsuarios_Correo UNIQUE (Correo),
    CONSTRAINT FK_CatUsuarios_CatRoles FOREIGN KEY (RolId) REFERENCES seg.CatRoles (RolId)
);
GO

IF OBJECT_ID('seg.OpCanciones', 'U') IS NOT NULL
    DROP TABLE seg.OpCanciones;
GO

CREATE TABLE seg.OpCanciones
(
    CancionId INT IDENTITY(1, 1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Titulo NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    FechaPublicacion DATETIME2 NOT NULL DEFAULT(SYSDATETIME()),
    TotalReproducciones BIGINT NOT NULL DEFAULT(0),
    MontoGanado DECIMAL(18, 4) NOT NULL DEFAULT(0),
    Activo BIT NOT NULL DEFAULT(1),
    FechaCreacion DATETIME2 NOT NULL DEFAULT(SYSDATETIME()),
    CONSTRAINT FK_OpCanciones_CatUsuarios FOREIGN KEY (UsuarioId) REFERENCES seg.CatUsuarios (UsuarioId)
);
GO

IF OBJECT_ID('seg.LogReproducciones', 'U') IS NOT NULL
    DROP TABLE seg.LogReproducciones;
GO

CREATE TABLE seg.LogReproducciones
(
    ReproduccionId BIGINT IDENTITY(1, 1) PRIMARY KEY,
    CancionId INT NOT NULL,
    Plataforma NVARCHAR(120) NULL,
    FechaReproduccion DATETIME2 NOT NULL DEFAULT(SYSDATETIME()),
    Importe DECIMAL(18, 6) NOT NULL DEFAULT(0),
    CONSTRAINT FK_LogReproducciones_OpCanciones FOREIGN KEY (CancionId) REFERENCES seg.OpCanciones (CancionId)
);
GO

IF OBJECT_ID('seg.trOpCancionesActualizarTotales', 'TR') IS NOT NULL
    DROP TRIGGER seg.trOpCancionesActualizarTotales;
GO

CREATE TRIGGER seg.trOpCancionesActualizarTotales
ON seg.LogReproducciones
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE c
    SET
        TotalReproducciones = TotalReproducciones + i.TotalInserciones,
        MontoGanado = MontoGanado + i.TotalImporte
    FROM seg.OpCanciones c
    INNER JOIN (
        SELECT CancionId, COUNT(1) AS TotalInserciones, SUM(Importe) AS TotalImporte
        FROM inserted
        GROUP BY CancionId
    ) i ON c.CancionId = i.CancionId;
END;
GO

-- Seed data
IF NOT EXISTS (SELECT 1 FROM seg.CatRoles WHERE Nombre = 'Artista')
BEGIN
    INSERT INTO seg.CatRoles (Nombre, Descripcion, EsAdmin)
    VALUES ('Artista', 'Artista registrado en la plataforma', 0);
END;
GO

IF NOT EXISTS (SELECT 1 FROM seg.CatRoles WHERE Nombre = 'Administrador')
BEGIN
    INSERT INTO seg.CatRoles (Nombre, Descripcion, EsAdmin)
    VALUES ('Administrador', 'Usuario con acceso total a estadísticas', 1);
END;
GO

DECLARE @AdminSalt VARBINARY(256) = 0xF3C1AF1AEE3B8D2ED4BC3739DF26BB47A2A05DF01D5C44513EAF35DB2E62340A;
DECLARE @AdminHash VARBINARY(512) = 0xA48FF124662ECDB74E72F9A51AA5A4B82CF20194ABA862D364CE90BE4AB2F1346840656D7557A500640B6E579ED41F89AA0AA5F945AF5510AB852740253E116;
DECLARE @AdminRolId INT = (SELECT TOP 1 RolId FROM seg.CatRoles WHERE EsAdmin = 1);

IF NOT EXISTS (SELECT 1 FROM seg.CatUsuarios WHERE Correo = 'reyl@difarmer.com')
BEGIN
    INSERT INTO seg.CatUsuarios (RolId, Nombre, Apellidos, Correo, PasswordHash, PasswordSalt, Activo)
    VALUES (@AdminRolId, 'Rey', 'Records', 'reyl@difarmer.com', @AdminHash, @AdminSalt, 1);
END;
GO
