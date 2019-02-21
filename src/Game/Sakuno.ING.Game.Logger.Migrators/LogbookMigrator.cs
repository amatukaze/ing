using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger.Migrators
{
    [Export(typeof(LogMigrator))]
    internal class LogbookMigrator : LogMigrator
    {
        public override bool RequireFolder => true;
        public override string Id => "Logbook";
        public override string Title => "Logbook";

        private readonly Encoding shiftJIS = CodePagesEncodingProvider.Instance.GetEncoding(932);

        public override bool SupportShipCreation => true;
        public override ValueTask<IReadOnlyCollection<ShipCreationEntity>> GetShipCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var ships = Compositor.Static<NavalBase>().MasterData.ShipInfos.ToDictionary(x => x.Name.Origin);
            return Helper.ParseCsv(source, "建造報告書.csv", 12,
                s =>
                {
                    var index = s[10].IndexOf('(');
                    string secretaryName = s[10].Substring(0, index);
                    string secretaryLevel = s[10].Substring(index + 3, s[10].Length - index - 4);
                    if (secretaryLevel[0] == '.') secretaryLevel = secretaryLevel.Substring(1);

                    return new ShipCreationEntity
                    {
                        TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Local) - timeZone,
                        SecretaryLevel = int.Parse(secretaryLevel),
                        Secretary = ships.TryGetOrDefault(secretaryName)?.Id ?? default,
                        ShipBuilt = ships.TryGetOrDefault(s[2])?.Id ?? default,
                        Consumption = new Materials
                        {
                            Fuel = int.Parse(s[4]),
                            Bullet = int.Parse(s[5]),
                            Steel = int.Parse(s[6]),
                            Bauxite = int.Parse(s[7]),
                            Development = int.Parse(s[8])
                        },
                        IsLSC = int.Parse(s[4]) >= 1000,
                        EmptyDockCount = int.Parse(s[9]),
                        AdmiralLevel = int.Parse(s[11])
                    };
                }, shiftJIS);
        }

        public override bool SupportEquipmentCreation => true;
        public override ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> GetEquipmentCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var ships = Compositor.Static<NavalBase>().MasterData.ShipInfos.ToDictionary(x => x.Name.Origin);
            var equipments = Compositor.Static<NavalBase>().MasterData.EquipmentInfos.ToDictionary(x => x.Name.Origin);
            return Helper.ParseCsv(source, "開発報告書.csv", 9,
                s =>
                {
                    var index = s[7].IndexOf('(');
                    string secretaryName = s[7].Substring(0, index);
                    string secretaryLevel = s[7].Substring(index + 3, s[7].Length - index - 4);
                    if (secretaryLevel[0] == '.') secretaryLevel = secretaryLevel.Substring(1);

                    return new EquipmentCreationEntity
                    {
                        TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Local) - timeZone,
                        SecretaryLevel = int.Parse(secretaryLevel),
                        Secretary = ships.TryGetOrDefault(secretaryName)?.Id ?? default,
                        EquipmentCreated = equipments.TryGetOrDefault(s[2])?.Id ?? default,
                        IsSuccess = s[2] != "失敗",
                        Consumption = new Materials
                        {
                            Fuel = int.Parse(s[3]),
                            Bullet = int.Parse(s[4]),
                            Steel = int.Parse(s[5]),
                            Bauxite = int.Parse(s[6])
                        },
                        AdmiralLevel = int.Parse(s[8])
                    };
                }, shiftJIS);
        }

        public override bool SupportExpeditionCompletion => true;
        public override ValueTask<IReadOnlyCollection<ExpeditionCompletionEntity>> GetExpeditionCompletionAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var expeditions = Compositor.Static<NavalBase>().MasterData.Expeditions.ToDictionary(x => x.Name.Origin);
            var useitems = Compositor.Static<NavalBase>().MasterData.UseItems.ToDictionary(x => x.Name.Origin);
            return Helper.ParseCsv(source, "遠征報告書.csv", 11,
                s => new ExpeditionCompletionEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Local) - timeZone,
                    Result = s[1] == "大成功" ? ExpeditionResult.GreatSuccess :
                        s[1] == "成功" ? ExpeditionResult.Success :
                        ExpeditionResult.Fail,
                    ExpeditionId = expeditions.TryGetOrDefault(s[2])?.Id ?? default,
                    MaterialsAcquired = new Materials
                    {
                        Fuel = int.Parse(s[3]),
                        Bullet = int.Parse(s[4]),
                        Steel = int.Parse(s[5]),
                        Bauxite = int.Parse(s[6])
                    },
                    RewardItem1 = string.IsNullOrEmpty(s[7]) ?
                        (UseItemRecord?)null :
                        new UseItemRecord
                        {
                            ItemId = useitems.TryGetOrDefault(s[7])?.Id ?? default,
                            Count = int.Parse(s[8])
                        },
                    RewardItem2 = string.IsNullOrEmpty(s[9]) ?
                        (UseItemRecord?)null :
                        new UseItemRecord
                        {
                            ItemId = useitems.TryGetOrDefault(s[9])?.Id ?? default,
                            Count = int.Parse(s[10])
                        }
                }, shiftJIS);
        }
    }
}
