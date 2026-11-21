# Tasks

## 1. Headless FleetTab vertical slice

- [x] 1.1 Scaffold `Sakuno.ING.Shell.UIComponentAutomation` and its xunit v3 test project, add them to `ing.slnx`, centralize the local MCP, Avalonia.Headless, and required Skia dependencies, and verify both projects build with `dotnet build` without changing production project references.
- [x] 1.2 Implement the minimal headless Application and Window with an Avalonia `Control` scenario root, FluentTheme, Shell Common themes, Inter font, required view-location setup, dispatcher/layout settling, and Skia frame capture; verify with a test that a real Overall View renders to a non-empty image with the expected viewport dimensions.
- [x] 1.3 Implement the explicit UI Component Scenario catalog, owned DryIoc scenario scopes, and `Overall/FleetTab/WithFleets` using real selection state and an unselected FleetTab; add a fleet-derived stable automation identifier and verify with tests that catalog metadata is unique and the scenario opens without full App bootstrap or real player services.
- [x] 1.4 Implement bounded visual-tree inspection, scenario-defined read-only state snapshots, unique `AutomationId`/`Name` resolution, and real headless clicking; verify with tests that inspecting FleetTab returns its actionable state, clicking changes the selected fleet ID reported from real selection state, and missing or ambiguous identifiers perform no interaction.
- [x] 1.5 Expose in-process screenshot capture with scenario and dimension metadata and verify the complete FleetTab open-inspect-click-inspect-capture path through the new test project, including an assertion on the selected fleet ID and visual review of the produced PNG.

## 2. Session lifecycle

- [x] 2.1 Add the single-active-session coordinator with serialized UI-thread execution and structured core errors; verify with tests that concurrent operations execute in order, operations without an active scenario fail cleanly, and an unknown scenario does not replace the active session.
- [x] 2.2 Implement candidate-and-swap scenario opening and reset plus close and process-shutdown disposal; verify with tests that successful reset creates fresh presentation state, replacement shares no mutable state, failed opening or reset preserves the previous active session, close releases owned resources, and recoverable operation failures preserve the active UI Component for inspection and capture.

## 3. Materials and reusable Control coverage

- [x] 3.1 Add `Overall/Materials/WithData` with deterministic services and representative material data; verify with tests and a reviewed PNG that Materials resolves its ViewModel through its normal initialization-time DI path rather than manual `DataContext` assignment, inspection reports the supplied material values from the real ViewModel, and capture renders those values.
- [x] 3.2 Complete semantic text input and structured operation diagnostics using a sample reusable Control hosted by a test-only UI Component Scenario; verify with tests that the sample Control supports inspection, text input, read-only state snapshots, and capture through the standard contract, real binding state changes, and disabled/invisible/ambiguous targets return distinct errors.

## 4. Stdio MCP integration and developer entry point

- [x] 4.1 Implement typed MCP handlers for `list_scenarios`, `open_scenario`, `inspect_component`, `click`, `input_text`, `capture_screenshot`, `reset_scenario`, and `close_scenario`, including stable error payloads, bounded inspection results, read-only state snapshots, and image plus `structuredContent` screenshot responses; verify handler tests cover parameters, results, and recoverable errors without terminating the server.
- [x] 4.2 Add transport-level integration tests that start the real stdio server and exercise the complete FleetTab and Materials workflows through an MCP client; verify FleetTab inspection reports the clicked fleet as selected, Materials inspection reports the supplied values, screenshots are valid images, reset and close succeed, EOF cleans up the process, and stdout contains only protocol messages.
- [x] 4.3 Add the protocol-safe build-and-exec launch path and project-level `.kimi-code/mcp.json`, document local setup and the available tools in the existing developer documentation, and verify in a fresh Kimi Code session that the configured server connects without manual server startup and can inspect and capture both initial scenarios with assertions on fleet selection and material values.
- [x] 4.4 Verify the increment with `DiffEngine_Disabled=true dotnet test`, `dotnet build`, and the documented MCP workflow, then inspect the working tree for generated logs, screenshots, or build artifacts that must not be committed.

## 5. Complete-change verification

- [ ] 5.1 Run `openspec validate add-avalonia-ui-component-automation --strict`, the full `DiffEngine_Disabled=true dotnet test`, and `dotnet build`; verify all added spec scenarios against the real MCP and in-process behavior and confirm no HotAvalonia, `dotnet watch`, HTTP listener, Native WebView, or full App bootstrap is involved.
