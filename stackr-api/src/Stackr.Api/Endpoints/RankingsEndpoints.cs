using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;
using Microsoft.OpenApi.Models;

namespace Stackr_Api.Endpoints;

public static class RankingsEndpoints
{
    public static RouteGroupBuilder MapRankingsEndpoints(this RouteGroupBuilder group)
    {
        // Endpoint to submit a complete ranked list
        // This endpoint accepts a RankedListSubmission object which includes a title, description, and a list of ranked items.
        // If a list with the same title already exists, it updates the description and adds the new rankings to the existing list.
        // If no such list exists, it creates a new list and adds the rankings.
        group.MapPost("/submit", async (AppDbContext db, RankedListSubmission submission) =>
        {
            // Validate the submission
            if (string.IsNullOrEmpty(submission.Title))
                return Results.BadRequest("Title is required");

            if (submission.Items.Count == 0)
                return Results.BadRequest("At least one item is required");

            // Check if a list with this title already exists
            var existingList = await db.RankingLists
                .FirstOrDefaultAsync(l => l.Name.ToLower() == submission.Title.ToLower());

            RankingList list;
            if (existingList == null)
            {
                // Create a new list if it doesn't exist
                list = new RankingList
                {
                    Name = submission.Title,
                    Description = submission.Description,
                    CreatedAt = DateTime.UtcNow
                };
                db.RankingLists.Add(list);
                await db.SaveChangesAsync();
            }
            else
            {
                // Update the description of the existing list
                list = existingList;
                list.Description = submission.Description;
                await db.SaveChangesAsync();
            }

            // Iterate over each item in the submission to create or update items and rankings
            foreach (var item in submission.Items)
            {
                // Check if the item already exists in the database
                var existingItem = await db.Items
                    .FirstOrDefaultAsync(i => i.Name.ToLower() == item.Name.ToLower());

                Item dbItem;
                if (existingItem == null)
                {
                    // Create a new item if it doesn't exist
                    dbItem = new Item
                    {
                        Name = item.Name,
                        Description = item.Description,
                        CreatedAt = DateTime.UtcNow
                    };
                    db.Items.Add(dbItem);
                    await db.SaveChangesAsync();
                }
                else
                {
                    // Use the existing item
                    dbItem = existingItem;
                }

                // Create a new ranking for the item
                var ranking = new Ranking
                {
                    ItemId = dbItem.Id,
                    RankingListId = list.Id,
                    Rank = item.Rank,
                    CreatedAt = DateTime.UtcNow
                };
                db.Rankings.Add(ranking);
            }

            // Save all changes to the database
            await db.SaveChangesAsync();
            return Results.Created($"/api/rankings/lists/{list.Id}", list);
        })
        .WithName("SubmitRankedList")
        .WithTags("Ranking Lists")
        .WithSummary("Submit a complete ranked list")
        .WithDescription("Submits a complete ranked list. If a list with the same title exists, adds the rankings to that list. Otherwise, creates a new list.");

        // Endpoint to create a new ranking list
        // This endpoint accepts a RankingList object and creates a new list in the database.
        // It requires a non-empty name for the list.
        group.MapPost("/lists", async (AppDbContext db, RankingList list) =>
        {
            // Validate the list
            if (string.IsNullOrEmpty(list.Name))
                return Results.BadRequest("List name is required");

            // Set the creation date and add the list to the database
            list.CreatedAt = DateTime.UtcNow;
            db.RankingLists.Add(list);
            await db.SaveChangesAsync();

            // Return the created list with its ID
            return Results.Created($"/api/rankings/lists/{list.Id}", list);
        })
        .WithName("CreateRankingList")
        .WithTags("Ranking Lists")
        .WithSummary("Create a new ranking list")
        .WithDescription("Creates a new ranking list with the provided name and description. The list can then be used to rank items.");

        // Endpoint to add items to an existing ranking list
        // This endpoint accepts a list of Ranking objects and adds them to the specified ranking list.
        // It checks that the list exists and that each item is valid and exists in the database.
        group.MapPost("/lists/{listId}/items", async (AppDbContext db, int listId, List<Ranking> rankings) =>
        {
            // Validate the list exists
            var list = await db.RankingLists.FindAsync(listId);
            if (list == null)
                return Results.NotFound("List not found");

            // Validate each ranking
            foreach (var ranking in rankings)
            {
                if (ranking.ItemId <= 0 || ranking.Rank <= 0)
                    return Results.BadRequest("Invalid ranking data");

                // Verify item exists
                var item = await db.Items.FindAsync(ranking.ItemId);
                if (item == null)
                    return Results.NotFound($"Item with ID {ranking.ItemId} not found");

                // Set the ranking list ID and creation date
                ranking.RankingListId = listId;
                ranking.CreatedAt = DateTime.UtcNow;
                db.Rankings.Add(ranking);
            }

            // Save all changes to the database
            await db.SaveChangesAsync();
            return Results.Ok("Rankings added successfully");
        })
        .WithName("AddItemsToList")
        .WithTags("Ranking Lists")
        .WithSummary("Add items to a ranking list")
        .WithDescription("Adds one or more items to a ranking list with their respective ranks. Each item must exist in the system.");

        // Endpoint to get aggregate rankings across all lists
        // This endpoint retrieves all rankings, groups them by item, and calculates the average rank.
        // It returns a list of AggregateRanking objects with statistics for each item.
        group.MapGet("/aggregate", async (AppDbContext db) => 
        {
            // Get all rankings with their related items and lists
            var rankings = await db.Rankings
                .Include(r => r.Item)
                .Include(r => r.RankingList)
                .ToListAsync();

            // Group by item and calculate weighted average rank
            var aggregatedRankings = rankings
                .GroupBy(r => new { r.ItemId, r.Item.Name })
                .Select(g => new AggregateRanking
                {
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    Score = g.Average(r => r.Rank),
                    ListCount = g.Select(r => r.RankingListId).Distinct().Count(),
                    TotalRankings = g.Count()
                })
                .OrderByDescending(r => r.Score)
                .ToList();

            // Return the aggregated rankings
            return Results.Ok(aggregatedRankings);
        })
        .WithName("GetAggregateRankings")
        .WithTags("Aggregate Rankings")
        .WithSummary("Get aggregate rankings")
        .WithDescription("Retrieves aggregated rankings across all lists, including weighted average scores and statistics.");

        // Endpoint to get rankings for a specific list
        // This endpoint retrieves all rankings for a specified list, ordered by rank.
        // It checks that the list exists and returns a list of Ranking objects.
        group.MapGet("/lists/{listId}", async (AppDbContext db, int listId) => 
        {
            // Check if the list exists
            var list = await db.RankingLists.FindAsync(listId);
            if (list == null)
                return Results.NotFound("List not found");

            // Retrieve and order the rankings by rank
            var rankings = await db.Rankings
                .Include(r => r.Item)
                .Where(r => r.RankingListId == listId)
                .OrderBy(r => r.Rank)
                .ToListAsync();

            // Return the rankings for the list
            return Results.Ok(rankings);
        })
        .WithName("GetListRankings")
        .WithTags("Ranking Lists")
        .WithSummary("Get rankings for a specific list")
        .WithDescription("Retrieves all rankings for a specific list, ordered by rank.");

        return group;
    }
} 