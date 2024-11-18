using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class EnvironmentTests
{

    [TestMethod]
    public void TestDockerWindows()
    {
            var environment = Environment.DockerWindows();

            Assert.AreEqual(@"C:\App\Config\", environment.Config);
            Assert.AreEqual(@"C:\App\Data\", environment.Data);
            Assert.AreEqual(@"C:\App\Certs\", environment.Certificates);
        }

    [TestMethod]
    public void TestDockerLinux()
    {
            var environment = Environment.DockerLinux();

            Assert.AreEqual("/app/config/", environment.Config);
            Assert.AreEqual("/app/data/", environment.Data);
            Assert.AreEqual("/app/certs/", environment.Certificates);
        }

    [TestMethod]
    public void TestLocal()
    {
            var environment = Environment.Local();

            Assert.AreEqual("./config/", environment.Config);
            Assert.AreEqual("./data/", environment.Data);
            Assert.AreEqual("./certs/", environment.Certificates);
        }

    [TestMethod]
    public void TestDefault()
    {
            var environment = Environment.Default();

            Assert.AreEqual("./config/", environment.Config);
            Assert.AreEqual("./data/", environment.Data);
            Assert.AreEqual("./certs/", environment.Certificates);
        }

}