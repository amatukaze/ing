using System;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;
using Sakuno.KanColle.Amatsukaze.Game.Json.MasterData;
using Sakuno.KanColle.Amatsukaze.Messaging;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    partial class GameListener
    {
        public IProducer<MasterDataUpdate> MasterDataUpdated;

        private MasterDataUpdate ParseMasterData(ParsedMessage<MasterDataJson> raw)
        {

        }
    }
}
