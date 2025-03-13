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

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add the test database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Create the database
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                using var scope = Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureDeleted();
            }
            catch (ObjectDisposedException)
            {
                // If the service provider is already disposed, we can safely ignore this
            }
        }

        base.Dispose(disposing);
    }
} 