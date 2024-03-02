using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.ViewModels
{
    class ImprovementInfoViewModel : ModelBase
    {
        ImprovementInfo r_Model;

        public EquipmentInfo Equipment { get; }

        public ImprovementResourceConsumption MaterialConsumption => r_Model.Resources;

        public ImprovementDetailViewModel Detail { get; }

        public int TotalImprovementMaterialConsumption { get; }

        bool r_Improvable = true;
        public bool Improvable
        {
            get { return r_Improvable; }
            set
            {
                if (r_Improvable != value)
                {
                    r_Improvable = value;
                    OnPropertyChanged(nameof(Improvable));
                }
            }
        }

        bool r_IsDetailExpanded;
        public bool IsDetailExpanded
        {
            get { return r_IsDetailExpanded; }
            set
            {
                if (r_IsDetailExpanded != value)
                {
                    r_IsDetailExpanded = value;
                    OnPropertyChanged(nameof(IsDetailExpanded));
                }
            }
        }

        internal ImprovementInfoViewModel(ImprovementInfo rpRawData, ImprovementBranch branch)
        {
            r_Model = rpRawData;

            Equipment = KanColleGame.Current.MasterInfo.Equipment[rpRawData.ID];

            Detail = new ImprovementDetailViewModel(Equipment, rpRawData, branch);

            TotalImprovementMaterialConsumption = Detail.Stages.Sum(r =>
            {
                switch (r.Stage)
                {
                    case ImprovementStageType.Initial:
                        return r.ImprovementMaterialConsumption.WithoutGS * 6;

                    case ImprovementStageType.Middle:
                        return r.ImprovementMaterialConsumption.WithoutGS * 4;

                    case ImprovementStageType.Upgradable:
                        return r.ImprovementMaterialConsumption.WithoutGS;

                    default:
                        throw new InvalidOperationException();
                }
            });
        }
    }
}
