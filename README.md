# ING

[![build and test](https://github.com/amatukaze/ing/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/amatukaze/ing/actions/workflows/build-and-test.yml)

A flexible, powerful and lightweight KanColle tool/browser.

## Screenshots (Legacy Version)
![Overview](https://raw.githubusercontent.com/KodamaSakuno/kodamasakuno.github.io/master/images/ing/001.jpg)
![Battle Information](https://raw.githubusercontent.com/KodamaSakuno/kodamasakuno.github.io/master/images/ing/002.jpg)

## Branches

| Branch | Description |
|---|---|
| `master` | Generation 2 (in progress): full rewrite with Avalonia + ReactiveUI |
| `legacy` | Generation 1: WPF, released version |

## Architecture

```mermaid
graph TD
    Launcher["Shell.Launcher<br/>(entry point)"] --> Shell["Shell<br/>Avalonia views & app composition"]
    Shell --> VM["ViewModels<br/>MVVM · ReactiveUI"]
    VM --> Game["Game / Game.Core<br/>domain models & state"]
    Game --> Provider["Game.Provider (.Json)<br/>data ingestion & API event dispatch"]
    Game --> Infra["Infrastructure<br/>(Sakuno.ING.Standard)"]
    Provider --> Infra
    SG["Source Generators<br/>(Game.Core / Game.Provider / Game.Provider.Json)"] -. "compile-time codegen" .-> Game
    SG -.-> Provider
```

## Build & Run

Requires the **.NET 10 SDK**.

```bash
git clone https://github.com/amatukaze/ing.git
cd ing

dotnet build

# Run the desktop client
dotnet run --project src/Shell/Sakuno.ING.Shell.Launcher

# Run all tests
dotnet test
```

### License
The MIT License (MIT)
