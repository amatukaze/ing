using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class DevelopmentHistoryViewModel : ModelBase, IDisposable
    {
        ObservableCollection<DevelopmentRecord> r_Records;
        public ReadOnlyObservableCollection<DevelopmentRecord> Records { get; }

        IDisposable r_NewDevelopmentSubscription;

        public DevelopmentHistoryViewModel()
        {
            r_Records = new ObservableCollection<DevelopmentRecord>();
            Records = new ReadOnlyObservableCollection<DevelopmentRecord>(r_Records);

            r_NewDevelopmentSubscription = SessionService.Instance.GetObservable("api_req_kousyou/createitem").ObserveOnDispatcher().Subscribe(r =>
            {
                var rFuelConsumption = int.Parse(r.Parameters["api_item1"]);
                var rBulletConsumption = int.Parse(r.Parameters["api_item2"]);
                var rSteelConsumption = int.Parse(r.Parameters["api_item3"]);
                var rBauxiteConsumption = int.Parse(r.Parameters["api_item4"]);
                var rData = (RawEquipmentDevelopment)r.Data;
                var rEquipmentID = rData.Success ? rData.Result.EquipmentID : (int?)null;
                var rSecretaryShip = KanColleGame.Current.Port.Fleets[1].Ships[0].Info;
                var rHeadquarterLevel = KanColleGame.Current.Port.Admiral.Level;

                r_Records.Insert(0, new DevelopmentRecord(rEquipmentID, rFuelConsumption, rBulletConsumption, rSteelConsumption, rBauxiteConsumption, rSecretaryShip, rHeadquarterLevel));
            });
        }

        public async void LoadRecords()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM development ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                    while (rReader.Read())
                        r_Records.Add(new DevelopmentRecord(rReader));
            }
        }

        public void Dispose() => r_NewDevelopmentSubscription?.Dispose();
    }
}
