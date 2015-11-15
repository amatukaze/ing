using Sakuno.SystemInterop;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    public class Power : ModelBase
    {
        PowerSource r_Source;
        public PowerSource Source
        {
            get { return r_Source; }
            private set
            {
                if (r_Source != value)
                {
                    r_Source = value;
                    OnPropertyChanged(nameof(Source));
                }
            }
        }

        int r_BatteryRemainingPercentage;
        public int BatteryRemainingPercentage
        {
            get { return r_BatteryRemainingPercentage; }
            private set
            {
                if (r_BatteryRemainingPercentage != value)
                {
                    r_BatteryRemainingPercentage = value;
                    OnPropertyChanged(nameof(BatteryRemainingPercentage));
                }
            }
        }

        internal Power()
        {
            if (OS.IsWin7OrLater)
            {
                PowerMonitor.PowerSourceChanged += r => Source = r;
                PowerMonitor.BatteryRemainingPercentageChanged += r => BatteryRemainingPercentage = r;
            }
        }
    }
}
