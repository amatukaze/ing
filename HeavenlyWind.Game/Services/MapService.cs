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
        public const string DataFilename = @"Data\nodes.json";

        public static MapService Instance { get; } = new MapService();

        Dictionary<int, HybridDictionary<int, Node>> r_Nodes;

        HybridDictionary<int, PastEventMapInfo> r_PastEventMaps = new HybridDictionary<int, PastEventMapInfo>();

        MapService() { }

        public void Initialize()
        {
            SessionService.Instance.Subscribe("api_get_member/require_info", delegate
            {
                var rDataFile = new FileInfo(DataFilename);
                if (!rDataFile.Exists)
                    r_Nodes = new Dictionary<int, HybridDictionary<int, Node>>();
                else
                    using (var rReader = new JsonTextReader(rDataFile.OpenText()))
                    {
                        var rData = JObject.Load(rReader);

                        r_Nodes = rData.Properties().ToDictionary(r => int.Parse(r.Name), r =>
                            r.Value.Select(rpNode => rpNode.ToObject<Node>()).ToHybridDictionary(rpNode => rpNode.ID));
                    }
            });
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
