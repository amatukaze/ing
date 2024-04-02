using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Sakuno.ING.Game.Provider.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class ApiHandlerGenerator : IIncrementalGenerator
{
    private record ApiHandlerInfo(ImmutableArray<string> Apis, string MethodName, ParameterKind[] ParameterKinds, ITypeSymbol ResponseDataType)
    {
        public ExpressionSyntax GenerateInitializer()
        {
            var deserializeMethod = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("JsonSerializer"), SyntaxFactory.IdentifierName("Deserialize"));
            var messageResponse = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Response"));
            var responseTypeName = ParameterKinds.Any(parameter => parameter is ParameterKind.ResponseData) ?
                ("SvData" + (ResponseDataType is not IArrayTypeSymbol arrayTypeSymbol ? ResponseDataType.Name : arrayTypeSymbol.ElementType.Name + "Array")) :
                "SvData";
            var context = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("_jsonModelContext"), SyntaxFactory.IdentifierName(responseTypeName));

            return SyntaxFactory.PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, SyntaxFactory.InvocationExpression(deserializeMethod, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(messageResponse),
                SyntaxFactory.Argument(context),
            }))));
        }

        public IEnumerable<ArgumentSyntax> GenerateHandlerArguments()
        {
            foreach (var parameterKind in ParameterKinds)
                yield return SyntaxFactory.Argument(parameterKind switch
                {
                    ParameterKind.Api => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Api")),
                    ParameterKind.RequestParams => SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("ParseRequest"),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Request")))))),
                    ParameterKind.ResponseData => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("response"), SyntaxFactory.IdentifierName("api_data")),

                    _ => throw new InvalidOperationException(),
                });
        }
    }

    private enum ParameterKind
    {
        Api,
        RequestParams,
        ResponseData,
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName("Sakuno.ING.Game.Provider.ApiAttribute",
            static (node, _) => node is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax { Identifier.Text: "GameProvider" } },
            static (context, _) =>
            {
                var apis = ImmutableArray.CreateRange(context.Attributes
                    .Where(attribute => attribute.AttributeClass!.ToDisplayString() is "Sakuno.ING.Game.Provider.ApiAttribute")
                    .Select(attribute => (string)attribute.ConstructorArguments[0].Value!));

                var method = (MethodDeclarationSyntax)context.TargetNode;
                var methodName = method.Identifier.Text;
                var parameters = new ParameterKind[method.ParameterList.Parameters.Count];
                var responseDataType = default(ITypeSymbol);

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterType = context.SemanticModel.GetDeclaredSymbol(method.ParameterList.Parameters[i])!.Type;

                    if (parameterType.SpecialType is SpecialType.System_String)
                    {
                        parameters[i] = ParameterKind.Api;
                        continue;
                    }

                    if (parameterType.ToDisplayString() is "System.Collections.Specialized.NameValueCollection")
                    {
                        parameters[i] = ParameterKind.RequestParams;
                        continue;
                    }

                    responseDataType = parameterType;
                    parameters[i] = ParameterKind.ResponseData;
                }

                return new ApiHandlerInfo(apis, methodName, parameters, responseDataType!);
            }).Collect();

        context.RegisterSourceOutput(provider, (context, infos) =>
        {
            var sections = new List<SwitchSectionSyntax>(infos.Length);

            foreach (var info in infos)
            {
                var labels = SyntaxFactory.List<SwitchLabelSyntax>(info.Apis.Select(api => SyntaxFactory.CaseSwitchLabel(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(api)))));
                var statements = SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.Block(
                    SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"),
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("response")
                        .WithInitializer(SyntaxFactory.EqualsValueClause(info.GenerateInitializer()))))),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("CheckResultCode"),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("response"), SyntaxFactory.IdentifierName("api_result"))))))),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(info.MethodName), SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(info.GenerateHandlerArguments())))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
                ));

                sections.Add(SyntaxFactory.SwitchSection(labels, statements));
            }

            var switchStatement = SyntaxFactory.SwitchStatement(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Api")))
                .AddSections(sections.ToArray());

            var handlerMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)), "HandleApiMessageCore")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("message")).WithType(SyntaxFactory.ParseTypeName("ApiMessage")))
                .WithBody(SyntaxFactory.Block(switchStatement, SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))));

            var @class = SyntaxFactory.ClassDeclaration("GameProvider")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(handlerMethod);
            var @namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("Sakuno.ING.Game.Provider"))
                .AddMembers(@class);

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(@namespace)
                .AddUsings(
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Text.Json")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Sakuno.ING.Messaging"))
                )
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .NormalizeWhitespace();

            context.AddSource("GameProvider.g.cs", SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
        });
    }
}
