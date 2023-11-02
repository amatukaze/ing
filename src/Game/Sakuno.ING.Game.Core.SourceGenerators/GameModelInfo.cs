using Microsoft.CodeAnalysis;

namespace Sakuno.ING.Game.SourceGenerators;

internal record GameModelInfo(string ClassName, string Subnamespace, IReadOnlyList<string> Usings, string IdType, string RawType, IReadOnlyList<(string Type, string Name)> Properties)
{
    public static GameModelInfo Create(AdditionalText file, string className, string subNamespace, CancellationToken cancellationToken)
    {
        string? idType = null, rawType = null;

        var usings = new List<string>();
        var properties = new List<(string, string)>();

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

            if (parts[0] is "@id")
            {
                if (idType is not null)
                    throw new InvalidOperationException("@id has been declared");

                idType = parts[1];
                continue;
            }

            if (parts[0] is "@raw")
            {
                if (rawType is not null)
                    throw new InvalidOperationException("@raw has been declared");

                rawType = parts[1];
                continue;
            }

            properties.Add((parts[0], parts[1]));
        }

        if (idType is null)
            throw new InvalidOperationException("@id is missing");
        if (rawType is null)
            throw new InvalidOperationException("@raw is missing");

        return new(className, subNamespace, usings, idType, rawType, properties);
    }
}
