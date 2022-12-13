using System.IO;
using System.Net;
using System.Threading.Tasks;

using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests
{

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

                using var runner = TestRunner.Run(config);

                using var response = await runner.GetResponse();

                Assert.AreEqual(HttpStatusCode.Moved, response.StatusCode);
            }
            finally
            {
                environment.Cleanup();
            }
        }

    }

}
