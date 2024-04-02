using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sakuno.ING.Standard.SourceGenerators;

public record ExportedTypeInfo(ClassDeclarationSyntax classDeclaration, ISymbol? contractTypeSymbol, bool isSingleton);
