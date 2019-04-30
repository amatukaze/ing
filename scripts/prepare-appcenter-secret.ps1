if ((Test-Path env:ING_AppSecret) -and -not (Test-Path ..\src\Core\UWP\Sakuno.ING.UWP\App.Secret.cs))
{
    $code = @"
    namespace Sakuno.ING.UWP
    {
        public partial class App
        {
            partial void TryStartAppCenter() => StartAppCenter("$env:ING_AppSecret");
        }
    }
"@
    Out-File -FilePath "..\src\Core\UWP\Sakuno.ING.UWP\App.Secret.cs" -InputObject $code
}
