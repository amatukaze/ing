using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger
{
    public abstract class LogMigrator : IIdentifiable<string>
    {
        public abstract string Id { get; }
        public abstract string Title { get; }
        public abstract bool RequireFolder { get; }

        public virtual bool SupportShipCreation => false;
        public virtual ValueTask<IReadOnlyCollection<ShipCreationEntity>> GetShipCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
            => throw new NotSupportedException();

        public virtual bool SupportEquipmentCreation => false;
        public virtual ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> GetEquipmentCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
            => throw new NotSupportedException();

        public virtual bool SupportExpeditionCompletion => false;
        public virtual ValueTask<IReadOnlyCollection<ExpeditionCompletionEntity>> GetExpeditionCompletionAsync(IFileSystemFacade source, TimeSpan timeZone)
            => throw new NotSupportedException();

        public virtual bool SupportBattleAndDrop => false;
        public virtual ValueTask<IReadOnlyCollection<BattleEntity>> GetBattleAndDropAsync(IFileSystemFacade source, TimeSpan timeZone)
            => throw new NotSupportedException();
    }
}
