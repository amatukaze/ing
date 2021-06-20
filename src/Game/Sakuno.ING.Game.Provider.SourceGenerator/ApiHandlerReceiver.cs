using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

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

            foreach (var attributeList in methodDeclaration.AttributeLists)
                foreach (var attribute in attributeList.Attributes)
                {
                    var symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);

                    if (!SymbolEqualityComparer.Default.Equals(symbolInfo.Symbol?.ContainingType, _attributeSymbol))
                        continue;

                    var apiExpression = (LiteralExpressionSyntax)attribute.ArgumentList!.Arguments[0].Expression;
                    var firstParameter = methodDeclaration.ParameterList.Parameters[0];

                    var info = new ApiInfo(apiExpression, methodDeclaration.Identifier.Text);

                    if (methodDeclaration.ParameterList.Parameters.Count is 2)
                    {
                        info.HasRequest = true;
                        info.ResponseSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration.ParameterList.Parameters[1])!.Type;
                    }
                    else
                    {
                        var parameterTypeSymbol = context.SemanticModel.GetDeclaredSymbol(firstParameter)!.Type;

                        if (SymbolEqualityComparer.Default.Equals(parameterTypeSymbol, _nameValueCollectionSymbol))
                            info.HasRequest = true;
                        else
                            info.ResponseSymbol = parameterTypeSymbol;
                    }

                    _candidateApis.Add(info);
                }
        }
    }
}
