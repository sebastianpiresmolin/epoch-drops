using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.Models;

namespace EpochDropsAPI.Handlers;

public static class UploadHandler
{
    public static async Task<IResult> Handle(List<Mob> mobs, EpochContext db)
    {
        foreach (var mob in mobs)
        {
            var existing = await db.Mobs
                .Include(m => m.Drops)
                .FirstOrDefaultAsync(m => m.Name == mob.Name);

            if (existing == null)
            {
                db.Mobs.Add(mob);
            }
            else
            {
                existing.KillCount += mob.KillCount;

                foreach (var drop in mob.Drops)
                {
                    var existingDrop = existing.Drops.FirstOrDefault(d => d.ItemId == drop.ItemId);
                    if (existingDrop != null)
                    {
                        existingDrop.Count += drop.Count;
                    }
                    else
                    {
                        existing.Drops.Add(new ItemDrop
                        {
                            ItemId = drop.ItemId,
                            Name = drop.Name,
                            Count = drop.Count,
                            Icon = drop.Icon,
                            Rarity = drop.Rarity,
                            ItemType = drop.ItemType,
                            ItemSubType = drop.ItemSubType,
                            EquipSlot = drop.EquipSlot,
                            Tooltip = drop.Tooltip
                        });

                    }
                }
            }
        }

        await db.SaveChangesAsync();
        return Results.Ok("Upload successful");
    }
}
