using Newtonsoft.Json.Linq;
using Sakuno.ING.Game.Json;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public interface IBattleDetailOwner
    {
        //byte[] StoreBattle(JsonElement json);

        byte[] StoreBattle(JToken json);

        BattleDetailJson LoadBattle(byte[] data);
    }
}
