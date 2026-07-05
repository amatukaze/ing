using Microsoft.CodeAnalysis;

namespace Sakuno.ING.Game.Provider.Json.SourceGenerators;

internal record JsonModelInfo(string ClassName, string Subnamespace, IReadOnlyList<string> Usings, string? IdType, IReadOnlyList<string> Implementations, IReadOnlyList<(string Type, string Name)> Properties, IReadOnlyList<(string, string, string)> Mappings)
{
    public static JsonModelInfo Create(AdditionalText file, string className, string subNamespace, CancellationToken cancellationToken)
    {
        string? idType = null;
        var usings = new List<string>();
        var implementations = new List<string>();
        var properties = new List<(string, string)>();
        var mappings = new List<(string, string, string)>();

        foreach (var lineInfo in file.GetText(cancellationToken)!.Lines)
        {
            var line = lineInfo.ToString().Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] is "@using")
            {
                if (parts.Length < 2)
                    throw new InvalidOperationException("@using requires a namespace");

                usings.Add(parts[1]);
                continue;
            }

            if (parts[0] is "@id")
            {
                if (parts.Length < 2)
                    throw new InvalidOperationException("@id requires a type");

                idType = parts[1];
                properties.Add((idType, "id"));
                continue;
            }

            if (parts[0] is "@implements")
            {
                if (parts.Length < 2)
                    throw new InvalidOperationException("@implements requires a type name");

                implementations.Add(parts[1]);
                continue;
            }

            if (parts[0].StartsWith("@"))
                throw new InvalidOperationException($"Unknown directive: {parts[0]}");

            if (parts.Length < 2)
                throw new InvalidOperationException($"Property declaration must have a type and a name: '{line}'");

            var propertyType = parts[0];
            var propertyName = parts[1];
            properties.Add((propertyType, propertyName));

            for (var i = 2; i < parts.Length; i++)
            {
                var implParts = parts[i].Split('.');

                if (implParts.Length != 2)
                    throw new InvalidOperationException($"Mapping must be in the format 'Implementation.Property': '{parts[i]}'");

                mappings.Add((implParts[0], implParts[1], propertyName));
            }
        }

        return new(className, subNamespace, usings, idType, implementations, properties, mappings);
    }
}
