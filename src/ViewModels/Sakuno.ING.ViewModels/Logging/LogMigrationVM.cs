using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.IO;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;

namespace Sakuno.ING.ViewModels.Logging
{
    [Export(typeof(LogMigrationVM), SingleInstance = false)]
    public class LogMigrationVM : BindableObject
    {
        private readonly Logger logger;
        private readonly IShellContextService shellContextService;
        private readonly ILocalizationService localization;

        public IBindableCollection<LogMigrator> Migrators { get; }

        private LogMigrator _selectedMigrator;
        public LogMigrator SelectedMigrator
        {
            get => _selectedMigrator;
            set
            {
                if (_selectedMigrator != value)
                {
                    _selectedMigrator = value;
                    SelectedPath = null;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(SelectedMigrator));
                }
            }
        }

        public bool Ready
            => SelectedMigrator != null
            && SelectedPath != null;

        private IFileSystemFacade _selectedFs;
        public IFileSystemFacade SelectedPath
        {
            get => _selectedFs;
            set
            {
                if (_selectedFs != value)
                {
                    _selectedFs = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(Ready));
                }
            }
        }

        public bool SelectShipCreation { get; set; }

        public bool SelectEquipmentCreation { get; set; }

        public bool SelectExpeditionCompletion { get; set; }

        public bool SelectBattleAndDrop { get; set; }

        public bool SelectMaterialsChange { get; set; }

        public LogMigrationVM(Logger logger, LogMigrator[] migrators, IShellContextService shellContextService, ILocalizationService localization)
        {
            this.logger = logger;
            this.shellContextService = shellContextService;
            this.localization = localization;
            Migrators = migrators.ToBindable();
        }

        private bool _ranged;
        public bool Ranged
        {
            get => _ranged;
            set => Set(ref _ranged, value);
        }

        public double TimeZoneOffset { get; set; }
        public DateTime DateFrom = DateTime.Now;
        public DateTime DateTo = DateTime.Now;

        private bool _running;
        public bool Running
        {
            get => _running;
            private set => Set(ref _running, value);
        }

        public async void PickPath()
        {
            IFileSystemFacade fs;
            if (SelectedMigrator.RequireFolder)
                fs = await shellContextService.Capture().PickFolderAsync();
            else
                fs = await shellContextService.Capture().OpenFileAsync();

            if (fs != null)
                SelectedPath = fs;
        }

        private async ValueTask<int> TryMigrate<T>(DbSet<T> dbSet, IReadOnlyCollection<T> logs, string id)
            where T : EntityBase
        {
            var timeZone = TimeSpan.FromHours(TimeZoneOffset);
            IEnumerable<T> source = logs;
            if (Ranged)
            {
                var timeFrom = DateTime.SpecifyKind(DateFrom, DateTimeKind.Utc) - timeZone;
                var timeTo = DateTime.SpecifyKind(DateTo, DateTimeKind.Utc) - timeZone;
                source = source.Where(e => e.TimeStamp > timeFrom && e.TimeStamp < timeTo);
            }

            int count = 0;
            var index = new HashSet<long>(dbSet.Select(e => e.TimeStamp.ToUnixTimeSeconds()));
            foreach (T e in source)
            {
                long time = e.TimeStamp.ToUnixTimeSeconds();
                e.Source = id;
                if (!index.Contains(time))
                {
                    await dbSet.AddAsync(e);
                    index.Add(time);
                    count++;
                }
            }
            return count;
        }

        public async void DoMigration()
        {
            var shellContext = shellContextService.Capture();

            if (!logger.PlayerLoaded)
            {
                await shellContext.ShowMessageAsync(localization.GetLocalized("Logging", "PlayerNotLoaded"),
                    localization.GetLocalized("Logging", "CannotMigrate"));
                return;
            }

            Running = true;

            try
            {
                int count = 0;
                using (var context = logger.CreateContext())
                {
                    var timeZone = TimeSpan.FromHours(TimeZoneOffset);
                    if (SelectShipCreation && SelectedMigrator.SupportShipCreation)
                        count += await TryMigrate(context.ShipCreationTable, await SelectedMigrator.GetShipCreationAsync(SelectedPath, timeZone).ConfigureAwait(false), SelectedMigrator.Id)
                            .ConfigureAwait(false);
                    if (SelectEquipmentCreation && SelectedMigrator.SupportEquipmentCreation)
                        count += await TryMigrate(context.EquipmentCreationTable, await SelectedMigrator.GetEquipmentCreationAsync(SelectedPath, timeZone).ConfigureAwait(false), SelectedMigrator.Id)
                            .ConfigureAwait(false);
                    if (SelectExpeditionCompletion && SelectedMigrator.SupportExpeditionCompletion)
                        count += await TryMigrate(context.ExpeditionCompletionTable, await SelectedMigrator.GetExpeditionCompletionAsync(SelectedPath, timeZone).ConfigureAwait(false), SelectedMigrator.Id)
                            .ConfigureAwait(false);
                    if (SelectBattleAndDrop && SelectedMigrator.SupportBattleAndDrop)
                        count += await TryMigrate(context.BattleTable, await SelectedMigrator.GetBattleAndDropAsync(SelectedPath, timeZone).ConfigureAwait(false), SelectedMigrator.Id)
                            .ConfigureAwait(false);
                    if (SelectMaterialsChange && SelectedMigrator.SupportMaterialsChange)
                        count += await TryMigrate(context.MaterialsChangeTable, await SelectedMigrator.GetMaterialsChangeAsync(SelectedPath, timeZone).ConfigureAwait(false), SelectedMigrator.Id)
                            .ConfigureAwait(false);

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                Running = false;
                await shellContext.ShowMessageAsync(string.Format(localization.GetLocalized("Logging", "MigrationCount"), count),
                    localization.GetLocalized("Logging", "MigrationCompleted"));
            }
            catch (Exception ex)
            {
                Running = false;
                await shellContext.ShowMessageAsync(ex.ToString(), localization.GetLocalized("Logging", "MigrationFailed"));
            }

            GC.Collect();
            GC.Collect();
        }
    }
}
