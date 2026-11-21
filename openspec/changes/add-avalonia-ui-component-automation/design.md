# Design

## Context

See proposal.md for motivation and specs/ui-component-automation/spec.md for the behavior contract.

The desktop launcher currently configures the full `App`, shell services, ReactiveUI, Inter font, and platform lifetime. `App` runs bootstrap and opens both the main and game-browser windows, so the automation host must not reuse that startup path. Overall Views use compiled bindings, an inherited `IServiceProvider` attached property, and, in some cases, resolve their own ViewModels during initialization. Shell Common also contains reusable Controls whose scenarios must use the same host without pretending they are page-level Views.

`FleetTab` is one fleet button and does not visually expose its selection state. Its scenario will supply an unselected tab backed by real multi-fleet selection state and expose a read-only selected-fleet snapshot for verification. `Materials` likewise needs read-only values derived from its real ViewModel so a non-empty screenshot is not mistaken for proof of correct DI and data flow.

No existing Shell test project or Avalonia headless infrastructure exists. ViewModel tests already demonstrate deterministic state construction with an `IGameProvider` substitute and real player-data services.

## Goals / Non-Goals

**Goals:**

- Keep the production desktop startup and runtime dependency graph unchanged.
- Separate the reusable Avalonia scenario host from the MCP transport adapter.
- Use one hosting contract for page-level Views and reusable Controls.
- Make scenario construction, replacement, reset, failure, and disposal explicit and deterministic.
- Reproduce the application resources needed for meaningful layout and screenshots.
- Keep the stdio stream exclusively machine-readable and make every tool result structured.

**Non-Goals:**

- A general runtime XAML loader, hot-reload mechanism, or automatic dependency inference for arbitrary UI Components.
- A shared or remote automation service, multiple simultaneous client sessions, or parallel UI mutation.
- Pixel-perfect rendering equivalence across machines or operating systems.
- Extracting new production abstractions solely to make the initial two production Views testable.
- Exposing arbitrary ViewModel properties, mutable test hooks, or service-provider access to agents.

## Decisions

### 1. Add an independent executable and test project

Create `src/Shell/Sakuno.ING.Shell.UIComponentAutomation/Sakuno.ING.Shell.UIComponentAutomation.csproj` as the executable and `src/Shell/Sakuno.ING.Shell.UIComponentAutomation.Tests/Sakuno.ING.Shell.UIComponentAutomation.Tests.csproj` for xunit v3 tests. Reference Overall Views, Shell Common, and ViewModels from the executable; do not reference the automation executable from production projects.

Use the installed `mcpserver` local template only as scaffolding for the MCP entry point. Remove unrelated publishing and self-contained options, retain `net11.0`, and move `ModelContextProtocol`, `Avalonia.Headless`, and required Skia package versions into Central Package Management. Follow the existing root-based project-reference convention.

Alternative considered: adding modes to `Shell.Launcher`. Rejected because it would couple an agent protocol to production startup and make the full application lifetime difficult to avoid.

### 2. Use a minimal Headless application with explicit resource parity

The executable owns a minimal Avalonia `Application` configured with headless rendering and Skia capture. It loads FluentTheme, Shell Common themes, Inter font, and only the view-location registrations needed by hosted scenarios. It creates its own Window and an Avalonia `Control` scenario root rather than reusing `App` or a desktop lifetime.

Using `Control` as the root contract allows a scenario to host either a page-level View or an independent reusable Control without different MCP tools or lifecycle rules. A scenario can declare its viewport; a shared default keeps captures consistent. Layout and rendering must settle before inspection, interaction results, or screenshots are returned. The first implementation increment must prove real Skia-backed capture rather than treating control construction as sufficient verification.

Alternative considered: hosting a real desktop window. Rejected for this increment because native window and input automation would add platform-specific behavior without improving the initial state and command verification contract.

### 3. Use explicit scenario definitions with owned service scopes

Each scenario definition provides a stable identifier, UI Component name, viewport if needed, and a factory for an owned session. Registration is explicit in a catalog rather than based on assembly scanning or arbitrary code supplied by the agent.

A session factory creates a DryIoc scope, registers deterministic in-memory external services, constructs real presentation services and ViewModels where required, attaches the scope's `IServiceProvider` to the host root before component initialization, and returns the root `Control` with its owned resources. Initial production identifiers are `Overall/FleetTab/WithFleets` and `Overall/Materials/WithData`.

The FleetTab scenario uses real `FleetSelectionState` and player-data behavior with several fleets, while displaying one unselected `FleetTabViewModel`. The Materials scenario registers the services required by its normal `OnInitialized` ViewModel resolution and supplies representative material data. Test-only mutable state hooks remain internal to the automation tests.

Alternative considered: setting only `DataContext` or globally replacing `AppLocator`. Rejected because Views such as `Materials` resolve ViewModels through inherited DI, and global mutable service state would break isolation and reset semantics.

### 4. Expose declared read-only scenario state snapshots

A scenario can declare a small read-only state snapshot for behavior that is not represented by its visual tree. The snapshot is derived from the real ViewModel, presentation service, or Control and is returned by `inspect_component` alongside control information. It is not a general object dump and cannot mutate scenario state or resolve arbitrary services.

FleetTab exposes the current selected fleet ID from real `FleetSelectionState`. Materials exposes the representative material values from its DI-resolved ViewModel. Tests must prove these snapshots track real objects and that Materials reaches its normal initialization-time DI path rather than receiving a manually assigned `DataContext`.

Alternative considered: relying only on screenshots and control text. Rejected because FleetTab has no visible selection state and a non-empty Materials image does not establish correct values or ViewModel provenance.

### 5. Serialize all session work through one coordinator

A session coordinator owns at most one active session per stdio process. MCP handlers submit work to a single serialized pipeline and marshal UI Component access to the Avalonia UI thread. There is no `sessionId` parameter and no parallel click, reset, or capture.

`list_scenarios` is independent of session state. `open_scenario` validates the identifier, constructs and lays out a candidate session, and only then swaps it for the active session. `reset_scenario` follows the same candidate-and-swap process using the current definition. The candidate is not active until the swap; construction failure disposes only the candidate and preserves the previous session. A successful swap disposes the previous session.

`close_scenario` disposes the active root Control, presentation state, and owned scope. Closing stdin triggers the same cleanup and process shutdown.

Alternative considered: discarding the old session before constructing its replacement. Rejected because a construction failure would destroy the most useful diagnostic state. Multiple named sessions were also rejected because stdio already provides process-level client isolation.

### 6. Expose a small, typed MCP tool surface

The initial tools are:

- `list_scenarios`
- `open_scenario`
- `inspect_component`
- `click`
- `input_text`
- `capture_screenshot`
- `reset_scenario`
- `close_scenario`

Tool parameters and results use typed schemas. Errors contain a stable code, message, and relevant details, distinguishing unknown scenario, no active scenario, construction failure, control not found, ambiguous control, inoperable control, and host failure. Parameter errors and recoverable operation failures do not terminate the server.

Inspection supports an optional stable root identifier and bounded depth so the default result remains useful without returning an unbounded tree. Its structured result includes both matching control information and the scenario's declared read-only state snapshot. Screenshots return MCP image content plus `structuredContent` containing scenario identifier and dimensions. Detailed logs go to stderr or a diagnostic file, never stdout.

Alternative considered: one generic operation-script tool. Rejected because typed tools provide better discovery, validation, and interactive use while preserving the same core operations for a possible future batch interface.

### 7. Traverse the real visual tree for inspection and semantic operations

Inspection reads control type, `AutomationId`, `Name`, text, visibility, enabled state, and bounds from the rendered visual tree. Interaction accepts an `AutomationId` or `Name`, resolves all matches, and succeeds only for one visible, enabled target. It never silently chooses the first match or uses display text and coordinates as fallback identifiers.

Add stable automation metadata only where the initial UI Components lack a usable unique `Name`. Repeated fleet controls must derive their identifier from the fleet ID rather than reuse a constant. Click and text input use Avalonia headless input facilities against the real controls, followed by the necessary layout and dispatcher processing.

Alternative considered: direct ViewModel command invocation. Rejected because it would bypass control visibility, hit testing, binding, and real UI Component interaction behavior.

### 8. Preserve recoverable failure state and bound diagnostics

A failed interaction leaves the active session unchanged and available for inspection or capture. Failed candidate construction during open or reset likewise preserves the previous session. The operation result includes relevant binding or host diagnostics without automatically dumping the complete tree or log. Scenario-construction failures are reported separately from operation failures.

A fatal host initialization failure terminates the process with a nonzero exit and diagnostics on stderr. MCP client disconnect or EOF performs best-effort disposal and exits without leaving a background process.

### 9. Verify behavior in-process and through the real MCP transport

The test project uses the repository's xunit v3/Microsoft Testing Platform conventions. Headless tests run sequentially because Avalonia application and scheduler setup is process-global.

Tests cover catalog uniqueness, isolated construction, FleetTab selection through the read-only snapshot, Materials DI resolution and material values, control inspection, ambiguous and missing targets, reset, disposal, candidate-construction failure preservation, and non-empty image capture with plausible dimensions. A sample reusable Control hosted by a test-only UI Component Scenario proves that a non-page root supports inspection, text input, state snapshots, and capture through the same contract. Screenshots are validated as images, not compared with pixel baselines.

A transport-level integration test starts the actual stdio server, lists scenarios, opens FleetTab, clicks its stable target, inspects the selected fleet ID, and captures an image. The Materials path inspects the supplied values and captures the rendered View. These tests verify tool schemas, observable state, and protocol-clean stdout in addition to the in-process host behavior.

### 10. Make project launch convenient without contaminating stdio

Add project-level `.kimi-code/mcp.json` configuration for the local server. Because `dotnet run` can emit build output before the child process starts, the launch path must build with output redirected away from protocol stdout and then execute the built server with clean stdout. A small repository launch wrapper may perform this build-and-exec sequence for the supported local development environment.

The configuration is an adapter only; scenario and host code remain unaware of Kimi Code. No `dotnet watch` or HotAvalonia package is included.

## Risks / Trade-offs

- **Avalonia 12.1.6 headless input or capture APIs differ from expectations** → Make FleetTab rendering, clicking, and Skia capture the first spike and increment; revisit the host decision before expanding the tool surface if a hard limitation appears.
- **Headless output differs from the desktop application because resources are missing** → Centralize minimal host resource setup, explicitly include FluentTheme, Shell Common themes, Inter font, and required view location, and inspect representative captures manually.
- **Components with initialization-time DI fail under a simple host** → Attach the scenario scope before initialization and use Materials as an early acceptance case rather than deferring DI support.
- **Read-only snapshots become a parallel ViewModel or expose excessive internals** → Keep snapshots small, scenario-declared, and derived from real presentation objects; do not expose arbitrary property traversal or service resolution.
- **Global Avalonia or ReactiveUI state leaks across tests or resets** → Initialize the host once per process, keep scenario state scoped, run tests sequentially, and assert that replacement and reset dispose owned resources.
- **Build logs corrupt MCP stdio** → Keep all build and host logs off stdout and cover the real launch path with a transport-level integration test.
- **A single active session slows comparisons across scenarios** → Accept explicit replacement and reset for a smaller, deterministic contract; use captured images for comparison instead of retaining multiple live UI Components.
- **The MCP template is preview and includes unnecessary defaults** → Use it only for scaffolding, pin dependencies centrally, and keep protocol code behind the tool adapter so the core host remains independent.
- **Stable identifiers become a production maintenance burden** → Add them only to actionable controls required by scenarios and prefer existing unique names when sufficient.

## Migration Plan

This is opt-in development tooling with no production data or runtime migration. Add the new projects and launch configuration while leaving the existing launcher unchanged. Developers opt in by allowing the project MCP server in a new agent session.

Rollback removes the MCP launch declaration and automation projects. Any automation identifiers added to production UI Components are inert metadata and can remain or be removed independently.

## Open Questions

- The exact Avalonia 12.1.6 headless dispatcher, input, and frame-capture API calls will be confirmed by the first implementation spike; this does not change the behavior contract unless the required capture capability is unavailable.
