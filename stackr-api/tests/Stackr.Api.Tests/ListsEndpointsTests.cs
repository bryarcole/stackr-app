using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Stackr_Api.data;
using Stackr_Api.Models;
using Stackr_Api;
using Xunit;

namespace Stackr.Api.Tests;

public class ListsEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly AppDbContext _dbContext;

    public ListsEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        
        var scope = _factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task CreateList_ReturnsCreatedList()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var newList = new RankingList
        {
            Name = "Test List",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/lists", newList);
        var createdList = await response.Content.ReadFromJsonAsync<RankingList>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(createdList);
        Assert.Equal(newList.Name, createdList.Name);
        Assert.Equal(newList.Description, createdList.Description);
    }

    [Fact]
    public async Task GetList_ReturnsNotFound_WhenListDoesNotExist()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Act
        var response = await _client.GetAsync("/api/lists/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateList_ReturnsNotFound_WhenListDoesNotExist()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var updatedList = new RankingList
        {
            Id = 999,
            Name = "Updated List",
            Description = "Updated Description",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/lists/999", updatedList);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteList_ReturnsNotFound_WhenListDoesNotExist()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Act
        var response = await _client.DeleteAsync("/api/lists/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
} 