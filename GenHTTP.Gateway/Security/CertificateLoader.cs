using System.Security.Cryptography.X509Certificates;

using GenHTTP.Engine.Ioxide;

using GenHTTP.Gateway.Configuration;

namespace GenHTTP.Gateway.Security;

public static class CertificateLoader
{

    public static CertificateProvider? GetProvider(Environment environment, GatewayConfiguration config)
    {
        var certificates = new Dictionary<string, HostCertificate>(StringComparer.OrdinalIgnoreCase);

        if (config.Hosts != null)
        {
            foreach (var host in config.Hosts)
            {
                var cert = host.Value?.Security?.Certificate;

                if (cert != null)
                {
                    certificates.Add(host.Key, LoadCertificate(environment, cert));
                }
            }
        }

        if (certificates.Count > 0)
        {
            // ioxide refuses all handshakes on a port without a default certificate,
            // so clients that do not name a known host get the first one configured
            return new CertificateProvider(certificates, certificates.Values.First());
        }

        return null;
    }

    private static HostCertificate LoadCertificate(Environment environment, CertificateConfiguration config)
    {
        if (config.Pem != null)
        {
            if (config.Key == null)
            {
                throw new InvalidOperationException($"Key file has not been specified for certificate '{config.Pem}'");
            }

            var certificatePath = Path.GetFullPath(Path.Combine(environment.Certificates, config.Pem));
            var keyPath = Path.GetFullPath(Path.Combine(environment.Certificates, config.Key));

            var chain = new X509Certificate2Collection();
            chain.ImportFromPemFile(certificatePath);

            PublishIssuers(chain);

            var certificate = X509Certificate2.CreateFromPemFile(certificatePath, keyPath);

            if (OperatingSystem.IsWindows())
            {
                // SChannel cannot use the ephemeral key of a certificate read from PEM
                certificate = X509CertificateLoader.LoadPkcs12(certificate.Export(X509ContentType.Pkcs12), null);
            }

            return new HostCertificate(certificate, new CertificateFiles(certificatePath, keyPath));
        }

        if (config.Pfx != null)
        {
            var bytes = File.ReadAllBytes(Path.Combine(environment.Certificates, config.Pfx));

            PublishIssuers(X509CertificateLoader.LoadPkcs12Collection(bytes, null));

            return new HostCertificate(X509CertificateLoader.LoadPkcs12(bytes, null), null);
        }

        throw new InvalidOperationException("Certificate file has not been specified (either 'pfx' or 'pem' and 'key')");
    }

    /// <summary>
    /// Adds the intermediate certificates to the store of the current user.
    /// </summary>
    /// <remarks>
    /// The chain sent to the client is built from the certificates the
    /// runtime finds locally. Kestrel would download missing issuers on
    /// demand, ioxide sends what it finds and clients would fail to verify.
    /// </remarks>
    private static void PublishIssuers(X509Certificate2Collection chain)
    {
        var issuers = chain.Where(c => !c.HasPrivateKey && c.Subject != c.Issuer).ToList();

        if (issuers.Count == 0)
        {
            return;
        }

        try
        {
            using var store = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadWrite);

            foreach (var issuer in issuers)
            {
                store.Add(issuer);
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Failed to publish the issuers of the certificate, clients may fail to verify the chain: {e.Message}");
        }
    }

}
