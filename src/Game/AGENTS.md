# AGENTS.md

## Source generator debugging

The `*.SourceGenerators` projects run only at compile time, so their output is
normally invisible. To inspect the generated code while debugging a generator,
temporarily enable compiler-generated file emission:

```bash
dotnet build -p:EmitCompilerGeneratedFiles=true
```

Generated files land under `obj/<Configuration>/<TFM>/generated/` (gitignored)
and can be read, diffed, or stepped through in the debugger. Two rules:

- This flag is a temporary debugging aid only — never commit it to a
  `.csproj`/`Directory.Build.props`, and clean up stale generated files
  (`dotnet clean` or `rm -rf obj/`) afterwards.
- The checked-in Verify snapshots under `*.SourceGenerators.Tests/Snapshots/`
  remain the source of truth for generator output — compare against them
  rather than eyeballing ad-hoc builds.

Generator behavior changes must still go through the Verify snapshot tests
(see the root `AGENTS.md`).
