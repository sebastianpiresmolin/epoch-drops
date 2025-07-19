using EpochDropsAPI.data;
using EpochDropsAPI.Models;
using EpochDropsAPI.dto;
using Microsoft.EntityFrameworkCore;

namespace EpochDropsAPI.helpers;

public static class ItemHelper
{
    public static async Task<Item> GetOrCreateItemAsync(EpochDropsDbContext db, UploadItemDrop drop)
    {
        var existing = await db.Items.FirstOrDefaultAsync(i => i.Id == drop.Id);
        if (existing != null)
            return existing;

        var newItem = new Item
        {
            Id = drop.Id,
            Name = drop.Name,
            Icon = drop.Icon,
            Tooltip = drop.Tooltip,
            Rarity = drop.Rarity,
            ItemType = drop.ItemType,
            ItemSubType = drop.ItemSubType,
            EquipSlot = drop.EquipSlot
        };

        db.Items.Add(newItem);

        try
        {
            await db.SaveChangesAsync();
            return newItem;
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("IX_Items_Id") == true)
            {
                // Someone else inserted it first — retrieve it now
                return await db.Items.FirstAsync(i => i.Id == drop.Id);
            }

            // Not a duplicate key error — rethrow
            throw;
        }
    }
}
