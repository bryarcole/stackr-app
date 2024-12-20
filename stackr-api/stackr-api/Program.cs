using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Stackr_Api.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Generic;


var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetSection("MongoDb:ConnectionString").Value;
var mongoDatabaseName = builder.Configuration.GetSection("MongoDb:DatabaseName").Value;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IRankingCountService, RankingCountService>();
builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});
  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "Stackr-API";
    config.Title = "Stackr-API v1";
    config.Version = "v1";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "Stackr-API";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/CreateStack", async (HttpContext httpContext) =>{

    var requestBody = await JsonSerializer.DeserializeAsync<CreateStackRequest>(httpContext.Request.Body);

    if (requestBody == null || string.IsNullOrEmpty(requestBody.Title)){
        return Results.BadRequest("Title is required.");
    }

     // File path for the new JSON file
    var fileName = $"{requestBody.Title.Replace(" ", "_")}.json";
    var filePath = Path.Combine("Stacks", fileName);;

    Directory.CreateDirectory("Stacks"); 

    var fileContent = JsonSerializer.Serialize(requestBody);
    //await File.WriteAllTextAsync(filePath, fileContent);

    return Results.Ok($"Stack file created successfully: {filePath}");

});


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
