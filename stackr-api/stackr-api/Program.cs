using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IRankingCountService, RankingCountService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/PostStackRanks", async (HttpContext httpContext,[FromServices] IRankingCountService rankingCountService) =>
{
    try{
        var rankings = await httpContext.Request.ReadFromJsonAsync<List<List<string>>>();

        if(rankings.Count < 1 || !rankings.Any()){
            return Results.BadRequest("Must have items to Rank");
        }

        var result = rankingCountService.CalculateRankScores(rankings);
        return Results.Ok(result);
    }
    catch(System.Text.Json.JsonException){
        return Results.BadRequest("Invalid Json format.");
    };
});

app.MapGet("/GetRankResults", ([FromServices] IRankingCountService rankingCountService) =>{
    var testRankings = new List<List<string>>{
        new List<string> {"Tom Brady", "John Elway", "Troy Aikman"},
        new List<string> {"Tom Brady", "Brett Farve", "Aaron Rodgers"},
        new List<string> {"John Elway", "Troy Aikman", "Aaron Rodgers", "Tony Romo"},
        new List<string> {"Troy Aikman", "Tom Brady", "Tony Romo", "Aaron Rodgers"}
    };

    var result = rankingCountService.CalculateRankScores(testRankings);
    return Results.Ok(result);
});


app.Run();
