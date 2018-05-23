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

        internal static IResolver Resolver;

        public override object ProvideValue(IServiceProvider serviceProvider) => Resolver?.Resolve(TargetType);
    }
}
