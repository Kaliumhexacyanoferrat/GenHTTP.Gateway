using System.Security.Cryptography.X509Certificates;

using GenHTTP.Engine.Ioxide;

namespace GenHTTP.Gateway.Security;

/// <summary>
/// Provides the certificates configured for the hosts of the gateway.
/// </summary>
/// <remarks>
/// Kestrel and the internal engine ask for a certificate per connection
/// via <see cref="Provide" />. The ioxide engine terminates TLS itself and
/// registers the certificates on startup, so it needs to know the hosts
/// upfront and prefers the PEM files over the loaded certificate (which
/// is the only form it accepts for HTTP/3).
/// </remarks>
public class CertificateProvider : IHostCertificateProvider, IFileCertificateProvider
{

    #region Get-/Setters

    public Dictionary<string, HostCertificate> Certificates { get; }

    public HostCertificate? Default { get; }

    public IEnumerable<string> Hosts => Certificates.Keys;

    #endregion

    #region Initialization

    public CertificateProvider(Dictionary<string, HostCertificate> certificates, HostCertificate? defaultCertificate)
    {
        Certificates = certificates;
        Default = defaultCertificate;
    }

    #endregion

    #region Functionality

    public X509Certificate2? Provide(string? host) => Resolve(host)?.Certificate;

    public CertificateFiles? ProvideFiles(string? host) => Resolve(host)?.Files;

    private HostCertificate? Resolve(string? host)
    {
        if (host != null && Certificates.TryGetValue(host, out var certificate))
        {
            return certificate;
        }

        return Default;
    }

    #endregion

}

/// <summary>
/// A loaded certificate and, if it has been read from PEM files, their paths.
/// </summary>
public record HostCertificate(X509Certificate2 Certificate, CertificateFiles? Files);
