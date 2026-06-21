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

    [Fact]
    public Task ImplementationNotFound()
    {
        var name = "ProjectRoot/Metadata/Test.jsondesc";
        var description = """
                          @using TestNamespace
                          @implements INonExistent

                          int a INonExistent.A
                          """;
        var source = """
                     namespace TestNamespace;

                     public interface ITestUpdated
                     {
                         int A { get; }
                     }
                     """;

        return Util.VerifyJsonModel(name, description, source);
    }

    [Fact]
    public Task MappingPropertyNotFound()
    {
        var name = "ProjectRoot/Metadata/Test.jsondesc";
        var description = """
                          @using TestNamespace
                          @implements ITestUpdated

                          int a ITestUpdated.C
                          """;
        var source = """
                     namespace TestNamespace;

                     public interface ITestUpdated
                     {
                         int A { get; }
                     }
                     """;

        return Util.VerifyJsonModel(name, description, source);
    }
}
