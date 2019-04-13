using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Gateway.Configuration;

namespace GenHTTP.Gateway.Security
{

    public class CertificateProvider : ICertificateProvider
    {

        #region Get-/Setters

        public Dictionary<string, X509Certificate2> Certificates { get; }

        public X509Certificate2? Default { get; }

        public IEnumerable<string> SupportedHosts => Certificates.Keys;

        #endregion

        public CertificateProvider(Dictionary<string, X509Certificate2> certificates,
                                   X509Certificate2? defaultCertificate)
        {
            Certificates = certificates;
            Default = defaultCertificate;
        }

        #region Functionality

        public X509Certificate2 Provide(string? host)
        {
            if (host != null)
            {
                if (Certificates.TryGetValue(host, out var cert))
                {
                    return cert;
                }
            }

            if (Default != null)
            {
                return Default;
            }

            throw new InvalidOperationException($"No certificate for host '{host}'");
        }

        #endregion

    }

}
