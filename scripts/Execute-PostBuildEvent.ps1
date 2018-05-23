param(
    [string]$SolutionDir=$(throw "Parameter missing: -SolutionDir"),
    [string]$ProjectPath=$(throw "Parameter missing: -ProjectPath"),
    [string]$Configuration=$(throw "Parameter missing: -Configuration"),
    [string]$TargetDir=$(throw "Parameter missing: -TargetDir"),
    [string]$TargetPath=$(throw "Parameter missing: -TargetPath")
)

$localPostBuildEventScript = $SolutionDir + 'scripts\Execute-LocalPostBuildEvent.ps1'

if (Test-Path $localPostBuildEventScript) {
    & $localPostBuildEventScript
}
