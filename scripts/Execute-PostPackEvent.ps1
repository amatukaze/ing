param(
    [string]$SolutionDir=$(throw "Parameter missing: -SolutionDir"),
    [string]$PackageId=$(throw "Parameter missing: -PackageId"),
    [string]$PackageVersion=$(throw "Parameter missing: -PackageVersion"),
    [string]$PackageOutputPath=$(throw "Parameter missing: -PackageOutputPath")
)

$localPostPackEventScript = $SolutionDir + 'scripts\Execute-LocalPostPackEvent.ps1'

if (Test-Path $localPostPackEventScript) {
    & $localPostPackEventScript
}
