using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Provider.SourceGenerator
{
    internal class ApiHandlerReceiver : ISyntaxContextReceiver
    {
        private INamedTypeSymbol? _attributeSymbol;
        private INamedTypeSymbol? _nameValueCollectionSymbol;

        private readonly List<ApiInfo> _candidateApis = new();
        public IReadOnlyList<ApiInfo> CandidateApis => _candidateApis;

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            _attributeSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName("Sakuno.ING.Game.ApiAttribute");
            _nameValueCollectionSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName("System.Collections.Specialized.NameValueCollection");

            if (context.Node is not MethodDeclarationSyntax { Parent: ClassDeclarationSyntax { Identifier: { Text: "GameProvider" } } } methodDeclaration)
                return;

            ApiInfo? info = null;

            foreach (var attributeList in methodDeclaration.AttributeLists)
                foreach (var attribute in attributeList.Attributes)
                {
                    var symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);

                    if (!SymbolEqualityComparer.Default.Equals(symbolInfo.Symbol?.ContainingType, _attributeSymbol))
                        continue;

                    if (info is null)
                    {
                        info = new ApiInfo(methodDeclaration.Identifier.Text);

                        var parameters = new Queue<ITypeSymbol>(methodDeclaration.ParameterList.Parameters
                            .Select(r => context.SemanticModel.GetDeclaredSymbol(r)!.Type));

                        if (parameters.Peek().SpecialType is SpecialType.System_String)
                        {
                            info.ShouldHandleApi = true;
                            parameters.Dequeue();
                        }

                        if (parameters.Count is 2)
                        {
                            parameters.Dequeue();

                            info.HasRequest = true;
                            info.ResponseSymbol = parameters.Dequeue();
                        }
                        else
                        {
                            var parameterTypeSymbol = parameters.Dequeue();

                            if (SymbolEqualityComparer.Default.Equals(parameterTypeSymbol, _nameValueCollectionSymbol))
                                info.HasRequest = true;
                            else
                                info.ResponseSymbol = parameterTypeSymbol;
                        }

                        _candidateApis.Add(info);
                    }

                    var apiExpression = (LiteralExpressionSyntax)attribute.ArgumentList!.Arguments[0].Expression;

                    info.Apis.Add(apiExpression);
                }
        }
    }
}
