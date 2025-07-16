namespace EpochDropsAPI.dto;

public class UploadItemDrop
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public int Count { get; set; }
    public List<string> Tooltip { get; set; } = new();
    public int Rarity { get; set; }
    public string ItemType { get; set; } = "";
    public string ItemSubType { get; set; } = "";
    public string EquipSlot { get; set; } = "";

}
