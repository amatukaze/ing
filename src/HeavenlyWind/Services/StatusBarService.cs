using Sakuno.KanColle.Amatsukaze.Controls;
using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.UserInterface.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class StatusBarService : ModelBase, IStatusBarService
    {
        public static StatusBarService Instance { get; } = new StatusBarService();

        string r_Message;
        public string Message
        {
            get { return r_Message; }
            set
            {
                var rMessage = value.Replace(Environment.NewLine, " ");
                if (r_Message != rMessage)
                {
                    r_Message = value.Replace(Environment.NewLine, " ");
                    OnPropertyChanged(nameof(Message));
                }
                IsMessageObsolete = false;
            }
        }

        bool r_IsMessageObsolete = true;
        public bool IsMessageObsolete
        {
            get { return r_IsMessageObsolete; }
            private set
            {
                r_IsMessageObsolete = value;
                OnPropertyChanged(nameof(IsMessageObsolete));
            }
        }

        public Power Power { get; } = new Power();

        DateTimeOffset r_Time;
        long r_InitialTick;
        public DateTimeOffset? CurrentTime { get; private set; }

        public SortieInfo Sortie => SortieInfo.Current;

        StatusBarService()
        {
            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(IsMessageObsolete)).Select(_ => IsMessageObsolete).Where(r => !r)
                .Throttle(TimeSpan.FromSeconds(30.0)).Subscribe(_ => IsMessageObsolete = true);

            ServiceManager.Register<IStatusBarService>(this);

            ApiService.Subscribe("api_port/port", _ => OnPropertyChanged(nameof(Sortie)));
            ApiService.Subscribe("api_req_map/start", _ => OnPropertyChanged(nameof(Sortie)));
            ApiService.Subscribe("api_req_map/next", _ => Message = string.Empty);
        }

        public void Initialize()
        {
            Logger.LogAdded += r => Message = r.Content;

            Task.Run(new Action(async () =>
            {
                var rServers = new[] { "jp.pool.ntp.org", "pool.ntp.org", "us.pool.ntp.org", "cn.pool.ntp.org", "jp.ntp.org.cn", "us.ntp.org.cn", "cn.ntp.org.cn" };

                var rRetryCount = 0;

                foreach (var rSuccess in rServers.Select(QueryCurrentTime))
                {
                    await Task.Delay(rRetryCount * 2000);
                    rRetryCount++;

                    if (await rSuccess)
                    {
                        StartTimer();
                        return;
                    }
                }
            }));

            RegisterCustomBBCodeTag();
        }

        async Task<bool> QueryCurrentTime(string rpHostname)
        {
            try
            {
                using (var rClient = new UdpClient(rpHostname, 123))
                {
                    var rData = new byte[48];
                    rData[0] = 0x1B;

                    await rClient.SendAsync(rData, rData.Length);

                    var rReceiveTask = rClient.ReceiveAsync();

                    rReceiveTask.ContinueWith(r => r.Exception?.Handle(_ => true), TaskContinuationOptions.OnlyOnFaulted).Forget();

                    if (await Task.WhenAny(rReceiveTask, Task.Delay(5000)) != rReceiveTask)
                        return false;

                    rData = (await rReceiveTask).Buffer;

                    var rIntegerPart = (ulong)rData[40] << 24 | (ulong)rData[41] << 16 | (ulong)rData[42] << 8 | rData[43];
                    var rFractionPart = (ulong)rData[44] << 24 | (ulong)rData[45] << 16 | (ulong)rData[46] << 8 | rData[47];
                    var rMilliseconds = rIntegerPart * 1000 + (rFractionPart >> 20);

                    r_Time = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(rMilliseconds).ToOffset(TimeSpan.FromHours(9.0));

                    r_InitialTick = Stopwatch.GetTimestamp();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        void StartTimer()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    var rCurrentTick = Stopwatch.GetTimestamp();

                    CurrentTime = r_Time.AddSeconds((rCurrentTick - r_InitialTick) / (double)Stopwatch.Frequency);
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }, TaskCreationOptions.LongRunning);
        }

        void RegisterCustomBBCodeTag()
        {
            BBCodeBlock.AddCustomTag("icon", rpInline =>
            {
                var rSpan = rpInline as Span;
                if (rSpan == null || rSpan.Inlines.Count != 1)
                    return null;

                var rRun = rSpan.Inlines.FirstInline as Run;
                if (rRun == null)
                    return null;

                var rIcon = rRun.Text;
                if (rIcon.OICEquals("fuel"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.Fuel });
                if (rIcon.OICEquals("bullet"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.Bullet });
                if (rIcon.OICEquals("steel"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.Steel });
                if (rIcon.OICEquals("bauxite"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.Bauxite });
                if (rIcon.OICEquals("ic"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.InstantConstruction });
                if (rIcon.OICEquals("bucket"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.Bucket });
                if (rIcon.OICEquals("dm"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.DevelopmentMaterial });
                if (rIcon.OICEquals("im"))
                    return GetUIContainer(new MaterialIcon() { Type = MaterialType.ImprovementMaterial });

                if (rIcon.OICEquals("firepower"))
                    return GetUIContainer(new CommonPropertyIcon() { Type = CommonProperty.Firepower });
                if (rIcon.OICEquals("torpedo"))
                    return GetUIContainer(new CommonPropertyIcon() { Type = CommonProperty.Torpedo });
                if (rIcon.OICEquals("aa"))
                    return GetUIContainer(new CommonPropertyIcon() { Type = CommonProperty.AA });
                if (rIcon.OICEquals("armor"))
                    return GetUIContainer(new CommonPropertyIcon() { Type = CommonProperty.Armor });
                if (rIcon.OICEquals("luck"))
                    return GetUIContainer(new CommonPropertyIcon() { Type = CommonProperty.Luck });
                if (rIcon.OICEquals("hp"))
                    return GetUIContainer(new CommonPropertyIcon() { Type = CommonProperty.HP });
                if (rIcon.OICEquals("asw"))
                    return GetUIContainer(new CommonPropertyIcon() { Type = CommonProperty.ASW });

                return null;
            });
        }

        InlineUIContainer GetUIContainer(UIElement rpElement)
        {
            var rElement = rpElement as FrameworkElement;
            if (rElement != null)
            {
                rElement.MaxWidth = 16;
                rElement.MaxHeight = 16;
            }

            return new InlineUIContainer(rpElement) { BaselineAlignment = BaselineAlignment.Center };
        }
    }
}
