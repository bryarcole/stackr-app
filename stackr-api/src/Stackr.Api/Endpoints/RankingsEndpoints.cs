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
            // Get all rankings with their related items
            var rankings = await db.Rankings
                .Include(r => r.Item)
                .ToListAsync();

            // Group by item and calculate average rank
            var aggregatedRankings = rankings
                .GroupBy(r => new { r.ItemId, r.Item.Name })
                .Select(g => new
                {
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    Score = g.Average(r => r.Rank)
                })
                .OrderByDescending(r => r.Score)
                .ToList();

            return Results.Ok(aggregatedRankings);
        });

        group.MapGet("/aggregate/{id}", async (AppDbContext db, int id) => 
        {
            // Get rankings for a specific item
            var rankings = await db.Rankings
                .Include(r => r.Item)
                .Where(r => r.ItemId == id)
                .ToListAsync();

            // Group by item and calculate average rank
            var aggregatedRankings = rankings
                .GroupBy(r => new { r.ItemId, r.Item.Name })
                .Select(g => new
                {
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    Score = g.Average(r => r.Rank)
                })
                .OrderByDescending(r => r.Score)
                .ToList();

            return Results.Ok(aggregatedRankings);
        });

        return group;
    }
} 