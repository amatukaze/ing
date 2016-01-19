using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class MapService
    {
        public static MapService Instance { get; } = new MapService();

        IDisposable r_ConnectionSubscription;

        SQLiteConnection r_Connection;

        Dictionary<int, Dictionary<int, Point>> r_Positions = new Dictionary<int, Dictionary<int, Point>>();

        Dictionary<int, PastEventMapInfo> r_PastEventMaps = new Dictionary<int, PastEventMapInfo>();

        MapService() { }

        public void Initialize()
        {
            r_ConnectionSubscription = SessionService.Instance.Subscribe("api_get_member/basic", _ =>
            {
                r_Connection = new SQLiteConnection(@"Data Source=Data\Maps.db").OpenAndReturn();

                using (var rCommand = r_Connection.CreateCommand())
                {
                    rCommand.CommandText = "SELECT * FROM node_position;";

                    var rCurrentMap = 0;
                    Dictionary<int, Point> rPositions = null;
                    using (var rReader = rCommand.ExecuteReader())
                        while (rReader.Read())
                        {
                            var rMapID = Convert.ToInt32(rReader["map"]);
                            if (rMapID != rCurrentMap)
                            {
                                r_Positions.Add(rMapID, rPositions = new Dictionary<int, Point>());
                                rCurrentMap = rMapID;
                            }

                            var rNode = Convert.ToInt32(rReader["node"]);
                            var rPositionX = Convert.ToDouble(rReader["x"]);
                            var rPositionY = Convert.ToDouble(rReader["y"]);

                            rPositions.Add(rNode, new Point(rPositionX, rPositionY));
                        }
                }

                r_ConnectionSubscription?.Dispose();
                r_ConnectionSubscription = null;
            });
        }

        public double GetAngle(int rpMapID, int rpSourceNode, int rpDestinationNode)
        {
            try
            {
                var rSourcePosition = r_Positions[rpMapID][rpSourceNode];
                var rDestinationPosition = r_Positions[rpMapID][rpDestinationNode];

                return Math.Atan2(rDestinationPosition.Y - rSourcePosition.Y, rDestinationPosition.X - rSourcePosition.X) * MathUtil.DegOf1Rad;
            }
            catch
            {
                return 0.0;
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
