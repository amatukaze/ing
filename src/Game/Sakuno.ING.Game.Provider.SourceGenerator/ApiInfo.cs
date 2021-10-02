using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Provider.SourceGenerator
{
    internal class ApiInfo
    {
        public IList<LiteralExpressionSyntax> Apis { get; } = new List<LiteralExpressionSyntax>();
        public string MethodName { get; }

        public bool ShouldHandleApi { get; set; }
        public bool HasRequest { get; set; }
        public ISymbol? ResponseSymbol { get; set; }

        public ApiInfo(string methodName)
        {
            MethodName = methodName;
        }
    }
}
