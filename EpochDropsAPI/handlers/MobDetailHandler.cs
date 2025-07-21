using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class MobDetailHandler
{
    public static async Task<IResult> Handle(int id, EpochDropsDbContext db)
    {
        var mob = await db.Mobs
            .Include(m => m.Drops)
            .FirstOrDefaultAsync(m => m.Id == id);

        return mob is null ? Results.NotFound() : Results.Ok(mob);
    }
}