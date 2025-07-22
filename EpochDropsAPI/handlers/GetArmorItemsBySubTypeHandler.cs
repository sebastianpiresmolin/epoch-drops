using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class GetArmorItemsBySubType
{
    public static async Task<IResult> Handle(
        string subtype,
        string slot,
        int page,
        EpochDropsDbContext db)
    {
        const int PageSize = 50;
        var upperSlot = $"INVTYPE_{slot.ToUpper()}";

        var query = db.Items
            .Where(i => i.ItemSubType == subtype && i.EquipSlot == upperSlot)
            .OrderByDescending(i => i.Rarity)
            .Skip((page - 1) * PageSize)
            .Take(PageSize);

        var totalCount = await db.Items.CountAsync(i =>
            i.ItemSubType == subtype && i.EquipSlot == upperSlot);

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