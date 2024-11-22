using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class ContentTests
{

    [TestMethod]
    public async Task TestStaticContent()
    {
            var environment = TestEnvironment.Create();

            File.WriteAllText(Path.Combine(environment.Root.FullName, "index.html"), "Hello World!");

            try
            {
                var config = @$"
hosts:
  localhost:
    default:
        content:
            directory: {environment.Root.FullName}
            index: index.html";

                await using var runner = await TestRunner.RunAsync(config);

                using var response = await runner.GetResponse();

                Assert.AreEqual("Hello World!", await response.GetContent());
            }
            finally
            {
                environment.Cleanup();
            }
        }

}
