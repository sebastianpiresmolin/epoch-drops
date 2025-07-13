using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using EpochDropsAPI.Models;

public class EpochContext : DbContext
{
    public DbSet<Mob> Mobs => Set<Mob>();
    public DbSet<ItemDrop> ItemDrops => Set<ItemDrop>();

    public EpochContext(DbContextOptions<EpochContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var converter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
        );

        modelBuilder.Entity<ItemDrop>()
            .Property(e => e.Tooltip)
            .HasConversion(converter);
    }
}
