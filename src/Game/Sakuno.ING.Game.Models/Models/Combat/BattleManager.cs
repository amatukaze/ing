namespace Sakuno.ING.Game.Models.Combat
{
    public enum BattleState
    {
        Idle,
        BeforeExercise,
        ExerciseDay,
        ExerciseNight,
        Routing,
        SortieDay,
        SortieNight,
        Completed
    }

    public partial class BattleManager : BindableObject
    {
        private readonly NavalBase navalBase;
        private Fleet sortieFleet, sortieFleet2;

        internal BattleManager(GameProvider listener, NavalBase navalBase)
        {
            this.navalBase = navalBase;

            listener.HomeportReturned += (t, m) =>
            {
                State = BattleState.Idle;
                sortieFleet = null;
                sortieFleet2 = null;
            };

            listener.ExerciseStarted += (t, m) =>
            {
                CurrentBattle = new Battle(this.navalBase.Fleets[m].Ships, null, CombinedFleetType.None, BattleKind.Normal);
                State = BattleState.BeforeExercise;
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
                State = BattleState.Routing;
            };

            listener.BattleStarted += (t, m) =>
            {
                State = State switch
                {
                    BattleState.Routing => BattleState.SortieDay,
                    BattleState.BeforeExercise => BattleState.ExerciseDay,
                    var other => other
                };
                CurrentBattle.Append(this.navalBase.MasterData, m.Parsed);
            };

            listener.BattleAppended += (t, m) =>
            {
                State = State switch
                {
                    BattleState.SortieDay => BattleState.SortieNight,
                    BattleState.ExerciseDay => BattleState.ExerciseNight,
                    var other => other
                };
                CurrentBattle.Append(this.navalBase.MasterData, m.Parsed);
            };

            listener.BattleCompleted += (t, m) =>
            {
                State = BattleState.Completed;
                CurrentBattleResult = new BattleResult(this.navalBase.MasterData, m, CurrentBattle.Ally);
            };
        }
    }
}
