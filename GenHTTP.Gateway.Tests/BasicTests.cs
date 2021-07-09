using System.IO;
using System.Net;

using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests
{

    [TestClass]
    public class BasicTests
    {

        [TestMethod]
        public void TestOverlay()
        {
            var config = @$"
hosts:
  localhost:";

            using var runner = TestRunner.Run(config);

            File.WriteAllText(Path.Combine(runner.Environment.Data, "file.txt"), "Hello World!");

            using var response = runner.GetResponse("/file.txt");

            Assert.AreEqual("Hello World!", response.GetContent());
        }

        [TestMethod]
        public void TestNotFound()
        {
            var config = @$"
hosts:
  localhost:";

            using var runner = TestRunner.Run(config);

            using var response = runner.GetResponse("/notfound");

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

}
