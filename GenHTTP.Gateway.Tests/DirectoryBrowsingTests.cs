using System.IO;

using GenHTTP.Gateway.Tests.Domain;

using Xunit;

namespace GenHTTP.Gateway.Tests
{

    public class DirectoryBrowsingTests
    {

        [Fact]
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

                Assert.Contains("hey.txt", response.GetContent());
            }
            finally
            {
                environment.Cleanup();
            }
        }

    }

}
