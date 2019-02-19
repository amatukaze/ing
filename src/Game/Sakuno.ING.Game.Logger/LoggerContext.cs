using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Logger.BinaryJson;

namespace Sakuno.ING.Game.Logger
{
    public class LoggerContext : LoggerContextBase
    {
        private readonly BattleApiDeserializer deserializer;

        internal LoggerContext(DbContextOptions<LoggerContextBase> options, BattleApiDeserializer deserializer) : base(options)
        {
            this.deserializer = deserializer;
        }

        internal byte[] StoreBattle(JsonElement json)
            => BinaryJsonExtensions.StoreBattle(json, new BinaryJsonIdResolver(JNameTable));

        public BattleDetailJson LoadBattle(byte[] data) => deserializer.Deserialize(data);
    }
}
