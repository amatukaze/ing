using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sakuno.ING.Game.Events.Combat;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.Combat;
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

        private readonly ITimedMessageProvider<RawMapRouting> mapRouting;
        public event TimedMessageHandler<RawMapRouting> MapRouting
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

        private readonly ITimedMessageProvider<BattleDetail> battleStarted;
        public event TimedMessageHandler<BattleDetail> BattleStarted
        {
            add => battleStarted.Received += value;
            remove => battleStarted.Received -= value;
        }

        private readonly ITimedMessageProvider<BattleDetail> battleAppended;
        public event TimedMessageHandler<BattleDetail> BattleAppended
        {
            add => battleAppended.Received += value;
            remove => battleAppended.Received -= value;
        }

        private readonly ITimedMessageProvider<RawBattleResult> battleCompleted;
        public event TimedMessageHandler<RawBattleResult> BattleCompleted
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

        private ITimedMessageProvider<BattleDetail> RegisterBattleDetail(params string[] apis)
            => provider.Where(arg => apis.Contains(arg.Key))
                .Select(arg => System.Text.Encoding.UTF8.GetBytes(arg.Response.ToArray()))
                .Select(m => (jSerializer.Deserialize<SvData<BattleApi>>(new JsonTextReader(new StreamReader(new MemoryStream(m)))), m))
                .Where(((SvData<BattleApi> svdata, byte[] memory) x) => x.svdata.api_result == 1)
                .Select(((SvData<BattleApi> svdata, byte[] memory) x) => new BattleDetail(new RawBattle(x.svdata.api_data), x.memory));
    }
}
