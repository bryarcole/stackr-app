using Microsoft.EntityFrameworkCore;
using Stackr_Api.data;
using Stackr_Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("StackrDB"));

var app = builder.Build();

app.MapGet("/", () => "Welcome to Stackr App!");

// Map endpoint groups
app.MapGroup("/items").MapItemsEndpoints();
app.MapGroup("/users").MapUsersEndpoints();
app.MapGroup("/lists").MapListsEndpoints();
app.MapGroup("/rankings").MapRankingsEndpoints();
app.MapGroup("/auth").MapAuthEndpoints();

app.Run(); 