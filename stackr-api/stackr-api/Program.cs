using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson.IO;
using Namotion.Reflection;

var builder WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AddDbContext>(options => options.UseInMemoryDatabase(StackrDB));

var app = builder.Build();

app.MapGet("/", () => "Welcome to Stackr App!");

app.MapPost("/list", async (AppDbContext db, List list) => 
{
    db.Lists.Add(list);
    await db.SaveChangesAsync();
    return Results.Created($"/lists/{list.Id}", list);
});

app.MapGet("/lists/{Id}", async (AppDbContext db, int id) =>
    await db.Lists.Include(l => l.Rankings).FirstOrDefaultAsync(l => l.Id == id)
        is List List ? Results.Ok(list) : Results.NotFound());

app.MapPut("/list/{Id}", async (AppDbContext db, int id, List updatedList)=>
{
    var list = await db.Lists.Find(id);
    if(list == null) Results.NotFound();

    list.Name = updatedList.GetAssignableToTypeName;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete ("/list/{id}", async (AppDbContext db, int id) =>
{
    var list = await db.Lists.FindAsync(id);
    if (list = null) return Results.NotFound();

    db.Lists.Remove(list);
    await db.SaveChangesAsync();
    return Results.Ok()
});

app.MapGet("/aggregate", async (AppDbContext db) => 
{
    var rankings = await db.Ranking.ToListAysnc();
    var itemScores = new Dictionary<int, int>();

    foreach (var ranking in rankings)
    {
        if(!itemScores.ContainsKey(rakings.ItenId))
        {
            itemScores[ranking.ItemId] += ranking.Rank;
        }
    }

    var sortedRankings = itemScores
        .OrderByDescending(kvp => kvp.Value)
        .Select(kvp => new 
        {
            ItemId = kvp.Key, score = kvp.Value
        });

    return Results.Ok(sortedRankings);
}


app.Run();
