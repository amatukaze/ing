using System;
using System.Globalization;
using Sakuno.KanColle.Amatsukaze.Settings;

namespace Sakuno.KanColle.Amatsukaze.Data
{
    internal class SettingItem<T> : SettingItemBase, ISettingItem<T>
    {
        static SettingItem()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                case TypeCode.Int32:
                case TypeCode.Single:
                case TypeCode.Boolean:
                    break;
                default:
                    throw new NotSupportedException($"{nameof(SettingItem<T>)} only supports int, bool, string, float, and int underlying enums. Using type {typeof(T)}");
            }
        }

        public SettingItem(SettingsManager manager, string key, T defaultValue) : base(manager, key, Convert(defaultValue)) { }

        private static string Convert(T value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    return (string)(object)value;
                case TypeCode.Int32:
                    return ((int)(object)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Single:
                    return ((float)(object)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Boolean:
                    return ((bool)(object)value).ToString(CultureInfo.InvariantCulture);
                default:
                    throw new NotSupportedException("How do you get here?");
            }
        }

        private static T Convert(string value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    return (T)(object)value;
                case TypeCode.Int32:
                    return (T)(object)int.Parse(value, CultureInfo.InvariantCulture);
                case TypeCode.Single:
                    return (T)(object)float.Parse(value, CultureInfo.InvariantCulture);
                case TypeCode.Boolean:
                    return (T)(object)bool.Parse(value);
                default:
                    throw new InvalidOperationException("How do you get here?");
            }
        }

        T ISettingItem<T>.Value
        {
            get => Convert(Value);
            set => Value = Convert(value);
        }
    }
}
