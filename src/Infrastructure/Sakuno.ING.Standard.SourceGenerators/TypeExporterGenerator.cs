using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Sakuno.ING.Standard.SourceGenerators;

[Generator]
public class TypeExporterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName("Sakuno.ING.Composition.ExportAttribute",
            static (node, _) => node is ClassDeclarationSyntax,
            static (context, _) =>
            {
                var node = (ClassDeclarationSyntax)context.TargetNode;
                var attributeData = context.Attributes.Single(attr => attr.AttributeClass!.Name is "ExportAttribute");
                var arg = attributeData.ConstructorArguments.SingleOrDefault().Value;
                var singletonValue = attributeData.NamedArguments.SingleOrDefault(arg => arg.Key is "Singleton").Value;
                var singleton = singletonValue.IsNull ? true : (bool)singletonValue.Value!;

                return new ExportedTypeInfo(node, (ISymbol?)arg, singleton);
            });

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()), static (context, tuple) =>
        {
            var (compilation, exportedClasses) = tuple;

            if (exportedClasses.Length is 0)
                return;

            var calls = new List<StatementSyntax>();

            foreach (var (classDeclaration, contractTypeSymbol, isSingleton) in exportedClasses)
            {
                var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(classDeclaration) is not { } symbol)
                    continue;

                var name = SyntaxFactory.GenericName(isSingleton ? "AddSingleton" : "AddTransient");

                if (contractTypeSymbol is not null)
                    name = name.AddTypeArgumentListArguments(SyntaxFactory.ParseTypeName(contractTypeSymbol.ToDisplayString()));

                name = name.AddTypeArgumentListArguments(SyntaxFactory.ParseTypeName(symbol.ToDisplayString()));

                calls.Add(SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("services"), name))));
            }

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "AddServices")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("services"))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.ThisKeyword))
                    .WithType(SyntaxFactory.ParseTypeName("IServiceCollection")))
                .AddBodyStatements(calls.ToArray());
            var @class = SyntaxFactory.ClassDeclaration("ServicesRegistration")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddMembers(method);
            var @namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(compilation.AssemblyName!))
                .AddMembers(@class);
            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.Extensions.DependencyInjection")))
                .AddMembers(@namespace)
                .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)))
                .NormalizeWhitespace();

            context.AddSource($"{compilation.AssemblyName}.g.cs", SyntaxFactory.SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText(context.CancellationToken));
        });
    }
}
