using GenHTTP.Api.Content;
using GenHTTP.Api.Infrastructure;

using GenHTTP.Modules.IO;
using GenHTTP.Modules.Layouting;

namespace GenHTTP.Gateway.Tests.Domain;

public class Upstream : IAsyncDisposable
{

    #region Get-/Setters

    private IServerHost Host { get; }

    public ushort Port { get; }

    #endregion

    #region Initialization

    protected Upstream(ushort port, IHandlerBuilder handler)
    {
        Host = GenHTTP.Engine.Kestrel.Host.Create()
                      .Port(port)
                      .Handler(handler);

        Port = port;
    }

    public static async ValueTask<Upstream> CreateAsync(string content)
    {
        var router = Layout.Create()
                           .Add(Content.From(Resource.FromString(content)));

        var upstream = await CreateAsync(router);

        await upstream.Host.StartAsync();

        return upstream;
    }

    public static async ValueTask<Upstream> CreateAsync(IHandlerBuilder handler)
    {
        var upstream = new Upstream(TestRunner.NextPort(), handler);

        await upstream.Host.StartAsync();

        return upstream;
    }

    #endregion

    #region IDisposable Support

    private bool disposed = false;

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                await Host.StopAsync();
            }

            disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    #endregion

}
