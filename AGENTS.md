# AGENTS.md

## Project

ING — a KanColle tool/browser, built using Avalonia 12 + ReactiveUI on .NET.

## Commands

- Build: `dotnet build`
- Test: `dotnet test`
- Run the desktop client: `dotnet run --project src/Shell/Sakuno.ING.Shell.Launcher`

## Repository layout

`src/` is grouped by architectural layer; every project is `Sakuno.ING.*` with its
test project (`.Tests`) alongside.

- `src/Infrastructure/Sakuno.ING.Standard` — shared primitives; everything depends on it
- `src/Game` — domain models & state (`Game`, `Game.Core`), API event dispatch
  (`Game.Provider`, `Game.Provider.Json`), source generators (`*.SourceGenerators`,
  `Game.SourceGenerator.Common`), and tests
- `src/ViewModels` — ReactiveUI view models
- `src/Shell` — Avalonia views, app composition, and the launcher entry point
- `src/Browser` — game browser window host

Dependency direction: Shell → ViewModels → Game / Game.Core → Provider → Infrastructure.
Source generators run at compile time only. Startup runs through pluggable
`IBootstrapStep`s (`src/Shell/Sakuno.ING.Shell/Bootstrap`) before the main window is created.

## Conventions

- `net11.0`; `LangVersion latest`, `ImplicitUsings` and `Nullable` enabled; file-scoped
  `Sakuno.ING.*` namespaces.
- Central Package Management: all package versions live in `Directory.Packages.props` —
  never put `Version=` on a `PackageReference`.
- Project references use the root-defined base path (`$(BasePath)src\...`); the
  `Directory.Build.props` chain is positional — projects must stay under `src/`.
- Tests: xunit v3 (`xunit.v3.mtp-v2`), NSubstitute, Verify snapshots for source-generator
  output (checked in under `Snapshots/`).
- Models are partly generated from `*.modeldesc` / `*.jsondesc` descriptors (e.g.
  `src/Game/Sakuno.ING.Game.Core/Models/Metadata/`); edit the descriptors, never the
  generated output. Generator changes are validated via the Verify snapshots.

## Verify snapshot tests

On mismatch Verify launches a GUI diff tool, which stalls headless and agent runs —
always disable it:

```bash
export DiffEngine_Disabled=true
# or inline: DiffEngine_Disabled=true dotnet test
```

Handle pending `*.received.*` files with the `verify.tool` dotnet tool (pinned in
`.config/dotnet-tools.json`; run `dotnet tool restore` once per clone):

- Review interactively: `dotnet verify review`
- Accept all pending changes: `dotnet verify accept --yes`
