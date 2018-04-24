using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public readonly struct ExpeditionStat : IEquatable<ExpeditionStat>
    {
        public int Total { get; }
        public int Success { get; }
        public int Fail => Total - Success;
        public int WinPercentage { get; }

        public ExpeditionStat(int success, int total)
        {
            Success = success;
            Total = total;
            WinPercentage = (int)Math.Round((double)success / total);
        }

        public ExpeditionStat(int success, int total, int percentage)
        {
            Success = success;
            Total = total;
            WinPercentage = percentage;
        }

        public bool Equals(ExpeditionStat other)
            => Success == other.Success
            && Total == other.Total;
    }
}
