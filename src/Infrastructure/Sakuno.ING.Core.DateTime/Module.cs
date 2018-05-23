using Sakuno.ING.Composition;
using System;

namespace Sakuno.ING.Services.DateTime
{
    class Module : IModule, IExposableModule
    {
        public void Expose(IBuilder builder)
        {
            builder.RegisterService<DateTimeService, IDateTimeService>();
        }

        public void Initialize(IResolver resolver)
        {
            var service = (DateTimeService)resolver.Resolve<IDateTimeService>();

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
