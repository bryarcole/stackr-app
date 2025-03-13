using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stackr_Api;
using Stackr_Api.data;

namespace Stackr.Api.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string _databaseName = "TestDb";
    private IServiceProvider _serviceProvider;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            }, ServiceLifetime.Singleton);

            _serviceProvider = services.BuildServiceProvider();
            var db = _serviceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_serviceProvider != null)
            {
                var db = _serviceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
            }
        }
        base.Dispose(disposing);
    }
} 