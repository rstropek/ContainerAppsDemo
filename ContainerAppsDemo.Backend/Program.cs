using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseHttpsRedirection();
app.MapGet("/health", () => Results.Ok(new { IsHealthy = true, }));

var formulaRegex = new Regex("^[0-9]+([+-][0-9]+)*$", RegexOptions.Compiled);
app.MapGet("/calculate", (string formula) =>
{
    if (!formulaRegex.IsMatch(formula))
    {
        return Results.BadRequest(new ProblemDetails
        {
            Type = "https://example.com/invalid-formula",
            Title = "Invalid formula",
            Status = StatusCodes.Status400BadRequest,
            Detail = "The formula is invalid.",
        });
    }

    ReadOnlySpan<char> formulaSpan = formula.AsSpan();
    var result = 0;
    var op = 1;
    while (true)
    {
        var indexOp = formulaSpan.IndexOfAny('+', '-');
        if (indexOp == -1) { indexOp = formulaSpan.Length; }
        result += int.Parse(formulaSpan[..indexOp]) * op;
        
        if (indexOp == formulaSpan.Length) { break; }
        op = formulaSpan[indexOp] == '+' ? 1 : -1;
        formulaSpan = formulaSpan[(indexOp + 1)..];
    }

    return Results.Ok(new { Result = result });
});

app.Run();
