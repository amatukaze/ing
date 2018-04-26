using System;
using System.Windows.Markup;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Views
{
    public class ResolveExtension : MarkupExtension
    {
        public ResolveExtension(Type targetType) => TargetType = targetType;

        [ConstructorArgument("targetType")]
        public Type TargetType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) => StaticResolver.Instance.Resolve(TargetType);
    }
}
