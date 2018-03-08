﻿using Sakuno.KanColle.Amatsukaze.Game.Knowledge;

namespace Sakuno.KanColle.Amatsukaze.Game.MasterData
{
    public abstract class EquipmentTypeInfo : IIdentifiable
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }

        public bool IsAvailableInExtraSlot { get; protected set; }

        public static implicit operator KnownEquipmentType(EquipmentTypeInfo equipmentType) => (KnownEquipmentType)equipmentType.Id;
    }
}
