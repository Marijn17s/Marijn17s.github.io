using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceFlowAPI.Converters;

public class DateOnlyConverter : JsonConverter<DateTime>
{
    private const string Format = "dd-MM-yyyy";
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTime.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format));
}