namespace Sakuno.ING.Game.Models.Combat
{
    public partial class BattleManager : BindableObject
    {
        private readonly NavalBase navalBase;
        private HomeportFleet sortieFleet, sortieFleet2, exerciseFleet;

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
                    exerciseFleet = null;
                }
            };

            listener.ExerciseStarted += (t, m) =>
            {
                exerciseFleet = this.navalBase.Fleets[m];
                CurrentBattle = new Battle(exerciseFleet, null, CombinedFleetType.None, BattleKind.Normal);
            };

            listener.SortieStarting += (t, m) =>
            {
                sortieFleet = this.navalBase.Fleets[m.FleetId];
                if (m.FleetId == 1 && this.navalBase.CombinedFleet != CombinedFleetType.None)
                    sortieFleet2 = this.navalBase.Fleets[(FleetId)2];
                navalBase.Quests.Knowledges?.OnSortieStart(m.MapId, sortieFleet, sortieFleet2);
            };

            listener.MapRouting += (t, m) =>
            {
                CurrentRouting = new MapRouting(this.navalBase, m);
                CurrentBattle = new Battle(sortieFleet, sortieFleet2, this.navalBase.CombinedFleet, m.BattleKind);
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
                if (exerciseFleet is null)
                    navalBase.Quests.Knowledges?.OnBattleComplete(CurrentRouting, CurrentBattle, CurrentBattleResult);
                else
                    navalBase.Quests.Knowledges?.OnExerciseComplete(exerciseFleet, CurrentBattleResult);
            };
        }
    }
}
