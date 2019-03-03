using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public class Battle : BindableObject
    {
        public static readonly DateTimeOffset EnemyIdChangeTime = new DateTimeOffset(2017, 4, 5, 3, 0, 0, TimeSpan.FromHours(9));
        public BattleKind Kind { get; }
        public Engagement Engagement { get; }
        public Side Ally { get; }
        public Side Enemy { get; }
        public bool HasNextPart { get; }
        public Battle(MasterDataRoot masterData, IReadOnlyList<Ship> fleet, IReadOnlyList<Ship> fleet2, RawBattle raw, CombinedFleetType combined, BattleKind kind)
        {
            Kind = kind;
            Engagement = raw.Engagement;
            Ally = new Side(masterData, fleet, fleet2, raw.Ally, false);
            Enemy = new Side(masterData, null, null, raw.Enemy, true);
            HasNextPart = raw.HasNextPart;

            if (Kind == BattleKind.CombinedNightToDay)
            {
                if (raw.SupportPhase != null)
                    phases.Add(SupportPhase = new SupportPhase(raw.SupportFireType, masterData, Enemy, raw.SupportPhase));
                else if (raw.AerialSupportPhase != null)
                    phases.Add(SupportPhase = new SupportPhase(raw.SupportFireType, masterData, Enemy, raw.AerialSupportPhase));

                if (raw.NightPhase1 != null)
                    phases.Add(CombinedNightPhase1 = new NightPhase(1, masterData, Ally, Enemy, raw.NightPhase1, true));
                if (raw.NightPhase2 != null)
                    phases.Add(CombinedNightPhase2 = new NightPhase(2, masterData, Ally, Enemy, raw.NightPhase2, true));
            }

            if (raw.LandBaseJetPhase != null)
                phases.Add(LandBaseJetPhase = new AerialPhase(1, masterData, Ally, Enemy, raw.LandBaseJetPhase, true));
            if (raw.JetPhase != null)
                phases.Add(JetPhase = new AerialPhase(2, masterData, Ally, Enemy, raw.AerialPhase, true));

            if (raw.LandBasePhases != null)
                phases.AddRange(LandBasePhases = raw.LandBasePhases.Select((x, i) => new LandBasePhase(i + 1, masterData, Enemy, x)).ToArray());

            if (raw.AerialPhase != null)
                phases.Add(AerialPhase = new AerialPhase(1, masterData, Ally, Enemy, raw.AerialPhase));
            if (raw.AerialPhase2 != null)
                phases.Add(AerialPhase2 = new AerialPhase(2, masterData, Ally, Enemy, raw.AerialPhase2));

            if (Kind != BattleKind.CombinedNightToDay)
            {
                if (raw.SupportPhase != null)
                    phases.Add(SupportPhase = new SupportPhase(raw.SupportFireType, masterData, Enemy, raw.SupportPhase));
                else if (raw.AerialSupportPhase != null)
                    phases.Add(SupportPhase = new SupportPhase(raw.SupportFireType, masterData, Enemy, raw.AerialSupportPhase));

                if (raw.NightPhase != null)
                    phases.Add(NightPhase = new NightPhase(0, masterData, Ally, Enemy, raw.NightPhase, false));
            }

            if (raw.OpeningAswPhase != null)
                phases.Add(OpeningAswPhase = new ShellingPhase(0, masterData, Ally, Enemy, raw.OpeningAswPhase));
            if (raw.OpeningTorpedoPhase != null)
                phases.Add(OpeningTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.OpeningTorpedoPhase, true));

            if (Kind == BattleKind.Combined)
                switch (combined)
                {
                    case CombinedFleetType.None:
                        if (raw.ShellingPhase1 != null)
                            phases.Add(ShellingPhase1 = new ShellingPhase(1, masterData, Ally, Enemy, raw.ShellingPhase1));
                        if (raw.ClosingTorpedoPhase != null)
                            phases.Add(ClosingTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.ClosingTorpedoPhase, false));
                        if (raw.ShellingPhase2 != null)
                            phases.Add(ShellingPhase2 = new ShellingPhase(2, masterData, Ally, Enemy, raw.ShellingPhase2));
                        if (raw.ShellingPhase3 != null)
                            phases.Add(ShellingPhase3 = new ShellingPhase(3, masterData, Ally, Enemy, raw.ShellingPhase3));
                        break;
                    case CombinedFleetType.CarrierTaskForceFleet:
                    case CombinedFleetType.TransportEscortFleet:
                        if (raw.ShellingPhase1 != null)
                            phases.Add(ShellingPhase1 = new ShellingPhase(1, masterData, Ally, Enemy, raw.ShellingPhase1));
                        if (raw.ShellingPhase2 != null)
                            phases.Add(ShellingPhase2 = new ShellingPhase(2, masterData, Ally, Enemy, raw.ShellingPhase2));
                        if (raw.ClosingTorpedoPhase != null)
                            phases.Add(ClosingTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.ClosingTorpedoPhase, false));
                        if (raw.ShellingPhase3 != null)
                            phases.Add(ShellingPhase3 = new ShellingPhase(3, masterData, Ally, Enemy, raw.ShellingPhase3));
                        break;
                    case CombinedFleetType.SurfaceTaskForceFleet:
                        if (raw.ShellingPhase1 != null)
                            phases.Add(ShellingPhase1 = new ShellingPhase(1, masterData, Ally, Enemy, raw.ShellingPhase1));
                        if (raw.ShellingPhase2 != null)
                            phases.Add(ShellingPhase2 = new ShellingPhase(2, masterData, Ally, Enemy, raw.ShellingPhase2));
                        if (raw.ShellingPhase3 != null)
                            phases.Add(ShellingPhase3 = new ShellingPhase(3, masterData, Ally, Enemy, raw.ShellingPhase3));
                        if (raw.ClosingTorpedoPhase != null)
                            phases.Add(ClosingTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.ClosingTorpedoPhase, false));
                        break;
                }
            else
                switch (combined)
                {
                    case CombinedFleetType.None:
                        if (raw.ShellingPhase1 != null)
                            phases.Add(ShellingPhase1 = new ShellingPhase(1, masterData, Ally, Enemy, raw.ShellingPhase1));
                        if (raw.ShellingPhase2 != null)
                            phases.Add(ShellingPhase2 = new ShellingPhase(2, masterData, Ally, Enemy, raw.ShellingPhase2));
                        if (raw.ClosingTorpedoPhase != null)
                            phases.Add(ClosingTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.ClosingTorpedoPhase, false));
                        break;
                    case CombinedFleetType.CarrierTaskForceFleet:
                    case CombinedFleetType.TransportEscortFleet:
                        if (raw.ShellingPhase1 != null)
                            phases.Add(ShellingPhase1 = new ShellingPhase(1, masterData, Ally, Enemy, raw.ShellingPhase1, true));
                        if (raw.ClosingTorpedoPhase != null)
                            phases.Add(ClosingTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.ClosingTorpedoPhase, false));
                        if (raw.ShellingPhase2 != null)
                            phases.Add(ShellingPhase2 = new ShellingPhase(2, masterData, Ally, Enemy, raw.ShellingPhase2, false));
                        if (raw.ShellingPhase3 != null)
                            phases.Add(ShellingPhase3 = new ShellingPhase(3, masterData, Ally, Enemy, raw.ShellingPhase3, false));
                        break;
                    case CombinedFleetType.SurfaceTaskForceFleet:
                        if (raw.ShellingPhase1 != null)
                            phases.Add(ShellingPhase1 = new ShellingPhase(1, masterData, Ally, Enemy, raw.ShellingPhase1, false));
                        if (raw.ShellingPhase2 != null)
                            phases.Add(ShellingPhase2 = new ShellingPhase(2, masterData, Ally, Enemy, raw.ShellingPhase2, false));
                        if (raw.ShellingPhase3 != null)
                            phases.Add(ShellingPhase3 = new ShellingPhase(3, masterData, Ally, Enemy, raw.ShellingPhase3, true));
                        if (raw.ClosingTorpedoPhase != null)
                            phases.Add(ClosingTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.ClosingTorpedoPhase, false));
                        break;
                }

            DecideMvp(Ally.Fleet);
            DecideMvp(Ally.Fleet2);
            DecideMvp(Enemy.Fleet);
            DecideMvp(Enemy.Fleet2);
            Ally.UpdateDamageRate();
            Enemy.UpdateDamageRate();

            int allyPercentage = (int)(Ally.DamageRate * 100);
            int enemyPercentage = (int)(Enemy.DamageRate * 100);
            switch (Kind)
            {
                case BattleKind.AirDefence:
                case BattleKind.RadarDefence:
                    if (allyPercentage <= 0)
                        Rank = BattleRank.Perfect;
                    else if (allyPercentage < 10)
                        Rank = BattleRank.A;
                    else if (allyPercentage < 20)
                        Rank = BattleRank.B;
                    else if (allyPercentage < 50)
                        Rank = BattleRank.C;
                    else if (allyPercentage < 80)
                        Rank = BattleRank.D;
                    else
                        Rank = BattleRank.E;
                    break;
                default:
                    if (Ally.SunkCount == 0)
                    {
                        if (Enemy.SunkCount == Enemy.Count)
                            Rank = allyPercentage <= 0 ? BattleRank.Perfect : BattleRank.S;
                        else if (Enemy.SunkCount >= Math.Round(Enemy.Count * 0.625))
                            Rank = BattleRank.A;
                        else if (Enemy.Fleet[0].ToHP <= 0)
                            Rank = BattleRank.B;
                        else if (enemyPercentage > allyPercentage * 2.5)
                            Rank = BattleRank.B;
                        else if (enemyPercentage > allyPercentage * 0.9)
                            Rank = BattleRank.C;
                        else
                            Rank = BattleRank.D;
                    }
                    else
                    {
                        if (Enemy.SunkCount == Enemy.Count)
                            Rank = BattleRank.B;
                        else if (Enemy.Fleet[0].ToHP <= 0 && Ally.SunkCount < Enemy.SunkCount)
                            Rank = BattleRank.B;
                        else if (enemyPercentage > allyPercentage * 2.5)
                            Rank = BattleRank.B;
                        else if (enemyPercentage > allyPercentage * 0.9)
                            Rank = BattleRank.C;
                        else if (Ally.SunkCount < Math.Round(Ally.Count * 0.625))
                            Rank = BattleRank.D;
                        else
                            Rank = BattleRank.E;
                    }
                    break;
            }
        }

        private void DecideMvp(IReadOnlyList<BattleParticipant> fleet)
        {
            if (fleet is null) return;
            if (fleet.Count == 0) return;
            var mvp = fleet[0];

            foreach (var s in fleet)
            {
                s.IsMvp = false;
                if (s.DamageGiven >= mvp.DamageGiven)
                    mvp = s;
            }

            if (fleet[0].DamageGiven >= mvp.DamageGiven)
                mvp = fleet[0];
            mvp.IsMvp = true;
        }

        private readonly BindableCollection<BattlePhase> phases = new BindableCollection<BattlePhase>();
        public IBindableCollection<BattlePhase> OrderedPhases => phases;

        public ShellingPhase ShellingPhase1 { get; }
        public ShellingPhase ShellingPhase2 { get; }
        public ShellingPhase ShellingPhase3 { get; }
        public ShellingPhase OpeningAswPhase { get; }
        public TorpedoPhase OpeningTorpedoPhase { get; }
        public TorpedoPhase ClosingTorpedoPhase { get; }
        public AerialPhase AerialPhase { get; }
        public AerialPhase AerialPhase2 { get; }
        public AerialPhase LandBaseJetPhase { get; }
        public AerialPhase JetPhase { get; }
        public IReadOnlyList<LandBasePhase> LandBasePhases { get; }
        public SupportPhase SupportPhase { get; }
        public NightPhase NightPhase { get; }
        public NightPhase CombinedNightPhase1 { get; }
        public NightPhase CombinedNightPhase2 { get; }

        public BattleRank Rank { get; }
    }
}
