using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Data.SQLite;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

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

        public int? FinalScore { get; private set; }

        internal AdmiralRankingPoints(Admiral rpAdmiral)
        {
            r_Owner = rpAdmiral;
            r_AdmiralID = rpAdmiral.ID;

            SessionService.Instance.SubscribeOnce("api_port/port", delegate
            {
                ReloadInitialRankingPoints();

                PreviousUpdateDifference = new AdmiralRankingPointsDifference(this, AdmiralRankingPointsDifferenceType.PreviousUpdate);
                DayDifference = new AdmiralRankingPointsDifference(this, AdmiralRankingPointsDifferenceType.Day);
                MonthDifference = new AdmiralRankingPointsDifference(this, AdmiralRankingPointsDifferenceType.Month);

                Update();

                var rRankUpdateTime = new DateTimeOffset(DateTimeOffset.Now.Date, TimeSpan.FromHours(6.0));
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

                var rNow = DateTimeOffset.Now;
                var rFinalizationTime = new DateTimeOffset(rNow.Year, rNow.Month, 1, 0, 0, 0, TimeSpan.FromHours(9.0)).AddMonths(1).AddHours(-2.0);
                if (rNow >= rFinalizationTime)
                    FinalizeThisMonth();
                else
                {
                    Observable.Return(Unit.Default).Delay(rFinalizationTime).Subscribe(delegate
                    {
                        FinalizeThisMonth();

                        ReloadInitialRankingPoints();

                        PreviousUpdateDifference.Reload();
                        DayDifference.Reload();
                        MonthDifference.Reload();

                        Update();
                    });
                }

                Observable.Return(Unit.Default).Delay(rFinalizationTime.AddHours(2.0)).Subscribe(_ => r_IsFinalized = false);

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

            OnPropertyChanged(nameof(PreviousUpdateDifference));
            OnPropertyChanged(nameof(DayDifference));
            OnPropertyChanged(nameof(MonthDifference));

            OnPropertyChanged(nameof(TotalScore));
        }

        void ReloadInitialRankingPoints()
        {
            using (var rCommand = RecordService.Instance.CreateCommand())
            {
                rCommand.CommandText = "SELECT (coalesce(((SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', 'start of month', '-9 hour')) - (SELECT max(experience) FROM admiral_experience WHERE time < strftime('%s', 'now', '+9 hour', 'start of year', '-9 hour'))), 0) / 50000.0) + " +
                    "((SELECT coalesce(sum(point), 0) FROM ranking_point_bonus WHERE time >= strftime('%s', 'now', 'start of month', '-1 month', '-9 hour') AND time < strftime('%s', 'now', 'start of month', '-11 hour')) / 35.0) AS initial, " +
                    "(SELECT coalesce(sum(point), 0) FROM ranking_point_bonus WHERE time >= strftime('%s', 'now', 'start of month', '-9 hour') AND time < strftime('%s', 'now', 'start of month', '+1 month', '-11 hour')) AS eo_bonus;";

                using (var rReader = rCommand.ExecuteReader())
                    if (rReader.Read())
                    {
                        Initial = Convert.ToDouble(rReader["initial"]);
                        ExtraOperationBonus = Convert.ToInt32(rReader["eo_bonus"]);
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
            r_IsFinalized = true;

            FinalScore = TotalScore;
            OnPropertyChanged(nameof(FinalScore));
        }
    }
}
