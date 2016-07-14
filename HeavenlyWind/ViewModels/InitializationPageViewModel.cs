using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    class InitializationPageViewModel : ModelBase
    {
        const int LoopbackAddress = 16777343;

        MainWindowViewModel r_Owner;

        InitializationStep r_Step;
        public InitializationStep Step
        {
            get { return r_Step; }
            private set
            {
                if (r_Step != value)
                {
                    r_Step = value;
                    OnPropertyChanged(nameof(Step));
                }
            }
        }

        public bool IsPortAvailable { get; private set; } = true;
        public bool IsUpstreamProxyAvailable { get; private set; }

        public ProcessInfo ProcessThatOccupyingPort { get; private set; }

        public ICommand StartCommand { get; }

        public InitializationPageViewModel(MainWindowViewModel rpOwner)
        {
            r_Owner = rpOwner;

            StartCommand = new DelegatedCommand(Start);
        }

        public async void Start()
        {
            Step = InitializationStep.Initializing;

            await Task.Run(new Action(CheckProxyPort));

            if (IsPortAvailable && IsUpstreamProxyAvailable)
            {
                KanColleProxy.Start();
                r_Owner.Page = r_Owner.GameInformation;

                Step = InitializationStep.None;
                return;
            }

            Step = InitializationStep.Error;
        }

        unsafe void CheckProxyPort()
        {
            var rPort = Preference.Current.Network.Port;
            var rUpstreamProxy = Preference.Current.Network.UpstreamProxy;

            var rIsPortAvailable = true;
            var rIsUpstreamProxyAvailable = false;

            if (!rUpstreamProxy.Enabled || rUpstreamProxy.Host != "127.0.0.1")
                rIsUpstreamProxyAvailable = true;

            var rBufferSize = 0;
            NativeMethods.IPHelperApi.GetExtendedTcpTable(IntPtr.Zero, ref rBufferSize, true, NativeConstants.AF.AF_INET, NativeConstants.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_LISTENER);

            var rBuffer = stackalloc byte[rBufferSize];
            NativeMethods.IPHelperApi.GetExtendedTcpTable((IntPtr)rBuffer, ref rBufferSize, true, NativeConstants.AF.AF_INET, NativeConstants.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_LISTENER);

            var rTable = (NativeStructs.MIB_TCPTABLE*)rBuffer;
            var rRow = &rTable->table;

            for (var i = 0; i < rTable->dwNumEntries; i++, rRow++)
            {
                if (rRow->dwLocalAddr != LoopbackAddress)
                    continue;

                if (rRow->LocalPort == rPort)
                {
                    rIsPortAvailable = false;

                    try
                    {
                        ProcessThatOccupyingPort = new ProcessInfo(Process.GetProcessById(rRow->dwOwningPid));
                    }
                    catch
                    {
                        ProcessThatOccupyingPort = null;
                    }
                }

                if (!rIsUpstreamProxyAvailable && rRow->LocalPort == rUpstreamProxy.Port)
                    rIsUpstreamProxyAvailable = true;
            }

            if (rIsPortAvailable)
                ProcessThatOccupyingPort = null;

            IsPortAvailable = rIsPortAvailable;
            IsUpstreamProxyAvailable = rIsUpstreamProxyAvailable;

            OnPropertyChanged(nameof(IsPortAvailable));
            OnPropertyChanged(nameof(IsUpstreamProxyAvailable));
            OnPropertyChanged(nameof(ProcessThatOccupyingPort));
        }
    }
}
