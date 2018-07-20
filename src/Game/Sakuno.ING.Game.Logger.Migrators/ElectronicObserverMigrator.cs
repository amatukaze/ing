using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger.Migrators
{
    [Export(typeof(ILogMigrator))]
    internal class ElectronicObserverMigrator : ILogMigrator,
        ILogProvider<ShipCreationEntity>,
        ILogProvider<EquipmentCreationEntity>
    {
        public string Id => "74EO";
        public string Title => "七四式電子観測儀";
        public bool RequireFolder => true;

        ValueTask<IReadOnlyCollection<ShipCreationEntity>> ILogProvider<ShipCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
            => Helper.ParseCsv(source, "ConstructionRecord.csv", 13,
                s => new ShipCreationEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[2]), DateTimeKind.Local) - timeZone,
                    ShipBuilt = (ShipInfoId)int.Parse(s[0]),
                    Consumption = new Materials
                    {
                        Fuel = int.Parse(s[3]),
                        Bullet = int.Parse(s[4]),
                        Steel = int.Parse(s[5]),
                        Bauxite = int.Parse(s[6]),
                        Development = int.Parse(s[7])
                    },
                    IsLSC = int.Parse(s[8]) > 0,
                    EmptyDockCount = int.Parse(s[9]),
                    Secretary = (ShipInfoId)int.Parse(s[10]),
                    AdmiralLevel = int.Parse(s[12])
                });

        ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> ILogProvider<EquipmentCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
            => Helper.ParseCsv(source, "DevelopmentRecord.csv", 11,
                s => new EquipmentCreationEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[2]), DateTimeKind.Local) - timeZone,
                    EquipmentCreated = (EquipmentInfoId)int.Parse(s[0]),
                    Consumption = new Materials
                    {
                        Fuel = int.Parse(s[3]),
                        Bullet = int.Parse(s[4]),
                        Steel = int.Parse(s[5]),
                        Bauxite = int.Parse(s[6])
                    },
                    Secretary = (ShipInfoId)int.Parse(s[7]),
                    AdmiralLevel = int.Parse(s[10])
                });
    }
}
