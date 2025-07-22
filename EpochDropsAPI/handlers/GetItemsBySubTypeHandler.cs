using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class GetItemsBySubType
{
    public static async Task<IResult> Handle(
    string subType,
    int page,
    EpochDropsDbContext db)
    {
        const int PageSize = 50;
        var query = db.Items
            .Where(i => i.ItemSubType == subType)
            .OrderByDescending(i => i.Rarity)
            .Skip((page - 1) * PageSize)
            .Take(PageSize);

        var totalCount = await db.Items.CountAsync(i => i.ItemSubType == subType);
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