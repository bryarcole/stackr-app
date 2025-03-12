using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using DnsClient.Protocol;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Namotion.Reflection;
using Stackr_Api.Models;
using System.IdentityModel.Tokens.Jwt;
using Stackr_Api.data;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("StackrDB"));

var app = builder.Build();

app.MapGet("/", () => "Welcome to Stackr App!");

app.MapPost("/list", async (AppDbContext db, RankingList list) => 
{
    db.RankingLists.Add(list);
    await db.SaveChangesAsync();
    return Results.Created($"/lists/{list.Id}", list);
});

app.MapGet("/lists/{Id}", async (AppDbContext db, int id) => 
{
    var list = await db.RankingLists.FindAsync(id);
    if(list == null) Results.NotFound();

    return Results.Ok(list);
});

app.MapPut("/list/{Id}", async (AppDbContext db, int id, RankingList updatedList)=>
{
    var list = await db.RankingLists.FindAsync(id);
    if(list == null) Results.NotFound();

    list = updatedList;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete ("/list/{id}", async (AppDbContext db, int id) =>
{
    var list = await db.RankingLists.FindAsync(id);
    if (list is null) return Results.NotFound();

    db.RankingLists.Remove(list);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/aggregate", async (AppDbContext db, int id) => 
{
    var rankings = await db.Rankings.Where(r => r.ItemId == id).ToListAsync(); // Ensure this is a collection

    var itemScores = new Dictionary<int, int>();

    foreach (var ranking in rankings) // Iterate over the rankings collection
    {
        if (!itemScores.ContainsKey(ranking.ItemId)) // Use 'ranking.ItemId'
        {
            itemScores[ranking.ItemId] = ranking.Rank; // Initialize if not present
        }
        else
        {
            itemScores[ranking.ItemId] += ranking.Rank; // Add to existing score
        }
    }

    var sortedRankings = itemScores
        .OrderByDescending(kvp => kvp.Value)
        .Select(kvp => new 
        {
            ItemId = kvp.Key, 
            score = kvp.Value
        });

    return Results.Ok(sortedRankings);
});


app.MapPost("/auth/register", async (AppDbContext db, User user) =>
{
    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok(new {message = "User registered succesfully" });

});

app.MapPost("auth/login", async (AppDbContext db, User loginUser) => 
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash))
        return Results.Unauthorized();

    var token = GenerateJwtToken(user);
    return Results.Ok(new {token});
});

string GenerateJwtToken(User user)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyHere"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[] { new Claim(ClaimTypes.Name, user.Email) };
    var token = new JwtSecurityToken("Issuer", "Audience", claims, expires: DateTime.Now.AddHours(3), signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.Run();
