using Newtonsoft.Json;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class HeavyDamageLinePreference : ModelBase
    {
        HeavyDamageLineType r_Type = HeavyDamageLineType.Default;
        [JsonProperty("color")]
        public HeavyDamageLineType Type
        {
            get { return r_Type; }
            set
            {
                if (r_Type != value)
                {
                    r_Type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        int r_Width = 3;
        [JsonProperty("width")]
        public int Width
        {
            get { return r_Width; }
            set
            {
                var rValue = value.Clamp(0, 22);
                if (r_Width != rValue)
                {
                    r_Width = rValue;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }
    }
}
