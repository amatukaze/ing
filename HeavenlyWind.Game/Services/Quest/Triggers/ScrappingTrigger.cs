using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers
{
    class ScrappingTrigger : Trigger
    {
        public int? EquipmentID { get; }

        public ScrappingTrigger(int? rpEquipmentID)
        {
            EquipmentID = rpEquipmentID;

            Observable = SessionService.Instance.GetObservable("api_req_kousyou/destroyitem2")
                .Where(r => EquipmentID == null || r.Parameters["api_slotitem_ids"].Split(',').Select(rpID => (int)KanColleGame.Current.Port.Equipment[int.Parse(rpID)].Info.Type).Contains(EquipmentID.Value));
        }

        public override string ToString() => "Scrapping: " + EquipmentID ?? "Any";
    }
}
