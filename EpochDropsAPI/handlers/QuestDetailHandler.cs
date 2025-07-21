using Microsoft.EntityFrameworkCore;
using EpochDropsAPI.data;

namespace EpochDropsAPI.handlers;

public static class QuestDetailHandler
{
    public static async Task<IResult> Handle(int id, EpochDropsDbContext db)
    {
        var quest = await db.QuestRewards
            .Include(q => q.Drops)
            .FirstOrDefaultAsync(q => q.Id == id);

        return quest is null ? Results.NotFound() : Results.Ok(quest);
    }
}