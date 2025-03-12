using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;

namespace Stackr_Api.Data
{
    public class RankingCountContext : DbContext
    {
        public RankingCountContext(DbContextOptions<RankingCountContext> options) : base(options) { }
        public DbSet<Stack> Stack {get; set;}

        public DbSet<Rank> Rankings { get; set; } = null!;
    }

}
