using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Game.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class IdentifierTypeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName("Sakuno.ING.Game.Models.IdentifierAttribute",
            static (node, _) => node is StructDeclarationSyntax structDeclarationSyntax && structDeclarationSyntax.Modifiers.Any(modifier => modifier.Kind() is SyntaxKind.PartialKeyword),
            static (context, _) =>
            {
                var node = (StructDeclarationSyntax)context.TargetNode;
                var attributeData = context.Attributes.Single(attr => attr.AttributeClass!.Name is "IdentifierAttribute");
                var noToStringValue = attributeData.NamedArguments.SingleOrDefault(arg => arg.Key is "NoToString").Value;
                var noToString = noToStringValue.IsNull ? false : (bool)noToStringValue.Value!;

                return (node.Identifier.Text, ((BaseNamespaceDeclarationSyntax)node.Parent!).Name.ToString(), noToString);
            });

        context.RegisterSourceOutput(provider, (context, info) =>
        {
            const string ModelNamespace = "Sakuno.ING.Game.Models";
            var (typeName, @namespace, noToString) = info;

            var prefix = @namespace is ModelNamespace ? string.Empty : $"{@namespace.Substring(ModelNamespace.Length + 1)}.";

            context.AddSource($"{prefix}{typeName}.g.cs", $@"#nullable enable

namespace {@namespace};

public readonly partial struct {typeName} : IIdentifier<{typeName}, int>, IEquatable<{typeName}>, IComparable<{typeName}>
{{
    private readonly int _value;

    public bool IsValid => _value > 0;

    public {typeName}(int value) => _value = value;

    public int CompareTo({typeName} other) => _value - other._value;
    public bool Equals({typeName} other) => _value == other._value;

    public static bool operator ==({typeName} left, {typeName} right) => left._value == right._value;
    public static bool operator !=({typeName} left, {typeName} right) => left._value != right._value;
    public static implicit operator int({typeName} id) => id._value;
    public static explicit operator {typeName}(int value) => new(value);

    public static {typeName} From(int value) => new(value);

    public override bool Equals(object? obj) => obj is {typeName} other && other == this;
    public override int GetHashCode() => _value;
    {(!noToString ? "public override string ToString() => _value.ToString();" : string.Empty)}
}}");
        });
    }
}
