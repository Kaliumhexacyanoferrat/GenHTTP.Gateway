using System.Net;
using System.Net.Cache;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Gateway.Configuration;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GenHTTP.Gateway.Tests.Domain;

public class TestRunner : IAsyncDisposable
{
    private static readonly object SyncRoot = new();

    private static readonly HttpClientHandler Handler = new()
    {
        AllowAutoRedirect = false
    };

    private static readonly HttpClient HttpClient = new(Handler)
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    private static ushort _nextPort = 20000;

    #region Get-/Setters

    public ushort Port { get; }

    public IServerHost Host { get; protected set; }

    public TestEnvironment Environment { get; protected set; }

    #endregion

    #region Initialization

    static TestRunner()
    {
        HttpWebRequest.DefaultCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
    }

    protected TestRunner(IServerHost host, ushort port, TestEnvironment testEnvironment)
    {
        Port = port;
        Host = host;
        Environment = testEnvironment;
    }

    public static ushort NextPort()
    {
        lock (SyncRoot)
        {
            return _nextPort++;
        }
    }

    public static async Task<TestRunner> RunAsync(string configuration, TestEnvironment? env = null)
    {
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                    .Build();

        return await RunAsync(deserializer.Deserialize<GatewayConfiguration>(configuration), env);
    }

    public static async Task<TestRunner> RunAsync(GatewayConfiguration configuration, TestEnvironment? env = null)
    {
        var port = NextPort();

        var environment = env ?? TestEnvironment.Create();

        var handler = Gateway.Handler.Build(environment, configuration);

        var host = Engine.Setup(environment, configuration, port)
                         .Handler(handler);

        await host.StartAsync();

        return new TestRunner(host, port, environment);
    }

    #endregion

    #region Functionality

    public HttpRequestMessage GetRequest(string? uri = null) => new HttpRequestMessage(HttpMethod.Get, $"http://localhost:{Port}{uri ?? ""}");

    public Task<HttpResponseMessage> GetResponse(HttpRequestMessage request) => HttpClient.SendAsync(request);

    public Task<HttpResponseMessage> GetResponse(string? uri = null) => HttpClient.SendAsync(GetRequest(uri));

    #endregion

    #region IDisposable Support

    private bool _disposed = false;

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                await Host.StopAsync();
                Environment.Cleanup();
            }

            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
