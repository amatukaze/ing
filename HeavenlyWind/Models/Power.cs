using Sakuno.SystemInterop;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    public class Power : ModelBase
    {
        public bool IsBatteryPresent { get; }

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

                IsOnExternalPower = value == PowerSource.AC;
            }
        }

        double r_BatteryRemainingPercentage;
        public double BatteryRemainingPercentage
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

        bool r_IsOnExternalPower;
        public bool IsOnExternalPower
        {
            get { return r_IsOnExternalPower; }
            private set
            {
                if (r_IsOnExternalPower != value)
                {
                    r_IsOnExternalPower = value;
                    OnPropertyChanged(nameof(IsOnExternalPower));
                }
            }
        }

        bool r_IsCharging;
        public bool IsCharging
        {
            get { return r_IsCharging; }
            private set
            {
                if (r_IsCharging != value)
                {
                    r_IsCharging = value;
                    OnPropertyChanged(nameof(IsCharging));
                }
            }
        }

        internal Power()
        {
            var rState = PowerManager.GetSystemBatteryState();

            IsBatteryPresent = rState.BatteryPresent;
            IsCharging = rState.Charging;

            PowerManager.PowerSourceChanged += r => Source = r;
            PowerManager.BatteryRemainingPercentageChanged += r => BatteryRemainingPercentage = r / 100.0;
            PowerManager.BatteryChargeStatusChanged += r => IsCharging = r;
        }
    }
}
