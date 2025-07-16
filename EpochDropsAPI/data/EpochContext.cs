using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using EpochDropsAPI.Models;

namespace EpochDropsAPI.data;

public class EpochDropsDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemDrop> ItemDrops { get; set; }
    public DbSet<Mob> Mobs { get; set; }
    public DbSet<QuestReward> QuestRewards { get; set; }
    public DbSet<QuestSource> QuestSources { get; set; }
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<QuestRewardDrop> QuestRewardDrops { get; set; } = null!;


    public EpochDropsDbContext(DbContextOptions<EpochDropsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.Id)
            .IsUnique();

        modelBuilder.Entity<Item>()
            .Property(i => i.Tooltip)
            .HasConversion(
                v => string.Join("|||", v),
                v => v.Split("|||", StringSplitOptions.None).ToList()
            );
    }
}
