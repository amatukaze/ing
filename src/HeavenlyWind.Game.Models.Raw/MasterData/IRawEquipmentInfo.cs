using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public interface IRawEquipmentInfo
    {
        int Id { get; }
        string Name { get; }
        string Description { get; }

        int TypeId { get; }
        int IconId { get; }
        IReadOnlyCollection<int> ExtraSlotAcceptingShips { get; }

        /// <summary>
        /// 火力
        /// </summary>
        int Firepower { get; }
        /// <summary>
        /// 雷装
        /// </summary>
        int Torpedo { get; }
        /// <summary>
        /// 対空
        /// </summary>
        int AntiAir { get; }
        /// <summary>
        /// 装甲
        /// </summary>
        int Armor { get; }
        /// <summary>
        /// 爆装
        /// </summary>
        int DiveBomberAttack { get; }
        /// <summary>
        /// 対潜
        /// </summary>
        int AntiSubmarine { get; }
        /// <summary>
        /// 命中
        /// </summary>
        int Accuracy { get; }
        /// <summary>
        /// 回避
        /// </summary>
        int Evasion { get; }
        /// <summary>
        /// 対爆
        /// </summary>
        int AntiBomber { get; }
        /// <summary>
        /// 迎撃
        /// </summary>
        int Interception { get; }
        /// <summary>
        /// 索敵
        /// </summary>
        int LightOfSight { get; }
        /// <summary>
        /// 射程
        /// </summary>
        FireRange FireRange { get; }

        int FlightRadius { get; }
        Materials DeploymentConsumption { get; }
        Materials DismantleAcquirement { get; }

        int Rarity { get; }
    }
}
