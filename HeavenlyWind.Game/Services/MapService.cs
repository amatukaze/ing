using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class MapService
    {
        const string DataFilename = @"Data\node_position.json";

        public static MapService Instance { get; } = new MapService();

        IDisposable r_ConnectionSubscription;

        SQLiteConnection r_Connection;

        Dictionary<int, Dictionary<int, Point>> r_Positions;

        Dictionary<int, PastEventMapInfo> r_PastEventMaps = new Dictionary<int, PastEventMapInfo>();

        MapService() { }

        public void Initialize()
        {
            r_ConnectionSubscription = SessionService.Instance.Subscribe("api_get_member/basic", _ =>
            {
                var rDataFile = new FileInfo(DataFilename);
                if (!rDataFile.Exists)
                    r_Positions = new Dictionary<int, Dictionary<int, Point>>();
                else
                    using (var rReader = new JsonTextReader(rDataFile.OpenText()))
                    {
                        var rData = JObject.Load(rReader);

                        r_Positions = rData.Properties().ToDictionary(r => int.Parse(r.Name), r =>
                            r.Value.ToDictionary(rpNode => (int)rpNode["id"], rpNode => new Point((double)rpNode["x"], (double)rpNode["y"])));
                    }

                r_ConnectionSubscription?.Dispose();
                r_ConnectionSubscription = null;
            });
        }

        public double? GetAngle(int rpMapID, int rpSourceNode, int rpDestinationNode)
        {
            try
            {
                var rSourcePosition = r_Positions[rpMapID][rpSourceNode];
                var rDestinationPosition = r_Positions[rpMapID][rpDestinationNode];

                return Math.Atan2(rDestinationPosition.Y - rSourcePosition.Y, rDestinationPosition.X - rSourcePosition.X) * MathUtil.DegOf1Rad;
            }
            catch
            {
                return null;
            }
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
