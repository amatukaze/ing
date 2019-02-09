using System;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Game.Json.Combat;
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

        public byte[] StoreBattle(ReadOnlyMemory<byte> json, bool wrappedWithStatusCode)
            => new BinaryJsonEncoder(json, new BinaryJsonIdResolver(JNameTable), wrappedWithStatusCode ? "api_data" : null).Result;

        public BattleApi LoadBattle(byte[] data) => deserializer.Deserialize(data);
    }
}
