using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using GenHTTP.Modules.IO;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;
using GenHTTP.Gateway.Tests.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class CertificateTests
{

    [TestMethod]
    public async Task TestLoader()
    {
            var environment = TestEnvironment.Create();

            try
            {
                using (var stream = await Resource.FromAssembly("Certificate.pfx").Build().GetContentAsync())
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

                Assert.IsNotNull(provider?.Provide("localhost"));

                Assert.IsNull(provider?.Provide("anotherhost"));

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