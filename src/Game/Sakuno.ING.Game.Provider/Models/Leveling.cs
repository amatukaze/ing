using System;

namespace Sakuno.ING.Game.Models
{
    public struct Leveling
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
    }
}
