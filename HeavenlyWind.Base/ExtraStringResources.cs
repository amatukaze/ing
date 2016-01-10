using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze
{
    public class ExtraStringResources : ModelBase
    {
        public Dictionary<int, string> Ships { get; internal set; }
        public Dictionary<int, string> Equipment { get; internal set; }

        internal ExtraStringResources() { }

        string GetName(Dictionary<int, string> rpDictionary, int rpID)
        {
            string rResult = null;
            rpDictionary?.TryGetValue(rpID, out rResult);
            return rResult;
        }

        public string GetShipName(int rpID) => GetName(Ships, rpID);
        public string GetEquipmentName(int rpID) => GetName(Equipment, rpID);
    }
}
