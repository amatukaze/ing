using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Game.Provider.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class ApiHandlerGenerator : IIncrementalGenerator
{
    private record ApiHandlerInfo(ImmutableArray<string> Apis, string MethodName, ImmutableArray<HandlerParameter> Parameters, string ResponseDataTypeName)
    {
        public IEnumerable<StatementSyntax> GenerateQueryStringParsing()
        {
            if (!Parameters.OfType<RequestQueryParameter>().Any())
                yield break;

            foreach (var parameter in Parameters.OfType<RequestQueryParameter>())
                yield return SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName(parameter.TypeName),
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(parameter.Name)
                        .WithInitializer(SyntaxFactory.EqualsValueClause(parameter.TypeName switch
                        {
                            "string" => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("string"), SyntaxFactory.IdentifierName("Empty")),
                            var s when s.EndsWith("[]") => SyntaxFactory.CollectionExpression(),
                            _ => SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression),
                        })))));

            var invocation = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("message"),
                    SyntaxFactory.IdentifierName("EnumerateRequestQueryString")),
                SyntaxFactory.ArgumentList());
            var block = SyntaxFactory.Block(GenerateParsingCore());

            yield return SyntaxFactory.ForEachStatement(SyntaxFactory.ParseTypeName("var"), "item", invocation, block);
        }

        private IfStatementSyntax GenerateParsingCore()
        {
            IfStatementSyntax? result = default;

            foreach (var parameter in Parameters.OfType<RequestQueryParameter>().Reverse())
            {
                var condition = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("item"), SyntaxFactory.IdentifierName("Name")),
                        SyntaxFactory.IdentifierName("SequenceEqual")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.Utf8StringLiteralExpression,
                            SyntaxFactory.ParseToken($"\"api_{parameter.SourceName}\"u8"))))));
                var method = parameter.TypeName switch
                {
                    "int" => (SimpleNameSyntax)SyntaxFactory.IdentifierName("DecodeValueAsInt"),
                    "int[]" => SyntaxFactory.IdentifierName("DecodeValueAsIntArray"),
                    "string" => SyntaxFactory.IdentifierName("DecodeValueAsString"),

                    var type when type.EndsWith("Id") => SyntaxFactory.GenericName(SyntaxFactory.Identifier("DecodeValueAsIdentifier"),
                        SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.ParseTypeName(parameter.TypeName)))),
                    var type when type.EndsWith("Id[]") => SyntaxFactory.GenericName(SyntaxFactory.Identifier("DecodeValueAsIdentifierArray"),
                        SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.ParseTypeName(parameter.TypeName.Remove(parameter.TypeName.Length - 2))))),

                    _ => throw new InvalidOperationException(),
                };
                var assignment = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(parameter.Name),
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("item"), method))));
                var statement = SyntaxFactory.IfStatement(condition, assignment);

                if (result is null)
                    result = statement;
                else
                    result = statement.WithElse(SyntaxFactory.ElseClause(result));

            }

            Debug.Assert(result is not null);
            return result!;
        }

        public ExpressionSyntax GenerateInitializer()
        {
            var deserializeMethod = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("JsonSerializer"), SyntaxFactory.IdentifierName("Deserialize"));
            var messageResponse = SyntaxFactory.RefExpression(SyntaxFactory.Token(SyntaxKind.RefKeyword), SyntaxFactory.IdentifierName("reader"));
            var context = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("JsonModelContext"), SyntaxFactory.IdentifierName("Default"));
            var typeInfo = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, context, SyntaxFactory.IdentifierName(ResponseDataTypeName));

            return SyntaxFactory.PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, SyntaxFactory.InvocationExpression(deserializeMethod, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
            [
                SyntaxFactory.Argument(messageResponse),
                SyntaxFactory.Argument(typeInfo)
            ]))));
        }

        public IEnumerable<ArgumentSyntax> GenerateHandlerArguments()
        {
            foreach (var parameterKind in Parameters)
                yield return SyntaxFactory.Argument(parameterKind switch
                {
                    ApiParameter => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Api")),
                    NameValueCollectionParameter => SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("ParseRequest"),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("message"), SyntaxFactory.IdentifierName("Request")))))),
                    RequestQueryParameter query => SyntaxFactory.IdentifierName(query.Name),
                    ResponseParameter => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("response"), SyntaxFactory.IdentifierName("api_data")),

                    _ => throw new InvalidOperationException(),
                });
        }
    }

    private abstract record HandlerParameter;
    private record ApiParameter : HandlerParameter;
    private record NameValueCollectionParameter : HandlerParameter;
    private record RequestQueryParameter(string Name, string TypeName, string SourceName) : HandlerParameter;
    private record ResponseParameter : HandlerParameter;

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
                var parameters = new HandlerParameter[method.ParameterList.Parameters.Count];
                var responseDataType = default(ITypeSymbol);

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterNode = method.ParameterList.Parameters[i];
                    var parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parameterNode)!;
                    var parameterType = parameterSymbol.Type;

                    var fromRequestAttribute = parameterSymbol.GetAttributes()
                        .SingleOrDefault(attribute => attribute.AttributeClass!.ToDisplayString() is "Sakuno.ING.Game.Provider.FromRequestAttribute");
                    if (fromRequestAttribute is not null)
                    {
                        var attributeArgument = (string)fromRequestAttribute.ConstructorArguments[0].Value!;

                        parameters[i] = new RequestQueryParameter(parameterNode.Identifier.Text, parameterType.ToDisplayString(), attributeArgument);
                        continue;
                    }

                    if (parameterType.SpecialType is SpecialType.System_String)
                    {
                        parameters[i] = new ApiParameter();
                        continue;
                    }

                    if (parameterType.ToDisplayString() is "System.Collections.Specialized.NameValueCollection")
                    {
                        parameters[i] = new NameValueCollectionParameter();
                        continue;
                    }

                    responseDataType = parameterType;
                    parameters[i] = new ResponseParameter();
                }

                var responseDataTypeName = "SvData" + (responseDataType is not IArrayTypeSymbol arrayTypeSymbol
                    ? responseDataType?.Name ?? string.Empty
                    : arrayTypeSymbol.ElementType.Name + "Array");

                return new ApiHandlerInfo(apis, methodName, ImmutableArray.Create(parameters), responseDataTypeName);
            }).Collect();

        context.RegisterSourceOutput(provider, (context, infos) =>
        {
            var sections = new List<SwitchSectionSyntax>(infos.Length);

            foreach (var info in infos)
            {
                var labels = SyntaxFactory.List<SwitchLabelSyntax>(info.Apis.Select(api => SyntaxFactory.CaseSwitchLabel(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(api)))));
                var statements = SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.Block((StatementSyntax[])
                [
                    ..info.GenerateQueryStringParsing(),
                    SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var"),
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("reader")
                            .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.ParseTypeName("Utf8JsonReader"),
                                SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("message"),
                                        SyntaxFactory.IdentifierName("Response"))))),
                                null)))))),
                    SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var"),
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("response")
                            .WithInitializer(SyntaxFactory.EqualsValueClause(info.GenerateInitializer()))))),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName("CheckResultCode"),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("response"),
                                SyntaxFactory.IdentifierName("api_result"))))))),
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(info.MethodName),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(info.GenerateHandlerArguments())))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
                ]));

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
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Sakuno.ING.Game")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Sakuno.ING.Game.Models"))
                )
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .NormalizeWhitespace();

            context.AddSource("GameProvider.g.cs", SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
        });
    }
}
