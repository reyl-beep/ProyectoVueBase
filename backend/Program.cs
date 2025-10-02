var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Json(new
    {
        Message = "Bienvenido a la Minimal API en ASP.NET Core",
        Timestamp = DateTimeOffset.UtcNow
    }))
    .WithName("GetWelcome")
    .WithTags("General");

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
    .WithName("GetHealth")
    .WithTags("General");

app.Run();
