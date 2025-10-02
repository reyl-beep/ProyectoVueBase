using System;
using System.Text;
using BackendApi.Configuration;
using BackendApi.Endpoints;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

const string frontendCorsPolicy = "FrontendPolicy";

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<SqlConnectionOptions>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var usingDefaultCorsOrigins = allowedOrigins.Length == 0;
var frontendCorsOrigins = usingDefaultCorsOrigins
    ? new[] { "http://localhost:5173" }
    : allowedOrigins;

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy.WithOrigins(frontendCorsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<ISqlProcedureExecutor, SqlProcedureExecutor>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

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
        "No frontend origins configured under 'Cors:AllowedOrigins'. Using default: {Origins}",
        string.Join(", ", frontendCorsOrigins));
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API v1");
});

app.UseCors(frontendCorsPolicy);

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
