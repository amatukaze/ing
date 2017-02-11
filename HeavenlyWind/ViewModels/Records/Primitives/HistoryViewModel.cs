using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using Sakuno.SystemInterop;
using Sakuno.SystemInterop.Dialogs;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives
{
    abstract class HistoryViewModel<T> : DisposableModelBase where T : ModelBase, IRecordID
    {
        HistoryRecordsView<T> r_Records;
        protected HistoryRecordsView<T> InternalRecords => r_Records;
        public IList<T> Records => r_Records;

        protected abstract string LoadCommandText { get; }

        protected T LastInsertedRecord { get; private set; }

        protected HistoryViewModel()
        {
            r_Records = new HistoryRecordsView<T>(this);

            RecordService.Instance.Update += Record_Update;
        }

        protected override void DisposeManagedResources()
        {
            r_Records.Dispose();

            RecordService.Instance.Update -= Record_Update;
        }

        protected SQLiteCommand CreateCommand() => RecordService.Instance.CreateCommand();

        protected virtual void SetCommandText(SQLiteCommand rpCommand) => rpCommand.CommandText = LoadCommandText;

        protected abstract T CreateRecordFromReader(SQLiteDataReader rpReader);

        public void LoadRecords()
        {
            Task.Run(() =>
            {
                using (var rCommand = CreateCommand())
                {
                    SetCommandText(rCommand);

                    LastInsertedRecord = null;
                    r_Records.Clear();

                    using (var rReader = rCommand.ExecuteReader())
                    {
                        var rRecords = new List<T>(rReader.VisibleFieldCount);

                        while (rReader.Read())
                        {
                            LastInsertedRecord = CreateRecordFromReader(rReader);
                            rRecords.Add(LastInsertedRecord);
                        }

                        r_Records.Load(rRecords);
                    }
                }

                OnInitialized();
            });
        }
        public virtual void OnInitialized() { }

        public void Refresh() => Task.Run((Action)r_Records.Refresh);

        public virtual bool Filter(T rpItem) => true;

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
            using (var rCommand = CreateCommand())
            {
                PrepareCommandOnRecordInsert(rCommand, rpTable, rpRowID);

                if (rCommand.CommandText.IsNullOrEmpty())
                    return;

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                    {
                        if (!BeforeNewRecordInserting(rReader))
                            return;

                        LastInsertedRecord = CreateRecordFromReader(rReader);
                        r_Records.Add(LastInsertedRecord);
                    }
            }
        }
        protected virtual void OnRecordDelete(string rpTable, long rpRowID) { }
        protected virtual void OnRecordUpdate(string rpTable, long rpRowID) { }

        protected abstract void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID);
        protected virtual bool BeforeNewRecordInserting(SQLiteDataReader rpReader) => true;

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
