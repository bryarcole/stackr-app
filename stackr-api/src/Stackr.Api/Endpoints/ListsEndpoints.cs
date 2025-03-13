using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class ListsEndpoints
{
    public static RouteGroupBuilder MapListsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (AppDbContext db) =>
        {
            var lists = await db.RankingLists.ToListAsync();
            return Results.Ok(lists);
        });

        group.MapGet("/{id}", async (int id, AppDbContext db) =>
        {
            var list = await db.RankingLists.FindAsync(id);
            if (list == null)
                return Results.NotFound();
            return Results.Ok(list);
        });

        group.MapPost("/", async (RankingList list, AppDbContext db) =>
        {
            db.RankingLists.Add(list);
            await db.SaveChangesAsync();
            return Results.Created($"/lists/{list.Id}", list);
        });

        group.MapPut("/{id}", async (int id, RankingList list, AppDbContext db) =>
        {
            var existingList = await db.RankingLists.FindAsync(id);
            if (existingList == null)
                return Results.NotFound();

            existingList.Name = list.Name;
            existingList.Description = list.Description;
            await db.SaveChangesAsync();
            return Results.Ok(existingList);
        });

        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            var list = await db.RankingLists.FindAsync(id);
            if (list == null)
                return Results.NotFound();

            db.RankingLists.Remove(list);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
} 