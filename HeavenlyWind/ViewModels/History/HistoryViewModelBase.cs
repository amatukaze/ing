using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    abstract class HistoryViewModelBase<T> : ModelBase, IDisposable where T : ModelBase
    {
        ObservableCollection<T> r_Records;
        protected ObservableCollection<T> InternalRecords => r_Records;
        public ReadOnlyObservableCollection<T> Records { get; }

        protected abstract string LoadCommandText { get; }

        protected T LastInsertRecord { get; private set; }

        public HistoryViewModelBase()
        {
            r_Records = new ObservableCollection<T>();
            Records = new ReadOnlyObservableCollection<T>(r_Records);

            RecordService.Instance.Update += Record_Update;
        }

        public void Dispose() => RecordService.Instance.Update -= Record_Update;

        protected abstract T CreateRecordFromReader(SQLiteDataReader rpReader);

        public void LoadRecords()
        {
            Task.Run(() =>
            {
                using (var rCommand = RecordService.Instance.CreateCommand())
                {
                    rCommand.CommandText = LoadCommandText;

                    using (var rReader = rCommand.ExecuteReader())
                        while (rReader.Read())
                        {
                            var rRecord = CreateRecordFromReader(rReader);
                            DispatcherUtil.UIDispatcher.BeginInvoke(new Action<T>(r_Records.Add), rRecord);
                        }

                    DispatcherUtil.UIDispatcher.BeginInvoke(new Action(() =>
                    {
                        if (r_Records.Count > 0)
                            LastInsertRecord = r_Records[0];
                    }));
                }
            });
        }

        protected abstract bool TableFilter(string rpTable);

        void Record_Update(UpdateEventArgs e)
        {
            Task.Run(() =>
            {
                var rTable = e.Database + "." + e.Table;

                if (!TableFilter(rTable))
                    return;

                switch (e.Event)
                {
                    case UpdateEventType.Insert:
                        OnRecordInsert(rTable, e.RowId);
                        break;

                    case UpdateEventType.Delete:
                        OnRecordDelete(rTable, e.RowId);
                        break;

                    case UpdateEventType.Update:
                        OnRecordUpdate(rTable, e.RowId);
                        break;
                }
            });
        }

        protected virtual void OnRecordInsert(string rpTable, long rpRowID)
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                PrepareCommandOnRecordInsert(rCommand, rpTable, rpRowID);

                if (rCommand.CommandText.IsNullOrEmpty())
                    return;

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                    {
                        LastInsertRecord = CreateRecordFromReader(rReader);
                        DispatcherUtil.UIDispatcher.BeginInvoke(new Action(() => r_Records.Insert(0, LastInsertRecord)));
                    }
            }
        }
        protected virtual void OnRecordDelete(string rpTable, long rpRowID) { }
        protected virtual void OnRecordUpdate(string rpTable, long rpRowID) { }

        protected abstract void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID);
    }
}
