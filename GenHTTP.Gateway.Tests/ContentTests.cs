using System.IO;

using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests
{

    [TestClass]
    public class ContentTests
    {

        [TestMethod]
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

                Assert.AreEqual("Hello World!", response.GetContent());
            }
            finally
            {
                environment.Cleanup();
            }
        }

    }

}
