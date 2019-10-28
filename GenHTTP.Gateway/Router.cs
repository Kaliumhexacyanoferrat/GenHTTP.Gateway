using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GenHTTP.Api.Modules;
using GenHTTP.Api.Routing;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Routing;

using GenHTTP.Modules.Core;

namespace GenHTTP.Gateway
{

    public static class Router
    {

        public static IRouterBuilder Build(Environment environment, GatewayConfiguration config)
        {
            var hosts = VirtualHosts.Create();

            if (config.Hosts != null)
            {
                foreach (var host in config.Hosts)
                {
                    hosts.Add(host.Key, GetRouter(environment, host.Value));
                }
            }

            return hosts;
        }

        private static IRouter GetRouter(Environment environment, HostConfiguration config)
        {
            var layout = Layout.Create();
            
            if (config.Default != null)
            {
                layout.Default(GetRouter(config.Default));
            }

            if (config.Routes != null)
            {
                foreach (var route in config.Routes)
                {
                    layout.Add(route.Key, GetRouter(route.Value));
                }
            }

            return new FileOverlay(environment, layout.Build());
        }

        private static IRouterBuilder GetRouter(RouteConfiguration config)
        {
            var layout = Layout.Create();

            var content = GetContent(config);

            if (content != null)
            {
                layout.Default(content);
            }
            else if (config.Listing != null)
            {
                layout.Default(DirectoryListing.From(config.Listing));
            }

            if (config.Routes != null)
            {
                foreach (var route in config.Routes)
                {
                    layout.Add(route.Key, GetRouter(route.Value));
                }
            }

            return layout;
        }

        private static IContentBuilder? GetContent(RouteConfiguration config)
        {
            if (config.Destination != null)
            {
                return ReverseProxy.Create()
                                   .Upstream(config.Destination)
                                   .ConnectTimeout(TimeSpan.FromMinutes(3))
                                   .ReadTimeout(TimeSpan.FromMinutes(3));
            }

            return null;
        }

    }

}
