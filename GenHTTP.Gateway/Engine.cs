using GenHTTP.Api.Infrastructure;
using GenHTTP.Engine.Kestrel;
using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;
using GenHTTP.Modules.Practices;

namespace GenHTTP.Gateway;

public static class Engine
{

    public static IServerHost Setup(Environment environment, GatewayConfiguration config,
        ushort port = 80, ushort securePort = 443)
    {
        var server = Host.Create()
                         .Defaults(secureUpgrade: false)
                         .Bind(null, port)
                         .Console();

        var certificateProvider = CertificateLoader.GetProvider(environment, config);

        if (certificateProvider != null)
        {
            server.Bind(null, securePort, certificateProvider, enableQuic: config.EnableQuic ?? false);
        }

#if DEBUG
        server.Development();
#endif

        return server;
    }

}
