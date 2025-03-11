using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AddDbContext>(options => options.UseInMemoryDatabase(StackrDB));

var app = builder.Build();

app.MapGet("/", () => "Welcome to Stackr App!");

app.MapPost("/list", async (addDbContext db, List list) => 
{
    db.Lists.Add(list);
    await db.SaveChangesAsync();
    return Results.Created($"/lists/{list.Id}", list);
});

app.MapGet("/lists/{Id}", async (addDbContext db, int id) =>
    await db.Lists.Include(l => l.Rankings).FirstOrDefaultAsync(l => l.Id == id)
        is List List ? Results.Ok(list) : Results.NotFound());


app.Run();
