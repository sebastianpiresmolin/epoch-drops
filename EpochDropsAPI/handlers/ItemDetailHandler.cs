using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class ItemDetailHandler
{
    public static async Task<IResult> Handle(int id, EpochDropsDbContext db)
    {
        var item = await db.Items
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
            return Results.NotFound();

        var mobsThatDrop = await db.ItemDrops
            .Where(d => d.ItemId == item.InternalId)
            .Include(d => d.Mob)
                .ThenInclude(m => m.Location)
            .Select(d => new
            {
                mobId = d.Mob.Id,
                mobName = d.Mob.Name,
                mobKillCount = d.Mob.Kills,
                dropCount = d.Count,
                zone = d.Mob.Location != null ? d.Mob.Location.Zone : null,
                subZone = d.Mob.Location != null ? d.Mob.Location.SubZone : null
            })
            .ToListAsync();

        var questRewards = await db.QuestRewardDrops
            .Where(qrd => qrd.ItemId == item.InternalId)
            .Include(qrd => qrd.QuestReward)
                .ThenInclude(qr => qr.Location)
            .Select(qrd => new
            {
                questId = qrd.QuestReward.Id,
                title = qrd.QuestReward.Title,
                xp = qrd.QuestReward.Xp,
                money = qrd.QuestReward.Money,
                sourceMob = qrd.QuestReward.SourceMobName,
                zone = qrd.QuestReward.Location.Zone,
                subZone = qrd.QuestReward.Location.SubZone
            })
            .ToListAsync();


        var result = new
        {
            item.Id,
            item.InternalId,
            item.Name,
            item.Icon,
            item.Rarity,
            item.ItemType,
            item.ItemSubType,
            item.EquipSlot,
            item.Tooltip,
            mobsThatDrop,
            questRewards,
        };

        return Results.Ok(result);
    }
}