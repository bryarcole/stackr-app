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
    public DbSet<RankList> RankLists { get; set; }
    public DbSet<Stack> Stacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Stack entity relationships
        modelBuilder.Entity<Stack>(entity =>
        {
            // Each stack must have one item
            entity.HasOne(r => r.Item)
                .WithMany()
                .HasForeignKey(r => r.ItemId)
                .IsRequired();

            // Each stack must have one rank list
            entity.HasOne(r => r.RankList)
                .WithMany()
                .HasForeignKey(r => r.RankListId)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
} 