/*

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Stackr_Api.Data;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Stackr_Api.Models;


var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetSection("MongoDb:ConnectionString").Value;
var mongoDatabaseName = builder.Configuration.GetSection("MongoDb:DatabaseName").Value;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IRankingCountService, RankingCountService>();
builder.Services.AddDbContext<RankingCountContext>(options =>
options.UseInMemoryDatabase("StackrDB") );

/*builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(mongoConnectionString));
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
//Get Stacks and save them to JSON file
app.MapPost("/CreateStackJSON", async (HttpContext httpContext) =>
{

    var requestBody = await JsonSerializer.DeserializeAsync<Stack>(httpContext.Request.Body);

    if (requestBody == null || string.IsNullOrEmpty(requestBody.Name)){
        return Results.BadRequest("Title is required.");
    }

     // File path for the new JSON file
    var fileName = $"{requestBody.Name.Replace(" ", "_")}.json";
    var filePath = Path.Combine("Stacks", fileName);;

    Directory.CreateDirectory("Stacks"); 

    var fileContent = JsonSerializer.Serialize(requestBody);
    //await File.WriteAllTextAsync(filePath, fileContent);

    return Results.Ok($"Stack file created successfully: {filePath}");

});

//Calculate ranks and save to JSON file
app.MapPost("/CalculateRanksJSON", async (HttpContext httpContext,[FromServices] IRankingCountService rankingCountService) =>
{
    try
    {
        var rankings = await httpContext.Request.ReadFromJsonAsync<List<string>>();

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





app.Run();

*/