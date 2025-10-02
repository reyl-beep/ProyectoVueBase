using System.Data;
using BackendApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackendApi.Services;

public interface ISqlProcedureExecutor
{
    Task<Resultado> ExecuteAsync(string procedureName, Action<SqlCommand>? configureCommand = null, Func<SqlDataReader, Task<object?>>? handleReader = null, CancellationToken cancellationToken = default);
}

public class SqlProcedureExecutor(ILogger<SqlProcedureExecutor> logger, IOptions<SqlConnectionOptions> connectionOptions) : ISqlProcedureExecutor
{
    private readonly ILogger<SqlProcedureExecutor> _logger = logger;
    private readonly SqlConnectionOptions _options = connectionOptions.Value;

    public async Task<Resultado> ExecuteAsync(string procedureName, Action<SqlCommand>? configureCommand = null, Func<SqlDataReader, Task<object?>>? handleReader = null, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_options.ConnectionString);
        await using var command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
        };

        var resultado = new SqlParameter("@pResultado", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        var mensaje = new SqlParameter("@pMsg", SqlDbType.VarChar, 500)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(resultado);
        command.Parameters.Add(mensaje);

        configureCommand?.Invoke(command);

        await connection.OpenAsync(cancellationToken);

        object? data = null;
        try
        {
            if (handleReader is null)
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            else
            {
                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                data = await handleReader(reader);
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error al ejecutar el procedimiento almacenado {Procedure}", procedureName);
            return Resultado.From(false, ex.Message);
        }

        var value = resultado.Value is bool b && b;
        var message = mensaje.Value?.ToString() ?? string.Empty;
        return Resultado.From(value, message, data);
    }
}

public class SqlConnectionOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
