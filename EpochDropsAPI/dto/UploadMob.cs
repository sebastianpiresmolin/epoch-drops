namespace EpochDropsAPI.dto;

using System.Text.Json.Serialization;
using EpochDropsAPI.Helpers;

public class UploadMob
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Giver { get; set; }
    public UploadLocation Location { get; set; } = new();
    public int Kills { get; set; }
    public List<UploadItemDrop> Drops { get; set; } = new();
    public UploadQuest? Quest { get; set; }

    [JsonConverter(typeof(LuaDateTimeConverter))]
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
}