using Sakuno.ING.Composition;
using System;

namespace Sakuno.ING.Timing.NTP
{
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<NTPService, ITimingService>();
        }

        public void Initialize(IResolver resolver)
        {
            var service = (NTPService)resolver.Resolve<ITimingService>();

            service.SyncDateTime();

            service.PropertyChanged += (s, e) =>
            {
                if (!service.IsSycned)
                    return;

                Console.Write('\r');
                Console.Write(service.Now.ToString("JST: yyyy/MM/dd HH:mm:ss"));
            };
        }
    }
}
