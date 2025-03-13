using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class ListsEndpoints
{
    public static RouteGroupBuilder MapListsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (AppDbContext db, RankingList list) => 
        {
            db.RankingLists.Add(list);
            await db.SaveChangesAsync();
            return Results.Created($"/lists/{list.Id}", list);
        });

        group.MapGet("/{id}", async (AppDbContext db, int id) => 
        {
            var list = await db.RankingLists.FindAsync(id);
            if(list == null) Results.NotFound();
            return Results.Ok(list);
        });

        group.MapPut("/{id}", async (AppDbContext db, int id, RankingList updatedList)=>
        {
            var list = await db.RankingLists.FindAsync(id);
            if(list == null) Results.NotFound();

            list = updatedList;
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        group.MapDelete("/{id}", async (AppDbContext db, int id) =>
        {
            var list = await db.RankingLists.FindAsync(id);
            if (list is null) return Results.NotFound();

            db.RankingLists.Remove(list);
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        return group;
    }
} 