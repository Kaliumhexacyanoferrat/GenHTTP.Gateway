﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Core;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;

namespace GenHTTP.Gateway
{

    public class Host
    {

        public static IServerBuilder Build(Environment environment, GatewayConfiguration config)
        {
            var server = Server.Create()
                               .Bind(IPAddress.Any, 80)
                               .Bind(IPAddress.IPv6Any, 80)
                               .Console();

            var certificateProvider = CertificateLoader.GetProvider(environment, config);

            if (certificateProvider != null)
            {
                server.Bind(IPAddress.Any, 443, certificateProvider)
                      .Bind(IPAddress.IPv6Any, 443, certificateProvider);
            }

#if DEBUG
            server.Development();
#endif

            return server;
        }

    }

}
