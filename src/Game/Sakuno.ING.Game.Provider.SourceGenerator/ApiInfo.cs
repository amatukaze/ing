using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Game.Provider.SourceGenerator
{
    internal class ApiInfo
    {
        public ExpressionSyntax Api { get; }
        public string MethodName { get; }

        public bool HasRequest { get; set; }
        public ISymbol? ResponseSymbol { get; set; }

        public ApiInfo(ExpressionSyntax api, string methodName)
        {
            Api = api;
            MethodName = methodName;
        }
    }
}
