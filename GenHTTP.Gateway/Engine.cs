using System.Linq;
using System.Net;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Core;
using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;

namespace GenHTTP.Gateway
{

    public static class Engine
    {

        public static IServerHost Setup(Environment environment, GatewayConfiguration config)
        {
            var server = Host.Create()
                             .Bind(IPAddress.Any, 80)
                             .Bind(IPAddress.IPv6Any, 80)
                             .Console();

            var certificateProvider = CertificateLoader.GetProvider(environment, config);

            if (certificateProvider != null)
            {
                server.Bind(IPAddress.Any, 443, certificateProvider)
                      .Bind(IPAddress.IPv6Any, 443, certificateProvider);

                if (config.HasInsecureHosts())
                {
                    server.SecureUpgrade(SecureUpgrade.Allow);
                }
            }

#if DEBUG
            server.Development();
#endif

            return server;
        }

        private static bool HasInsecureHosts(this GatewayConfiguration config)
        {
            return config.Hosts?.Values.Any(h => h.Security?.Certificate?.Pfx == null) ?? true;
        }

    }

}
