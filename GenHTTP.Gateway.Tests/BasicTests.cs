using System.Net;

using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class BasicTests
{

    [TestMethod]
    public async Task TestOverlay()
    {
            var config = @$"
hosts:
  localhost:";

            await using var runner = await TestRunner.RunAsync(config);

            File.WriteAllText(Path.Combine(runner.Environment.Data, "file.txt"), "Hello World!");

            using var response = await runner.GetResponse("/file.txt");

            Assert.AreEqual("Hello World!", await response.GetContent());
        }

    [TestMethod]
    public async Task TestNotFound()
    {
            var config = @$"
hosts:
  localhost:";

            await using var runner = await TestRunner.RunAsync(config);

            using var response = await runner.GetResponse("/notfound");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

    [TestMethod]
    public void TestInitialization()
    {
            var environment = TestEnvironment.Create();

            try
            {
                var config = Program.SetupConfig(environment);

                Assert.IsTrue(File.Exists(Path.Combine(environment.Config, "gateway.yaml")));

                Assert.IsTrue(Directory.Exists(Path.Combine(environment.Data, ".well-known")));
            }
            finally
            {
                environment.Cleanup();
            }
        }

}
