using System;
using System.IO;

using GenHTTP.Api.Content;
using GenHTTP.Modules.Core;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Routing;

namespace GenHTTP.Gateway
{

    public static class Handler
    {

        public static IHandlerBuilder Build(Environment environment, GatewayConfiguration config)
        {
            var hosts = VirtualHosts.Create();

            if (config.Hosts != null)
            {
                foreach (var host in config.Hosts)
                {
                    hosts.Add(host.Key, GetHandler(environment, host.Value));
                }
            }

            return hosts;
        }

        private static IHandlerBuilder GetHandler(Environment environment, HostConfiguration? config)
        {
            var layout = Layout.Create()
                               .Add(new FileOverlayBuilder(environment));

            if (config?.Default != null)
            {
                layout.Fallback(GetHandler(config.Default));
            }

            if (config?.Routes != null)
            {
                foreach (var route in config.Routes)
                {
                    layout.Add(route.Key, GetHandler(route.Value));
                }
            }

            return layout;
        }

        private static IHandlerBuilder GetHandler(RouteConfiguration config)
        {
            var layout = Layout.Create();

            var content = GetContent(config);

            if (content != null)
            {
                layout.Fallback(content);
            }
            else if (config.Listing != null)
            {
                layout.Fallback(DirectoryListing.From(config.Listing));
            }
            else if (config.Content != null)
            {
                if (config.Content.Directory != null)
                {
                    var directory = Static.Files(config.Content.Directory);
                    
                    var staticContent = Layout.Create().Fallback(directory);

                    if (config.Content.Index != null)
                    {
                        var indexFile = Path.Combine(config.Content.Directory, config.Content.Index);
                        layout.Index(Download.FromFile(indexFile));
                    }

                    layout.Fallback(staticContent);
                }
            }

            if (config.Routes != null)
            {
                foreach (var route in config.Routes)
                {
                    layout.Add(route.Key, GetHandler(route.Value));
                }
            }

            return layout;
        }

        private static IHandlerBuilder? GetContent(RouteConfiguration config)
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