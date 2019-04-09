using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Core;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;
using GenHTTP.Gateway.Utilities;

namespace GenHTTP.Gateway
{

    public class Host
    {

        public static IServerBuilder Build(GatewayConfiguration config)
        {
            var server = Server.Create()
                               .Bind(IPAddress.Any, 8080)
                               .Bind(IPAddress.IPv6Any, 8080)
                               .Compression(new BrotliCompression())
                               .Console();

            var certificateProvider = CertificateLoader.GetProvider(config);

            if (certificateProvider != null)
            {
                server.Bind(IPAddress.Any, 8443, certificateProvider)
                      .Bind(IPAddress.IPv6Any, 8443, certificateProvider);
            }

#if DEBUG
            server.Development();
#endif

            return server;
        }

    }

}
