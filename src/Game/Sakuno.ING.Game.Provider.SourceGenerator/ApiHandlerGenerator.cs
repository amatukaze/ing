using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sakuno.ING.Game.Provider.SourceGenerator
{
    [Generator]
    internal class ApiHandlerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new ApiHandlerReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not ApiHandlerReceiver receiver)
                return;

            var sections = new List<SwitchSectionSyntax>();

            foreach (var info in receiver.CandidateApis)
            {
                var labels = SyntaxFactory.List<SwitchLabelSyntax>(info.Apis.Select(SyntaxFactory.CaseSwitchLabel));
                var statements = SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.Block(new StatementSyntax[]
                {
                    SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"),
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("deserialized")
                            .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.InvocationExpression(GetInitializer(info), SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("message")))))))))),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("CheckResultCode"), SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("deserialized")))))),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(info.MethodName), SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(GetArguments(info))))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)),
                }));

                sections.Add(SyntaxFactory.SwitchSection(labels, statements));

                ExpressionSyntax GetInitializer(ApiInfo info) => (info.HasRequest, info.ResponseSymbol) switch
                {
                    (false, var symbol) => SyntaxFactory.GenericName("Deserialize").AddTypeArgumentListArguments(SyntaxFactory.ParseTypeName(symbol!.ToDisplayString())),
                    (true, null) => SyntaxFactory.IdentifierName("DeserializeRequestOnly"),
                    (true, var symbol) => SyntaxFactory.GenericName("DeserializeWithRequest").AddTypeArgumentListArguments(SyntaxFactory.ParseTypeName(symbol!.ToDisplayString())),
                };
                IEnumerable<ArgumentSyntax> GetArguments(ApiInfo info)
                {
                    if (info.ShouldHandleApi)
                        yield return SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Api")));

                    if (info.HasRequest)
                        yield return SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("deserialized"), SyntaxFactory.IdentifierName("Request")));

                    if (info.ResponseSymbol is not null)
                        yield return SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("deserialized"), SyntaxFactory.IdentifierName("api_data")));
                }
            }

            var switchStatement = SyntaxFactory.SwitchStatement(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Api")))
                .AddSections(sections.ToArray());

            var handlerMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)), "HandleApiMessageCore")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("message")).WithType(SyntaxFactory.ParseTypeName("Sakuno.ING.Messaging.ApiMessage")))
                .WithBody(SyntaxFactory.Block(switchStatement, SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))));

            var @class = SyntaxFactory.ClassDeclaration("GameProvider")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(handlerMethod)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .WithTrailingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.DisableKeyword), true)));
            var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game"))
                .AddMembers(@class);

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(@namespace)
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                .NormalizeWhitespace();

            context.AddSource("GameProvider.g.cs", SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
        }
    }
}
