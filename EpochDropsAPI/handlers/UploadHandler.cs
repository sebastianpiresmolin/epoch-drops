using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EpochDropsAPI.Models;
using EpochDropsAPI.data;
using EpochDropsAPI.dto;
using EpochDropsAPI.helpers;
using Microsoft.AspNetCore.Mvc;


namespace EpochDropsAPI.handlers;

public static class UploadHandler
{
    public static async Task<IResult> Handle(
        HttpContext context,
        List<UploadMob> uploadMobs,
        [FromServices] EpochDropsDbContext db,
        [FromServices] IOptions<UploaderSettings> uploaderOptions)
    {
        // ✅ Validate secret
        var receivedKey = context.Request.Headers["X-Upload-key"].ToString();
        var expectedKey = uploaderOptions.Value.SecretKey;

        if (string.IsNullOrWhiteSpace(receivedKey) || receivedKey != expectedKey)
        {
            Console.WriteLine("🚫 Unauthorized upload attempt.");
            return Results.Unauthorized();
        }

        foreach (var uploadMob in uploadMobs)
        {
            Console.WriteLine($"Processing: {uploadMob.Name} (type: {uploadMob.Type})");

            // 🎯 QUEST upload
            if (uploadMob.Type == "quest" && uploadMob.Quest != null)
            {
                var quest = uploadMob.Quest;

                var location = new Location
                {
                    Zone = uploadMob.Location.Zone,
                    SubZone = uploadMob.Location.SubZone,
                    X = uploadMob.Location.X,
                    Y = uploadMob.Location.Y
                };
                db.Locations.Add(location);
                await db.SaveChangesAsync();

                var exists = await db.QuestRewards.AnyAsync(q =>
                    q.Title == quest.Title && q.SourceMobName == (uploadMob.Giver ?? uploadMob.Name));

                if (!exists)
                {
                    Console.WriteLine($"🎯 Adding new quest reward: {quest.Title}");

                    var newQuestReward = new QuestReward
                    {
                        Title = quest.Title,
                        Xp = quest.Xp,
                        Money = quest.Money,
                        SourceMobName = uploadMob.Quest?.Giver ?? uploadMob.Name,
                        Location = location,
                    };

                    db.QuestRewards.Add(newQuestReward);
                    await db.SaveChangesAsync();

                    foreach (var drop in uploadMob.Drops)
                    {
                        if (drop.Id == 0) continue;

                        var item = await ItemHelper.GetOrCreateItemAsync(db, drop);

                        db.QuestRewardDrops.Add(new QuestRewardDrop
                        {
                            Count = drop.Count,
                            ItemId = item.InternalId,
                            QuestRewardId = newQuestReward.Id
                        });

                        Console.WriteLine($"🎁 Added quest drop: {item.Name} x{drop.Count}");
                    }

                    await db.SaveChangesAsync();
                }

                continue;
            }

            // 🧟 MOB upload
            var mobLocation = new Location
            {
                Zone = uploadMob.Location.Zone,
                SubZone = uploadMob.Location.SubZone,
                X = uploadMob.Location.X,
                Y = uploadMob.Location.Y
            };
            db.Locations.Add(mobLocation);
            await db.SaveChangesAsync();

            var mob = await db.Mobs
                .Include(m => m.Drops)
                .FirstOrDefaultAsync(m => m.Name == uploadMob.Name);

            if (mob == null)
            {
                mob = new Mob
                {
                    Name = uploadMob.Name,
                    Kills = uploadMob.Kills,
                    LastSeen = DateTime.SpecifyKind(uploadMob.LastSeen, DateTimeKind.Utc),
                    Location = mobLocation,
                    Drops = new List<ItemDrop>()
                };
                db.Mobs.Add(mob);
                Console.WriteLine($"🧟 Added mob: {mob.Name}");
            }
            else
            {
                mob.Kills += uploadMob.Kills;
                mob.LastSeen = DateTime.SpecifyKind(uploadMob.LastSeen, DateTimeKind.Utc);
                mob.Location = mobLocation;
                Console.WriteLine($"🔁 Updating mob: {mob.Name}");
            }

            foreach (var dropDto in uploadMob.Drops)
            {
                if (dropDto.Id == 0)
                {
                    Console.WriteLine("⚠️ Drop has null or 0 ItemId – skipping.");
                    continue;
                }

                var item = await ItemHelper.GetOrCreateItemAsync(db, dropDto);

                var existingDrop = mob.Drops.FirstOrDefault(d => d.ItemId == item.InternalId);
                if (existingDrop != null)
                {
                    existingDrop.Count += dropDto.Count;
                    Console.WriteLine($"🔁 Updated drop count for {item.Name}");
                }
                else
                {
                    mob.Drops.Add(new ItemDrop
                    {
                        ItemId = item.InternalId,
                        Count = dropDto.Count
                    });
                    Console.WriteLine($"➕ Added drop: {item.Name} x{dropDto.Count}");
                }
            }

            await db.SaveChangesAsync();
        }

        return Results.Ok("Upload successful");
    }
}
