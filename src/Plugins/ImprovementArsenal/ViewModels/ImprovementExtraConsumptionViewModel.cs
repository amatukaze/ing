using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.ViewModels
{
    class ImprovementExtraConsumptionViewModel : ModelBase
    {
        ImprovementExtraConsumption _info;

        public ExtraConsumptionType Type => _info.Type;

        public EquipmentInfo Equipment { get; }
        public ItemInfo Item { get; }

        public int Count => _info.Count;

        public ImprovementExtraConsumptionViewModel(ImprovementExtraConsumption info)
        {
            _info = info;

            switch (Type)
            {
                case ExtraConsumptionType.Equipment:
                    Equipment = KanColleGame.Current.MasterInfo.Equipment[_info.ID];
                    break;

                case ExtraConsumptionType.Item:
                    Item = KanColleGame.Current.MasterInfo.Items[_info.ID];
                    break;
            }
        }
    }
}
