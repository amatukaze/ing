using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    class ConstructionHistoryViewModel : ModelBase, IDisposable
    {
        ObservableCollection<ConstructionRecord> r_Records;
        public ReadOnlyObservableCollection<ConstructionRecord> Records { get; }

        IDisposable r_NewConstructionSubscription;

        public ConstructionHistoryViewModel()
        {
            r_Records = new ObservableCollection<ConstructionRecord>();
            Records = new ReadOnlyObservableCollection<ConstructionRecord>(r_Records);

            r_NewConstructionSubscription = ConstructionDock.NewConstruction.ObserveOnDispatcher().Subscribe(r =>
            {
                var rPort = KanColleGame.Current.Port;
                var rShip = r.Ship;
                var rSecretaryShip = rPort.Fleets[1].Ships[0].Info;
                var rHeadquarterLevel = rPort.Admiral.Level;
                var rEmptyDockCount = !r.IsLargeShipConstruction.Value ? (int?)null : rPort.ConstructionDocks.Values.Count(rpDock => rpDock.State == ConstructionDockState.Idle);

                r_Records.Add(new ConstructionRecord(rShip, r.FuelConsumption, r.BulletConsumption, r.SteelConsumption, r.BauxiteConsumption, r.DevelopmentMaterialConsumption, rSecretaryShip, rHeadquarterLevel, rEmptyDockCount));
            });
        }

        public async void LoadRecords()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT * FROM construction ORDER BY time DESC;";

                using (var rReader = await rCommand.ExecuteReaderAsync())
                    while (rReader.Read())
                        r_Records.Add(new ConstructionRecord(rReader));
            }
        }

        public void Dispose() => r_NewConstructionSubscription?.Dispose();
    }
}
