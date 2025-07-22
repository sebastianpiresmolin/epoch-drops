using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class GetItemsByCategory
{
    public static async Task<IResult> Handle(string category, int page, EpochDropsDbContext db)
    {
        const int PageSize = 50;

        // Map UI category name to DB ItemType
        var dbCategory = category switch
        {
            "Consumables" => "Consumable",
            "Containers" => "Container",
            "Keys" => "Key", // lowercase
            _ => category // fallback to provided
        };

        var query = db.Items
            .Where(i => i.ItemType == dbCategory)
            .OrderByDescending(i => i.Rarity)
            .Skip((page - 1) * PageSize)
            .Take(PageSize);

        var totalCount = await db.Items.CountAsync(i => i.ItemType == dbCategory);

        var items = await query.Select(i => new
        {
            i.Id,
            i.Name,
            i.Rarity,
            i.Icon
        }).ToListAsync();

        return Results.Ok(new
        {
            items,
            page,
            totalPages = (int)Math.Ceiling((double)totalCount / PageSize)
        });
    }
}

