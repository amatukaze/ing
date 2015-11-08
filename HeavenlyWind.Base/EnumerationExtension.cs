using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Sakuno.KanColle.Amatsukaze
{
    public class EnumerationExtension : MarkupExtension
    {
        public Type Type { get; }

        public EnumerationExtension(Type rpEnumType)
        {
            if (rpEnumType == null)
                throw new ArgumentNullException(nameof(rpEnumType));
            if (!rpEnumType.IsEnum)
                throw new ArgumentException("Type must be an Enum.");

            Type = rpEnumType;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider) => new Binding() { Source = Enum.GetValues(Type) }.ProvideValue(rpServiceProvider);
    }
}
