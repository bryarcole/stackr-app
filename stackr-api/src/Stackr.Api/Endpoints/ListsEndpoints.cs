using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class ListsEndpoints
{
    public static RouteGroupBuilder MapListsEndpoints(this RouteGroupBuilder group)
    {
        // Endpoint to retrieve all ranking lists
        // This endpoint returns a list of all ranking lists in the system.
        group.MapGet("/", async (AppDbContext db) =>
        {
            // Retrieve all ranking lists from the database
            var lists = await db.RankLists.ToListAsync();
            return Results.Ok(lists);
        })
        .WithName("GetLists")
        .WithTags("Lists")
        .WithSummary("Get all lists")
        .WithDescription("Retrieves a list of all ranking lists in the system.");

        // Endpoint to retrieve a specific ranking list by ID
        // This endpoint returns the details of a specific ranking list identified by its ID.
        group.MapGet("/{id}", async (int id, AppDbContext db) =>
        {
            // Find the ranking list by ID
            var list = await db.RankLists.FindAsync(id);
            if (list == null)
                return Results.NotFound();
            return Results.Ok(list);
        })
        .WithName("GetList")
        .WithTags("Lists")
        .WithSummary("Get list by ID")
        .WithDescription("Retrieves a specific ranking list by its ID.");

        // Endpoint to create a new ranking list
        // This endpoint accepts a RankingList object and creates a new list in the database.
        group.MapPost("/", async (RankList list, AppDbContext db) =>
        {
            // Set the creation date and add the list to the database
            list.CreatedAt = DateTime.UtcNow;
            db.RankLists.Add(list);
            await db.SaveChangesAsync();
            return Results.Created($"/lists/{list.Id}", list);
        })
        .WithName("CreateList")
        .WithTags("Lists")
        .WithSummary("Create a new list")
        .WithDescription("Creates a new ranking list with the provided information.");

        // Endpoint to update an existing ranking list
        // This endpoint accepts a RankingList object and updates the specified list in the database.
        group.MapPut("/{id}", async (int id, RankList list, AppDbContext db) =>
        {
            // Find the existing ranking list by ID
            var existingList = await db.RankLists.FindAsync(id);
            if (existingList == null)
                return Results.NotFound();

            // Update the list's properties
            existingList.Name = list.Name;
            existingList.Description = list.Description;
            await db.SaveChangesAsync();
            return Results.Ok(existingList);
        })
        .WithName("UpdateList")
        .WithTags("Lists")
        .WithSummary("Update a list")
        .WithDescription("Updates an existing ranking list's information.");

        // Endpoint to delete a ranking list
        // This endpoint deletes a specified ranking list from the database.
        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            // Find the ranking list by ID
            var list = await db.RankLists.FindAsync(id);
            if (list == null)
                return Results.NotFound();

            // Remove the list from the database
            db.RankLists.Remove(list);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeleteList")
        .WithTags("Lists")
        .WithSummary("Delete a list")
        .WithDescription("Deletes a ranking list from the system.");

        return group;
    }
} 