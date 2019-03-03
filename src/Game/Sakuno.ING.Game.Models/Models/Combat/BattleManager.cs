namespace Sakuno.ING.Game.Models.Combat
{
    public enum BattleState
    {
        Idle,
        BeforePractice,
        PracticeDay,
        PracticeNight,
        Routing,
        SortieDay,
        SortieNight,
        Completed
    }

    public class BattleManager : BindableObject
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

            listener.PracticeStarted += (t, m) =>
            {
                CurrentBattle = new Battle(this.navalBase.Fleets[m].Ships, null, CombinedFleetType.None, BattleKind.Normal);
                State = BattleState.BeforePractice;
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
                if (State == BattleState.Routing)
                    State = BattleState.SortieDay;
                else if (State == BattleState.BeforePractice)
                    State = BattleState.PracticeDay;
                CurrentBattle.Append(this.navalBase.MasterData, m.Parsed);
            };

            listener.BattleAppended += (t, m) =>
            {
                if (State == BattleState.SortieDay)
                    State = BattleState.SortieNight;
                else if (State == BattleState.PracticeDay)
                    State = BattleState.PracticeNight;
                CurrentBattle.Append(this.navalBase.MasterData, m.Parsed);
            };

            listener.BattleCompleted += (t, m) =>
            {

            };
        }

        private BattleState _state;
        public BattleState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        private MapRouting _currentRouting;
        public MapRouting CurrentRouting
        {
            get => _currentRouting;
            set => Set(ref _currentRouting, value);
        }

        private Battle _currentBattle;
        public Battle CurrentBattle
        {
            get => _currentBattle;
            set => Set(ref _currentBattle, value);
        }
    }
}
