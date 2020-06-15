using System.Net;

using GenHTTP.Api.Content;
using GenHTTP.Api.Infrastructure;
using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;
using GenHTTP.Modules.Core;

namespace GenHTTP.Gateway
{

    public class Engine
    {

        #region Get-/Setters

        public IServerHost Host { get; }

        public CertificateProvider? CertificateProvider { get; }

        #endregion

        #region Initialization

        public Engine(Environment environment, GatewayConfiguration configuration)
        {
            var handler = Handler.Build(environment, configuration);

            var certificateProvider = new CertificateProvider(environment, configuration);

            Host = CreateHost(handler, certificateProvider);
        }

        public static IServerHost CreateHost(IHandlerBuilder handler, CertificateProvider? certificateProvider,
                                             ushort port = 80, ushort securePort = 443)
        {
            var server = Core.Host.Create()
                                  .Bind(IPAddress.Any, port)
                                  .Bind(IPAddress.IPv6Any, port)
                                  .Handler(handler)
                                  .Defaults(secureUpgrade: false)
                                  .Console();

            if (certificateProvider != null)
            {
                server.Bind(IPAddress.Any, securePort, certificateProvider);
                server.Bind(IPAddress.IPv6Any, securePort, certificateProvider);
            }

#if DEBUG
            server.Development();
#endif

            return server;
        }

        #endregion

        #region Functionality

        public int Run() => Host.Run();

        #endregion

    }

}
