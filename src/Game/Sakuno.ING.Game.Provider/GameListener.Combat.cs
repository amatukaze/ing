using System.Collections.Specialized;
using Sakuno.ING.Game.Events.Combat;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameProvider
    {
        #region Events
        private readonly ITimedMessageProvider<EnemyDebuffConfirm> enemyDebuffConfirmed;
        public event TimedMessageHandler<EnemyDebuffConfirm> EnemyDebuffConfirmed
        {
            add => enemyDebuffConfirmed.Received += value;
            remove => enemyDebuffConfirmed.Received -= value;
        }

        private readonly ITimedMessageProvider<MapPartUnlock> mapPartUnlocked;
        public event TimedMessageHandler<MapPartUnlock> MapPartUnlocked
        {
            add => mapPartUnlocked.Received += value;
            remove => mapPartUnlocked.Received -= value;
        }

        private readonly ITimedMessageProvider<SortieStart> sortieStarting;
        public event TimedMessageHandler<SortieStart> SortieStarting
        {
            add => sortieStarting.Received += value;
            remove => sortieStarting.Received -= value;
        }

        private readonly ITimedMessageProvider<IRawMapRouting> mapRouting;
        public event TimedMessageHandler<IRawMapRouting> MapRouting
        {
            add => mapRouting.Received += value;
            remove => mapRouting.Received -= value;
        }

        private readonly ITimedMessageProvider<FleetId> practiceStarted;
        public event TimedMessageHandler<FleetId> PracticeStarted
        {
            add => practiceStarted.Received += value;
            remove => practiceStarted.Received -= value;
        }

        private readonly ITimedMessageProvider<IRawBattle> battleStarted;
        public event TimedMessageHandler<IRawBattle> BattleStarted
        {
            add => battleStarted.Received += value;
            remove => battleStarted.Received -= value;
        }

        private readonly ITimedMessageProvider<IRawBattle> battleAppended;
        public event TimedMessageHandler<IRawBattle> BattleAppended
        {
            add => battleAppended.Received += value;
            remove => battleAppended.Received -= value;
        }

        private readonly ITimedMessageProvider<IRawBattleResult> battleCompleted;
        public event TimedMessageHandler<IRawBattleResult> BattleCompleted
        {
            add => battleCompleted.Received += value;
            remove => battleCompleted.Received -= value;
        }
        #endregion

        private static SortieStart ParseSortieStart(NameValueCollection req)
            => new SortieStart
            (
                (FleetId)req.GetInt("api_deck_id"),
                (MapId)(req.GetInt("api_maparea_id") * 10 + req.GetInt("api_mapinfo_no"))
            );

        private static FleetId ParsePracticeStart(NameValueCollection req)
            => (FleetId)req.GetInt("api_deck_id");
    }
}
