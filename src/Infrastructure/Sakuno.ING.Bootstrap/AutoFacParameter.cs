using System;
using System.Reflection;
using Autofac;
using Autofac.Core;

namespace Sakuno.ING.Bootstrap
{
    public class AutoFacParameter<T> : Parameter
    {
        public AutoFacParameter(T value) => Value = value;

        public T Value { get; }
        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            if (pi.ParameterType.IsAssignableFrom(typeof(T)))
            {
                valueProvider = () => Value;
                return true;
            }
            else
            {
                valueProvider = null;
                return false;
            }
        }
    }
}
