using System;
using Sakuno.ING.Localization;
using Sakuno.ING.Timing;

namespace Sakuno.ING.Composition
{
    public static class StaticResolver
    {
        public static IResolver Instance { get; private set; }

        public static void Initialize(IResolver resolver)
        {
            if (Instance != null)
                throw new InvalidOperationException("Resolver instance can be only set at initializion.");

            Instance = resolver;
            LocalizationService = resolver.TryResolve<ILocalizationService>();
            TimingService = resolver.TryResolve<ITimingService>();
        }

        public static ILocalizationService LocalizationService { get; private set; }
        public static ITimingService TimingService { get; private set; }
    }
}
