using Sakuno.Collections;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze
{
    public class ExtraStringResources : ModelBase
    {
        public HybridDictionary<int, string> Ships { get; internal set; }
        public HybridDictionary<int, string> ShipTypes { get; internal set; }
        public HybridDictionary<int, string> AbyssalShip { get; internal set; }
        public HybridDictionary<int, string> Equipment { get; internal set; }
        public HybridDictionary<int, string> Items { get; internal set; }
        public HybridDictionary<int, string> Expeditions { get; internal set; }
        public HybridDictionary<int, string> Quests { get; internal set; }
        public HybridDictionary<int, string> QuestDescriptions { get; internal set; }
        public HybridDictionary<int, string> Areas { get; internal set; }
        public HybridDictionary<int, string> Maps { get; internal set; }
        public HybridDictionary<int, string> ShipLocking { get; internal set; }

        internal ExtraStringResources() { }

        public HybridDictionary<int, string> GetTranslations(ExtraStringResourceType rpType)
        {
            switch (rpType)
            {
                case ExtraStringResourceType.Ship:
                    return Ships;

                case ExtraStringResourceType.ShipType:
                    return ShipTypes;

                case ExtraStringResourceType.AbyssalShip:
                    return AbyssalShip;

                case ExtraStringResourceType.Equipment:
                    return Equipment;

                case ExtraStringResourceType.Item:
                    return Items;

                case ExtraStringResourceType.Expedition:
                    return Expeditions;

                case ExtraStringResourceType.Quest:
                    return Quests;

                case ExtraStringResourceType.QuestDescription:
                    return QuestDescriptions;

                case ExtraStringResourceType.Area:
                    return Areas;

                case ExtraStringResourceType.Map:
                    return Maps;

                case ExtraStringResourceType.ShipLocking:
                    return ShipLocking;

                default: throw new ArgumentException(nameof(rpType));
            }
        }

        string GetName(IDictionary<int, string> rpDictionary, int rpID)
        {
            if (rpDictionary == null)
                return null;

            string rResult;
            rpDictionary.TryGetValue(rpID, out rResult);
            return rResult;
        }

        public string GetShipName(int rpID) => GetName(Ships, rpID);
        public string GetShipTypeName(int rpID) => GetName(ShipTypes, rpID);
        public string GetAbyssalShip(int rpID) => GetName(AbyssalShip, rpID);
        public string GetEquipmentName(int rpID) => GetName(Equipment, rpID);
        public string GetItemName(int rpID) => GetName(Items, rpID);
        public string GetExpeditionName(int rpID) => GetName(Expeditions, rpID);
        public string GetQuestName(int rpID) => GetName(Quests, rpID);
        public string GetQuestDescription(int rpID) => GetName(QuestDescriptions, rpID);
        public string GetAreaName(int rpID) => GetName(Areas, rpID);
        public string GetMapName(int rpID) => GetName(Maps, rpID);
        public string GetShipLockingName(int rpID) => GetName(ShipLocking, rpID);
    }
}
