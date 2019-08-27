param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Setting', 'Log', IgnoreCase = $true)]
    [string]$Project,
    [Parameter(Mandatory = $true)]
    [string]$Verb,
    [string]$Noun
)

$current = $PSCommandPath | Split-Path  -Parent | Split-Path -Parent
switch ($Project) {
    'Setting' { 
        $folder = "$current\src\Infrastructure"
        $projectName = "Sakuno.ING.Data"
    }
    'Log' {
        $folder = "$current\src\Game"
        $projectName = "Sakuno.ING.Game.Logger"
    }
}

dotnet ef migrations $Verb --project "$folder\$projectName\" --startup-project "$folder\$projectName.Design\" --msbuildprojectextensionspath "$current\intermediate\$projectName.Design\" --framework netcoreapp3.0 $Noun