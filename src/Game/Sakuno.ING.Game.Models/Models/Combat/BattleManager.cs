namespace Sakuno.ING.Game.Models.Combat
{
    public partial class BattleManager : BindableObject
    {
        private readonly NavalBase navalBase;
        private Fleet sortieFleet, sortieFleet2;

        internal BattleManager(GameProvider listener, NavalBase navalBase)
        {
            this.navalBase = navalBase;

            listener.HomeportReturned += (t, m) =>
            {
                using (EnterBatchNotifyScope())
                {
                    CurrentRouting = null;
                    CurrentBattle = null;
                    sortieFleet = null;
                    sortieFleet2 = null;
                }
            };

            listener.ExerciseStarted += (t, m) =>
            {
                CurrentBattle = new Battle(this.navalBase.Fleets[m].Ships, null, CombinedFleetType.None, BattleKind.Normal);
            };

            listener.SortieStarting += (t, m) =>
            {
                sortieFleet = this.navalBase.Fleets[m.FleetId];
                if (m.FleetId == 1 && this.navalBase.CombinedFleet != CombinedFleetType.None)
                    sortieFleet2 = this.navalBase.Fleets[(FleetId)2];
            };

            listener.MapRouting += (t, m) =>
            {
                CurrentRouting = new MapRouting(this.navalBase, m);
                CurrentBattle = new Battle(sortieFleet.Ships, sortieFleet2?.Ships, this.navalBase.CombinedFleet, m.BattleKind);
            };

            listener.BattleStarted += (t, m) =>
            {
                CurrentBattle.Append(this.navalBase.MasterData, m.Parsed);
            };

            listener.BattleAppended += (t, m) =>
            {
                CurrentBattle.Append(this.navalBase.MasterData, m.Parsed);
            };

            listener.BattleCompleted += (t, m) =>
            {
                CurrentBattleResult = new BattleResult(this.navalBase.MasterData, m, CurrentBattle.Ally);
            };
        }
    }
}
