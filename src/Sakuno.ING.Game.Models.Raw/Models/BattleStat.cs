using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct BattleStat : IEquatable<BattleStat>
    {
        public int Total => Win + Lose;
        public int Win { get; }
        public int Lose { get; }
        public int WinPercentage { get; }

        public BattleStat(int win, int lose)
        {
            Win = win;
            Lose = lose;
            WinPercentage = (int)Math.Round((double)win / (win + lose));
        }

        public BattleStat(int win, int lose, int percentage)
        {
            Win = win;
            Lose = lose;
            WinPercentage = percentage;
        }

        public bool Equals(BattleStat other)
            => Win == other.Win
            && Lose == other.Lose;
    }
}
