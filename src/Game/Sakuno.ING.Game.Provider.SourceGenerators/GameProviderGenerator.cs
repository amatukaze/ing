using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Game.Provider.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class GameProviderGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is PropertyDeclarationSyntax
            {
                Parent: InterfaceDeclarationSyntax { Identifier.Text: "IGameProvider" },
                Type: GenericNameSyntax { Identifier.Text: "IObservable" },
            },
            static (context, cancellationToken) =>
            {
                var property = (PropertyDeclarationSyntax)context.Node;
                var name = property.Identifier.Text;
                var symbol = (ITypeSymbol)context.SemanticModel.GetSymbolInfo(((GenericNameSyntax)property.Type).TypeArgumentList.Arguments[0], cancellationToken).Symbol!;

                return (name, symbol.ToDisplayString());
            }).Collect();

        context.RegisterSourceOutput(provider, (context, infos) =>
        {
            var members = new List<MemberDeclarationSyntax>();

            foreach (var (name, type) in infos)
            {
                var fieldName = $"_{char.ToLowerInvariant(name[0])}{name.Substring(1)}";
                var observableType = SyntaxFactory.ParseTypeName($"IObservable<{type}>");

                var subjectField = SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.ParseTypeName($"Subject<{type}>"),
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(fieldName).WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.ImplicitObjectCreationExpression())))))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
                var observableProperty = SyntaxFactory.PropertyDeclaration(observableType, name)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.AssignmentExpression(SyntaxKind.CoalesceAssignmentExpression, SyntaxFactory.FieldExpression(),
                            SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(fieldName), SyntaxFactory.IdentifierName("AsObservable")))
                    )))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                var triggerMethod = SyntaxFactory
                    .MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                        $"On{name}")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                        .WithType(SyntaxFactory.ParseTypeName(type)))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName(fieldName), SyntaxFactory.IdentifierName("OnNext"))
                        ).AddArgumentListArguments(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("value")))))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                members.Add(subjectField);
                members.Add(observableProperty);
                members.Add(triggerMethod);
            }

            var @class = SyntaxFactory.ClassDeclaration("GameProvider")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(members.ToArray());
            var @namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game.Provider"))
                .AddMembers(@class);

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(@namespace)
                .AddUsings(
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive.Linq")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive.Subjects"))
                )
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .NormalizeWhitespace();

            context.AddSource("GameProvider.g.cs", SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
        });
    }
}
