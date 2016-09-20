using System;
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

            Observable = ApiService.GetObservable("api_req_kousyou/destroyitem2")
                .Where(r => EquipmentID == null || r.Parameters["api_slotitem_ids"].Split(',').Select(rpID => GetEquipmentType(int.Parse(rpID))).Contains(EquipmentID.Value));
        }

        int GetEquipmentType(int rpID)
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT equipment FROM equipment_fate WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", rpID);

                return (int)KanColleGame.Current.MasterInfo.Equipment[Convert.ToInt32(rCommand.ExecuteScalar())].Type;
            }
        }

        public override string ToString() => "Scrapping: " + EquipmentID ?? "Any";
    }
}
