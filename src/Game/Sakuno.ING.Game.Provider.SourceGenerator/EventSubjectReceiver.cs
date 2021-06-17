using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Provider.SourceGenerator
{
    internal class EventSubjectReceiver : ISyntaxContextReceiver
    {
        private INamedTypeSymbol? _subjectSymbol;

        private readonly List<FieldDeclarationSyntax> _candidateFields = new();
        public IReadOnlyList<FieldDeclarationSyntax> CandidateFields => _candidateFields;

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            _subjectSymbol ??= context.SemanticModel.Compilation.GetTypeByMetadataName("System.Reactive.Subjects.Subject`1")?.ConstructUnboundGenericType();

            if (_subjectSymbol is null)
                return;

            if (context.Node is not FieldDeclarationSyntax { Parent: ClassDeclarationSyntax { Identifier: { Text: "GameProvider" } } } fieldDeclaration)
                return;

            if (context.SemanticModel.GetSymbolInfo(fieldDeclaration.Declaration.Type).Symbol is not INamedTypeSymbol { IsGenericType: true } symbol)
                return;

            if (SymbolEqualityComparer.Default.Equals(symbol.ConstructUnboundGenericType(), _subjectSymbol))
                _candidateFields.Add(fieldDeclaration);
        }
    }
}
