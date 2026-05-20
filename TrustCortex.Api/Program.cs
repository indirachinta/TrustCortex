using TrustCortex.Application;
using TrustCortex.Application.DTOs;
using TrustCortex.Application.UseCases;
using TrustCortex.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddOpenApi();
builder.Services.AddTrustCortexApplication();
builder.Services.AddTrustCortexInfrastructure();

var app = builder.Build();

app.MapOpenApi();
app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
app.MapGet("/swagger/index.html", () => Results.Content(
    """
    <!doctype html>
    <html>
    <head>
      <title>TrustCortex API</title>
      <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
    </head>
    <body>
      <div id="swagger-ui"></div>
      <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
      <script>
        window.ui = SwaggerUIBundle({ url: '/openapi/v1.json', dom_id: '#swagger-ui' });
      </script>
    </body>
    </html>
    """,
    "text/html"));

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
    .WithName("Health");

app.MapPost("/api/ask", async (
        AskRequest request,
        AskQuestionUseCase useCase,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var response = await useCase.ExecuteAsync(request, cancellationToken);
            return Results.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    })
    .WithName("Ask");

app.Run();
