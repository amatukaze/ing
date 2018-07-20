using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger.Migrators
{
    [Export(typeof(ILogMigrator))]
    internal class PoiMigrator : ILogMigrator,
        ILogProvider<ShipCreationEntity>,
        ILogProvider<EquipmentCreationEntity>,
        ILogProvider<ExpeditionCompletionEntity>
    {
        public string Id => "Poi";
        public string Title => "Poi";
        public bool RequireFolder => true;

        ValueTask<IReadOnlyCollection<ShipCreationEntity>> ILogProvider<ShipCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var ships = Compositor.Static<NavalBase>().MasterData.ShipInfos.ToDictionary(x => x.Name.Origin);
            return Helper.ParseCsv(source, "createship/data", 12,
                s =>
                {
                    var index = s[10].IndexOf('(');
                    string secretaryName = s[10].Substring(0, index);
                    string secretaryLevel = s[10].Substring(index + 4, s[10].Length - index - 5);

                    return new ShipCreationEntity
                    {
                        TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(s[0])),
                        ShipBuilt = ships.TryGetOrDefault(s[2])?.Id ?? default,
                        Consumption = new Materials
                        {
                            Fuel = int.Parse(s[4]),
                            Bullet = int.Parse(s[5]),
                            Steel = int.Parse(s[6]),
                            Bauxite = int.Parse(s[7]),
                            Development = int.Parse(s[8])
                        },
                        EmptyDockCount = int.Parse(s[9]),
                        Secretary = ships.TryGetOrDefault(secretaryName)?.Id ?? default,
                        SecretaryLevel = int.Parse(secretaryLevel),
                        AdmiralLevel = int.Parse(s[11])
                    };
                }, trimHeader: false);
        }

        ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> ILogProvider<EquipmentCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var ships = Compositor.Static<NavalBase>().MasterData.ShipInfos.ToDictionary(x => x.Name.Origin);
            var equipments = Compositor.Static<NavalBase>().MasterData.EquipmentInfos.ToDictionary(x => x.Name.Origin);
            return Helper.ParseCsv(source, "createitem/data", 10,
                s =>
                {
                    var index = s[8].IndexOf('(');
                    string secretaryName = s[8].Substring(0, index);
                    string secretaryLevel = s[8].Substring(index + 4, s[10].Length - index - 5);

                    return new EquipmentCreationEntity
                    {
                        TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(s[0])),
                        IsSuccess = s[1] != "失敗",
                        EquipmentCreated = equipments.TryGetOrDefault(s[2])?.Id ?? default,
                        Consumption = new Materials
                        {
                            Fuel = int.Parse(s[4]),
                            Bullet = int.Parse(s[5]),
                            Steel = int.Parse(s[6]),
                            Bauxite = int.Parse(s[7])
                        },
                        Secretary = ships.TryGetOrDefault(secretaryName)?.Id ?? default,
                        SecretaryLevel = int.Parse(secretaryLevel),
                        AdmiralLevel = int.Parse(s[9])
                    };
                }, trimHeader: false);
        }

        ValueTask<IReadOnlyCollection<ExpeditionCompletionEntity>> ILogProvider<ExpeditionCompletionEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var expeditions = Compositor.Static<NavalBase>().MasterData.Expeditions.ToDictionary(x => x.Name.Origin);
            var useitems = Compositor.Static<NavalBase>().MasterData.UseItems.ToDictionary(x => x.Name.Origin);
            return Helper.ParseCsv(source, "mission/data", 11,
                s => new ExpeditionCompletionEntity
                {
                    TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(s[0])),
                    ExpeditionId = expeditions.TryGetOrDefault(s[1])?.Id ?? default,
                    Result = s[2] == "大成功" ? ExpeditionResult.GreatSuccess :
                        s[2] == "成功" ? ExpeditionResult.Success :
                        ExpeditionResult.Fail,
                    MaterialsAcquired = new Materials
                    {
                        Fuel = int.Parse(s[3]),
                        Bullet = int.Parse(s[4]),
                        Steel = int.Parse(s[5]),
                        Bauxite = int.Parse(s[6])
                    },
                    RewardItem1 = string.IsNullOrEmpty(s[7]) ?
                        (ItemRecord?)null :
                        new ItemRecord
                        {
                            ItemId = useitems.TryGetOrDefault(s[7])?.Id ?? default,
                            Count = int.Parse(s[8])
                        },
                    RewardItem2 = string.IsNullOrEmpty(s[9]) ?
                        (ItemRecord?)null :
                        new ItemRecord
                        {
                            ItemId = useitems.TryGetOrDefault(s[9])?.Id ?? default,
                            Count = int.Parse(s[10])
                        }
                }, trimHeader: false);
        }
    }
}
