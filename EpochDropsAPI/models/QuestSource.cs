namespace EpochDropsAPI.Models;

public class QuestSource
{
    public int Id { get; set; }
    public string NpcName { get; set; } = "";
    public string Zone { get; set; } = "";
    public string SubZone { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }

    public List<QuestReward> Rewards { get; set; } = new();
}