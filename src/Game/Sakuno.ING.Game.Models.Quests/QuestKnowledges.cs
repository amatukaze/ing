using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Models.Quests.Json;

namespace Sakuno.ING.Game.Models.Quests
{
    [Export(typeof(IQuestKnowledges))]
    public class QuestKnowledges : IQuestKnowledges
    {
        private readonly IStatePersist statePersist;
        private readonly Dictionary<QuestId, KnownQuestTarget> targets;

        private KnownQuestTarget Create(params QuestCounter[] counters)
            => new KnownQuestTarget(statePersist, counters);

        public QuestKnowledges(IStatePersist statePersist)
        {
            this.statePersist = statePersist;

            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(QuestCounterDescriptionJson), "default.json");
            targets = CreateTargetsAsync(stream).Result;
        }

        private async Task<Dictionary<QuestId, KnownQuestTarget>> CreateTargetsAsync(Stream jsonStream)
        {
            var result = new Dictionary<QuestId, KnownQuestTarget>();
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                    {
                        new MapDescriptorConverter(),
                        new BattleRankConverter()
                    },
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                var json = await JsonSerializer.DeserializeAsync<ImmutableDictionary<string, ImmutableArray<QuestCounterDescriptionJson>>>(jsonStream, options);
                foreach (var quest in json)
                {
                    var id = (QuestId)int.Parse(quest.Key);
                    var q = quest.Value;

                    var counters = q.Select((x, i) =>
                        x.Type switch
                        {
                            "repair" => (QuestCounter)new SingletonEventCounter(id, x.Count, SingletonEvent.ShipRepair, i),
                            "supply" => new SingletonEventCounter(id, x.Count, SingletonEvent.ShipSupply, i),
                            "shipConstruct" => new SingletonEventCounter(id, x.Count, SingletonEvent.ShipConstruct, i),
                            "shipDismantle" => new ShipDismantleCounter(id, x.Count, i),
                            "equipmentCreate" => new SingletonEventCounter(id, x.Count, SingletonEvent.EquipmentCreate, i),
                            "battle" => new BattleWinCounter(id, x.Count, i, x.RankRequired ?? BattleRank.B),
                            "enemySunk" => new EnemySunkCounter(id, x.Count, x.ShipType, i),
                            "boss" => new BattleBossCounter(id, x.Count, m => x.Map?.Satisfy(m) ?? true,
                                f => x.Fleet.IsDefault || x.Fleet.All(fd => fd.Satisfy(f)), x.RankRequired ?? BattleRank.B, i),
                            "sortie" => new SortieStartCounter(id, x.Count, i),
                            "exercise" => new ExerciseCounter(id, x.Count, i,
                                f => x.Fleet.IsDefault || x.Fleet.All(fd => fd.Satisfy(f)), x.RankRequired ?? BattleRank.B, x.PeriodOverride),
                            "expedition" => new ExpeditionCounter(id, x.Count, e => x.Expedition.IsDefault || x.Expedition.Contains(e), i),
                            "shipPowerup" => new ShipPowerupCounter(id, x.Count, null, i),
                            "equipmentDismantle" => (x.Equipment, x.EquipmentType) switch
                            {
                                ({ IsDefault: true }, { IsDefault: true }) => new EquipmentDismantleCounter(id, x.Count, i),
                                ({ IsDefault: false } equip, { IsDefault: true }) => new EquipmentDismantleTypedCounter(id, x.Count, i, e => equip.Contains(e.Id)),
                                ({ IsDefault: true }, { IsDefault: false } type) => new EquipmentDismantleTypedCounter(id, x.Count, i, e => type.Contains(e.Type?.Id ?? 0)),
                                _ => throw new ArgumentException("Counter parameter conflict.")
                            },
                            _ => throw new ArgumentException("Unknown counter type")
                        }).ToArray();
                    result.Add(id, Create(counters));
                }
            }
            catch
            {
                return new Dictionary<QuestId, KnownQuestTarget>();
            }
            return result;
        }

        public void Load()
        {
            foreach (var target in targets.Values)
                foreach (var c in target.Counters)
                    c.Load(statePersist);
        }

        public QuestTarget GetTargetFor(QuestId id)
        {
            targets.TryGetValue(id, out var target);
            return target;
        }

        public void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result)
        {
            foreach (var target in targets.Values)
                target.OnBattleComplete(routing, battle, result);
            statePersist.SaveChanges();
        }

        public void OnSortieStart(MapId mapId, HomeportFleet fleet1, HomeportFleet fleet2)
        {
            foreach (var target in targets.Values)
                target.OnSortieStart(mapId, fleet1, fleet2);
            statePersist.SaveChanges();
        }

        public void OnExpeditionComplete(HomeportFleet fleet, ExpeditionInfo expedition, ExpeditionResult result)
        {
            foreach (var target in targets.Values)
                target.OnExpeditionComplete(fleet, expedition, result);
            statePersist.SaveChanges();
        }

        public void OnShipPowerup(HomeportShip ship, IReadOnlyCollection<HomeportShip> consumed, bool success)
        {
            foreach (var target in targets.Values)
                target.OnShipPowerup(ship, consumed, success);
            statePersist.SaveChanges();
        }

        public void OnShipDismantle(IReadOnlyCollection<HomeportShip> ships)
        {
            foreach (var target in targets.Values)
                target.OnShipDismantle(ships);
            statePersist.SaveChanges();
        }

        public void OnEquipmentDismantle(IReadOnlyCollection<HomeportEquipment> equipment)
        {
            foreach (var target in targets.Values)
                target.OnEquipmentDismantle(equipment);
            statePersist.SaveChanges();
        }

        public void OnSingletonEvent(SingletonEvent @event)
        {
            foreach (var target in targets.Values)
                target.OnSingletonEvent(@event);
            statePersist.SaveChanges();
        }

        public void OnExerciseComplete(HomeportFleet fleet, BattleResult currentBattleResult)
        {
            foreach (var target in targets.Values)
                target.OnExerciseComplete(fleet, currentBattleResult);
            statePersist.SaveChanges();
        }
    }
}
