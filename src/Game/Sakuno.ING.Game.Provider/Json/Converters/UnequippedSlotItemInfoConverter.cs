using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class UnequippedSlotItemInfoConverter : JsonConverter<RawUnequippedSlotItemInfo[]>
    {
        private ReadOnlySpan<byte> PropertyNamePrefix => new[]
        {
            (byte)'a', (byte)'p', (byte)'i', (byte)'_', (byte)'s', (byte)'l', (byte)'o', (byte)'t', (byte)'t', (byte)'y', (byte)'p', (byte)'e'
        };
        private const int TypeIdOffset = 12;

        public override RawUnequippedSlotItemInfo[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new InvalidOperationException();

            var result = new List<RawUnequippedSlotItemInfo>(100);

            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = !reader.HasValueSequence ? reader.ValueSpan : reader.ValueSequence.ToArray();
                if (!propertyName.StartsWith(PropertyNamePrefix))
                    throw new NotSupportedException("Unsupported property name");
                if (!Utf8Parser.TryParse(propertyName.Slice(TypeIdOffset), out int typeId, out var consumed) || consumed + TypeIdOffset != propertyName.Length)
                    throw new InvalidOperationException("Bad property name");

                reader.Read();

                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new InvalidOperationException();

                var slotItemIds = JsonSerializer.Deserialize<SlotItemId[]>(ref reader, options)!;

                if (reader.TokenType != JsonTokenType.EndArray)
                    throw new InvalidOperationException();

                result.Add(new RawUnequippedSlotItemInfo()
                {
                    TypeId = (SlotItemTypeId)typeId,
                    SlotItemIds = slotItemIds,
                });
            }

            return result.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, RawUnequippedSlotItemInfo[] value, JsonSerializerOptions options) =>
            throw new NotSupportedException();
    }
}
