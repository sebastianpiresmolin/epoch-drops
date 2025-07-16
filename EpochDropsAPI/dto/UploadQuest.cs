namespace EpochDropsAPI.dto;

public class UploadQuest
{
    public string Giver { get; set; } = "";
    public string Title => Name; // or adapt as needed
    public string Name { get; set; } = "";
    public int Xp { get; set; }
    public int Money { get; set; }
    public Dictionary<string, int>? Reputation { get; set; }
}