using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sakuno.KanColle.Amatsukaze.Game.Events;

namespace Sakuno.KanColle.Amatsukaze.Game.Test
{
    [TestClass]
    public class MasterDataTest
    {
        private static MasterDataUpdate parseResult;
        [ClassInitialize]
        public static void LoadData(TestContext context)
        {
            var provider = new UnitTestProvider();
            var gameListener = new GameListener(provider);

            gameListener.MasterDataUpdated.Received += u => parseResult = u;

            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(MasterDataTest), "Data.masterdata.json"))
                provider.Push("api_start2", DateTimeOffset.Now, string.Empty, stream);
        }
        [TestMethod]
        public void TestDataLoading()
        {
            Assert.IsNotNull(parseResult);
        }
    }
}
