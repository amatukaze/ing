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
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                switch (Type)
                {
                    case AdmiralRankingPointsDifferenceType.PreviousUpdate:
                        var rNow = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(9.0));
                        if (rNow.Hour >= 2 && rNow.Hour < 14)
                            rCommand.CommandText = "SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', 'start of day', '-7 hour')), (SELECT min(experience) FROM admiral_experience WHERE time >= strftime('%s', 'now', 'start of day', '-7 hour')), @current_exp);";
                        else
                            rCommand.CommandText = "SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', 'start of day', '+5 hour')), (SELECT min(experience) FROM admiral_experience WHERE time >= strftime('%s', 'now', 'start of day', '+5 hour')), @current_exp);";
                        break;

                    case AdmiralRankingPointsDifferenceType.Day:
                        rCommand.CommandText = "SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', 'start of day', '-7 hour')), (SELECT min(experience) FROM admiral_experience WHERE time >= strftime('%s', 'now', 'start of day', '-7 hour')), @current_exp);";
                        break;

                    case AdmiralRankingPointsDifferenceType.Month:
                        rCommand.CommandText = "SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', 'start of month', '-11 hour')), (SELECT min(experience) FROM admiral_experience WHERE time >= strftime('%s', 'now', 'start of month', '-11 hour')), @current_exp);";
                        break;
                }
                rCommand.Parameters.AddWithValue("@current_exp", r_Owner.AdmiralExperience);

                r_AdmiralExperience = Convert.ToInt32(rCommand.ExecuteScalar());
            }
        }
    }
}
