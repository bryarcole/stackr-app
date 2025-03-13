using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Stackr_Api.data;
using Stackr_Api.Models;
using Stackr_Api;
using Xunit;

namespace Stackr.Api.Tests;

public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly AppDbContext _dbContext;

    public AuthEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        
        var scope = _factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Register_ReturnsSuccess_WhenUserIsValid()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var newUser = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", newUser);
        var result = await response.Content.ReadFromJsonAsync<object>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Create a user first
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var loginUser = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginUser);
        var result = await response.Content.ReadFromJsonAsync<object>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var loginUser = new User
        {
            Username = "nonexistent",
            Email = "nonexistent@example.com",
            PasswordHash = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginUser);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 