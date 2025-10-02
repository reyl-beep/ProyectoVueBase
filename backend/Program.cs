using System.Text;
using BackendApi.Configuration;
using BackendApi.Endpoints;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<SqlConnectionOptions>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<ISqlProcedureExecutor, SqlProcedureExecutor>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var configuredFrontendOrigins = builder.Configuration
    .GetSection("Frontend:Cors:AllowedOrigins")
    .Get<string[]>()
    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim())
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray() ?? Array.Empty<string>();

var frontendCorsOrigins = configuredFrontendOrigins.Length > 0
    ? configuredFrontendOrigins
    : new[] { "http://localhost:5173" };

var usingDefaultCorsOrigins = configuredFrontendOrigins.Length == 0;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.Issuer,
            ValidAudience = jwtSection.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Key))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (usingDefaultCorsOrigins)
{
    app.Logger.LogWarning(
        "No frontend origins configured under 'Frontend:Cors:AllowedOrigins'. Using default: {Origins}",
        string.Join(", ", frontendCorsOrigins));
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API v1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"))
    .WithName("RedirectToSwagger")
    .WithTags("General");

app.MapGet("/health", () => Results.Ok(new Resultado { Value = true, Message = "Healthy", Data = null }))
    .WithName("GetHealth")
    .WithTags("General");

app.MapAuthEndpoints();
app.MapDashboardEndpoints();

app.Run();
