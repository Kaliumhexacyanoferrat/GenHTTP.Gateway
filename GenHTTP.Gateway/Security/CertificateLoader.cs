using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Gateway.Configuration;

namespace GenHTTP.Gateway.Security
{

    public class CertificateProvider : ICertificateProvider
    {
        private const string LETS_ENCRYPT = "letsencrypt";

        #region Get-/Setters

        public Dictionary<string, X509Certificate2> Cache { get; }

        public Environment Environment { get; }

        public GatewayConfiguration Configuration { get; }

        #endregion

        #region Initialization

        public CertificateProvider(Environment environment, GatewayConfiguration config)
        {
            Environment = environment;
            Configuration = config;

            Cache = new Dictionary<string, X509Certificate2>();
        }

        #endregion

        #region Functionality

        public X509Certificate2? Provide(string? host)
        {
            if (host != null)
            {
                if (Cache.ContainsKey(host))
                {
                    return Cache[host];
                }

                var hostConfiguration = Configuration.Hosts?.FirstOrDefault(h => h.Key == host);

                var cert = hostConfiguration?.Value?.Security?.Certificate;

                if (cert != null)
                {
                    var loaded = LoadCertificate(host, Environment, cert);

                    lock (Cache)
                    {
                        if (!Cache.ContainsKey(host))
                        {
                            Cache.Add(host, loaded);
                        }
                    }

                    return loaded;
                }
            }

            return null;
        }

        private X509Certificate2 LoadCertificate(string host, Environment environment, CertificateConfiguration config)
        {
            if (config.Pfx == null)
            {
                throw new InvalidOperationException("Certificate file has not been specified");
            }

            var actualFile = (config.Pfx == LETS_ENCRYPT) ? $"{host}.pfx" : config.Pfx;

            return new X509Certificate2(File.ReadAllBytes(Path.Combine(environment.Certificates, actualFile)));
        }

        #endregion

    }

}
