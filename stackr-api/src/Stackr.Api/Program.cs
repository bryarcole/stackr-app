using Microsoft.EntityFrameworkCore;
using Stackr_Api.data;
using Stackr_Api.Endpoints;

namespace Stackr_Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"), ServiceLifetime.Singleton);

        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        var api = app.MapGroup("/api");
        api.MapGroup("/auth").MapAuthEndpoints();
        api.MapGroup("/users").MapUsersEndpoints();
        api.MapGroup("/lists").MapListsEndpoints();
        api.MapGroup("/items").MapItemsEndpoints();
        api.MapGroup("/rankings").MapRankingsEndpoints();

        app.Run();
    }
} 