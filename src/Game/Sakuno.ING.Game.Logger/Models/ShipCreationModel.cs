using System;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Models
{
    public class ShipCreationModel : ITimedEntity
    {
        private readonly ShipCreationEntity entity;

        public ShipCreationModel(ITable<ShipInfoId, ShipInfo> shipInfoTable, ShipCreationEntity entity)
        {
            this.entity = entity;
            ShipBuilt = shipInfoTable[entity.ShipBuilt];
            Secretary = shipInfoTable[entity.Secretary];
        }

        public DateTimeOffset TimeStamp => entity.TimeStamp;
        public Materials Consumption => entity.Consumption;
        public bool IsLSC => entity.IsLSC;
        public ShipInfo ShipBuilt { get; }
        public int EmptyDockCount => entity.EmptyDockCount;
        public ShipInfo Secretary { get; }
        public int SecretaryLevel => entity.SecretaryLevel;
        public int AdmiralLevel => entity.AdmiralLevel;
    }
}
