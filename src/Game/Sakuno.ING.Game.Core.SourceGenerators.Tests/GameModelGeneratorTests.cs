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

    [Fact]
    public Task PatchModel()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @id TestId
                          @raw ITestUpdated

                          int A
                          int B

                          @patchbase ITestPatched
                          @patch TestPatch B
                          """;
        var source = """
                     using Sakuno.ING.Game;

                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                         int B { get; }
                     }

                     public interface ITestPatched : IPatch<TestId>
                     {
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }

    [Fact]
    public Task PatchPropertyNotDefined()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @id TestId
                          @raw ITestUpdated

                          int A

                          @patchbase ITestPatched
                          @patch TestPatch B
                          """;
        var source = """
                     using Sakuno.ING.Game;

                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                     }

                     public interface ITestPatched : IPatch<TestId>
                     {
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }

    [Fact]
    public Task MissingId()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @raw ITestUpdated

                          int A
                          """;
        var source = """
                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }

    [Fact]
    public Task MissingRaw()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @id TestId

                          int A
                          """;
        var source = """
                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }

    [Fact]
    public Task UnknownDirective()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @id TestId
                          @raw ITestUpdated
                          @unknown value

                          int A
                          """;
        var source = """
                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }

    [Fact]
    public Task PropertyWithoutName()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @id TestId
                          @raw ITestUpdated

                          int
                          """;
        var source = """
                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }

    [Fact]
    public Task PatchWithoutPatchBase()
    {
        var name = "ProjectRoot/Models/Metadata/Test.modeldesc";
        var description = """
                          @using TestNamespace
                          @id TestId
                          @raw ITestUpdated

                          int A

                          @patch TestPatch A
                          """;
        var source = """
                     namespace TestNamespace;

                     interface ITestUpdated
                     {
                         int A { get; }
                     }
                     """;

        return Util.VerifyGameModel(name, description, source);
    }
}
