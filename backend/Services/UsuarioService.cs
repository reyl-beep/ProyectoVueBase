using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BackendApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace BackendApi.Services;

public interface IUsuarioService
{
    Task<Resultado<TokenResponse>> RegistrarAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<Resultado<TokenResponse>> IniciarSesionAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<Resultado<DashboardUsuario>> ObtenerDashboardAsync(int usuarioId, CancellationToken cancellationToken);
    Task<Resultado<DashboardGlobal>> ObtenerDashboardGlobalAsync(CancellationToken cancellationToken);
}

public class UsuarioService(ILogger<UsuarioService> logger, ISqlProcedureExecutor executor, IJwtTokenService jwtTokenService) : IUsuarioService
{
    private const string RolPorDefecto = "Artista";
    private readonly ILogger<UsuarioService> _logger = logger;
    private readonly ISqlProcedureExecutor _executor = executor;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<Resultado<TokenResponse>> RegistrarAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var rol = await ObtenerRolAsync(RolPorDefecto, cancellationToken);
        if (!rol.Value || rol.Data is not RolInfo rolInfo)
        {
            return new Resultado<TokenResponse>
            {
                Value = false,
                Message = rol.Message
            };
        }

        var salt = RandomNumberGenerator.GetBytes(32);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(request.Password), salt, 150000, HashAlgorithmName.SHA512, 64);

        var usuarioIdParam = new SqlParameter("@pUsuarioId", System.Data.SqlDbType.Int)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        var resultadoInsercion = await _executor.ExecuteAsync("seg.procCatUsuariosIns", command =>
        {
            command.Parameters.AddWithValue("@pRolId", rolInfo.RolId);
            command.Parameters.AddWithValue("@pNombre", request.Nombre);
            command.Parameters.AddWithValue("@pApellidos", (object?)request.Apellidos ?? DBNull.Value);
            command.Parameters.AddWithValue("@pCorreo", request.Correo.Trim().ToLowerInvariant());
            command.Parameters.Add(new SqlParameter("@pPasswordHash", System.Data.SqlDbType.VarBinary, hash.Length) { Value = hash });
            command.Parameters.Add(new SqlParameter("@pPasswordSalt", System.Data.SqlDbType.VarBinary, salt.Length) { Value = salt });
            command.Parameters.Add(usuarioIdParam);
        }, cancellationToken: cancellationToken);

        if (!resultadoInsercion.Value)
        {
            _logger.LogWarning("No se pudo registrar el usuario {Correo}: {Mensaje}", request.Correo, resultadoInsercion.Message);
            return new Resultado<TokenResponse>
            {
                Value = false,
                Message = resultadoInsercion.Message
            };
        }

        var nuevoUsuarioId = usuarioIdParam.Value is int id ? id : 0;
        var usuario = await ObtenerUsuarioPorIdAsync(nuevoUsuarioId, cancellationToken);
        if (!usuario.Value || usuario.Data is not UsuarioAutenticado usuarioAutenticado)
        {
            _logger.LogError("El usuario {Correo} fue creado pero no se pudo recuperar sus datos: {Mensaje}", request.Correo, usuario.Message);
            return new Resultado<TokenResponse>
            {
                Value = false,
                Message = usuario.Message
            };
        }

        var token = _jwtTokenService.CreateToken(usuarioAutenticado);
        return new Resultado<TokenResponse>
        {
            Value = true,
            Message = resultadoInsercion.Message,
            Data = token
        };
    }

    public async Task<Resultado<TokenResponse>> IniciarSesionAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var login = await _executor.ExecuteAsync("seg.procCatUsuariosConLogin",
            command =>
            {
                command.Parameters.AddWithValue("@pCorreo", request.Correo.Trim().ToLowerInvariant());
            },
            async reader =>
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    var usuarioId = reader.GetInt32("UsuarioId");
                    var nombre = reader.GetString("Nombre");
                    var apellidos = reader.GetNullableString("Apellidos");
                    var correo = reader.GetString("Correo");
                    var rolNombre = reader.GetString("RolNombre");
                    var esAdmin = reader.GetBoolean("EsAdmin");
                    var hash = reader.GetBytes("PasswordHash");
                    var salt = reader.GetBytes("PasswordSalt");

                    return new UsuarioCompleto(usuarioId, nombre, apellidos, correo, rolNombre, esAdmin, hash, salt);
                }

                return null;
            },
            cancellationToken);

        if (!login.Value || login.Data is not UsuarioCompleto datosUsuario)
        {
            _logger.LogWarning("Intento de inicio de sesión fallido para {Correo}: {Mensaje}", request.Correo, login.Message);
            return new Resultado<TokenResponse>
            {
                Value = false,
                Message = login.Message
            };
        }

        var hashIngresado = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(request.Password), datosUsuario.PasswordSalt, 150000, HashAlgorithmName.SHA512, datosUsuario.PasswordHash.Length);
        if (!CryptographicOperations.FixedTimeEquals(hashIngresado, datosUsuario.PasswordHash))
        {
            _logger.LogWarning("Credenciales incorrectas para {Correo}", request.Correo);
            return new Resultado<TokenResponse>
            {
                Value = false,
                Message = "Las credenciales proporcionadas no son válidas."
            };
        }

        await _executor.ExecuteAsync("seg.procCatUsuariosActUltimoAcceso",
            command =>
            {
                command.Parameters.AddWithValue("@pUsuarioId", datosUsuario.UsuarioId);
                command.Parameters.AddWithValue("@pFecha", DateTime.UtcNow);
            },
            cancellationToken: cancellationToken);

        var usuario = new UsuarioAutenticado(datosUsuario.UsuarioId, datosUsuario.Nombre, datosUsuario.Apellidos, datosUsuario.Correo, datosUsuario.RolNombre, datosUsuario.EsAdmin);
        var token = _jwtTokenService.CreateToken(usuario);

        return new Resultado<TokenResponse>
        {
            Value = true,
            Message = "Inicio de sesión exitoso.",
            Data = token
        };
    }

    public async Task<Resultado<DashboardUsuario>> ObtenerDashboardAsync(int usuarioId, CancellationToken cancellationToken)
    {
        var usuario = await ObtenerUsuarioPorIdAsync(usuarioId, cancellationToken);
        if (!usuario.Value || usuario.Data is not UsuarioAutenticado usuarioAutenticado)
        {
            _logger.LogWarning("No se encontró el usuario {UsuarioId} para el dashboard: {Mensaje}", usuarioId, usuario.Message);
            return new Resultado<DashboardUsuario>
            {
                Value = false,
                Message = usuario.Message
            };
        }

        var canciones = await _executor.ExecuteAsync("seg.procOpCancionesConPorUsuario",
            command =>
            {
                command.Parameters.AddWithValue("@pUsuarioId", usuarioId);
            },
            async reader =>
            {
                var lista = new List<CancionResumen>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    lista.Add(new CancionResumen(
                        reader.GetInt32("CancionId"),
                        reader.GetString("Titulo"),
                        reader.GetNullableString("Descripcion"),
                        reader.GetInt64("TotalReproducciones"),
                        reader.GetDecimal("MontoGanado"),
                        reader.GetDateTime("FechaPublicacion"),
                        reader.GetBoolean("Activo")));
                }

                return lista.AsReadOnly();
            }, cancellationToken);

        if (!canciones.Value || canciones.Data is not IReadOnlyCollection<CancionResumen> cancionesUsuario)
        {
            _logger.LogWarning("No se pudieron recuperar las canciones del usuario {UsuarioId}: {Mensaje}", usuarioId, canciones.Message);
            return new Resultado<DashboardUsuario>
            {
                Value = false,
                Message = canciones.Message
            };
        }

        return new Resultado<DashboardUsuario>
        {
            Value = true,
            Message = "Información de usuario recuperada correctamente.",
            Data = new DashboardUsuario(usuarioAutenticado, cancionesUsuario)
        };
    }

    public async Task<Resultado<DashboardGlobal>> ObtenerDashboardGlobalAsync(CancellationToken cancellationToken)
    {
        var resultado = await _executor.ExecuteAsync("seg.procOpCancionesConResumen", handleReader: async reader =>
        {
            var agrupado = new Dictionary<int, List<CancionResumen>>();
            var usuarios = new Dictionary<int, UsuarioAutenticado>();

            while (await reader.ReadAsync(cancellationToken))
            {
                if (reader.IsDBNull(reader.GetOrdinal("UsuarioId")))
                {
                    continue;
                }

                var usuarioId = reader.GetInt32("UsuarioId");
                if (!usuarios.ContainsKey(usuarioId))
                {
                    var usuario = new UsuarioAutenticado(
                        usuarioId,
                        reader.GetString("Nombre"),
                        reader.GetNullableString("Apellidos"),
                        reader.GetString("Correo"),
                        reader.GetString("RolNombre"),
                        string.Equals(reader.GetString("RolNombre"), "Administrador", StringComparison.OrdinalIgnoreCase));
                    usuarios.Add(usuarioId, usuario);
                }

                if (!agrupado.TryGetValue(usuarioId, out var lista))
                {
                    lista = new List<CancionResumen>();
                    agrupado[usuarioId] = lista;
                }

                if (!reader.IsDBNull(reader.GetOrdinal("CancionId")))
                {
                    lista.Add(new CancionResumen(
                        reader.GetInt32("CancionId"),
                        reader.GetString("Titulo"),
                        null,
                        reader.GetInt64("TotalReproducciones"),
                        reader.GetDecimal("MontoGanado"),
                        reader.GetDateTime("FechaPublicacion"),
                        true));
                }
            }

            var dashboards = usuarios.Select(kvp => new DashboardUsuario(
                kvp.Value,
                agrupado.TryGetValue(kvp.Key, out var cancionesUsuario)
                    ? cancionesUsuario.AsReadOnly()
                    : Array.Empty<CancionResumen>())).ToList();

            return dashboards.AsReadOnly();
        }, cancellationToken: cancellationToken);

        if (!resultado.Value || resultado.Data is not IReadOnlyCollection<DashboardUsuario> usuarios)
        {
            _logger.LogWarning("No se pudo construir el dashboard global: {Mensaje}", resultado.Message);
            return new Resultado<DashboardGlobal>
            {
                Value = false,
                Message = resultado.Message
            };
        }

        return new Resultado<DashboardGlobal>
        {
            Value = true,
            Message = resultado.Message,
            Data = new DashboardGlobal(usuarios)
        };
    }

    private async Task<Resultado> ObtenerRolAsync(string nombre, CancellationToken cancellationToken)
    {
        return await _executor.ExecuteAsync("seg.procCatRolesConPorNombre",
            command => command.Parameters.AddWithValue("@pNombre", nombre),
            async reader =>
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    return new RolInfo(reader.GetInt32("RolId"), reader.GetString("Nombre"));
                }

                return null;
            }, cancellationToken);
    }

    private async Task<Resultado> ObtenerUsuarioPorIdAsync(int usuarioId, CancellationToken cancellationToken)
    {
        return await _executor.ExecuteAsync("seg.procCatUsuariosConPorId",
            command => command.Parameters.AddWithValue("@pUsuarioId", usuarioId),
            async reader =>
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    return new UsuarioAutenticado(
                        reader.GetInt32("UsuarioId"),
                        reader.GetString("Nombre"),
                        reader.GetNullableString("Apellidos"),
                        reader.GetString("Correo"),
                        reader.GetString("RolNombre"),
                        reader.GetBoolean("EsAdmin"));
                }

                return null;
            }, cancellationToken);
    }

    private sealed record RolInfo(int RolId, string Nombre);

    private sealed record UsuarioCompleto(
        int UsuarioId,
        string Nombre,
        string? Apellidos,
        string Correo,
        string RolNombre,
        bool EsAdmin,
        byte[] PasswordHash,
        byte[] PasswordSalt);
}
