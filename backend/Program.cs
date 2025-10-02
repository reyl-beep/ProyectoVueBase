var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    Message = "Bienvenido a la Minimal API en ASP.NET Core",
    Timestamp = DateTimeOffset.UtcNow
}));

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }));

app.Run();
