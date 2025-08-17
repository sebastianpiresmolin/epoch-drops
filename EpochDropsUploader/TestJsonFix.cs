using System;
using System.Text.Json;
using System.Text.RegularExpressions;

public class TestJsonFix
{
    public static void Main(string[] args)
    {
        // Test the JSON fix with the problematic data from your Lua file
        string testJson = "[{\"type\":\"quest\",\"name\":\"A Humble Task\",\"drops\":{}},{\"name\":\"Plainstrider\",\"drops\":[{\"name\":\"Leg Meat\"}]}]";
        
        Console.WriteLine("Original JSON:");
        Console.WriteLine(testJson);
        Console.WriteLine();
        
        string fixedJson = FixJsonFormat(testJson);
        Console.WriteLine("Fixed JSON:");
        Console.WriteLine(fixedJson);
        Console.WriteLine();
        
        // Test if it deserializes correctly
        try
        {
            var parsed = JsonSerializer.Deserialize<JsonElement>(fixedJson);
            Console.WriteLine("✅ JSON deserialization successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ JSON deserialization failed: " + ex.Message);
        }
    }
    
    private static string FixJsonFormat(string json)
    {
        // Simple regex replacement for empty drops objects
        json = Regex.Replace(json, @"""drops"":\s*\{\s*\}", @"""drops"":[]");
        return json;
    }
}
