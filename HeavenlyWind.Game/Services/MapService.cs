using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class MapService
    {
        public static MapService Instance { get; } = new MapService();

        IDictionary<int, HybridDictionary<int, Node>> r_Nodes;

        HybridDictionary<int, PastEventMapInfo> r_PastEventMaps = new HybridDictionary<int, PastEventMapInfo>();

        MapService() { }

        internal void Initialize()
        {
            ApiService.Subscribe("api_get_member/require_info", delegate
            {
                DataStore.Updated += rpName =>
                {
                    if (rpName == "map_node")
                        Reload();
                };

                Reload();
            });
        }
        void Reload()
        {
            byte[] rContent;
            if (!DataStore.TryGet("map_node", out rContent))
                r_Nodes = new ListDictionary<int, HybridDictionary<int, Node>>();
            else
            {
                var rReader = new JsonTextReader(new StreamReader(new MemoryStream(rContent)));
                var rData = JObject.Load(rReader);

                r_Nodes = rData.Properties().ToDictionary(
                    r => int.Parse(r.Name),
                    r => r.Value.Select(rpNode => rpNode.ToObject<Node>()).ToHybridDictionary(rpNode => rpNode.ID));
            }
        }

        public bool ContainsMap(int rpMapID) => r_Nodes.ContainsKey(rpMapID);

        public string GetNodeWikiID(int rpMapID, int rpNodeID)
        {
            HybridDictionary<int, Node> rMap;
            Node rNode;
            if (r_Nodes.TryGetValue(rpMapID, out rMap) && rMap.TryGetValue(rpNodeID, out rNode))
                return rNode.WikiID;

            return null;
        }

        public int? GetNodeUniqueID(int rpMapID, int rpNodeID)
        {
            var rWikiID = GetNodeWikiID(rpMapID, rpNodeID);
            if (rWikiID == null)
                return null;

            return r_Nodes[rpMapID].Where(r => r.Value.WikiID == rWikiID).Min(r => r.Key);
        }

        public double? GetAngle(int rpMapID, int rpSourceNode, int rpDestinationNode)
        {
            HybridDictionary<int, Node> rMap;
            Node rSourceNode, rDestinationNode;

            if (r_Nodes.TryGetValue(rpMapID, out rMap) && rMap.TryGetValue(rpSourceNode, out rSourceNode) && rMap.TryGetValue(rpDestinationNode, out rDestinationNode))
                return Math.Atan2(rDestinationNode.PositionY - rSourceNode.PositionY, rDestinationNode.PositionX - rSourceNode.PositionX) * MathUtil.DegOf1Rad;

            return null;
        }

        public IMapMasterInfo GetMasterInfo(int rpMapID)
        {
            MapMasterInfo rMap;
            if (KanColleGame.Current.MasterInfo.Maps.TryGetValue(rpMapID, out rMap))
                return rMap;

            PastEventMapInfo rPastEventMap;
            if (r_PastEventMaps.TryGetValue(rpMapID, out rPastEventMap))
                return rPastEventMap;

            r_PastEventMaps.Add(rpMapID, rPastEventMap = new PastEventMapInfo(rpMapID));

            return rPastEventMap;
        }
    }
}
