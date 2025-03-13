using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;

namespace Stackr_Api.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<RankingList> RankingLists { get; set; }
    public DbSet<Ranking> Rankings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ranking>()
            .HasOne(r => r.Item)
            .WithMany()
            .HasForeignKey(r => r.ItemId)
            .IsRequired();

        modelBuilder.Entity<Ranking>()
            .HasOne(r => r.RankingList)
            .WithMany()
            .HasForeignKey(r => r.RankingListId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
} 