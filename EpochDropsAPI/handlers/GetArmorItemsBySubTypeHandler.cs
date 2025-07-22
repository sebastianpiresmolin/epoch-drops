using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class GetArmorItemsBySubType
{
    private static readonly Dictionary<string, string> SlotMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        // Miscellaneous
        { "Amulets", "INVTYPE_NECK" },
        { "Cloaks", "INVTYPE_CLOAK" },
        { "Rings", "INVTYPE_FINGER" },
        { "Trinkets", "INVTYPE_TRINKET" },
        { "Off-hand", "INVTYPE_HOLDABLE" },
        { "Shirts", "INVTYPE_BODY" },
        { "Tabards", "INVTYPE_TABARD" },

        // Special categories
        { "Shields", "INVTYPE_SHIELD" },
        { "Librams", "INVTYPE_RELIC" },
        { "Idols", "INVTYPE_RELIC" },
        { "Totems", "INVTYPE_RELIC" }
    };

    public static async Task<IResult> Handle(
        string subtype,
        string slot,
        int page,
        EpochDropsDbContext db)
    {
        const int PageSize = 50;

        // Map special/misc slot names to correct EquipSlot
        string equipSlot = SlotMappings.TryGetValue(slot, out var mapped)
            ? mapped
            : $"INVTYPE_{slot.ToUpper()}";

        var query = db.Items
            .Where(i => i.ItemSubType == subtype && i.EquipSlot == equipSlot)
            .OrderByDescending(i => i.Rarity)
            .Skip((page - 1) * PageSize)
            .Take(PageSize);

        var totalCount = await db.Items.CountAsync(i =>
            i.ItemSubType == subtype && i.EquipSlot == equipSlot);

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
