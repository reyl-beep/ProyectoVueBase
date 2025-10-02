using Microsoft.Data.SqlClient;

namespace BackendApi.Services;

internal static class SqlDataReaderExtensions
{
    public static int GetInt32(this SqlDataReader reader, string column) => reader.GetInt32(reader.GetOrdinal(column));
    public static long GetInt64(this SqlDataReader reader, string column) => reader.GetInt64(reader.GetOrdinal(column));
    public static string GetString(this SqlDataReader reader, string column) => reader.GetString(reader.GetOrdinal(column));
    public static bool GetBoolean(this SqlDataReader reader, string column) => reader.GetBoolean(reader.GetOrdinal(column));
    public static decimal GetDecimal(this SqlDataReader reader, string column) => reader.GetDecimal(reader.GetOrdinal(column));
    public static DateTime GetDateTime(this SqlDataReader reader, string column) => reader.GetDateTime(reader.GetOrdinal(column));
    public static byte[] GetBytes(this SqlDataReader reader, string column)
    {
        var length = reader.GetBytes(reader.GetOrdinal(column), 0, null, 0, 0);
        var buffer = new byte[length];
        reader.GetBytes(reader.GetOrdinal(column), 0, buffer, 0, (int)length);
        return buffer;
    }
    public static string? GetNullableString(this SqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
    public static DateTime? GetNullableDateTime(this SqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }
}
