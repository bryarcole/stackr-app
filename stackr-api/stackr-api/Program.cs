var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/StackRankings", (List<List<string>> rankings, IBordaCountService bordaCountService) =>
{
    if (rankings == null || !rankings.Any())
    {
        return Results.BadRequest("Rankings cannot be null or empty.");
    }

    var result = bordaCountService.CalculateBordaScores(rankings);
    return Results.Ok(result);
});

app.Run();
