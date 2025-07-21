using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class QuestDetailHandler
{
    public static async Task<IResult> Handle(int id, EpochDropsDbContext db)
    {
        var quest = await db.QuestRewards
            .Include(q => q.Location)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quest == null)
            return Results.NotFound();

        var rewardItems = await db.QuestRewardDrops
            .Where(r => r.QuestRewardId == id)
            .Include(r => r.Item)
            .Select(r => new
            {
                itemId = r.Item.Id,
                itemName = r.Item.Name,
                icon = r.Item.Icon,
                rarity = r.Item.Rarity,
                count = r.Count
            })
            .ToListAsync();

        var result = new
        {
            quest.Id,
            quest.Title,
            quest.Xp,
            quest.Money,
            quest.SourceMobName,
            zone = quest.Location?.Zone,
            subZone = quest.Location?.SubZone,
            rewards = rewardItems
        };

        return Results.Ok(result);
    }

}