namespace BackendApi.Models;

public class Resultado
{
    public bool Value { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public static Resultado From(bool value, string message, object? data = null) => new()
    {
        Value = value,
        Message = message,
        Data = data
    };
}

public class Resultado<T> : Resultado
{
    public new T? Data
    {
        get => (T?)base.Data;
        set => base.Data = value;
    }
}
