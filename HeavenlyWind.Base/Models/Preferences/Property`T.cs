using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class Property<T> : Property
    {
        public T Default { get; }

        T r_OldValue;
        T r_Value;
        public T Value
        {
            get { return r_Value; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(r_Value, value))
                {
                    SetDirectly(value);
                    NotifyUpdate();
                }
            }
        }
        public bool IsModified { get; private set; }

        public event EventHandler<PropertyValueChangedEventArgs<T>> ValueChanged = delegate { };

        public Property(string rpKey) : this(rpKey, default(T)) { }
        public Property(string rpKey, T rpDefaultValue) : base(rpKey)
        {
            Default = r_Value = rpDefaultValue;

            Instances.Add(rpKey, this);
        }

        public virtual void SetDirectly(T rpValue)
        {
            r_OldValue = r_Value;
            r_Value = rpValue;

            Save();
        }

        public void NotifyUpdate()
        {
            IsModified = !EqualityComparer<T>.Default.Equals(Default, r_Value);

            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(IsModified));

            ValueChanged(this, new PropertyValueChangedEventArgs<T>(r_OldValue, r_Value));
            r_OldValue = default(T);
        }

        internal override void Reload(object rpValue)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Object:
                    r_Value = JsonConvert.DeserializeObject<T>((string)rpValue);
                    break;

                case TypeCode.Boolean:
                    r_Value = (T)(object)Convert.ToBoolean(rpValue);
                    break;

                case TypeCode.Int16:
                    r_Value = (T)(object)Convert.ToInt16(rpValue);
                    break;
                case TypeCode.UInt16:
                    r_Value = (T)(object)Convert.ToUInt16(rpValue);
                    break;

                case TypeCode.Int32:
                    r_Value = (T)(object)Convert.ToInt32(rpValue);
                    break;
                case TypeCode.UInt32:
                    r_Value = (T)(object)Convert.ToUInt32(rpValue);
                    break;

                case TypeCode.String:
                    r_Value = rpValue != DBNull.Value ? (T)rpValue : (T)(object)string.Empty;
                    break;

                default:
                    r_Value = (T)rpValue;
                    break;
            }

            OnPropertyReloaded();
        }

        public override void Save()
        {
            using (var rCommand = Preference.Instance.Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT OR REPLACE INTO preference.preference(key, value) VALUES(@key, @value);";
                rCommand.Parameters.AddWithValue("@key", Key);

                object rValue;

                if (Type.GetTypeCode(typeof(T)) != TypeCode.Object)
                    rValue = r_Value;
                else
                    rValue = JsonConvert.SerializeObject(r_Value);

                rCommand.Parameters.AddWithValue("@value", rValue);

                rCommand.ExecuteNonQuery();
            }
        }

        internal override void SetValue(object rpValue) => SetDirectly((T)rpValue);

        public IDisposable Subscribe(Action<T> rpAction) => Subscribe(rpAction, false);
        public IDisposable Subscribe(Action<T> rpAction, bool rpInvokeFirst)
        {
            if (rpInvokeFirst)
                rpAction(r_Value);

            return new ValueChangedEventListener(this, rpAction);
        }

        public static implicit operator T(Property<T> rpProperty) => rpProperty.Value;

        public override string ToString() => Value?.ToString();

        class ValueChangedEventListener : IDisposable
        {
            Property<T> r_Owner;

            Action<T> r_Action;

            public ValueChangedEventListener(Property<T> rpOwner, Action<T> rpAction)
            {
                r_Owner = rpOwner;

                r_Action = rpAction;

                r_Owner.ValueChanged += OnValueChanged;
            }

            void OnValueChanged(object sender, PropertyValueChangedEventArgs<T> e) => r_Action(e.NewValue);

            public void Dispose()
            {
                r_Owner.ValueChanged -= OnValueChanged;

                Interlocked.Exchange(ref r_Action, null);
            }
        }
    }
}
