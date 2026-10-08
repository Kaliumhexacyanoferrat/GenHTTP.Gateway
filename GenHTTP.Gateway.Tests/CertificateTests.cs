using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

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

                Assert.AreSame(provider?.Provide("localhost"), provider?.Provide("anotherhost"));

                Assert.IsNull(provider?.ProvideFiles("localhost"));

                CollectionAssert.AreEquivalent(new[] { "localhost" }, provider?.Hosts.ToList());

                Engine.Setup(environment, parsed);
            }
            finally
            {
                environment.Cleanup();
            }
        }

    [TestMethod]
    public void TestPemLoader()
    {
        var environment = TestEnvironment.Create();

        try
        {
            WritePemCertificate(environment);

            var config = @$"
hosts:
  localhost:
    security:
      certificate:
        pem: fullchain.pem
        key: privkey.pem";

            var provider = CertificateLoader.GetProvider(environment, GetConfiguration(config));

            Assert.IsNotNull(provider);

            Assert.IsTrue(provider.Provide("localhost")!.HasPrivateKey);

            var files = provider.ProvideFiles("localhost");

            Assert.AreEqual(Path.Combine(environment.Certificates, "fullchain.pem"), files?.Certificate);
            Assert.AreEqual(Path.Combine(environment.Certificates, "privkey.pem"), files?.Key);
        }
        finally
        {
            environment.Cleanup();
        }
    }

    [TestMethod]
    public void TestPemWithoutKey()
    {
        var environment = TestEnvironment.Create();

        try
        {
            var config = @$"
hosts:
  localhost:
    security:
      certificate:
        pem: fullchain.pem";

            Assert.ThrowsExactly<InvalidOperationException>(() => CertificateLoader.GetProvider(environment, GetConfiguration(config)));
        }
        finally
        {
            environment.Cleanup();
        }
    }

    [TestMethod]
    [DataRow("internal")]
    [DataRow("kestrel")]
    public async Task TestPemServesHttps(string engine)
    {
        var environment = TestEnvironment.Create();

        WritePemCertificate(environment);

        var config = @$"
engine: {engine}
hosts:
  localhost:
    security:
      certificate:
        pem: fullchain.pem
        key: privkey.pem";

        await using var runner = await TestRunner.RunAsync(config, environment);

        File.WriteAllText(Path.Combine(runner.Environment.Data, "file.txt"), "Hello World!");

        using var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var client = new HttpClient(handler);

        using var response = await client.GetAsync($"https://localhost:{runner.SecurePort}/file.txt");

        Assert.AreEqual("Hello World!", await response.GetContent());
    }

    private static void WritePemCertificate(TestEnvironment environment)
    {
        using var key = RSA.Create(2048);

        var request = new CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        using var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));

        File.WriteAllText(Path.Combine(environment.Certificates, "fullchain.pem"), certificate.ExportCertificatePem());
        File.WriteAllText(Path.Combine(environment.Certificates, "privkey.pem"), key.ExportPkcs8PrivateKeyPem());
    }

    private static GatewayConfiguration GetConfiguration(string yaml)
    {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                        .Build();

            return deserializer.Deserialize<GatewayConfiguration>(yaml);
        }

}