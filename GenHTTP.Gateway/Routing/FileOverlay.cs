using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using GenHTTP.Api.Content;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.IO;

namespace GenHTTP.Gateway.Routing
{

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

        public IHandler Parent { get; }

        public IHandler Overlay { get; }

        public string DataDirectory { get; }

        #endregion

        #region Initialization

        public FileOverlay(IHandler parent, Func<IHandler, IHandler> contentFactory, Environment environment)
        {
            Parent = parent;

            DataDirectory = new DirectoryInfo(environment.Data).FullName;

            Overlay = Resources.From(ResourceTree.FromDirectory(environment.Data))
                               .Build(this);

            Content = contentFactory(this);
        }

        #endregion

        #region Functionality

        public IEnumerable<ContentElement> GetContent(IRequest request) => Content.GetContent(request);

        public async ValueTask<IResponse?> HandleAsync(IRequest request) => await Overlay.HandleAsync(request) ?? await Content.HandleAsync(request);

        public async ValueTask PrepareAsync()
        {
            await Content.PrepareAsync();
            await Overlay.PrepareAsync();
        }

        #endregion

    }

}
