
using GenHTTP.Gateway.Tests.Domain;
using System.IO;
using System.Net;
using Xunit;

namespace GenHTTP.Gateway.Tests
{

    public class BasicTests
    {

        [Fact]
        public void TestOverlay()
        {
            var config = @$"
hosts:
  localhost:";

            using var runner = TestRunner.Run(config);

            File.WriteAllText(Path.Combine(runner.Environment.Data, "file.txt"), "Hello World!");

            using var response = runner.GetResponse("/file.txt");

            Assert.Equal("Hello World!", response.GetContent());
        }

        [Fact]
        public void TestNotFound()
        {
            var config = @$"
hosts:
  localhost:";

            using var runner = TestRunner.Run(config);

            using var response = runner.GetResponse("/notfound");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void TestInitialization()
        {
            var environment = TestEnvironment.Create();

            try
            {
                var config = Program.SetupConfig(environment);

                Assert.True(File.Exists(Path.Combine(environment.Config, "gateway.yaml")));

                Assert.True(Directory.Exists(Path.Combine(environment.Data, ".well-known")));
            }
            finally
            {
                environment.Cleanup();   
            }
        }

    }

}
