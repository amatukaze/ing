using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.ING.Game.Provider.SourceGenerator
{
    [Generator]
    internal class ProviderEventGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new EventSubjectReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not EventSubjectReceiver receiver)
                return;

            var members = new List<MemberDeclarationSyntax>();

            foreach (var field in receiver.CandidateFields)
            {
                var symbol = (INamedTypeSymbol)context.Compilation.GetSemanticModel(field.SyntaxTree).GetSymbolInfo(field.Declaration.Type).Symbol!;
                var typeArgument = symbol.TypeArguments[0];

                var fieldName = field.Declaration.Variables[0].Identifier.Text;
                var observableType = SyntaxFactory.ParseTypeName($"IObservable<{typeArgument.ToDisplayString()}>");
                var fieldDeclaration = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.NullableType(observableType)))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                    .AddDeclarationVariables(SyntaxFactory.VariableDeclarator(fieldName + "Observable"));

                var propertyName = char.ToUpperInvariant(fieldName[1]) + fieldName.Substring(2);
                var propertyDeclaration = SyntaxFactory.PropertyDeclaration(observableType, propertyName)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.AssignmentExpression(SyntaxKind.CoalesceAssignmentExpression, SyntaxFactory.IdentifierName(fieldName + "Observable"),
                            SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(fieldName), SyntaxFactory.IdentifierName("AsObservable")))
                    )))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                members.Add(fieldDeclaration);
                members.Add(propertyDeclaration);
            }

            var @class = SyntaxFactory.ClassDeclaration("GameProvider")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(members.ToArray())
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .WithTrailingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.DisableKeyword), true)));
            var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game"))
                .AddMembers(@class);

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(@namespace)
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")), SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive.Linq")))
                .NormalizeWhitespace();

            context.AddSource("GameProvider.g.cs", SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
        }
    }
}
