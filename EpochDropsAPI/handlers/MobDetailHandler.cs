using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class MobDetailsHandler
{
    public static async Task<IResult> Handle(int id, EpochDropsDbContext db)
    {
        var mob = await db.Mobs
            .Include(m => m.Location)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mob == null)
            return Results.NotFound();

        var totalKills = mob.Kills;

        var items = await db.ItemDrops
            .Where(d => d.MobId == id)
            .Join(db.Items,
                drop => drop.ItemId,
                item => item.InternalId,
                (drop, item) => new
                {
                    itemId = item.Id,
                    name = item.Name,
                    icon = item.Icon,
                    rarity = item.Rarity,
                    dropCount = drop.Count,
                    dropChance = totalKills > 0
                        ? $"{(drop.Count * 100.0 / totalKills):0.00}%"
                        : "N/A"
                })
            .ToListAsync();

        var result = new
        {
            mob.Id,
            mob.Name,
            zone = mob.Location?.Zone,
            subZone = mob.Location?.SubZone,
            totalKills,
            items
        };

        return Results.Ok(result);
    }
}
