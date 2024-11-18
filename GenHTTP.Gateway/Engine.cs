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
                         .Bind(IPAddress.Any, port)
                         .Bind(IPAddress.IPv6Any, port)
                         .Defaults(secureUpgrade: false)
                         .Console();

        var certificateProvider = CertificateLoader.GetProvider(environment, config);

        if (certificateProvider != null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                server.Bind(IPAddress.Any, securePort, certificateProvider)
                      .Bind(IPAddress.IPv6Any, securePort, certificateProvider);
            }
            else
            {
                server.Bind(IPAddress.Any, securePort, certificateProvider);
            }
        }

#if DEBUG
        server.Development();
#endif

        return server;
    }

}
