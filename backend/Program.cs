var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API v1");
});

app.MapGet("/", () => Results.Redirect("/swagger"))
    .WithName("RedirectToSwagger")
    .WithTags("General");

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
    .WithName("GetHealth")
    .WithTags("General");

app.Run();
