namespace BackendApi.Models;

public record UsuarioAutenticado
(
    int UsuarioId,
    string Nombre,
    string? Apellidos,
    string Correo,
    string Rol,
    bool EsAdmin
);

public record CancionResumen
(
    int CancionId,
    string Titulo,
    string? Descripcion,
    long TotalReproducciones,
    decimal MontoGanado,
    DateTime FechaPublicacion,
    bool Activo
);

public record DashboardUsuario
(
    UsuarioAutenticado Usuario,
    IReadOnlyCollection<CancionResumen> Canciones
);

public record DashboardGlobal
(
    IReadOnlyCollection<DashboardUsuario> Usuarios
);

public record LoginRequest(string Correo, string Password);

public record RegisterRequest
(
    string Nombre,
    string? Apellidos,
    string Correo,
    string Password
);

public record TokenResponse(string Token, DateTime Expiration);
