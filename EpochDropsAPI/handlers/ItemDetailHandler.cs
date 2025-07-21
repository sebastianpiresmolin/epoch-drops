using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;
public static class ItemDetailHandler
{
    public static async Task<IResult> Handle(int id, EpochDropsDbContext db)
    {
        var item = await db.Items.FirstOrDefaultAsync(i => i.Id == id);

        return item is null ? Results.NotFound() : Results.Ok(item);
    }
}