using System.IO;

using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests
{

    [TestClass]
    public class DirectoryBrowsingTests
    {

        [TestMethod]
        public void TestListing()
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

                using var runner = TestRunner.Run(config);

                using var response = runner.GetResponse();

                Assert.IsTrue(response.GetContent().Contains("hey.txt"));
            }
            finally
            {
                environment.Cleanup();
            }
        }

    }

}
