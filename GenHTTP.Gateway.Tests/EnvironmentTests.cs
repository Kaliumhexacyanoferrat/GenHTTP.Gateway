using Xunit;

namespace GenHTTP.Gateway.Tests
{

    public class EnvironmentTests
    {

        [Fact]
        public void TestDockerWindows()
        {
            var environment = Environment.DockerWindows();

            Assert.Equal(@"C:\App\Config\", environment.Config);
            Assert.Equal(@"C:\App\Data\", environment.Data);
            Assert.Equal(@"C:\App\Certs\", environment.Certificates);
        }

        [Fact]
        public void TestDockerLinux()
        {
            var environment = Environment.DockerLinux();

            Assert.Equal("/app/config/", environment.Config);
            Assert.Equal("/app/data/", environment.Data);
            Assert.Equal("/app/certs/", environment.Certificates);
        }

        [Fact]
        public void TestLocal()
        {
            var environment = Environment.Local();

            Assert.Equal("./config/", environment.Config);
            Assert.Equal("./data/", environment.Data);
            Assert.Equal("./certs/", environment.Certificates);
        }

        [Fact]
        public void TestDefault()
        {
            var environment = Environment.Default();

            Assert.Equal("./config/", environment.Config);
            Assert.Equal("./data/", environment.Data);
            Assert.Equal("./certs/", environment.Certificates);
        }

    }

}
