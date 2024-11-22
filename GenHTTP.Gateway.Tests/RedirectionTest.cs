using System.Net;
using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class RedirectionTest
{

    [TestMethod]
    public async Task TestRedirection()
    {
            var environment = TestEnvironment.Create();

            File.WriteAllText(Path.Combine(environment.Root.FullName, "index.html"), "Hello World!");

            try
            {
                var config = @$"
hosts:
  localhost:
    default:
        location: https://google.com";

                await using var runner = await TestRunner.RunAsync(config);

                using var response = await runner.GetResponse();

                Assert.AreEqual(HttpStatusCode.Moved, response.StatusCode);
            }
            finally
            {
                environment.Cleanup();
            }
        }

}
