using System;
using System.Windows.Markup;
using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Views
{
    public class ResolveExtension : MarkupExtension
    {
        public ResolveExtension(Type targetType) => TargetType = targetType;

        [ConstructorArgument("targetType")]
        public Type TargetType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) => StaticResolver.Instance.Resolve(TargetType);
    }
}
