using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Sakuno.ING.Game.Logger.BinaryJson
{
    internal class BinaryJsonEncoder
    {
        private readonly IReadOnlyDictionary<string, int> jNames;

        public byte[] Result { get; }

        public BinaryJsonEncoder(ReadOnlyMemory<byte> utf8Json, IReadOnlyDictionary<string, int> jNames, string subname = null)
        {
            this.jNames = jNames;
            using (var document = JsonDocument.Parse(utf8Json))
            {
                var writer = new BinaryJsonWriter();
                if (subname == null)
                    Format(document.RootElement, writer);
                else if (document.RootElement.TryGetProperty(subname, out var v))
                    Format(v, writer);
                Result = writer.Complete();
            }
        }

        private void Format(JsonElement element, BinaryJsonWriter writer)
        {
            switch (element.Type)
            {
                case JsonValueType.Object:
                    writer.WriteStartObject();
                    foreach (var child in element.EnumerateObject())
                    {
                        writer.WriteJName(jNames[child.Name]);
                        Format(child.Value, writer);
                    }
                    writer.WriteEndObject();
                    break;

                case JsonValueType.Array:
                    writer.WriteArraySize(element.GetArrayLength());
                    foreach (var child in element.EnumerateArray())
                        Format(child, writer);
                    break;

                case JsonValueType.String:
                    writer.WriteString(element.GetString());
                    break;

                case JsonValueType.Number:
                    if (element.TryGetInt32(out int ivalue))
                        writer.WriteInteger(ivalue);
                    else
                        writer.WriteDecimal(element.GetDecimal());
                    break;

                case JsonValueType.True:
                    writer.WriteInteger(1);
                    break;

                case JsonValueType.False:
                    writer.WriteInteger(0);
                    break;

                case JsonValueType.Undefined:
                case JsonValueType.Null:
                    writer.WriteNull();
                    break;
                default:
                    throw new InvalidOperationException("Unknown json value type.");
            }
        }
    }
}
