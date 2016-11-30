using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Statistics;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Statistics
{
    class SortieStatisticViewModel : DisposableModelBase
    {
        public IList<SortieStatisticTimeSpanGroupViewModel> TimeSpans { get; private set; }

        public SortieStatisticCustomTimeSpanGroupViewModel CustomTimeSpan { get; }

        SortieStatisticTimeSpanGroupViewModel r_SelectedTimeSpan;
        public SortieStatisticTimeSpanGroupViewModel SelectedTimeSpan
        {
            get { return r_SelectedTimeSpan; }
            set
            {
                if (r_SelectedTimeSpan != value)
                {
                    if (r_SelectedTimeSpan != null)
                        r_SelectedTimeSpan.Maps = null;

                    r_SelectedTimeSpan = value;
                    OnPropertyChanged(nameof(SelectedTimeSpan));

                    r_SelectedTimeSpan?.Reload();
                }
            }
        }

        public IList<SortieStatisticMapViewModel> Maps { get; }

        bool? r_IsAllMapsSelected = true;
        public bool? IsAllMapsSelected
        {
            get { return r_IsAllMapsSelected; }
            set
            {
                if (r_IsAllMapsSelected != value)
                {
                    r_IsAllMapsSelected = value;
                    if (r_IsAllMapsSelected.HasValue)
                    {
                        foreach (var rMap in Maps)
                            rMap.SetIsSelectedWithoutCallback(r_IsAllMapsSelected.Value);

                        UpdateSelection();
                        OnPropertyChanged(nameof(IsAllMapsSelected));
                    }
                }
            }
        }

        public ICommand SelectThisMapOnlyCommand { get; }

        public SortieStatisticViewModel()
        {
            SelectThisMapOnlyCommand = new DelegatedCommand<SortieStatisticMapViewModel>(SelectThisMapOnly);

            IEnumerable<SortieStatisticTimeSpanGroupViewModel> rTimeSpans = Enumerable.Range(0, 6).Select(r => new SortieStatisticDefaultTimeSpanGroupViewModel(this, (SortieStatisticTimeSpanType)r));

            CustomTimeSpan = new SortieStatisticCustomTimeSpanGroupViewModel(this);

            TimeSpans = rTimeSpans.Concat(new[] { CustomTimeSpan }).ToArray();
            r_SelectedTimeSpan = TimeSpans[0];

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                var rMaps = new List<SortieStatisticMapViewModel>(48);

                rCommand.CommandText = "SELECT DISTINCT map, difficulty FROM sortie JOIN sortie_consumption USING(id) ORDER BY map, difficulty;";
                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                        rMaps.Add(new SortieStatisticMapViewModel(MapService.Instance.GetMasterInfo(rReader.GetInt32(0)), (EventMapDifficulty)rReader.GetInt32Optional(1).GetValueOrDefault()) { IsSelectedChangedCallback = UpdateSelection, SelectThisMapOnlyCommand = SelectThisMapOnlyCommand });
                Maps = rMaps;

                rCommand.CommandText = "SELECT min(id) FROM sortie_consumption;";
                CustomTimeSpan.MinDisplayDateStart = DateTimeUtil.FromUnixTime((long)rCommand.ExecuteScalar()).ToOffset(DateTimeOffset.Now.Offset).DateTime.Date;
            }
        }

        public void Load() => r_SelectedTimeSpan.Reload();

        protected override void DisposeManagedResources()
        {
            SelectedTimeSpan = null;
            TimeSpans = null;
        }

        void UpdateSelection()
        {
            var rTypeCount = Maps.Count;
            var rSelectedCount = Maps.Count(r => r.IsSelected);

            if (rSelectedCount == 0)
                r_IsAllMapsSelected = false;
            else if (rSelectedCount == rTypeCount)
                r_IsAllMapsSelected = true;
            else
                r_IsAllMapsSelected = null;

            OnPropertyChanged(nameof(IsAllMapsSelected));

            Load();
        }

        void SelectThisMapOnly(SortieStatisticMapViewModel rpMap)
        {
            foreach (var rMap in Maps)
                rMap.SetIsSelectedWithoutCallback(rMap == rpMap);

            UpdateSelection();
        }
    }
}
