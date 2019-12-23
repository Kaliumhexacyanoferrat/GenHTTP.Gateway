using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using GenHTTP.Api.Infrastructure;

namespace GenHTTP.Gateway.Security
{

    public class CertificateProvider : ICertificateProvider
    {

        #region Get-/Setters

        public Dictionary<string, X509Certificate2> Certificates { get; }

        public X509Certificate2? Default { get; }

        #endregion

        public CertificateProvider(Dictionary<string, X509Certificate2> certificates,
                                   X509Certificate2? defaultCertificate)
        {
            Certificates = certificates;
            Default = defaultCertificate;
        }

        #region Functionality

        public X509Certificate2? Provide(string? host)
        {
            if (host != null)
            {
                if (Certificates.TryGetValue(host, out var cert))
                {
                    return cert;
                }
            }

            return Default;
        }

        #endregion

    }

}
