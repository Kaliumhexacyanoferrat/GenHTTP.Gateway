using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class DirectoryBrowsingTests
{

    [TestMethod]
    public async Task TestListing()
    {
            var environment = TestEnvironment.Create();

            File.WriteAllText(Path.Combine(environment.Root.FullName, "hey.txt"), "Hello World");

            try
            {
                var config = @$"
hosts:
  localhost:
    default:
        listing: {environment.Root.FullName}";

                await using var runner = await TestRunner.RunAsync(config);

                using var response = await runner.GetResponse();

                Assert.IsTrue((await response.GetContent()).Contains("hey.txt"));
            }
            finally
            {
                environment.Cleanup();
            }
        }

}
