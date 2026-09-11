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
                         .Bind(null, port);

        var certificateProvider = CertificateLoader.GetProvider(environment, config);

        if (certificateProvider != null)
        {
            var protocols = (config.EnableQuic ?? false) ? HttpProtocols.All : HttpProtocols.Http1AndHttp2;
            
            server.Bind(null, securePort, certificateProvider, httpProtocols: protocols);
        }

#if DEBUG
        server.Development();
#endif

        return server;
    }

}
