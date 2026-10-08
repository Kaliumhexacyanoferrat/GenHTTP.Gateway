using GenHTTP.Api.Infrastructure;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;

using GenHTTP.Modules.Practices;

namespace GenHTTP.Gateway;

public static class Engine
{
    
    public const ushort DefaultPort = 80;

    public const ushort DefaultSecurePort = 443;

    public static IServerHost Setup(Environment environment, GatewayConfiguration config)
    {
        var server = CreateHost(config.Engine)
                         .Defaults(secureUpgrade: false)
                         .Bind(null, config.Ports?.Plain ?? DefaultPort);

        var certificateProvider = CertificateLoader.GetProvider(environment, config);

        if (certificateProvider != null)
        {
            server.Bind(null, config.Ports?.Secure ?? DefaultSecurePort, certificateProvider, httpProtocols: GetProtocols(config));
        }

#if DEBUG
        server.Development();
#endif

        return server;
    }

    public static IServerHost CreateHost(string? engine)
    {
        switch (engine?.Trim().ToLowerInvariant())
        {
            case null:
            case "":
            case "kestrel": return GenHTTP.Engine.Kestrel.Host.Create();
            case "internal": return GenHTTP.Engine.Internal.Host.Create();
            case "ioxide": return GenHTTP.Engine.Ioxide.Host.Create();
        }

        throw new NotSupportedException($"Engine '{engine}' is not supported (expected one of: internal, kestrel, ioxide)");
    }

    public static HttpProtocols GetProtocols(GatewayConfiguration config)
    {
        if (config.Protocols == null)
        {
            return HttpProtocols.Http1AndHttp2;
        }

        var protocols = HttpProtocols.None;

        foreach (var protocol in config.Protocols)
        {
            protocols |= protocol.Trim().ToLowerInvariant() switch
            {
                "http1" or "h1" => HttpProtocols.Http1,
                "http2" or "h2" => HttpProtocols.Http2,
                "http3" or "h3" => HttpProtocols.Http3,
                _ => throw new NotSupportedException($"Protocol '{protocol}' is not supported (expected one of: http1, http2, http3)")
            };
        }

        if (protocols == HttpProtocols.None)
        {
            throw new InvalidOperationException("At least one HTTP protocol needs to be enabled");
        }

        return protocols;
    }

}
