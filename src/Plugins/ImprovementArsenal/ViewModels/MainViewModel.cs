using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.ViewModels
{
    class MainViewModel : ModelBase
    {
        const string DataStoreItemName = "improvement_arsenal";

        static Port r_Port = KanColleGame.Current.Port;

        bool r_IsLoading;
        public bool IsLoading
        {
            get { return r_IsLoading; }
            private set
            {
                if (r_IsLoading != value)
                {
                    r_IsLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        ImprovementInfoViewModel[] r_Infos;

        public ImprovementDay[] Days => ImprovementDay.Yes;

        DayOfWeek r_SelectedDay;
        public DayOfWeek SelectedDay
        {
            get { return r_SelectedDay; }
            set
            {
                if (r_SelectedDay != value)
                {
                    r_SelectedDay = value;
                    OnPropertyChanged(nameof(SelectedDay));

                    Update();
                }
            }
        }

        public IList<EquipmentIconType> Types { get; private set; }

        EquipmentIconType r_SelectedType;
        public EquipmentIconType SelectedType
        {
            get { return r_SelectedType; }
            set
            {
                if (r_SelectedType != value)
                {
                    r_SelectedType = value;
                    OnPropertyChanged(nameof(SelectedType));

                    Update();
                }
            }
        }

        public IList<ImprovementInfoViewModel> Infos { get; private set; }

        bool r_HasException;
        public bool HasException
        {
            get { return r_HasException; }
            set
            {
                if (r_HasException != value)
                {
                    r_HasException = value;
                    OnPropertyChanged(nameof(HasException));
                }
            }
        }
        Exception r_Exception;
        public Exception Exception
        {
            get { return r_Exception; }
            set
            {
                if (r_Exception != value)
                {
                    r_Exception = value;
                    OnPropertyChanged(nameof(Exception));
                }
            }
        }
        bool r_Retryable;
        public bool Retryable
        {
            get { return r_Retryable; }
            set
            {
                if (r_Retryable != value)
                {
                    r_Retryable = value;
                    OnPropertyChanged(nameof(Retryable));
                }
            }
        }

        bool r_IsNewVersionAvailable;
        public bool IsNewVersionAvailable
        {
            get { return r_IsNewVersionAvailable; }
            set
            {
                if (r_IsNewVersionAvailable != value)
                {
                    r_IsNewVersionAvailable = value;
                    OnPropertyChanged(nameof(IsNewVersionAvailable));
                }
            }
        }

        public void Initialize()
        {
            if (r_Infos != null)
                return;

            var timestamp = default(DateTimeOffset?);

            if (DataStore.TryGet(DataStoreItemName, out DataStoreItem rData) && rData.Timestamp >= DateTimeUtil.FromUnixTime(1506616394))
            {
                try
                {
                    InitializeCore(rData.Content);
                    timestamp = rData.Timestamp;
                }
                catch { }
            }

            Task.Run(() => CheckForUpdate(timestamp));
        }

        public void Reload()
        {
            if (DataStore.TryGet(DataStoreItemName, out DataStoreItem rData))
                InitializeCore(rData.Content);
        }

        void CheckForUpdate(DateTimeOffset? rpTimestamp)
        {
            var rRequest = WebRequest.CreateHttp("http://heavenlywind.cc/api/improvement_arsenal?type=lab");
            rRequest.UserAgent = ProductInfo.UserAgent;
            rRequest.Method = "GET";

            if (rpTimestamp.HasValue)
                rRequest.IfModifiedSince = rpTimestamp.Value.LocalDateTime;

            try
            {
                using (var rResponse = (HttpWebResponse)rRequest.GetResponse())
                using (var rResponseStream = rResponse.GetResponseStream())
                {
                    var rBuffer = new MemoryStream();
                    var rLastModified = new DateTimeOffset(rResponse.LastModified, DateTimeOffset.Now.Offset).ToUnixTime();

                    rResponseStream.CopyTo(rBuffer);

                    var rContent = rBuffer.ToArray();
                    DataStore.Set(DataStoreItemName, rContent, rLastModified);

                    if (!rpTimestamp.HasValue)
                        InitializeCore(rContent);
                    else if (rLastModified > rpTimestamp.Value.ToUnixTime())
                    {
                        IsNewVersionAvailable = true;
                        OnPropertyChanged(nameof(IsNewVersionAvailable));
                    }
                }
            }
            catch (WebException e)
            {
                var rResponse = e.Response as HttpWebResponse;
                if ((rResponse == null || rResponse.StatusCode != HttpStatusCode.NotModified))
                {
                    HasException = true;
                    Exception = e;
                    Retryable = true;

                    return;
                }
            }
            catch (Exception e) when (!Debugger.IsAttached)
            {
                HasException = true;
                Exception = e;
                Retryable = false;
            }
        }
        void InitializeCore(byte[] rpContent)
        {
            HasException = false;
            Exception = null;

            try
            {
                var rSerializer = new JsonSerializer();
                var rReader = new JsonTextReader(new StreamReader(new MemoryStream(rpContent)));
                var rModels = rSerializer.Deserialize<ImprovementInfo[]>(rReader);

                var infos = new List<ImprovementInfoViewModel>();

                foreach (var model in rModels)
                {
                    if (model.Branches == null)
                        infos.Add(new ImprovementInfoViewModel(model, null));
                    else
                        foreach (var branch in model.Branches)
                            infos.Add(new ImprovementInfoViewModel(model, branch));
                }

                r_Infos = infos.OrderBy(r => r.Equipment.Type).ToArray();

                var rToday = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(9.0));
                rToday -= rToday.TimeOfDay;
                SelectedDay = rToday.DayOfWeek;
                ImprovementDay.UpdateToday(SelectedDay);

                Observable.Timer(rToday.AddDays(1.0), TimeSpan.FromDays(1.0)).Subscribe(_ =>
                {
                    SelectedDay = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(9.0)).DayOfWeek;
                    ImprovementDay.UpdateToday(SelectedDay);
                });

                Types = new[] { EquipmentIconType.None }.Concat(r_Infos.Select(r => r.Equipment.Icon).Distinct()).ToArray();
                OnPropertyChanged(nameof(Types));

                Update();
            }
            catch (Exception e) when (!Debugger.IsAttached)
            {
                HasException = true;
                Exception = e;
                Retryable = false;
            }
        }
        void Update()
        {
            if (r_Infos == null)
                return;

            var rInfos = r_Infos.Where(r => r.Detail.SecondShips != null && r.Detail.SecondShips.Any(rpShip => rpShip.Days.Any(rpDay => rpDay.Improvable && rpDay.Day == r_SelectedDay)));

            if (r_SelectedType != EquipmentIconType.None)
                rInfos = rInfos.Where(r => r.Equipment.Icon == r_SelectedType);

            rInfos = rInfos.OrderBy(r => r.Equipment.Type);

            Infos = rInfos.ToArray();

            foreach (var rInfo in Infos)
                rInfo.Detail.Update(SelectedDay);

            OnPropertyChanged(nameof(Infos));
        }
    }
}
