using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.ViewModels
{
    class ImprovementStageInfoViewModel : ModelBase
    {
        public ImprovementStageType Stage { get; }

        ImprovementStage r_Info;

        public ImprovementMaterialConsumption DevelopmentMaterialConsumption => r_Info.DevelopmentMaterialConsumption;

        public ImprovementMaterialConsumption ImprovementMaterialConsumption => r_Info.ImprovementMaterialConsumption;

        public ImprovementExtraConsumptionViewModel[] ExtraConsumption { get; }

        public ImprovementStageInfoViewModel(ImprovementStageType rpStage, ImprovementStage rpInfo, EquipmentInfo rpEquipment)
        {
            Stage = rpStage;
            r_Info = rpInfo;

            if (r_Info.ExtraConsumption != null)
                ExtraConsumption = r_Info.ExtraConsumption.Select(r => new ImprovementExtraConsumptionViewModel(r)).ToArray();
        }
    }
}
