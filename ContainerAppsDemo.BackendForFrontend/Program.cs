using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddHttpClient("backend", (client) =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendBaseUrl"]);
});
var app = builder.Build();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHttpsRedirection();
app.MapGet("/health", async (IHttpClientFactory factory) => {
    var client = factory.CreateClient("backend");
    var response = await client.GetAsync($"/health");
    response.EnsureSuccessStatusCode();
    return Results.Ok(new { IsHealthy = true });
});

app.MapGet("/calculate", async (int? a, int? b, IHttpClientFactory factory) =>
{
    if (a is null || b is null)
    {
        return Results.BadRequest(new ProblemDetails
        {
            Type = "https://example.com/missing-arguments",
            Title = "Missing arguments",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Arguments are missing.",
        });
    }

    var client = factory.CreateClient("backend");
    var response = await client.GetAsync($"/calculate?formula={a}%2b{b}");
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<BackendResult>();

    return Results.Ok(result);
});

app.Run();

record BackendResult(int Result);