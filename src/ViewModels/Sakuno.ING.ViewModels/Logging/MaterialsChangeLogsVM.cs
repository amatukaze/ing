using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Timing;

namespace Sakuno.ING.ViewModels.Logging
{
    [Export(typeof(MaterialsChangeLogsVM), SingleInstance = false)]
    public class MaterialsChangeLogsVM : BindableObject
    {
        private readonly Logger logger;
        private readonly DateTimeOffset now;

        public MaterialsChangeLogsVM(Logger logger, ITimingService timing)
        {
            this.logger = logger;
            now = timing.Now;

            using var context = logger.CreateContext();

            var dailyCatalogs = new List<MaterialsDailyCatalog>();
            MaterialsChangeEntity lastDay = null;
            foreach (var (t, e) in context.MaterialsChangeTable
                .AsEnumerable()
                .GroupBy(x => x.TimeStamp.LocalDateTime.Date)
                .Select(x => (x.Key, x.Last())))
            {
                dailyCatalogs.Insert(0, new MaterialsDailyCatalog(t, e.Materials, (e.Materials - lastDay?.Materials) ?? default));
                lastDay = e;
            }
            DailyCatalogs = dailyCatalogs;
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                Update();
            }
        }

        private void Update()
        {
            using var context = logger.CreateContext();

            MaterialsChangeEntity[] entities;
            if (Duration != TimeSpan.Zero)
            {
                Start = now - Duration;
                entities = context.MaterialsChangeTable.Where(x => x.TimeStamp > Start).ToArray();
            }
            else
            {
                Start = context.MaterialsChangeTable.First().TimeStamp;
                entities = context.MaterialsChangeTable.ToArray();
            }

            if (entities.Length >= 1000)
            {
                var filtered = new List<MaterialsChangeEntity>(1000);
                // TODO: TimeSpan.operator/ exists in netstandard 2.1
                var threshold = TimeSpan.FromSeconds((entities[entities.Length - 1].TimeStamp - entities[0].TimeStamp).TotalSeconds / 1000);
                var nextTime = entities[0].TimeStamp;
                foreach (var e in entities)
                    if (e.TimeStamp >= nextTime)
                    {
                        filtered.Add(e);
                        nextTime += threshold;
                    }
                Entities = filtered;
            }
            else
                Entities = entities;
        }

        private IReadOnlyList<MaterialsChangeEntity> _entities;
        public IReadOnlyList<MaterialsChangeEntity> Entities
        {
            get => _entities;
            private set => Set(ref _entities, value);
        }

        public DateTimeOffset Start { get; private set; }

        public IReadOnlyList<MaterialsDailyCatalog> DailyCatalogs { get; }
    }

    public class MaterialsDailyCatalog
    {
        public MaterialsDailyCatalog(DateTime date, Materials materials, Materials diff)
        {
            Date = date;
            Materials = materials;
            Diff = diff;
        }

        public DateTime Date { get; }
        public Materials Materials { get; }
        public Materials Diff { get; }
    }
}
