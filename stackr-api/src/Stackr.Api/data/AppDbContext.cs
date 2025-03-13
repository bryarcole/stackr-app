using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;

namespace Stackr_Api.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Define the database tables
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<RankingList> RankingLists { get; set; }
    public DbSet<Ranking> Rankings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Ranking entity relationships
        modelBuilder.Entity<Ranking>(entity =>
        {
            // Each ranking must have one item
            entity.HasOne(r => r.Item)
                .WithMany()
                .HasForeignKey(r => r.ItemId)
                .IsRequired();

            // Each ranking must have one ranking list
            entity.HasOne(r => r.RankingList)
                .WithMany()
                .HasForeignKey(r => r.RankingListId)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
} 