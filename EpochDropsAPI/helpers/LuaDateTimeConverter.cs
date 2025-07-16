namespace EpochDropsAPI.Helpers;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

public class LuaDateTimeConverter : JsonConverter<DateTime>
{
    private const string LuaDateFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateTime.TryParseExact(value, LuaDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt;
        }

        throw new JsonException($"Invalid date format. Expected '{LuaDateFormat}' but got '{value}'");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(LuaDateFormat));
    }
}
