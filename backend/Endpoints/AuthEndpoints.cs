using System.Security.Claims;
using BackendApi.Models;
using BackendApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Autenticación");

        group.MapPost("/register", async ([FromBody] RegisterRequest request, IUsuarioService usuarioService, CancellationToken cancellationToken) =>
        {
            var resultado = await usuarioService.RegistrarAsync(request, cancellationToken);
            return Results.Json(resultado);
        })
        .WithName("RegistrarUsuario")
        .Produces<Resultado>(StatusCodes.Status200OK);

        group.MapPost("/login", async ([FromBody] LoginRequest request, IUsuarioService usuarioService, CancellationToken cancellationToken) =>
        {
            var resultado = await usuarioService.IniciarSesionAsync(request, cancellationToken);
            return Results.Json(resultado);
        })
        .WithName("IniciarSesion")
        .Produces<Resultado>(StatusCodes.Status200OK);
    }

    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard")
            .RequireAuthorization()
            .WithTags("Dashboard");

        group.MapGet("/me", async (ClaimsPrincipal principal, IUsuarioService usuarioService, CancellationToken cancellationToken) =>
        {
            if (!int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            {
                return Results.Json(Resultado.From(false, "No fue posible identificar al usuario."));
            }

            var resultado = await usuarioService.ObtenerDashboardAsync(usuarioId, cancellationToken);
            return Results.Json(resultado);
        })
        .WithName("DashboardPersonal")
        .Produces<Resultado>(StatusCodes.Status200OK);

        group.MapGet("/admin", [Authorize(Roles = "Admin")] async (IUsuarioService usuarioService, CancellationToken cancellationToken) =>
        {
            var resultado = await usuarioService.ObtenerDashboardGlobalAsync(cancellationToken);
            return Results.Json(resultado);
        })
        .WithName("DashboardAdministrativo")
        .Produces<Resultado>(StatusCodes.Status200OK);
    }
}
