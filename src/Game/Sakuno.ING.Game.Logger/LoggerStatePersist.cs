using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Data;
using Sakuno.ING.Game.Logger.Entities.Homeport;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger
{
    [Export(typeof(IStatePersist))]
    public class LoggerStatePersist : IStatePersist
    {
        private LoggerContext context;
        private readonly IDataService dataService;

        public LoggerStatePersist(IDataService dataService)
        {
            this.dataService = dataService;
        }

        private const int ID_LastHomeportUpdate = 1;
        private const int ID_LastHomeportRepair = 2;
        private const int ID_ConsumptionBeforeSortie_Fuel = 3;
        private const int ID_ConsumptionBeforeSortie_Bullet = 4;
        private const int ID_ConsumptionBeforeSortie_Steel = 5;
        private const int ID_ConsumptionBeforeSortie_Bauxite = 6;
        private const int ID_ConsumptionBeforeSortie_InstantBuild = 7;
        private const int ID_ConsumptionBeforeSortie_InstantRepair = 8;
        private const int ID_ConsumptionBeforeSortie_Development = 9;
        private const int ID_ConsumptionBeforeSortie_Improvement = 10;
        private const int ID_LastSortieFleets = 11;

        public DateTimeOffset LastHomeportUpdate
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(this[ID_LastHomeportUpdate]);
            set => this[ID_LastHomeportUpdate] = value.ToUnixTimeMilliseconds();
        }
        public DateTimeOffset LastHomeportRepair
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(this[ID_LastHomeportRepair]);
            set => this[ID_LastHomeportRepair] = value.ToUnixTimeMilliseconds();
        }
        public IReadOnlyList<FleetId> LastSortieFleets
        {
            get
            {
                var result = new List<FleetId>(2);
                long v = this[ID_LastSortieFleets];
                for (int i = 0; i < 8; i++)
                    if ((v & (1 << i)) != 0)
                        result.Add((FleetId)(i + 1));
                return result;
            }
            set
            {
                int v = 0;
                foreach (var id in value)
                    v |= 1 << (id - 1);
                this[ID_LastSortieFleets] = v;
            }
        }
        public Materials ConsumptionBeforeSortie
        {
            get => new Materials
            {
                Fuel = (int)this[ID_ConsumptionBeforeSortie_Fuel],
                Bullet = (int)this[ID_ConsumptionBeforeSortie_Bullet],
                Steel = (int)this[ID_ConsumptionBeforeSortie_Steel],
                Bauxite = (int)this[ID_ConsumptionBeforeSortie_Bauxite],
                InstantBuild = (int)this[ID_ConsumptionBeforeSortie_InstantBuild],
                InstantRepair = (int)this[ID_ConsumptionBeforeSortie_InstantRepair],
                Development = (int)this[ID_ConsumptionBeforeSortie_Development],
                Improvement = (int)this[ID_ConsumptionBeforeSortie_Improvement],
            };
            set
            {
                this[ID_ConsumptionBeforeSortie_Fuel] = value.Fuel;
                this[ID_ConsumptionBeforeSortie_Bullet] = value.Bullet;
                this[ID_ConsumptionBeforeSortie_Steel] = value.Steel;
                this[ID_ConsumptionBeforeSortie_Bauxite] = value.Bauxite;
                this[ID_ConsumptionBeforeSortie_InstantBuild] = value.InstantBuild;
                this[ID_ConsumptionBeforeSortie_InstantRepair] = value.InstantRepair;
                this[ID_ConsumptionBeforeSortie_Development] = value.Development;
                this[ID_ConsumptionBeforeSortie_Improvement] = value.Improvement;
            }
        }

        private long this[int id]
        {
            get => context?.HomeportStateTable.Find(id)?.Value ?? 0;
            set
            {
                var entity = context.HomeportStateTable.Find(id);
                if (entity is null)
                    context.HomeportStateTable.Add(new HomeportStateEntity
                    {
                        Id = id,
                        Value = value
                    });
                else
                {
                    entity.Value = value;
                    context.HomeportStateTable.Update(entity);
                }
            }
        }

        public void Initialize(int admiralId)
        {
            context?.Dispose();
            context = new LoggerContext(dataService.ConfigureDbContext<LoggerContext>(admiralId.ToString(), "logs"));
        }

        public void SaveChanges() => context.SaveChanges();

        public void SetQuestProgress(QuestId questId, int counterId, int value)
        {
            var entity = context.QuestProcessTable.Find(questId, counterId);
            if (entity is null)
                context.QuestProcessTable.Add(new QuestProcessEntity
                {
                    QuestId = questId,
                    CounterId = counterId,
                    Value = value
                });
            else
            {
                entity.Value = value;
                context.QuestProcessTable.Update(entity);
            }
        }
        public int? GetQuestProgress(QuestId questId, int counterId)
            => context.QuestProcessTable.Find(questId, counterId)?.Value;
        public void ClearQuestProgress(QuestId questId)
            => context.QuestProcessTable.RemoveRange(context.QuestProcessTable.Where(x => x.QuestId == questId));

        public void SetLastSortie(ShipId id, DateTimeOffset timeStamp)
        {
            var entity = context.ShipSortieStateTable.Find(id);
            if (entity is null)
                context.ShipSortieStateTable.Add(new ShipSortieStateEntity
                {
                    Id = id,
                    LastSortie = timeStamp
                });
            else
            {
                entity.LastSortie = timeStamp;
                context.ShipSortieStateTable.Update(entity);
            }
        }
        public DateTimeOffset? GetLastSortie(ShipId id)
            => context.ShipSortieStateTable.Find(id)?.LastSortie;
        public void ClearLastSortie(ShipId id)
            => context.ShipSortieStateTable.RemoveRange(context.ShipSortieStateTable.Where(x => x.Id == id));
    }
}
