using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    [JsonConverter(typeof(PreferencePropertyJsonConverter))]
    public class Property<T> : ModelBase
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

        static Property()
        {
            var rType = typeof(Property<T>);
            var rValueType = rType.GetGenericArguments()[0];
            switch (Type.GetTypeCode(rValueType))
            {
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                    throw new ArgumentException(rValueType.FullName);
            }

            var rValueProperty = rType.GetProperty(nameof(Value));

            var rPropertyParameter = Expression.Parameter(typeof(object), "rpProperty");
            var rConvertToPropertyType = Expression.Convert(rPropertyParameter, rType);

            var rGetter = Expression.Lambda(Expression.Call(rConvertToPropertyType, rValueProperty.GetMethod), rPropertyParameter).Compile();

            var rTokenParameter = Expression.Parameter(typeof(JToken), "rpToken");

            UnaryExpression rConvertToValueType;
            if (!rValueType.IsEnum)
                rConvertToValueType = Expression.Convert(rTokenParameter, rValueType);
            else
            {
                var rConvertToUnderlyingType = Expression.Convert(rTokenParameter, rValueType.GetEnumUnderlyingType());
                rConvertToValueType = Expression.Convert(rConvertToUnderlyingType, rValueType);
            }

            var rSetter = Expression.Lambda(Expression.Call(rConvertToPropertyType, rValueProperty.SetMethod, rConvertToValueType), rPropertyParameter, rTokenParameter).Compile();

            Property.Cache.TryAdd(rType, new Property(rGetter, rSetter));
        }
        public Property() { }
        public Property(T rpDefaultValue)
        {
            Default = r_Value = rpDefaultValue;
        }

        public virtual void SetDirectly(T rpValue)
        {
            r_OldValue = r_Value;
            r_Value = rpValue;
        }
        public void NotifyUpdate()
        {
            IsModified = !EqualityComparer<T>.Default.Equals(Default, r_Value);

            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(IsModified));

            ValueChanged(this, new PropertyValueChangedEventArgs<T>(r_OldValue, r_Value));
            r_OldValue = default(T);
        }

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
