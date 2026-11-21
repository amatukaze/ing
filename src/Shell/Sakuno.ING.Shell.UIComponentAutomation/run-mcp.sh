#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$PROJECT_DIR/Sakuno.ING.Shell.UIComponentAutomation.csproj"

dotnet build "$PROJECT" --nologo --verbosity quiet >&2
exec dotnet "$PROJECT_DIR/bin/Debug/net11.0/Sakuno.ING.Shell.UIComponentAutomation.dll"
