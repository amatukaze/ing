# AGENTS.md

## Source generators

Models are authored via small directive-based descriptor files — copy
the nearest existing example as the starting point.

- `Sakuno.ING.Game.Core.SourceGenerators` → `Game.Core`
  - `GameModelGenerator`: consumes `*.modeldesc` files under
    `Models/Metadata/` and generates the model class, a `Patch`
    dispatch method, and the patch records declared by `@patch` under
    `Sakuno.ING.Game.Patches` (the `@patchbase` interface, e.g.
    `IFleetPatched`, is hand-written in `Sakuno.ING.Game.Events`).
    File name = class name; subdirectories under `Models/Metadata/`
    map to sub-namespaces of `Sakuno.ING.Game.Models`. Directives:
    `@using`, `@id <IdentifierType>`, `@raw <raw JSON interface>`,
    `@patchbase <base interface>`, `@patch <record> <properties...>`.
    Example: `Models/Metadata/Fleet.modeldesc`.
  - `IdentifierTypeGenerator`: picks up partial structs annotated with
    `[Identifier]` under `Sakuno.ING.Game.Models` and generates the
    int-backed identifier struct (`Empty`, `From`, equality/comparison,
    and `ToString` unless `NoToString = true`). Example:
    `Models/FleetId.cs`.
- `Sakuno.ING.Game.Provider.SourceGenerators` → `Game.Provider`
  - `GameProviderGenerator`: scans `IGameProvider` for
    `IObservable<T>` properties and generates `IGameProviderSource.g.cs`
    plus the subject-backed `GameProvider.g.cs`. Fully automatic —
    declaring the observable on `IGameProvider` (see `IGameProvider.cs`)
    is all that's needed.
  - `ApiHandlerGenerator`: picks up `[Api("api/path")]` methods — by
    convention private handlers in `partial class EventDispatcher` — and
    generates the dispatching `EventDispatcher.g.cs`. Handler
    parameters: an optional JSON response parameter (at most one, its
    type name determines the `SvData...` wrapper), an optional `string`
    API parameter, an optional `NameValueCollection` request body, and
    `[FromRequest("name")]` query parameters of type `int`, `bool`,
    `string`, or any `*Id` (arrays only for `int` and `*Id`). One
    method may carry multiple `[Api]` attributes. Example:
    `EventDispatcher.Homeport.cs`.
- `Sakuno.ING.Game.Provider.Json.SourceGenerators` → `Game.Provider.Json`
  - `JsonModelGenerator`: consumes `*.jsondesc` files under
    `Metadata/` and generates the JSON DTO type mapped onto the
    implementation interface. Directives: `@using`,
    `@id <IdentifierType>`, `@implements <interface>`; each property
    line is
    `rawType rawName [Interface.Member]`, where the optional third
    column maps onto the implementation member. Example:
    `Metadata/RawRepairDock.jsondesc`.
- `Sakuno.ING.Game.SourceGenerator.Common` — shared helpers
  (in-memory additional texts, mock analyzer config options) used only
  by the generator test projects.

## Source generator debugging

To inspect generated code while debugging a generator, temporarily enable
compiler-generated file emission:

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

