using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using Stackr_Api.data;
using Stackr_Api.Models;
using Stackr_Api;
using Xunit;
using Xunit.Abstractions;

namespace Stackr.Api.Tests;

public class RankingsEndpointsTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly AppDbContext _dbContext;
    private bool _disposed;

    public RankingsEndpointsTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
        _dbContext = _factory.Services.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task GetAggregateRankings_ReturnsEmptyList_WhenNoRankingsExist()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Act
        var response = await _client.GetAsync("/api/rankings/aggregate");
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");
        var rankings = JsonSerializer.Deserialize<JsonElement>(content);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Empty(rankings.EnumerateArray());
    }

    [Fact]
    public async Task GetAggregateRankings_ReturnsCorrectAggregation()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var item = new Item
        {
            Name = "Test Item",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync();

        var rankingList = new RankingList
        {
            Name = "Test List",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.RankingLists.Add(rankingList);
        await _dbContext.SaveChangesAsync();

        var ranking = new Ranking
        {
            ItemId = item.Id,
            Item = item,
            RankingListId = rankingList.Id,
            RankingList = rankingList,
            Rank = 1,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Rankings.Add(ranking);
        await _dbContext.SaveChangesAsync();

        // Verify the data is in the database
        var dbRankings = await _dbContext.Rankings
            .Include(r => r.Item)
            .Include(r => r.RankingList)
            .ToListAsync();
        _output.WriteLine($"Rankings in database: {dbRankings.Count}");
        foreach (var r in dbRankings)
        {
            _output.WriteLine($"Ranking: ItemId={r.ItemId}, Item.Name={r.Item.Name}, Rank={r.Rank}");
        }

        // Act
        var response = await _client.GetAsync("/api/rankings/aggregate");
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");
        var rankings = JsonSerializer.Deserialize<JsonElement>(content);
        var rankingsList = rankings.EnumerateArray().ToList();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Single(rankingsList);
        var firstRanking = rankingsList[0];
        Assert.Equal(1, firstRanking.GetProperty("Score").GetInt32());
        Assert.Equal("Test Item", firstRanking.GetProperty("Name").GetString());
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Database.EnsureDeletedAsync().Wait();
                _disposed = true;
            }
        }
    }
} 