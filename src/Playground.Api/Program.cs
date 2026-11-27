var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();

var app = builder.Build();

Item[] items =
[
    new(1, "Container image"),
    new(2, "Helm chart"),
    new(3, "Terraform module")
];

// Liveness: is the process able to answer? Runs no dependency checks.
app.MapHealthChecks("/health/live", new() { Predicate = _ => false });
// Readiness: should this pod receive traffic? Add dependency checks here.
app.MapHealthChecks("/health/ready");

app.MapGet("/api/info", () => new Info(
    Environment.MachineName,
    app.Configuration["APP_VERSION"] ?? "local"));

app.MapGet("/api/items", () => items);

app.MapGet("/api/items/{id:int}", (int id) =>
    items.FirstOrDefault(i => i.Id == id) is { } item
        ? Results.Ok(item)
        : Results.NotFound());

app.Run();

public record Item(int Id, string Name);

public record Info(string Host, string Version);
