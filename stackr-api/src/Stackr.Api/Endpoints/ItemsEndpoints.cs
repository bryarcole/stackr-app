using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class ItemsEndpoints
{
    public static RouteGroupBuilder MapItemsEndpoints(this RouteGroupBuilder group)
    {
        // Endpoint to retrieve all items
        // This endpoint returns a list of all items in the system.
        group.MapGet("/", async (AppDbContext db) => 
        {
            // Retrieve all items from the database
            var items = await db.Items.ToListAsync();
            return Results.Ok(items);
        })
        .WithName("GetItems")
        .WithTags("Items")
        .WithSummary("Get all items")
        .WithDescription("Retrieves a list of all items in the system.");

        // Endpoint to retrieve a specific item by ID
        // This endpoint returns the details of a specific item identified by its ID.
        group.MapGet("/{id}", async (AppDbContext db, int id) => 
        {
            // Find the item by ID
            var item = await db.Items.FindAsync(id);
            if(item == null) 
                return Results.NotFound();
            return Results.Ok(item);
        })
        .WithName("GetItem")
        .WithTags("Items")
        .WithSummary("Get item by ID")
        .WithDescription("Retrieves a specific item by its ID.");

        return group;
    }
} 