using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Gateway.Configuration;

namespace GenHTTP.Gateway.Security
{

    public static class CertificateLoader
    {

        public static ICertificateProvider? GetProvider(Environment environment, GatewayConfiguration config)
        {
            var certificates = new Dictionary<string, X509Certificate2>();

            if (config.Hosts != null)
            {
                foreach (var host in config.Hosts)
                {
                    var cert = host.Value.Security?.Certificate;

                    if (cert != null)
                    {
                        certificates.Add(host.Key, LoadCertificate(environment, cert));
                    }
                }
            }

            if (certificates.Count > 0)
            {
                return new CertificateProvider(certificates, null);
            }

            return null;
        }

        private static X509Certificate2 LoadCertificate(Environment environment, CertificateConfiguration config)
        {
            if (config.Pfx == null)
            {
                throw new InvalidOperationException("Certificate file has not been specified");
            }

            return new X509Certificate2(File.ReadAllBytes(Path.Combine(environment.Certificates, config.Pfx)));
        }

    }

}
