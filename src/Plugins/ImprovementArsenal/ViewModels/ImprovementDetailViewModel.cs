using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.ViewModels
{
    class ImprovementDetailViewModel : ModelBase
    {
        public EquipmentInfo UpgradeTo { get; }
        public int UpgradedLevel { get; }

        public ImprovementStageInfoViewModel[] Stages { get; }

        public ImprovementSecondShipInfoViewModel[] SecondShips { get; }

        public ImprovementDetailViewModel(EquipmentInfo rpEquipment, ImprovementInfo info, ImprovementBranch branch)
        {
            if (branch != null && branch.Destination != 0)
            {
                UpgradeTo = KanColleGame.Current.MasterInfo.Equipment[branch.Destination];
                UpgradedLevel = branch.Level;
            }

            var rFirstStage = new ImprovementStageInfoViewModel(ImprovementStageType.Initial, info.InitialStage, rpEquipment);
            var rSecondStage = new ImprovementStageInfoViewModel(ImprovementStageType.Middle, info.MiddleStage, rpEquipment);

            if (branch == null || branch.Destination == 0)
                Stages = new[] { rFirstStage, rSecondStage };
            else
                Stages = new[] { rFirstStage, rSecondStage, new ImprovementStageInfoViewModel(ImprovementStageType.Upgradable, branch, rpEquipment) };

            var assistants = info.Assistants ?? branch?.Assistants/* ?? throw new FormatException()*/;

            SecondShips = assistants?.Select(r => new ImprovementSecondShipInfoViewModel(r.ID, r.Days)).ToArray();
        }

        public void Update(DayOfWeek rpDay)
        {
            foreach (var rSecondShip in SecondShips)
                rSecondShip.IsVisible = rSecondShip.Days[(int)rpDay].Improvable;
        }
    }
}
