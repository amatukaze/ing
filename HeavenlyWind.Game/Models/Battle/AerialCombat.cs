namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class AerialCombat : ModelBase
    {
        public Stage Stage1 { get; internal set; }
        public Stage Stage2 { get; internal set; }

        public AntiAirCutIn AntiAirCutIn { get; internal set; }
        public AntiAirCutIn SecondAntiAirCutIn { get; internal set; }

        public AerialCombatResult Result { get; internal set; }

        public class Stage : ModelBase
        {
            public int? FriendPlaneCount { get; internal set; }
            public int? EnemyPlaneCount { get; internal set; }

            public int? FriendPlaneRemaningCount { get; internal set; }
            public int? EnemyPlaneRemaningCount { get; internal set; }
        }
    }
}
