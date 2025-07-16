using System.ComponentModel.DataAnnotations.Schema;

namespace EpochDropsAPI.Models;

public class QuestReward
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public int Xp { get; set; }

    public int Money { get; set; }

    public string SourceMobName { get; set; } = "";

    public Location Location { get; set; } = null!;
    public List<QuestRewardDrop> Drops { get; set; } = new();

}