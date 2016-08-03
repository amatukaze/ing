using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.SystemInterop;
using Sakuno.SystemInterop.Dialogs;
using Sakuno.UserInterface;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.History
{
    abstract class HistoryViewModelBase<T> : ModelBase, IDisposable where T : ModelBase
    {
        ObservableCollection<T> r_Records;
        protected ObservableCollection<T> InternalRecords => r_Records;
        public ReadOnlyObservableCollection<T> Records { get; }

        protected abstract string LoadCommandText { get; }

        protected T LastInsertRecord { get; private set; }

        public ICommand ExportAsCsvFileCommand { get; }

        public HistoryViewModelBase()
        {
            r_Records = new ObservableCollection<T>();
            Records = new ReadOnlyObservableCollection<T>(r_Records);

            RecordService.Instance.Update += Record_Update;

            ExportAsCsvFileCommand = new DelegatedCommand(ExportAsCsvFile);
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

        public void ExportAsCsvFile()
        {
            using (var rDialog = new CommonSaveFileDialog())
            {
                rDialog.FileTypes.Add(new CommonFileDialogFileType(StringResources.Instance.Main.Export_CSV_FileType, "csv"));
                rDialog.DefaultExtension = "csv";

                if (rDialog.Show(WindowUtil.GetTopWindow()) != CommonFileDialogResult.OK)
                    return;

                var rFilename = rDialog.Filename;

                using (var rWriter = new StreamWriter(File.Open(rFilename, FileMode.Create), Encoding.Default))
                    ExportAsCsvFileCore(rWriter);

                var rButton = new TaskDialogButton(StringResources.Instance.Main.Export_CSV_Message_OpenFile);
                var rResult = new TaskDialog()
                {
                    Caption = StringResources.Instance.Main.Product_Name,
                    Icon = TaskDialogIcon.Information,
                    Instruction = StringResources.Instance.Main.Export_CSV_Message,
                    CommonButtons = TaskDialogCommonButtons.OK,
                    Buttons = { rButton },

                    OwnerWindow = WindowUtil.GetTopWindow(),
                    ShowAtTheCenterOfOwner = true,
                }.ShowAndDispose();
                if (rResult.SelectedButton == rButton)
                    Process.Start(rFilename);
            }
        }

        protected virtual void ExportAsCsvFileCore(StreamWriter rpWriter) { }
    }
}
