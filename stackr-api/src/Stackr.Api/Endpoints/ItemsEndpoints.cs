using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class ItemsEndpoints
{
    public static RouteGroupBuilder MapItemsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (AppDbContext db) => 
        {
            var items = await db.Items.ToListAsync();
            return Results.Ok(items);
        });

        group.MapGet("/{id}", async (AppDbContext db, int id) => 
        {
            var item = await db.Items.FindAsync(id);
            if(item == null) Results.NotFound();
            return Results.Ok(item);
        });

        return group;
    }
} 