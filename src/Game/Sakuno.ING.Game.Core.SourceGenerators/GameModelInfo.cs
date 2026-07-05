using Microsoft.CodeAnalysis;

namespace Sakuno.ING.Game.SourceGenerators;

internal record GameModelInfo(string ClassName, string Subnamespace, IReadOnlyList<string> Usings, string IdType, string RawType, IReadOnlyList<(string Type, string Name)> Properties, string? PatchBaseType, IReadOnlyList<(string InterfaceName, IReadOnlyList<string> PropertyNames)> Patches)
{
    public static GameModelInfo Create(AdditionalText file, string className, string subNamespace, CancellationToken cancellationToken)
    {
        string? idType = null, rawType = null, patchBaseType = null;

        var usings = new List<string>();
        var properties = new List<(string, string)>();
        var patches = new List<(string, IReadOnlyList<string>)>();

        foreach (var lineInfo in file.GetText(cancellationToken)!.Lines)
        {
            var line = lineInfo.ToString().AsSpan().Trim();

            if (line.IsEmpty)
                continue;

            if (line[0] is '@')
            {
                ParseDirective(line, usings, ref idType, ref rawType, ref patchBaseType, patches);
                continue;
            }

            properties.Add(ParseProperty(line));
        }

        if (idType is null)
            throw new InvalidOperationException("@id is missing");
        if (rawType is null)
            throw new InvalidOperationException("@raw is missing");
        if (patches.Count > 0 && patchBaseType is null)
            throw new InvalidOperationException("@patch requires @patchbase");

        return new(className, subNamespace, usings, idType, rawType, properties, patchBaseType, patches);
    }

    private static void ParseDirective(ReadOnlySpan<char> line, List<string> usings, ref string? idType, ref string? rawType, ref string? patchBaseType, List<(string, IReadOnlyList<string>)> patches)
    {
        var separatorIndex = line.IndexOf(' ');
        var directive = separatorIndex is -1 ? line : line.Slice(0, separatorIndex);
        var argument = separatorIndex is -1 ? ReadOnlySpan<char>.Empty : line.Slice(separatorIndex + 1).TrimStart();

        if (directive.SequenceEqual("@using".AsSpan()))
        {
            if (argument.IsEmpty)
                throw new InvalidOperationException("@using requires a namespace");

            usings.Add(argument.ToString());
            return;
        }

        if (directive.SequenceEqual("@id".AsSpan()))
        {
            if (argument.IsEmpty)
                throw new InvalidOperationException("@id requires a type");
            if (idType is not null)
                throw new InvalidOperationException("@id has been declared");

            idType = argument.ToString();
            return;
        }

        if (directive.SequenceEqual("@raw".AsSpan()))
        {
            if (argument.IsEmpty)
                throw new InvalidOperationException("@raw requires a type");
            if (rawType is not null)
                throw new InvalidOperationException("@raw has been declared");

            rawType = argument.ToString();
            return;
        }

        if (directive.SequenceEqual("@patchbase".AsSpan()))
        {
            if (argument.IsEmpty)
                throw new InvalidOperationException("@patchbase requires an interface name");
            if (patchBaseType is not null)
                throw new InvalidOperationException("@patchbase has been declared");

            patchBaseType = argument.ToString();
            return;
        }

        if (directive.SequenceEqual("@patch".AsSpan()))
        {
            ParsePatchDirective(argument, patches);
            return;
        }

        throw new InvalidOperationException($"Unknown directive: {directive.ToString()}");
    }

    private static void ParsePatchDirective(ReadOnlySpan<char> argument, List<(string, IReadOnlyList<string>)> patches)
    {
        if (argument.IsEmpty)
            throw new InvalidOperationException("@patch requires a class name and at least one property name");

        var tokens = argument.ToString().Split([' '], StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length < 2)
            throw new InvalidOperationException("@patch requires a class name and at least one property name");

        var className = tokens[0];
        var propertyNames = tokens.Skip(1).ToArray();

        patches.Add((className, propertyNames));
    }

    private static (string Type, string Name) ParseProperty(ReadOnlySpan<char> line)
    {
        var tokens = line.ToString().Split([' '], StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length < 2)
            throw new InvalidOperationException($"Property declaration must have a type and a name: '{line.ToString()}'");

        var type = tokens[0];
        var name = tokens[1];

        return (type, name);
    }
}
