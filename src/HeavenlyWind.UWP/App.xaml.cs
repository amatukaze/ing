using Sakuno.KanColle.Amatsukaze.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!Bootstraper.IsInitialized)
            {
                var assemblyNames = new[]
                {
                    "HeavenlyWind.Data",
                    "HeavenlyWind.UWP.Data",
                };

                var emptyDictionary = new Dictionary<string, string>();

                Bootstraper.Initialize(Array.Empty<string>(),
                    assemblyNames.Select(Assembly.Load).Prepend(Assembly.GetExecutingAssembly())
                        .Select(asm => new PackageStartupInfo
                        {
                            Id = asm.GetName().Name,
                            Version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                                ?? asm.GetName().Version.ToString(),
                            Dependencies = emptyDictionary,
                            Module = new Lazy<Assembly>(asm)
                        }).ToArray(),
                    null);
            }

            if (e.PrelaunchActivated == false)
                Bootstraper.Startup();
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
