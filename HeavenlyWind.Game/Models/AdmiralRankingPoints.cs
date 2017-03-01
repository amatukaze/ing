using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Data.SQLite;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class AdmiralRankingPoints : ModelBase
    {
        Admiral r_Owner;

        int r_AdmiralID;

        public int AdmiralExperience => r_Owner.Experience;

        public double Initial { get; private set; }
        public int ExtraOperationBonus { get; private set; }
        public int TotalScore => (int)(Initial + ExtraOperationBonus + MonthDifference.RankingPoints);

        public AdmiralRankingPointsDifference PreviousUpdateDifference { get; private set; }
        public AdmiralRankingPointsDifference DayDifference { get; private set; }
        public AdmiralRankingPointsDifference MonthDifference { get; private set; }

        bool r_IsFinalized;
        public bool IsFinalized
        {
            get { return r_IsFinalized; }
            private set
            {
                if (r_IsFinalized != value)
                {
                    r_IsFinalized = value;
                    OnPropertyChanged();
                }
            }
        }

        int? r_FinalScore;
        public int? FinalScore
        {
            get { return r_FinalScore; }
            private set
            {
                if (r_FinalScore != value)
                {
                    r_FinalScore = value;
                    OnPropertyChanged();
                }
            }
        }

        internal AdmiralRankingPoints(Admiral rpAdmiral)
        {
            r_Owner = rpAdmiral;
            r_AdmiralID = rpAdmiral.ID;

            ApiService.SubscribeOnce("api_port/port", delegate
            {
                ReloadInitialRankingPoints();

                PreviousUpdateDifference = new AdmiralRankingPointsDifference(this, AdmiralRankingPointsDifferenceType.PreviousUpdate);
                DayDifference = new AdmiralRankingPointsDifference(this, AdmiralRankingPointsDifferenceType.Day);
                MonthDifference = new AdmiralRankingPointsDifference(this, AdmiralRankingPointsDifferenceType.Month);

                Update();

                var rNow = DateTimeOffset.Now;
                var rRankUpdateTime = rNow.ToOffset(TimeSpan.FromHours(7.0)).DateAsOffset();
                var rDayTimeSpan = TimeSpan.FromDays(1.0);

                Observable.Timer(rRankUpdateTime.AddDays(1.0), rDayTimeSpan).Subscribe(delegate
                {
                    PreviousUpdateDifference.Reload();
                    DayDifference.Reload();
                    OnPropertyChanged(nameof(PreviousUpdateDifference));
                    OnPropertyChanged(nameof(DayDifference));
                });

                rRankUpdateTime += TimeSpan.FromHours(12.0);
                if (DateTimeOffset.Now > rRankUpdateTime)
                    rRankUpdateTime += rDayTimeSpan;

                Observable.Timer(rRankUpdateTime, rDayTimeSpan).Subscribe(delegate
                {
                    PreviousUpdateDifference.Reload();
                    OnPropertyChanged(nameof(PreviousUpdateDifference));
                });

                var rNextStartTime = rNow.ToOffset(TimeSpan.FromHours(9.0)).StartOfNextMonth();
                Observable.Timer(rNextStartTime.AddHours(8.0)).Subscribe(_ => FinalScore = null);
                Observable.Timer(rNextStartTime.AddHours(-2.0)).Subscribe(_ => FinalizeThisMonth());
                Observable.Timer(rNextStartTime).Subscribe(delegate
                {
                    PreviousUpdateDifference.Reload();
                    DayDifference.Reload();
                    MonthDifference.Reload();

                    OnPropertyChanged(nameof(PreviousUpdateDifference));
                    OnPropertyChanged(nameof(DayDifference));
                    OnPropertyChanged(nameof(MonthDifference));

                    IsFinalized = false;
                });

                RecordService.Instance.Update += RecordService_Update;
            });
        }

        internal void Update()
        {
            if (r_AdmiralID != r_Owner.ID)
            {
                ReloadInitialRankingPoints();

                PreviousUpdateDifference.Reload();
                DayDifference.Reload();
                MonthDifference.Reload();

                r_AdmiralID = r_Owner.ID;
            }

            OnPropertyChanged(string.Empty);
        }

        void ReloadInitialRankingPoints()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT CASE strftime('%m', 'now', '+11 hours') == '01' WHEN 1 THEN 0 ELSE (coalesce(((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour')) - (SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+9 hour', 'start of year', '-9 hour'))), 0) / 50000.0) + ((SELECT coalesce(sum(point), 0) FROM ranking_point_bonus WHERE time >= strftime('%s', 'now', '+9 hour', 'start of month', '-1 month', '-9 hour') AND time < strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour')) / 35.0) END AS initial, " +
                    "(SELECT coalesce(sum(point), 0) FROM ranking_point_bonus WHERE time >= strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour') AND time < strftime('%s', 'now', '+9 hour', 'start of month', '+1 month', '-9 hour')) AS eo_bonus;";

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                    {
                        Initial = rReader.GetDouble("initial");
                        ExtraOperationBonus = rReader.GetInt32("eo_bonus");
                    }
            }
        }

        void RecordService_Update(UpdateEventArgs e)
        {
            if (r_IsFinalized || e.Database != "main" || e.Table != "ranking_point_bonus")
                return;

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT point FROM ranking_point_bonus WHERE sortie = @sortie;";
                rCommand.Parameters.AddWithValue("@sortie", e.RowId);

                ExtraOperationBonus += Convert.ToInt32(rCommand.ExecuteScalar());
                OnPropertyChanged(nameof(TotalScore));
            }
        }

        void FinalizeThisMonth()
        {
            IsFinalized = true;

            ExtraOperationBonus = 0;

            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT CASE strftime('%m', 'now', '+11 hours') == '01' WHEN 1 THEN 0 ELSE (coalesce(((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')) - (SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+9 hour', 'start of year', '-9 hour'))), 0) / 50000.0) + " +
                    "((SELECT coalesce(sum(point), 0) FROM ranking_point_bonus WHERE time >= strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour') AND time < strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')) / 35.0) END AS initial, " +
                    "(SELECT coalesce((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')), (SELECT min(experience) FROM admiral_experience WHERE time >= strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')), @current_exp)) AS admiral_experience, " +
                    "(CASE strftime('%m', 'now', '+11 hours') == '02' WHEN 1 THEN 0 ELSE (coalesce(((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour')) - (SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+9 hour', 'start of year', '-9 hour'))), 0) / 50000.0) END + " +
                    "(SELECT coalesce(sum(point), 0) FROM ranking_point_bonus WHERE time >= strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour') AND time < strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')) + " +
                    "(coalesce((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')), (SELECT min(experience) FROM admiral_experience WHERE time >= strftime('%s', 'now', '+11 hour', 'start of month', '-11 hour')), @current_exp) - coalesce((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour')), (SELECT min(experience) FROM admiral_experience WHERE time >= strftime('%s', 'now', '+9 hour', 'start of month', '-9 hour')), 0)) * 7.0 / 10000.0) AS final_score;";
                rCommand.Parameters.AddWithValue("@current_exp", AdmiralExperience);

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                    {
                        Initial = rReader.GetDouble("initial");

                        var rAdmiralExperience = rReader.GetInt32("admiral_experience");
                        PreviousUpdateDifference.AdmiralExperience = rAdmiralExperience;
                        DayDifference.AdmiralExperience = rAdmiralExperience;
                        MonthDifference.AdmiralExperience = rAdmiralExperience;

                        FinalScore = (int)rReader.GetDouble("final_score");
                    }
            }

            OnPropertyChanged(string.Empty);
        }
    }
}
