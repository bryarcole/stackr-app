using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;
using Stackr_Api.data;

namespace Stackr_Api.Endpoints;

public static class RankingsEndpoints
{
    public static RouteGroupBuilder MapRankingsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/aggregate/{id}", async (AppDbContext db, int id) => 
        {
            var rankings = await db.Rankings.Where(r => r.ItemId == id).ToListAsync();

            var itemScores = new Dictionary<int, int>();

            foreach (var ranking in rankings)
            {
                if (!itemScores.ContainsKey(ranking.ItemId))
                {
                    itemScores[ranking.ItemId] = ranking.Rank;
                }
                else
                {
                    itemScores[ranking.ItemId] += ranking.Rank;
                }
            }

            var sortedRankings = itemScores
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new 
                {
                    ItemId = kvp.Key, 
                    score = kvp.Value
                });

            return Results.Ok(sortedRankings);
        });

        return group;
    }
} 