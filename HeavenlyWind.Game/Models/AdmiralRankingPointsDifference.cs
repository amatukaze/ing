using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class AdmiralRankingPointsDifference : ModelBase
    {
        AdmiralRankingPoints r_Owner;

        public AdmiralRankingPointsDifferenceType Type { get; }

        int r_AdmiralExperience;
        public int AdmiralExperience
        {
            get { return r_Owner.AdmiralExperience - r_AdmiralExperience; }
            internal set { r_AdmiralExperience = value; }
        }

        public double RankingPoints => AdmiralExperience * 7.0 / 10000.0;

        public AdmiralRankingPointsDifference(AdmiralRankingPoints rpOwner, AdmiralRankingPointsDifferenceType rpType)
        {
            r_Owner = rpOwner;

            Type = rpType;

            Reload();
        }
        internal void Reload()
        {
            const string FinalizationTime = "strftime('%s', 'now', '+7 hour', 'start of month', '+1 month', '-11 hour')";
            const string MonthlyUpdateTime = "strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')";
            const string DailyUpdateTime = "strftime('%s', 'now', '+7 hour', 'start of day', '-7 hour')";

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                switch (Type)
                {
                    case AdmiralRankingPointsDifferenceType.PreviousUpdate:
                        const string AggregateTime = "(CASE CAST(strftime('%H', 'now', '+7 hour') AS INTEGER) / 12 WHEN 1 THEN strftime('%s', 'now', '-5 hour', 'start of day', '+5 hour') ELSE " + DailyUpdateTime + " END)";
                        const string PreviousAggregate = "(CASE CAST(strftime('%s', 'now') AS INTEGER) < " + FinalizationTime + " WHEN 1 THEN " + AggregateTime + " ELSE " + FinalizationTime + " END)";
                        rCommand.CommandText = "SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < " + PreviousAggregate + "), (SELECT min(experience) FROM admiral_experience WHERE time >= " + PreviousAggregate + "), @current_exp);";
                        break;

                    case AdmiralRankingPointsDifferenceType.Day:
                        const string Daily = "(CASE CAST(strftime('%s', 'now') AS INTEGER) < " + FinalizationTime + " WHEN 1 THEN " + DailyUpdateTime + " ELSE " + FinalizationTime + " END)";
                        rCommand.CommandText = "SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < " + Daily + "), (SELECT min(experience) FROM admiral_experience WHERE time >= " + Daily + "), @current_exp);";
                        break;

                    case AdmiralRankingPointsDifferenceType.Month:
                        rCommand.CommandText = "SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < " + MonthlyUpdateTime + "), (SELECT min(experience) FROM admiral_experience WHERE time >= " + MonthlyUpdateTime + "), @current_exp);";
                        break;
                }
                rCommand.Parameters.AddWithValue("@current_exp", r_Owner.AdmiralExperience);

                r_AdmiralExperience = Convert.ToInt32(rCommand.ExecuteScalar());
            }
        }
    }
}
