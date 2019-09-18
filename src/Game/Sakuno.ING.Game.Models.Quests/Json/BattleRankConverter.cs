using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Models.Quests.Json
{
    internal class BattleRankConverter : JsonConverter<BattleRank>
    {
        public override BattleRank Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new FormatException();
            return reader.GetString() switch
            {
                "perfect" => BattleRank.Perfect,
                "s" => BattleRank.S,
                "a" => BattleRank.A,
                "b" => BattleRank.B,
                "c" => BattleRank.C,
                "d" => BattleRank.D,
                "e" => BattleRank.E,
                _ => throw new FormatException()
            };
        }

        public override void Write(Utf8JsonWriter writer, BattleRank value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
