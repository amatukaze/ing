using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct Leveling : IComparable<Leveling>, IEquatable<Leveling>
    {
        public int Level { get; }

        public int Experience { get; }

        public int CurrentLevelExperience { get; }
        public int NextLevelExperience { get; }
        public int ExperienceRemaining => Math.Max(NextLevelExperience - Experience, 0);

        public bool IsMaxLevel { get; }

        public Leveling(int level)
        {
            Level = level;
            Experience = 0;
            CurrentLevelExperience = 0;
            NextLevelExperience = 0;
            IsMaxLevel = false;
        }

        public Leveling(int level, int experience, int currentLevelExperience, int nextLevelExperience, bool isMaxLevel)
        {
            Level = level;
            Experience = experience;
            CurrentLevelExperience = currentLevelExperience;
            NextLevelExperience = nextLevelExperience;
            IsMaxLevel = isMaxLevel;
        }

        public int CompareTo(Leveling other)
        {
            if (Level > other.Level) return 1;
            else if (Level < other.Level) return -1;
            else if (Experience > other.Experience) return 1;
            else if (Experience < other.Experience) return -1;
            else return 0;
        }

        public override bool Equals(object obj) => obj is Leveling leveling && Equals(leveling);
        public bool Equals(Leveling other)
            => Level == other.Level
            && Experience == other.Experience
            && CurrentLevelExperience == other.CurrentLevelExperience
            && NextLevelExperience == other.NextLevelExperience
            && IsMaxLevel == other.IsMaxLevel;

        public static bool operator ==(Leveling left, Leveling right) => left.Equals(right);
        public static bool operator !=(Leveling left, Leveling right) => !(left == right);

        public override int GetHashCode()
        {
            var hashCode = 50065545;
            hashCode = hashCode * -1521134295 + Level.GetHashCode();
            hashCode = hashCode * -1521134295 + Experience.GetHashCode();
            hashCode = hashCode * -1521134295 + CurrentLevelExperience.GetHashCode();
            hashCode = hashCode * -1521134295 + NextLevelExperience.GetHashCode();
            hashCode = hashCode * -1521134295 + IsMaxLevel.GetHashCode();
            return hashCode;
        }
    }
}
