using System;
using System.Reflection;
using Sakuno.ING.Game.Models;
using Xunit;

namespace Sakuno.ING.Game.Tests
{
    public static class HomeportTest
    {
        [Fact]
        public static void TestHomeportParse()
        {
            var provider = new UnitTestProvider();
            var gameListener = new GameListener(provider);
            var navalBase = new NavalBase(gameListener, null, null);

            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(MasterDataTest), "Data.port.2018-05-28_015208.676_START2.json"))
                provider.Push("api_start2", DateTimeOffset.Now, string.Empty, stream);
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(MasterDataTest), "Data.port.2018-05-28_015210.298_REQUIRE_INFO.json"))
                provider.Push("api_get_member/require_info", DateTimeOffset.Now, string.Empty, stream);
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(MasterDataTest), "Data.port.2018-05-28_015212.349_PORT.json"))
                provider.Push("api_port/port", DateTimeOffset.Now, string.Empty, stream);
        }
    }
}
