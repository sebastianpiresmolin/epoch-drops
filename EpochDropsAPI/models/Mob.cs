namespace EpochDropsAPI.Models;

using System.ComponentModel.DataAnnotations.Schema;
public class Mob
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Location Location { get; set; }
    public int Kills { get; set; }
    private DateTime _lastSeen;
    public DateTime LastSeen
    {
        get => _lastSeen;
        set => _lastSeen = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public ICollection<ItemDrop> Drops { get; set; }

    [NotMapped]
    public QuestReward? Quest { get; set; }
}
