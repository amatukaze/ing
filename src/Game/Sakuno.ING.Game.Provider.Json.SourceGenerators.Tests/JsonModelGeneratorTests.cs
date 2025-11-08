namespace Sakuno.ING.Game.Provider.Json.SourceGenerators.Tests;

public class JsonModelGeneratorTests
{
    [Fact]
    public Task SimpleModel()
    {
        var name = "ProjectRoot/Metadata/Test.jsondesc";
        var description = """
                          @using TestNamespace
                          @implements ITestUpdated

                          int a ITestUpdated.A
                          int b ITestUpdated.B
                          int c
                          """;
        var source = """
                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                         int B { get; }
                     }
                     """;

        return Util.VerifyJsonModel(name, description, source);
    }
}
