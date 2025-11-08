using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sakuno.ING.Game.SourceGenerator.Common;

public class MockAnalyzerConfigOptionsProvider(AnalyzerConfigOptions options) : AnalyzerConfigOptionsProvider
{
    public override AnalyzerConfigOptions GlobalOptions => options;

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => options;
    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => options;
}
