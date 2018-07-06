using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger.Migrators
{
    [Export(typeof(ILogMigrator))]
    internal class LogbookMigrator : ILogMigrator,
        ILogProvider<ShipCreation>,
        ILogProvider<EquipmentCreation>,
        ILogProvider<ExpeditionCompletion>
    {
        public bool RequireFolder => true;
        public string Id => "Logbook";

        private readonly Encoding shiftJIS = CodePagesEncodingProvider.Instance.GetEncoding(932);

        private static TValue TryGetOrDefault<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key)
            => dictionary.TryGetValue(key, out TValue value) ? value : default;

        async ValueTask<IReadOnlyCollection<ShipCreation>> ILogProvider<ShipCreation>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            var folder = source as DirectoryInfo ?? throw new ArgumentException("Source must be a folder.");
            if (!folder.TryGetFile("建造報告書.csv", out var file)) return Array.Empty<ShipCreation>();

            var table = new List<ShipCreation>();
            var ships = Compositor.Static<NavalBase>().MasterData.ShipInfos.ToDictionary(x => x.Name);
            using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), shiftJIS))
            {
                await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    var s = line.Split(',');
                    if (s.Length < 12) continue;

                    var index = s[10].IndexOf('(');
                    string secretaryName = s[10].Substring(0, index);
                    string secretaryLevel = s[10].Substring(index + 3, s[10].Length - index - 4);
                    if (secretaryLevel[0] == '.') secretaryLevel = secretaryLevel.Substring(1);
                    table.Add(new ShipCreation
                    {
                        TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Local) - timeZone,
                        SecretaryLevel = int.Parse(secretaryLevel),
                        Secretary = TryGetOrDefault(ships, secretaryName)?.Id ?? default,
                        ShipBuilt = TryGetOrDefault(ships, s[2])?.Id ?? default,
                        Consumption = new MaterialsEntity
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
                    });
                }
            }
            return table;
        }

        async ValueTask<IReadOnlyCollection<EquipmentCreation>> ILogProvider<EquipmentCreation>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            var folder = source as DirectoryInfo ?? throw new ArgumentException("Source must be a folder.");
            if (!folder.TryGetFile("開発報告書.csv", out var file)) return Array.Empty<EquipmentCreation>();

            var table = new List<EquipmentCreation>();
            var ships = Compositor.Static<NavalBase>().MasterData.ShipInfos.ToDictionary(x => x.Name);
            var equipments = Compositor.Static<NavalBase>().MasterData.EquipmentInfos.ToDictionary(x => x.Name);
            using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), shiftJIS))
            {
                await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    var s = line.Split(',');
                    if (s.Length < 9) continue;

                    var index = s[7].IndexOf('(');
                    string secretaryName = s[7].Substring(0, index);
                    string secretaryLevel = s[7].Substring(index + 3, s[7].Length - index - 4);
                    if (secretaryLevel[0] == '.') secretaryLevel = secretaryLevel.Substring(1);
                    table.Add(new EquipmentCreation
                    {
                        TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Local) - timeZone,
                        SecretaryLevel = int.Parse(secretaryLevel),
                        Secretary = TryGetOrDefault(ships, secretaryName)?.Id ?? default,
                        EquipmentCreated = TryGetOrDefault(equipments, s[2])?.Id ?? default,
                        IsSuccess = s[2] != "失敗",
                        Consumption = new MaterialsEntity
                        {
                            Fuel = int.Parse(s[3]),
                            Bullet = int.Parse(s[4]),
                            Steel = int.Parse(s[5]),
                            Bauxite = int.Parse(s[6])
                        },
                        AdmiralLevel = int.Parse(s[8])
                    });
                }
            }
            return table;
        }

        async ValueTask<IReadOnlyCollection<ExpeditionCompletion>> ILogProvider<ExpeditionCompletion>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            var folder = source as DirectoryInfo ?? throw new ArgumentException("Source must be a folder.");
            if (!folder.TryGetFile("遠征報告書.csv", out var file)) return Array.Empty<ExpeditionCompletion>();

            var table = new List<ExpeditionCompletion>();
            var expeditions = Compositor.Static<NavalBase>().MasterData.Expeditions.ToDictionary(x => x.Name);
            var useitems = Compositor.Static<NavalBase>().MasterData.UseItems.ToDictionary(x => x.Name);
            using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), shiftJIS))
            {
                await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    var s = line.Split(',');
                    if (s.Length < 11) continue;

                    table.Add(new ExpeditionCompletion
                    {
                        TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Local) - timeZone,
                        Result = s[1] == "大成功" ? ExpeditionResult.GreatSuccess :
                            s[1] == "成功" ? ExpeditionResult.Success :
                            ExpeditionResult.Fail,
                        ExpeditionId = TryGetOrDefault(expeditions, s[2])?.Id ?? default,
                        MaterialsAcquired = new MaterialsEntity
                        {
                            Fuel = int.Parse(s[3]),
                            Bullet = int.Parse(s[4]),
                            Steel = int.Parse(s[5]),
                            Bauxite = int.Parse(s[6])
                        },
                        RewardItem1 = new ItemRecordEntity
                        {
                            ItemId = string.IsNullOrEmpty(s[7]) ? default : TryGetOrDefault(useitems, s[7])?.Id ?? default,
                            Count = string.IsNullOrEmpty(s[8]) ? 0 : int.Parse(s[8])
                        },
                        RewardItem2 = new ItemRecordEntity
                        {
                            ItemId = string.IsNullOrEmpty(s[9]) ? default : TryGetOrDefault(useitems, s[9])?.Id ?? default,
                            Count = string.IsNullOrEmpty(s[10]) ? 0 : int.Parse(s[10])
                        }
                    });
                }
            }
            return table;
        }
    }
}
