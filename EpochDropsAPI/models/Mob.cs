namespace EpochDropsAPI.Models;
public class Mob
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int KillCount { get; set; }
    public List<ItemDrop> Drops { get; set; } = new();
}
