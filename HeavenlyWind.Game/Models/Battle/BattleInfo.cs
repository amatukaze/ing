using Sakuno.KanColle.Amatsukaze.Game.Models.Battle.Stages;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class BattleInfo : ModelBase
    {
        internal static BattleInfo Current { get; private set; }

        public long ID { get; } = (long)DateTimeUtil.ToUnixTime(DateTimeOffset.Now);

        public bool IsInitialized { get; private set; }

        public BattleParticipants Participants { get; } = new BattleParticipants();

        public BattleStage CurrentStage { get; private set; }
        public BattleStage First { get; private set; }
        public BattleStage Second { get; private set; }

        public Formation FriendFormation { get; private set; }
        public Formation EnemyFormation { get; private set; }
        public EngagementForm EngagementForm { get; private set; }

        public AerialCombat AerialCombat { get; } = new AerialCombat();

        public BattleResult Result { get; } = new BattleResult();

        public bool IsSupportFleetReady { get; private set; }

        static BattleInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => Current = null);

            var rFirstStages = new[]
            {
                "api_req_sortie/battle",
                "api_req_battle_midnight/sp_midnight",
                "api_req_sortie/airbattle",
                "api_req_sortie/ld_airbattle",
                "api_req_combined_battle/airbattle",
                "api_req_combined_battle/battle",
                "api_req_combined_battle/battle_water",
                "api_req_combined_battle/sp_midnight",
                "api_req_combined_battle/ld_airbattle",

                "api_req_practice/battle",
            };
            SessionService.Instance.Subscribe(rFirstStages, r => Current?.ProcessFirstStage(r));

            var rSecondStages = new[]
            {
                "api_req_battle_midnight/battle",
                "api_req_combined_battle/midnight_battle",

                "api_req_practice/midnight_battle",
            };
            SessionService.Instance.Subscribe(rSecondStages, r => Current?.ProcessSecondStage(r));
        }
        internal BattleInfo(BattleType rpBattleType, bool rpIsBossBattle)
        {
            Current = this;

            var rSortie = KanColleGame.Current.Sortie;
            Participants.FriendMain = rSortie.MainShips;
            Participants.FriendEscort = rSortie.EscortShips;

            CurrentStage = new FakeStage(this);
            OnPropertyChanged(nameof(CurrentStage));

            if (rpBattleType == BattleType.Normal)
            {
                var rSupportFleets = KanColleGame.Current.Port.Fleets.Table.Values
                    .Where(r => r.ExpeditionStatus.Expedition != null && !r.ExpeditionStatus.Expedition.CanReturn)
                    .Select(r => r.ExpeditionStatus.Expedition)
                    .SingleOrDefault(r => r.MapArea.ID == rSortie.Map.ID / 10 && r.Time == (!rpIsBossBattle ? 15 : 30));

                IsSupportFleetReady = rSupportFleets != null;
                OnPropertyChanged(nameof(IsSupportFleetReady));
            }
        }
        internal BattleInfo(Fleet rpParticipantFleet)
        {
            Current = this;

            Participants.FriendMain = rpParticipantFleet.Ships.Select(r => new FriendShip(r)).ToList<IParticipant>();
        }

        void ProcessFirstStage(ApiData rpData)
        {
            SetEnemy((RawBattleBase)rpData.Data);
            SetFormationAndEngagementForm(rpData);

            switch (rpData.Api)
            {
                case "api_req_sortie/battle":
                case "api_req_practice/battle":
                    First = new DayNormalStage(this, rpData);
                    break;

                case "api_req_battle_midnight/sp_midnight": First = new NightOnlyStage(this, rpData); break;

                case "api_req_sortie/airbattle":
                case "api_req_combined_battle/airbattle":
                    First = new AerialCombatStage(this, rpData);
                    break;

                case "api_req_sortie/ld_airbattle":
                case "api_req_combined_battle/ld_airbattle":
                    First = new AerialAttackStage(this, rpData);
                    break;

                case "api_req_combined_battle/battle": First = new CombinedFleetCTFDayNormalStage(this, rpData); break;
                case "api_req_combined_battle/battle_water": First = new CombinedFleetSTFDayNormalStage(this, rpData); break;

                case "api_req_combined_battle/sp_midnight": First = new CombinedFleetNightOnlyStage(this, rpData); break;
            }

            First.Process(rpData);
            First.ProcessMVP();
            Result.Update(First);

            IsInitialized = true;
            IsSupportFleetReady = false;

            CurrentStage = First;
            OnPropertyChanged(nameof(First));
            OnPropertyChanged(nameof(CurrentStage));
            OnPropertyChanged(nameof(AerialCombat));
            OnPropertyChanged(nameof(IsInitialized));
            OnPropertyChanged(nameof(IsSupportFleetReady));
        }
        void SetEnemy(RawBattleBase rpData)
        {
            Participants.Enemy = rpData.EnemyShipTypeIDs.Skip(1).TakeWhile(r => r != -1).Select((r, i) =>
            {
                var rLevel = rpData.EnemyShipLevels[i + 1];
                var rEquipment = rpData.EnemyEquipment[i];

                return new EnemyShip(r, rLevel, rEquipment);
            }).ToList<IParticipant>().AsReadOnly();
        }
        void SetFormationAndEngagementForm(ApiData rpData)
        {
            var rFormationRawData = rpData.Data as IRawFormationAndEngagementForm;

            FriendFormation = (Formation)rFormationRawData.FormationAndEngagementForm[0];
            EnemyFormation = (Formation)rFormationRawData.FormationAndEngagementForm[1];
            EngagementForm = (EngagementForm)rFormationRawData.FormationAndEngagementForm[2];

            OnPropertyChanged(nameof(FriendFormation));
            OnPropertyChanged(nameof(EnemyFormation));
            OnPropertyChanged(nameof(EngagementForm));
        }

        void ProcessSecondStage(ApiData rpData)
        {
            switch (rpData.Api)
            {
                case "api_req_battle_midnight/battle":
                case "api_req_practice/midnight_battle":
                    Second = new NightNormalStage(this, rpData);
                    break;

                case "api_req_combined_battle/midnight_battle": Second = new CombinedFleetNightNormalStage(this, rpData); break;
            }

            Second.Process(rpData);
            InheritFromPreviousStage(Second);
            Second.ProcessMVP();
            Result.Update(Second);

            CurrentStage = Second;
            OnPropertyChanged(nameof(Second));
            OnPropertyChanged(nameof(CurrentStage));
        }
        void InheritFromPreviousStage(BattleStage rpStage)
        {
            if (rpStage.FriendEscort == null)
                for (var i = 0; i < rpStage.FriendMain.Count; i++)
                {
                    rpStage.FriendMain[i].DamageGivenToOpponent += CurrentStage.FriendMain[i].DamageGivenToOpponent;
                    rpStage.FriendMain[i].Inaccurate = CurrentStage.FriendMain[i].Inaccurate;
                }

            for (var i = 0; i < rpStage.Enemy.Count; i++)
                rpStage.Enemy[i].DamageGivenToOpponent += CurrentStage.Enemy[i].DamageGivenToOpponent;

            if (rpStage.FriendEscort != null)
                for (var i = 0; i < rpStage.FriendEscort.Count; i++)
                {
                    rpStage.FriendEscort[i].DamageGivenToOpponent += CurrentStage.FriendEscort[i].DamageGivenToOpponent;
                    rpStage.FriendEscort[i].Inaccurate = CurrentStage.FriendEscort[i].Inaccurate;
                }
        }
    }
}
