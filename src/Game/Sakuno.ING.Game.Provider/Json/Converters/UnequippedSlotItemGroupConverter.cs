using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Provider.Json.Converters;

public class UnequippedSlotItemGroupConverter : JsonConverter<RawUnequippedSlotItems[]>
{
    private static ReadOnlySpan<byte> PropertyNamePrefix => "api_slottype"u8;
    private const int TypeIdOffset = 12;

    public override RawUnequippedSlotItems[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
            throw new InvalidOperationException();

        var result = new List<RawUnequippedSlotItems>(100);

        while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
        {
            var propertyName = !reader.HasValueSequence ? reader.ValueSpan : reader.ValueSequence.ToArray();

            if (!propertyName.StartsWith(PropertyNamePrefix))
                throw new NotSupportedException("Unsupported property name");
            if (!Utf8Parser.TryParse(propertyName.Slice(TypeIdOffset), out int typeId, out var consumed) || consumed + TypeIdOffset != propertyName.Length)
                throw new InvalidOperationException("Bad property name");

            reader.Read();

            if (reader.TokenType is not JsonTokenType.StartArray)
                throw new InvalidOperationException();

            var slotItemIds = JsonSerializer.Deserialize<SlotItemId[]>(ref reader, options)!;

            if (reader.TokenType is not JsonTokenType.EndArray)
                throw new InvalidOperationException();

            result.Add(new()
            {
                TypeId = (SlotItemTypeId)typeId,
                SlotItemIds = slotItemIds,
            });
        }

        return result.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, RawUnequippedSlotItems[] value, JsonSerializerOptions options) =>
        throw new NotSupportedException();
}
