namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

internal static class Repository
{
    public static string Root { get; } = FindRoot();

    public static string TestResults { get; } = Path.Combine(Root, "TestResults", "UIComponentAutomation");

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ing.slnx")))
            directory = directory.Parent;

        return directory?.FullName ?? throw new DirectoryNotFoundException("Could not locate the repository root.");
    }
}
