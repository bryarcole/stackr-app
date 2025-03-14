using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Net.Http.Json;
using Stackr_Api.data;
using Stackr_Api.Models;
using Stackr_Api;
using Stackr_Api.Endpoints;
using Xunit;
using Xunit.Abstractions;

namespace Stackr.Api.Tests;
/*
public class RankingsEndpointsTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
}

    public RankingsEndpointsTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
        _scope = _factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
/*
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
        var rankings = JsonSerializer.Deserialize<List<AggregateRanking>>(content);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Empty(rankings);
    }

    [Fact]
    public async Task GetAggregateRankings_ReturnsCorrectAggregation()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Create test data
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

        // Verify data was saved
        var dbRankings = await _dbContext.Rankings
            .Include(r => r.Item)
            .ToListAsync();
        _output.WriteLine($"Rankings in database: {dbRankings.Count}");

        // Act
        var response = await _client.GetAsync("/api/rankings/aggregate");
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");
        var rankings = JsonSerializer.Deserialize<List<AggregateRanking>>(content);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Single(rankings);
        var firstRanking = rankings[0];
        Assert.Equal(1, firstRanking.Score);
        Assert.Equal("Test Item", firstRanking.Name);
    }

    [Fact]
    public async Task CreateRankingList_ReturnsCreatedList()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var newList = new RankingList
        {
            Name = "New Ranking List",
            Description = "A test ranking list"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rankings/lists", newList);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");
        var createdList = await response.Content.ReadFromJsonAsync<RankingList>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(createdList);
        Assert.Equal(newList.Name, createdList?.Name);
        Assert.Equal(newList.Description, createdList?.Description);
        Assert.NotEqual(0, createdList?.Id);
    }

    [Fact]
    public async Task CreateRankingList_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var newList = new RankingList
        {
            Name = "",
            Description = "A test ranking list"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rankings/lists", newList);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddItemsToList_ReturnsSuccess()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Create a list
        var list = new RankingList
        {
            Name = "Test List",
            Description = "Test Description"
        };
        _dbContext.RankingLists.Add(list);
        await _dbContext.SaveChangesAsync();

        // Create test items
        var items = new[]
        {
            new Item { Name = "Item 1", Description = "Description 1" },
            new Item { Name = "Item 2", Description = "Description 2" }
        };
        _dbContext.Items.AddRange(items);
        await _dbContext.SaveChangesAsync();

        // Create rankings
        var rankings = new[]
        {
            new Ranking { ItemId = items[0].Id, Rank = 1 },
            new Ranking { ItemId = items[1].Id, Rank = 2 }
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/rankings/lists/{list.Id}/items", rankings);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        // Verify rankings were saved
        var savedRankings = await _dbContext.Rankings
            .Where(r => r.RankingListId == list.Id)
            .ToListAsync();
        Assert.Equal(2, savedRankings.Count);
    }

    [Fact]
    public async Task AddItemsToList_ReturnsNotFound_WhenListDoesNotExist()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var rankings = new[]
        {
            new Ranking { ItemId = 1, Rank = 1 }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rankings/lists/999/items", rankings);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetListRankings_ReturnsCorrectRankings()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Create a list
        var list = new RankingList
        {
            Name = "Test List",
            Description = "Test Description"
        };
        _dbContext.RankingLists.Add(list);
        await _dbContext.SaveChangesAsync();

        // Create test items
        var items = new[]
        {
            new Item { Name = "Item 1", Description = "Description 1" },
            new Item { Name = "Item 2", Description = "Description 2" }
        };
        _dbContext.Items.AddRange(items);
        await _dbContext.SaveChangesAsync();

        // Create rankings
        var rankings = new[]
        {
            new Ranking { ItemId = items[0].Id, RankingListId = list.Id, Rank = 1 },
            new Ranking { ItemId = items[1].Id, RankingListId = list.Id, Rank = 2 }
        };
        _dbContext.Rankings.AddRange(rankings);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/rankings/lists/{list.Id}");
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");
        var resultRankings = await response.Content.ReadFromJsonAsync<List<Ranking>>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(resultRankings);
        Assert.Equal(2, resultRankings.Count);
        Assert.Equal(1, resultRankings[0].Rank);
        Assert.Equal(2, resultRankings[1].Rank);
    }

    [Fact]
    public async Task GetListRankings_ReturnsNotFound_WhenListDoesNotExist()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Act
        var response = await _client.GetAsync("/api/rankings/lists/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SubmitRankedList_CreatesNewList_WhenTitleDoesNotExist()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var submission = new RankedListSubmission
        {
            Title = "My Favorite Movies",
            Description = "A list of my favorite movies",
            Items = new List<RankedItem>
            {
                new() { Name = "The Godfather", Description = "1972 crime film", Rank = 1 },
                new() { Name = "The Shawshank Redemption", Description = "1994 drama", Rank = 2 },
                new() { Name = "Pulp Fiction", Description = "1994 crime film", Rank = 3 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rankings/submit", submission);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");
        var createdList = await response.Content.ReadFromJsonAsync<RankingList>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(createdList);
        Assert.Equal(submission.Title, createdList.Name);
        Assert.Equal(submission.Description, createdList.Description);

        // Verify items and rankings were created
        var rankings = await _dbContext.Rankings
            .Include(r => r.Item)
            .Where(r => r.RankingListId == createdList.Id)
            .OrderBy(r => r.Rank)
            .ToListAsync();

        Assert.Equal(3, rankings.Count);
        Assert.Equal("The Godfather", rankings[0].Item.Name);
        Assert.Equal(1, rankings[0].Rank);
        Assert.Equal("The Shawshank Redemption", rankings[1].Item.Name);
        Assert.Equal(2, rankings[1].Rank);
        Assert.Equal("Pulp Fiction", rankings[2].Item.Name);
        Assert.Equal(3, rankings[2].Rank);
    }
/*
    [Fact]
    public async Task SubmitRankedList_AddsToExistingList_WhenTitleExists()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Create initial list
        var initialList = new RankingList
        {
            Name = "My Favorite Movies",
            Description = "Initial description",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.RankingLists.Add(initialList);
        await _dbContext.SaveChangesAsync();

        var submission = new RankedListSubmission
        {
            Title = "My Favorite Movies",
            Description = "Updated description",
            Items = new List<RankedItem>
            {
                new() { Name = "The Godfather", Description = "1972 crime film", Rank = 1 },
                new() { Name = "The Shawshank Redemption", Description = "1994 drama", Rank = 2 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rankings/submit", submission);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");
        var updatedList = await response.Content.ReadFromJsonAsync<RankingList>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(updatedList);
        Assert.Equal(initialList.Id, updatedList.Id);
        Assert.Equal(submission.Title, updatedList.Name);
        Assert.Equal(submission.Description, updatedList.Description);

        // Verify items and rankings were added
        var rankings = await _dbContext.Rankings
            .Include(r => r.Item)
            .Where(r => r.RankingListId == updatedList.Id)
            .OrderBy(r => r.Rank)
            .ToListAsync();

        Assert.Equal(2, rankings.Count);
        Assert.Equal("The Godfather", rankings[0].Item.Name);
        Assert.Equal(1, rankings[0].Rank);
        Assert.Equal("The Shawshank Redemption", rankings[1].Item.Name);
        Assert.Equal(2, rankings[1].Rank);
    }

    [Fact]
    public async Task SubmitRankedList_ReturnsBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var submission = new RankedListSubmission
        {
            Title = "",
            Description = "A list of my favorite movies",
            Items = new List<RankedItem>
            {
                new() { Name = "The Godfather", Description = "1972 crime film", Rank = 1 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rankings/submit", submission);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SubmitRankedList_ReturnsBadRequest_WhenNoItemsProvided()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        var submission = new RankedListSubmission
        {
            Title = "My Favorite Movies",
            Description = "A list of my favorite movies",
            Items = new List<RankedItem>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rankings/submit", submission);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

    [Fact]
    public async Task RegisterUser_ReturnsSuccess_WhenValidDataProvided()
    {
        // Arrange
        // Ensure the database is in a clean state before the test
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Create a new user object with valid data
        var newUser = new User
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "SecurePassword123"
        };

        // Act
        // Send a POST request to the registration endpoint with the new user data
        var response = await _client.PostAsJsonAsync("/api/auth/register", newUser);
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response content: {content}");

        // Assert
        // Verify that the response indicates success
        Assert.True(response.IsSuccessStatusCode);
        // Optionally, verify that the user was added to the database
        var createdUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
        Assert.NotNull(createdUser);
        Assert.Equal(newUser.Username, createdUser?.Username);
        Assert.NotEqual(newUser.Password, createdUser?.PasswordHash); // Ensure password is hashed
    }

    [Fact]
    public async Task RegisterUser_ReturnsBadRequest_WhenPasswordIsMissing()
    {
        // Arrange
        // Ensure the database is in a clean state before the test
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        // Create a new user object with missing password
        var newUser = new User
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "" // Missing password
        };

        // Act
        // Send a POST request to the registration endpoint with the new user data
        var response = await _client.PostAsJsonAsync("/api/auth/register", newUser);

        // Assert
        // Verify that the response indicates a bad request due to missing password
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeletedAsync().Wait();
        _scope.Dispose();
    }
} 
*/