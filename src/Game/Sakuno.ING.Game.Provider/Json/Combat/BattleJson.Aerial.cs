using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Combat
{
    partial class BattleJson
    {
        public class Aerial : IRawAerialPhase
        {
            public class Stage1
            {
                public int api_f_count;
                public int api_e_count;
                public int api_f_lostcount;
                public int api_e_lostcount;
                public AirFightingResult api_disp_seiku;
                public EquipmentInfoId?[] api_touch_plane;
            }
            public Stage1 api_stage1 = new Stage1();
            public AirFightingResult FightingResult => api_stage1.api_disp_seiku;

            public class Stage2
            {
                public int api_f_count;
                public int api_e_count;
                public int api_f_lostcount;
                public int api_e_lostcount;
                public class AAFire : IRawAntiAirFire
                {
                    [JsonProperty("api_idx")]
                    public int Index { get; set; }
                    [JsonProperty("api_kind")]
                    public int Type { get; set; }
                    [JsonProperty("api_use_items")]
                    public IReadOnlyList<EquipmentInfoId> EquipmentUsed { get; set; }
                }
                public AAFire api_air_fire;
            }
            public Stage2 api_stage2 = new Stage2();
            public IRawAntiAirFire AntiAirFire => api_stage2.api_air_fire;

            public class AllySide : IRawAerialSide
            {
                private readonly Aerial owner;
                public AllySide(Aerial owner) => this.owner = owner;

                public ClampedValue FightedPlanes => ClampedValue.FromShortage(owner.api_stage1.api_f_count, owner.api_stage1.api_f_lostcount);
                public ClampedValue ShootedPlanes => ClampedValue.FromShortage(owner.api_stage2.api_f_count, owner.api_stage2.api_f_lostcount);
                public EquipmentInfoId? TouchingPlane => owner.api_stage1.api_touch_plane.ElementAtOrDefault(0);
                public IRawAntiAirFire AntiAirFire => owner.api_stage2.api_air_fire;
            }
            public class EnemySide : IRawAerialSide
            {
                private readonly Aerial owner;
                public EnemySide(Aerial owner) => this.owner = owner;

                public ClampedValue FightedPlanes => ClampedValue.FromShortage(owner.api_stage1.api_e_count, owner.api_stage1.api_e_lostcount);
                public ClampedValue ShootedPlanes => ClampedValue.FromShortage(owner.api_stage2.api_e_count, owner.api_stage2.api_e_lostcount);
                public EquipmentInfoId? TouchingPlane => owner.api_stage1.api_touch_plane.ElementAtOrDefault(1);
                public IRawAntiAirFire AntiAirFire => null;
            }

            public IRawAerialSide Ally { get; }
            public IRawAerialSide Enemy { get; }

            public Aerial()
            {
                Ally = new AllySide(this);
                Enemy = new EnemySide(this);
            }

            public class Stage3
            {
#pragma warning disable IDE1006 // Naming Styles
                public bool[] api_frai_flag { set => value.AlignSet(ally, (r, v) => r.torpedo = v); }
                public bool[] api_erai_flag { set => value.AlignSet(enemy, (r, v) => r.torpedo = v); }
                public bool[] api_fbak_flag { set => value.AlignSet(ally, (r, v) => r.diving = v); }
                public bool[] api_ebak_flag { set => value.AlignSet(enemy, (r, v) => r.diving = v); }
                public int[] api_fcl_flag { set => value.AlignSet(ally, (r, v) => r.critical = v == 2); }
                public int[] api_ecl_flag { set => value.AlignSet(enemy, (r, v) => r.critical = v == 2); }
                public double[] api_fdam { set => value.AlignSet(ally, (r, v) => r.damage = v); }
                public double[] api_edam { set => value.AlignSet(enemy, (r, v) => r.damage = v); }
#pragma warning restore IDE1006 // Naming Styles

                public readonly List<PartialHit> ally = new List<PartialHit>();
                public readonly List<PartialHit> enemy = new List<PartialHit>();

                public IEnumerable<SingleAttack> GetAttacks(int baseIndex)
                    => ally.Skip(1).Select((x, i) => new SingleAttack
                    {
                        IsEnemy = true,
                        Type = (x.torpedo ? 1 : 0) | (x.diving ? 2 : 0),
                        Hit = new Hit
                        {
                            TargetIndex = i + baseIndex,
                            IsCritical = x.critical,
                            damage = x.damage
                        }
                    }).Concat(enemy.Skip(1).Select((x, i) => new SingleAttack
                    {
                        Type = (x.torpedo ? 1 : 0) | (x.diving ? 2 : 0),
                        Hit = new Hit
                        {
                            TargetIndex = i + baseIndex,
                            IsCritical = x.critical,
                            damage = x.damage
                        }
                    }));
            }
            public Stage3 api_stage3 = new Stage3();
            public Stage3 api_stage3_combined = new Stage3();

            public IReadOnlyList<IRawAttack> Attacks
                => api_stage3.GetAttacks(0)
                    .Concat(api_stage3_combined.GetAttacks(6))
                    .Where(x => x.Type > 0)
                    .ToArray();
        }

        public Aerial api_kouku;
        public IRawAerialPhase AerialPhase { get; }

        public Aerial api_kouku2;
        public IRawAerialPhase AerialPhase2 { get; }

        public Aerial api_injection_kouku;
        public IRawAerialPhase JetPhase { get; }

        public Aerial api_air_base_injection;
        public IRawAerialPhase LandBaseJetPhase { get; }

        public class LandBase : Aerial, IRawLandBaseAerialPhase
        {
            public class Squadron
            {
                public EquipmentInfoId api_mst_id;
                public int api_count;
            }
            public Squadron[] api_squadron_plane;
            public IReadOnlyList<EquipmentRecord> Squadrons
                => api_squadron_plane?.Select(x => new EquipmentRecord
                {
                    Id = x.api_mst_id,
                    Count = x.api_count
                }).ToArray() ?? Array.Empty<EquipmentRecord>();
        }
        public LandBase[] api_air_base_attack;
        public IReadOnlyList<IRawLandBaseAerialPhase> LandBasePhases
            => api_air_base_attack ?? Array.Empty<IRawLandBaseAerialPhase>();
    }
}
