using GenHTTP.Api.Content;
using GenHTTP.Api.Protocol;

using GenHTTP.Modules.IO;

namespace GenHTTP.Gateway.Routing;

/// <summary>
/// Blends the data directory over the actual handler used for
/// a virtual host.
/// </summary>
/// <remarks>
/// This way, content from the data directory can be served
/// without exluding files from being handled by virtual hosts.
/// </remarks>
public class FileOverlay : IConcern
{

        #region Get-/Setters

    public IHandler Content { get; }

    public IHandler Overlay { get; }

    public string DataDirectory { get; }

        #endregion

        #region Initialization

    public FileOverlay(IHandler content, Environment environment)
    {
        DataDirectory = new DirectoryInfo(environment.Data).FullName;

        Overlay = Resources.From(ResourceTree.FromDirectory(environment.Data))
                           .Build();

        Content = content;
    }

        #endregion

        #region Functionality

    public async ValueTask<IResponse?> HandleAsync(IRequest request)
    {
        var targetFile = Path.Combine(DataDirectory, "." + request.Target.Path.ToString(false));

        if (File.Exists(targetFile))
        {
            return await Overlay.HandleAsync(request);
        }

        return await Content.HandleAsync(request);
    }

    public async ValueTask PrepareAsync()
    {
        await Content.PrepareAsync();
        await Overlay.PrepareAsync();
    }

        #endregion

}
