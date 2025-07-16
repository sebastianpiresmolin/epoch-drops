namespace EpochDropsAPI.Models;

public class ItemDrop
{
    public int Id { get; set; }
    public int Count { get; set; }

    public int MobId { get; set; }
    public Mob Mob { get; set; } = null!;

    public int ItemId { get; set; }
    public Item? Item { get; set; } = null!;
}
