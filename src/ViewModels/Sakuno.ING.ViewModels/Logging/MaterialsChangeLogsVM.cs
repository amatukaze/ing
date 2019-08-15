using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Entities;
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
            if (Duration != TimeSpan.Zero)
            {
                Start = now - Duration;
                Entities = context.MaterialsChangeTable.Where(x => x.TimeStamp > Start).ToArray();
            }
            else
            {
                Start = context.MaterialsChangeTable.First().TimeStamp;
                Entities = context.MaterialsChangeTable.ToArray();
            }
        }

        private IReadOnlyList<MaterialsChangeEntity> _entities;
        public IReadOnlyList<MaterialsChangeEntity> Entities
        {
            get => _entities;
            private set => Set(ref _entities, value);
        }

        public DateTimeOffset Start { get; set; }
    }
}
