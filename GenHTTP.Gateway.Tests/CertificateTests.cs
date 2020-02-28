using System.IO;

using Xunit;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;
using GenHTTP.Gateway.Tests.Domain;
using GenHTTP.Modules.Core;

namespace GenHTTP.Gateway.Tests
{

    public class CertificateTests
    {

        [Fact]
        public void TestLoader()
        {
            var environment = TestEnvironment.Create();

            try
            {
                using (var stream = Data.FromResource("Certificate.pfx").Build().GetResource())
                {
                    using (var cert = File.OpenWrite(Path.Combine(environment.Certificates, "host.pfx")))
                    {
                        stream.CopyTo(cert);
                    }
                }
                var config = @$"
hosts:
  localhost:
    security:
      certificate:
        pfx: host.pfx";

                var parsed = GetConfiguration(config);

                var provider = CertificateLoader.GetProvider(environment, parsed);

                Assert.NotNull(provider?.Provide("localhost"));

                Assert.Null(provider?.Provide("anotherhost"));

                Engine.Setup(environment, parsed);
            }
            finally
            {
                environment.Cleanup();
            }
        }

        private static GatewayConfiguration GetConfiguration(string yaml)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                        .Build();

            return deserializer.Deserialize<GatewayConfiguration>(yaml);
        }

    }

}
