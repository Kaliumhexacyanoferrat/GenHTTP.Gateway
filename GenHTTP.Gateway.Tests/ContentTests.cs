using System.IO;

using Xunit;

using GenHTTP.Gateway.Tests.Domain;

namespace GenHTTP.Gateway.Tests
{

    public class ContentTests
    {

        [Fact]
        public void TestStaticContent()
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

                using var runner = TestRunner.Run(config);

                using var response = runner.GetResponse();

                Assert.Equal("Hello World!", response.GetContent());
            }
            finally
            {
                environment.Cleanup();
            }
        }

    }

}
