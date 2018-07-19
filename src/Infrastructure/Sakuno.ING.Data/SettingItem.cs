using System;
using System.Globalization;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Data
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

        public SettingItem(SettingsManager manager, string key, T defaultValue) : base(manager, key, Convert(defaultValue))
        {
            InitialValue = Convert(base.Value);
        }

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

        public new T Value
        {
            get => Convert(base.Value);
            set => base.Value = Convert(value);
        }

        public T InitialValue { get; }

        public event Action<T> ValueChanged;

        protected override void OnValueChanged(string value) => ValueChanged?.Invoke(Convert(value));
    }
}
