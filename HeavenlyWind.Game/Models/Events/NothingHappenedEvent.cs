using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum NothingHappenedMessage { Imagination, NoSighOfTheEnemy, ManualSelection }

    public class NothingHappenedEvent : SortieEvent
    {
        public NothingHappenedMessage Message => (NothingHappenedMessage)RawData.NodeEventSubType;

        public string CurrentNode { get; }
        public IList<NodeSelection> NodeSelections { get; }

        internal NothingHappenedEvent(MapInfo rpMap, RawMapExploration rpData) : base(rpData)
        {
            if (Message == NothingHappenedMessage.ManualSelection && MapService.Instance.ContainsMap(rpMap.ID))
            {
                CurrentNode = MapService.Instance.GetNodeWikiID(rpMap.ID, rpData.StartNode ?? rpData.Node) ?? rpData.Node.ToString();
                NodeSelections = rpData.NodeSelection.Nodes.Select(r => new NodeSelection(rpMap, rpData.StartNode ?? rpData.Node, r)).ToList().AsReadOnly();
            }
        }

        public class NodeSelection
        {
            public string ID { get; }

            public double? DirectionAngle { get; }

            public NodeSelection(MapInfo rpMap, int rpCurrentNode, int rpDestinationNode)
            {
                ID = MapService.Instance.GetNodeWikiID(rpMap.ID, rpDestinationNode) ?? rpDestinationNode.ToString();
                DirectionAngle = MapService.Instance.GetAngle(rpMap.ID, rpCurrentNode, rpDestinationNode);
            }
        }
    }
}
