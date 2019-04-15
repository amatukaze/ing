using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public partial class Battle : BindableObject
    {
        public BattleKind Kind { get; }
        public CombinedFleetType FleetType { get; }
        public Side Ally { get; }

        public Battle(IReadOnlyList<Ship> fleet, IReadOnlyList<Ship> fleet2, CombinedFleetType fleetType, BattleKind kind)
        {
            Kind = kind;
            FleetType = fleetType;
            Ally = new Side(fleet, fleet2);
            Incomplete = true;
        }

        public void Append(MasterDataRoot masterData, RawBattle raw)
        {
            using (EnterBatchNotifyScope())
            {
                HasStarted = true;
                Engagement = raw.Engagement;
                Ally.Load(raw.Ally);
                if (Enemy is null)
                    Enemy = new Side(masterData, raw.Enemy, true);
                else
                    Enemy.Load(raw.Enemy);
                Incomplete = raw.HasNextPart;

                if (Kind == BattleKind.CombinedNightToDay)
                {
                    if (raw.SupportPhase != null)
                        phases.Add(SupportPhase = new SupportPhase(raw.SupportFireType, masterData, Enemy, raw.SupportPhase));
                    else if (raw.AerialSupportPhase != null)
                        phases.Add(SupportPhase = new SupportPhase(raw.SupportFireType, masterData, Enemy, raw.AerialSupportPhase));

                    if (raw.NightPhase1 != null)
                        phases.Add(CombinedNightPhase1 = new CombinedNightPhase(1, masterData, Ally.Fleet, Enemy, raw.NightPhase1));
                    if (raw.NightPhase2 != null)
                        phases.Add(CombinedNightPhase2 = new CombinedNightPhase(2, masterData, Ally.Fleet, Enemy, raw.NightPhase2));
                }

                if (raw.LandBaseJetPhase != null)
                    phases.Add(LandBaseJetPhase = new JetPhase(masterData, Ally, Enemy, raw.LandBaseJetPhase, true));
                if (raw.JetPhase != null)
                    phases.Add(JetPhase = new JetPhase(masterData, Ally, Enemy, raw.AerialPhase, false));

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

                    if (raw.NpcFleet != null)
                    {
                        NpcFleet = raw.NpcFleet.Select((s, i) => new BattleParticipant(i + 1, new BattlingShip(masterData, s), s, false)).ToArray();
                        if (raw.NpcPhase != null)
                            NpcPhase = new NpcPhase(masterData, NpcFleet, Enemy, raw.NpcPhase);
                    }
                    if (raw.NightPhase != null)
                        phases.Add(NightPhase = new NightPhase(masterData, Ally, Enemy, raw.NightPhase));
                }

                if (raw.OpeningAswPhase != null)
                    phases.Add(OpeningAswPhase = new OpeningAswPhase(masterData, Ally, Enemy, raw.OpeningAswPhase));
                if (raw.OpeningTorpedoPhase != null)
                    phases.Add(OpeningTorpedoPhase = new TorpedoPhase(masterData, Ally, Enemy, raw.OpeningTorpedoPhase, true));

                if (Kind == BattleKind.Combined)
                    switch (FleetType)
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
                    switch (FleetType)
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

                CompletePart(Ally.Fleet);
                CompletePart(Ally.Fleet2);
                CompletePart(Enemy.Fleet);
                CompletePart(Enemy.Fleet2);
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
                            else if (Enemy.Fleet[0].IsSunk)
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
                            else if (Enemy.Fleet[0].IsSunk && Ally.SunkCount < Enemy.SunkCount)
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
        }

        private void CompletePart(IReadOnlyList<BattleParticipant> fleet)
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

            foreach (var s in fleet)
                s.Notify();
        }

        private readonly BindableCollection<BattlePhase> phases = new BindableCollection<BattlePhase>();
        public IBindableCollection<BattlePhase> OrderedPhases => phases;
    }
}
