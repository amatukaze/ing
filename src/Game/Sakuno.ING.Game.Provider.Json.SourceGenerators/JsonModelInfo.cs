using Microsoft.CodeAnalysis;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators;

internal record JsonModelInfo(string ClassName, string Subnamespace, IReadOnlyList<string> Usings, IReadOnlyList<string> Implementations, IReadOnlyList<(string Type, string Name)> Properties, IReadOnlyList<(string, string, string)> Mappings)
{
    public static JsonModelInfo Create(AdditionalText file, string className, string subNamespace, CancellationToken cancellationToken)
    {
        var usings = new List<string>();
        var implementations = new List<string>();
        var properties = new List<(string, string)>();
        var mappings = new List<(string, string, string)>();

        foreach (var lineInfo in file.GetText(cancellationToken)!.Lines)
        {
            if (lineInfo.Span.IsEmpty)
                continue;

            var parts = lineInfo.ToString().Split(' ');

            if (parts[0] is "@using")
            {
                usings.Add(parts[1]);
                continue;
            }

            if (parts[0] is "@implements")
            {
                implementations.Add(parts[1]);
                continue;
            }

            properties.Add((parts[0], parts[1]));

            for (var i = 2; i < parts.Length; i++)
            {
                var implParts = parts[i].Split('.');

                mappings.Add((implParts[0], implParts[1], parts[1]));
            }
        }

        return new(className, subNamespace, usings, implementations, properties, mappings);
    }
}
