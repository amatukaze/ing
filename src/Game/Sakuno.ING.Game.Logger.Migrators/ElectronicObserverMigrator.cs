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
    [Export(typeof(LogMigrator))]
    internal class ElectronicObserverMigrator : LogMigrator
    {
        public override string Id => "74EO";
        public override string Title => "七四式電子観測儀";
        public override bool RequireFolder => true;

        public override bool SupportShipCreation => true;
        public override ValueTask<IReadOnlyCollection<ShipCreationEntity>> GetShipCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
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

        public override bool SupportEquipmentCreation => true;
        public override ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> GetEquipmentCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
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
