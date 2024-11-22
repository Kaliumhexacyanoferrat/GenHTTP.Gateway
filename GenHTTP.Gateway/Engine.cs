using System.Net;
using System.Runtime.InteropServices;

using GenHTTP.Api.Infrastructure;

using GenHTTP.Engine.Kestrel;
using GenHTTP.Modules.Practices;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;

namespace GenHTTP.Gateway;

public static class Engine
{

    public static IServerHost Setup(Environment environment, GatewayConfiguration config,
        ushort port = 80, ushort securePort = 443)
    {
        var server = Host.Create()
                         .Defaults(secureUpgrade: false)
                         .Console();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            server.Bind(IPAddress.Any, port)
                  .Bind(IPAddress.IPv6Any, port);
        }
        else
        {
            server.Bind(IPAddress.Any, port);
        }

        var certificateProvider = CertificateLoader.GetProvider(environment, config);

        if (certificateProvider != null)
        {
            var quic = config.EnableQuic ?? false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                server.Bind(IPAddress.Any, securePort, certificateProvider, enableQuic: quic)
                      .Bind(IPAddress.IPv6Any, securePort, certificateProvider, enableQuic: quic);
            }
            else
            {
                server.Bind(IPAddress.Any, securePort, certificateProvider, enableQuic: quic);
            }
        }

#if DEBUG
        server.Development();
#endif

        return server;
    }

}
