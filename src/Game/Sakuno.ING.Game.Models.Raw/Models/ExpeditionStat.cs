using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct ExpeditionStat : IEquatable<ExpeditionStat>
    {
        public int Total { get; }
        public int Success { get; }
        public int Fail => Total - Success;
        public double WinPercentage { get; }

        public ExpeditionStat(int success, int total)
        {
            Success = success;
            Total = total;
            WinPercentage = (double)success / total;
        }

        public ExpeditionStat(int success, int total, double percentage)
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
