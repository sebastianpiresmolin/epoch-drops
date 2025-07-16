namespace EpochDropsAPI.Models;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


public class Item
{
    [Key]
    public int InternalId { get; set; }
    public int Id { get; set; } // In-game item ID
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public int Rarity { get; set; }
    public string ItemType { get; set; } = "";
    public string ItemSubType { get; set; } = "";
    public string EquipSlot { get; set; } = "";
    public List<string> Tooltip { get; set; } = new();

    public List<ItemDrop> Drops { get; set; } = new();
    public List<QuestReward> QuestRewards { get; set; } = new();
}

