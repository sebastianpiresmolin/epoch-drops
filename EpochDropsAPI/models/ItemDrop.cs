namespace EpochDropsAPI.Models;

public class ItemDrop
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Name { get; set; } = "";
    public int Count { get; set; }

    public string Icon { get; set; } = "";
    public int Rarity { get; set; }
    public string ItemType { get; set; } = "";
    public string ItemSubType { get; set; } = "";
    public string EquipSlot { get; set; } = "";
    public List<string> Tooltip { get; set; } = new();

    public int MobId { get; set; }
    public Mob Mob { get; set; } = null!;
}
