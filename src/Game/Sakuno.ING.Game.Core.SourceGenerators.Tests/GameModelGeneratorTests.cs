namespace Sakuno.ING.Game.Core.SourceGenerators.Tests;

public class GameModelGeneratorTests
{
    [Fact]
    public Task SimpleModel()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @id TestId
                          @raw ITestUpdated

                          int A
                          int B
                          """;
        var source = """
                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                         int B { get; }
                         int C { get; }
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }
}
