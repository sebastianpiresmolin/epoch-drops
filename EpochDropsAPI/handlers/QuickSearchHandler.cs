using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class QuickSearchHandler
{
    public static async Task<IResult> Handle(string q, EpochDropsDbContext db)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Results.Ok(new List<object>());

        q = q.Trim().ToLower();

        var itemResults = await db.Items
        .Where(i => i.Name.ToLower().Contains(q))
        .OrderBy(i => i.Name)
        .Take(3)
        .Select(i => new { type = "Item", id = i.Id, name = i.Name, rarity = (int?)i.Rarity, icon = i.Icon })
        .ToListAsync();

        var mobResults = await db.Mobs
            .Where(m => m.Name.ToLower().Contains(q))
            .OrderBy(m => m.Name)
            .Take(1)
            .Select(m => new
            {
                type = "Mob",
                id = m.Id,
                name = m.Name,
                rarity = (int?)null,
                icon = (string?)null
            })
            .ToListAsync();

        var questResults = await db.QuestRewards
            .Where(qr => qr.Title.ToLower().Contains(q))
            .OrderBy(qr => qr.Title)
            .Take(1)
            .Select(qr => new
            {
                type = "Quest",
                id = qr.Id,
                name = qr.Title,
                rarity = (int?)null,
                icon = (string?)null
            })
            .ToListAsync();

        var allResults = itemResults.Concat(mobResults).Concat(questResults).Take(5);
        return Results.Ok(allResults);
    }
}
