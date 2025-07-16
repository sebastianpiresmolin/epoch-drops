namespace EpochDropsAPI.Models;

public class QuestRewardDrop
{
    public int Id { get; set; }
    public int Count { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public int QuestRewardId { get; set; }
    public QuestReward QuestReward { get; set; } = null!;
}

