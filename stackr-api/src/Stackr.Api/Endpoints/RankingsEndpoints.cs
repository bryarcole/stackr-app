using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class RankingsEndpoints
{
    public static RouteGroupBuilder MapRankingsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/aggregate", async (AppDbContext db) => 
        {
            var rankings = await db.Rankings
                .Include(r => r.Item)
                .Include(r => r.RankingList)
                .GroupBy(r => new { r.ItemId, r.Item.Name })
                .Select(g => new
                {
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    Score = g.Average(r => r.Rank)
                })
                .OrderByDescending(r => r.Score)
                .ToListAsync();

            return Results.Ok(rankings);
        });

        group.MapGet("/aggregate/{id}", async (AppDbContext db, int id) => 
        {
            var rankings = await db.Rankings
                .Include(r => r.Item)
                .Where(r => r.ItemId == id)
                .GroupBy(r => new { r.ItemId, r.Item.Name })
                .Select(g => new
                {
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    Score = g.Average(r => r.Rank)
                })
                .OrderByDescending(r => r.Score)
                .ToListAsync();

            return Results.Ok(rankings);
        });

        return group;
    }
} 