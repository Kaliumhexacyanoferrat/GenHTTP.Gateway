﻿using GenHTTP.Api.Content;
using GenHTTP.Api.Infrastructure;

using GenHTTP.Modules.Basics;
using GenHTTP.Modules.VirtualHosting;
using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.Security.Providers;
using GenHTTP.Modules.DirectoryBrowsing;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.ReverseProxy;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Routing;

namespace GenHTTP.Gateway;

public static class Handler
{

    public static IHandlerBuilder Build(Environment environment, GatewayConfiguration config)
    {
        var hosts = VirtualHosts.Create();

        if (config.Hosts != null)
        {
            foreach (var host in config.Hosts)
            {
                if (host.Key == "any")
                {
                    hosts.Default(GetHandler(environment, host.Value));
                }
                else
                {
                    hosts.Add(host.Key, GetHandler(environment, host.Value));
                }
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
            layout.Add(GetHandler(config.Default));
        }

        if (config?.Routes != null)
        {
            foreach (var route in config.Routes)
            {
                layout.Add(route.Key, GetHandler(route.Value));
            }
        }

        if (config?.Security?.Certificate != null)
        {
            layout.Add(new SecureUpgradeConcernBuilder().Mode(SecureUpgrade.Force));
        }

        return layout;
    }

    private static IHandlerBuilder GetHandler(RouteConfiguration config)
    {
        var layout = Layout.Create();

        var content = GetContent(config);

        if (content != null)
        {
            layout.Add(content);
        }
        else if (config.Listing != null)
        {
            layout.Add(Listing.From(ResourceTree.FromDirectory(config.Listing)));
        }
        else if (config.Content != null)
        {
            if (config.Content.Directory != null)
            {
                var directory = Resources.From(ResourceTree.FromDirectory(config.Content.Directory));

                var staticContent = Layout.Create().Add(directory);

                if (config.Content.Index != null)
                {
                    var indexFile = Path.Combine(config.Content.Directory, config.Content.Index);
                    layout.Index(Download.From(Resource.FromFile(indexFile)));
                }

                layout.Add(staticContent);
            }
        }
        else if (config.Location != null)
        {
            layout.Add(Redirect.To(config.Location));
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
            return Proxy.Create()
                        .Upstream(config.Destination)
                        .ConnectTimeout(TimeSpan.FromMinutes(3))
                        .ReadTimeout(TimeSpan.FromMinutes(3));
        }

        return null;
    }
}
