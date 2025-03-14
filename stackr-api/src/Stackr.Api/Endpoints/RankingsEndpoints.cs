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
            var existingList = await db.RankLists
                .FirstOrDefaultAsync(l => l.Name.ToLower() == submission.Title.ToLower());

            RankList list;
            if (existingList == null)
            {
                // Create a new list if it doesn't exist
                list = new RankList
                {
                    Name = submission.Title,
                    Description = submission.Description,
                    CreatedAt = DateTime.UtcNow
                };
                db.RankLists.Add(list);
                await db.SaveChangesAsync();
            }
            else
            {
                // Update the description of the existing list
                list = existingList;
                list.Description = submission.Description;
                await db.SaveChangesAsync();
            }

            // Iterate over each item in the submission to create or update items and stacks
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

                // Create a new stack for the item
                var stack = new Stack
                {
                    ItemId = dbItem.Id,
                    RankListId = list.Id,
                    CreatedAt = DateTime.UtcNow
                };
                db.Stacks.Add(stack);
            }

            // Save all changes to the database
            await db.SaveChangesAsync();
            return Results.Created($"/api/rankings/lists/{list.Id}", list);
        })
        .WithName("SubmitRankedList")
        .WithTags("Rank Lists")
        .WithSummary("Submit a complete rank list")
        .WithDescription("Submits a complete rank list. If a list with the same title exists, adds the stacks to that list. Otherwise, creates a new list.");

        // Endpoint to create a new rank list
        // This endpoint accepts a RankList object and creates a new list in the database.
        // It requires a non-empty name for the list.
        group.MapPost("/lists", async (AppDbContext db, RankList list) =>
        {
            // Validate the list
            if (string.IsNullOrEmpty(list.Name))
                return Results.BadRequest("List name is required");

            // Set the creation date and add the list to the database
            list.CreatedAt = DateTime.UtcNow;
            db.RankLists.Add(list);
            await db.SaveChangesAsync();

            // Return the created list with its ID
            return Results.Created($"/api/rankings/lists/{list.Id}", list);
        })
        .WithName("CreateRankList")
        .WithTags("Rank Lists")
        .WithSummary("Create a new rank list")
        .WithDescription("Creates a new rank list with the provided name and description. The list can then be used to rank items.");

        // Endpoint to add items to an existing rank list
        // This endpoint accepts a list of Stack objects and adds them to the specified rank list.
        // It checks that the list exists and that each item is valid and exists in the database.
        group.MapPost("/lists/{listId}/items", async (AppDbContext db, int listId, List<Stack> stacks) =>
        {
            // Validate the list exists
            var list = await db.RankLists.FindAsync(listId);
            if (list == null)
                return Results.NotFound("List not found");

            // Validate each stack
            foreach (var stack in stacks)
            {
                if (stack.ItemId <= 0 || stack.Rank <= 0)
                    return Results.BadRequest("Invalid stack data");

                // Verify item exists
                var item = await db.Items.FindAsync(stack.ItemId);
                if (item == null)
                    return Results.NotFound($"Item with ID {stack.ItemId} not found");

                // Set the rank list ID and creation date
                stack.RankListId = listId;
                stack.CreatedAt = DateTime.UtcNow;
                db.Stacks.Add(stack);
            }

            // Save all changes to the database
            await db.SaveChangesAsync();
            return Results.Ok("Stacks added successfully");
        })
        .WithName("AddItemsToRankList")
        .WithTags("Rank Lists")
        .WithSummary("Add items to a rank list")
        .WithDescription("Adds one or more items to a rank list with their respective ranks. Each item must exist in the system.");

        // Endpoint to get aggregate stacks across all lists
        // This endpoint retrieves all stacks, groups them by item, and calculates the average rank.
        // It returns a list of AggregateRanking objects with statistics for each item.
        group.MapGet("/aggregate", async (AppDbContext db) => 
        {
            // Get all stacks with their related items and lists
            var stacks = await db.Stacks
                .Include(r => r.Item)
                .Include(r => r.RankList)
                .ToListAsync();

            // Group by item and calculate weighted average rank
            var aggregatedStacks = stacks
                .GroupBy(r => new { r.ItemId, r.Item.Name })
                .Select(g => new AggregateRanking
                {
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    Score = g.Average(r => r.Rank),
                    ListCount = g.Select(r => r.RankListId).Distinct().Count(),
                    TotalRankings = g.Count()
                })
                .OrderByDescending(r => r.Score)
                .ToList();

            // Return the aggregated stacks
            return Results.Ok(aggregatedStacks);
        })
        .WithName("GetAggregateStacks")
        .WithTags("Aggregate Stacks")
        .WithSummary("Get aggregate stacks")
        .WithDescription("Retrieves aggregated stacks across all lists, including weighted average scores and statistics.");

        // Endpoint to get stacks for a specific list
        // This endpoint retrieves all stacks for a specified list, ordered by rank.
        // It checks that the list exists and returns a list of Stack objects.
        group.MapGet("/lists/{listId}", async (AppDbContext db, int listId) => 
        {
            // Check if the list exists
            var list = await db.RankLists.FindAsync(listId);
            if (list == null)
                return Results.NotFound("List not found");

            // Retrieve and order the stacks by rank
            var stacks = await db.Stacks
                .Include(r => r.Item)
                .Where(r => r.RankListId == listId)
                .OrderBy(r => r.Rank)
                .ToListAsync();

            // Return the stacks for the list
            return Results.Ok(stacks);
        })
        .WithName("GetListStacks")
        .WithTags("Rank Lists")
        .WithSummary("Get stacks for a specific list")
        .WithDescription("Retrieves all stacks for a specific list, ordered by rank.");

        return group;
    }
} 