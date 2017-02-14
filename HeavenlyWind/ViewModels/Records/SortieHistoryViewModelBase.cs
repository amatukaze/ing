using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records
{
    abstract class SortieHistoryViewModelBase<T> : HistoryViewModelWithTimeFilter<T> where T : SortieRecordBase, IRecordID
    {
        public FilterKeyCollection<SortieMapFilterKey> Maps { get; } = new FilterKeyCollection<SortieMapFilterKey>(SortieMapFilterKey.All, SortieMapFilterKey.Comparer);

        protected abstract string LoadMapsCommandText { get; }

        protected SortieMapFilterKey r_Map = SortieMapFilterKey.All;
        public SortieMapFilterKey SelectedMap
        {
            get { return r_Map; }
            set
            {
                if (r_Map != value)
                {
                    r_Map = value;
                    OnPropertyChanged(nameof(SelectedMap));

                    OnSelectedMapChanged();

                    Refresh();
                }
            }
        }

        public override void OnInitialized()
        {
            var rMasterInfo = KanColleGame.Current.MasterInfo;

            using (var rCommand = CreateCommand())
            {
                rCommand.CommandText = LoadMapsCommandText;

                using (var rReader = rCommand.ExecuteReader())
                {
                    var rMaps = new List<SortieMapFilterKey>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                    {
                        var rMap = Convert.ToInt32(rReader["map"]);

                        EventMapDifficulty rDifficulty;
                        var rDifficultyData = rReader["difficulty"];
                        if (rDifficultyData == DBNull.Value)
                            rDifficulty = EventMapDifficulty.None;
                        else
                            rDifficulty = (EventMapDifficulty)Convert.ToInt32(rDifficultyData);

                        rMaps.Add(new SortieMapFilterKey(MapService.Instance.GetMasterInfo(rMap), rDifficulty));
                    }

                    Maps.Update(rMaps);
                }
            }
        }

        protected virtual void OnSelectedMapChanged() { }

        protected override void OnRecordInsert(string rpTable, long rpRowID)
        {
            base.OnRecordInsert(rpTable, rpRowID);

            var rMap = LastInsertedRecord.Map;
            var rDifficulty = LastInsertedRecord.EventMapDifficulty.GetValueOrDefault();

            foreach (var rKey in Maps)
                if (rMap == rKey.Map && rDifficulty == rKey.EventMapDifficulty)
                    return;

            Maps.Add(new SortieMapFilterKey(LastInsertedRecord.Map, rDifficulty));
        }
    }
}
