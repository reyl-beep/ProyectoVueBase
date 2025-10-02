-- Sample operational data for ReyRecords platform
-- Ensure base catalog tables exist before executing

DECLARE @ArtistaRolId INT = (SELECT TOP 1 RolId FROM seg.CatRoles WHERE Nombre = 'Artista');

IF NOT EXISTS (SELECT 1 FROM seg.CatUsuarios WHERE Correo = 'artist@reyrecords.com')
BEGIN
    DECLARE @Salt VARBINARY(256) = 0x1A2B3C4D5E6F7A8B9C0D1E2F3A4B5C6D7E8F9A0B1C2D3E4F5A6B7C8D9E0F1020;
    DECLARE @Hash VARBINARY(512) = 0x94F8C61395C0BAA8C3A1F9F3A27AD6B2EE5999888BBABC5A24A0AF3A890CFA673FB730F3F73B8CD10F1A183B4E3B0D65541C96CC356C5CFEA6D5C9D28FB2D2A0;

    INSERT INTO seg.CatUsuarios (RolId, Nombre, Apellidos, Correo, PasswordHash, PasswordSalt, Activo)
    VALUES (@ArtistaRolId, 'Luna', 'García', 'artist@reyrecords.com', @Hash, @Salt, 1);
END;

DECLARE @UsuarioId INT = (SELECT UsuarioId FROM seg.CatUsuarios WHERE Correo = 'artist@reyrecords.com');

IF NOT EXISTS (SELECT 1 FROM seg.OpCanciones WHERE UsuarioId = @UsuarioId)
BEGIN
    INSERT INTO seg.OpCanciones (UsuarioId, Titulo, Descripcion, FechaPublicacion, TotalReproducciones, MontoGanado)
    VALUES
        (@UsuarioId, 'Neon Nights', 'Synthwave atmosférico', DATEADD(MONTH, -4, SYSDATETIME()), 1250000, 1875.50),
        (@UsuarioId, 'Electric Bloom', 'Pop futurista', DATEADD(MONTH, -2, SYSDATETIME()), 830000, 1210.75);
END;

INSERT INTO seg.LogReproducciones (CancionId, Plataforma, Importe)
SELECT c.CancionId, 'Spotify', 0.0011
FROM seg.OpCanciones c
WHERE c.UsuarioId = @UsuarioId
  AND NOT EXISTS (SELECT 1 FROM seg.LogReproducciones lr WHERE lr.CancionId = c.CancionId);
