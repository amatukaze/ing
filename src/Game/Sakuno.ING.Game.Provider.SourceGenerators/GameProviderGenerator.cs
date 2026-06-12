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
            var sourceMembers = new List<MemberDeclarationSyntax>();
            var classMembers = new List<MemberDeclarationSyntax>();

            foreach (var (name, type) in infos)
            {
                var sourceMethod = SyntaxFactory
                    .MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                        $"On{name}")
                    .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                        .WithType(SyntaxFactory.ParseTypeName(type)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                sourceMembers.Add(sourceMethod);

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

                classMembers.Add(subjectField);
                classMembers.Add(observableProperty);
                classMembers.Add(triggerMethod);
            }

            var sourceInterface = SyntaxFactory.InterfaceDeclaration("IGameProviderSource")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(sourceMembers.ToArray());
            var sourceNamespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game.Provider"))
                .AddMembers(sourceInterface);

            var sourceCompilationUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(sourceNamespace)
                .AddUsings(
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive"))
                )
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .NormalizeWhitespace();

            context.AddSource("IGameProviderSource.g.cs", SyntaxFactory.SyntaxTree(sourceCompilationUnit, encoding: Encoding.UTF8).GetText());

            var @class = SyntaxFactory.ClassDeclaration("GameProvider")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IGameProvider")),
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IGameProviderSource")))
                .AddMembers(classMembers.ToArray());
            var classNamespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game.Provider"))
                .AddMembers(@class);

            var classCompilationUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(classNamespace)
                .AddUsings(
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive.Linq")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive.Subjects"))
                )
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .NormalizeWhitespace();

            context.AddSource("GameProvider.g.cs", SyntaxFactory.SyntaxTree(classCompilationUnit, encoding: Encoding.UTF8).GetText());
        });
    }
}
