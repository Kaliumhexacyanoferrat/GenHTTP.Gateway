using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using GenHTTP.Api.Modules;
using GenHTTP.Api.Protocol;
using GenHTTP.Api.Routing;
using GenHTTP.Modules.Core;
using GenHTTP.Modules.Core.General;

namespace GenHTTP.Gateway.Routing
{

    /// <summary>
    /// Blends the data directory over the actual router used for
    /// a virtual host.
    /// </summary>
    /// <remarks>
    /// This way, content from the data directory can be served
    /// without exluding files from being handled by virtual hosts.
    /// </remarks>
    public class FileOverlay : RouterBase
    {

        #region Get-/Setters

        public IRouter Inner { get; }

        public IRouter Overlay { get; }
        
        public string DataDirectory { get; }

        #endregion

        #region Initialization

        public FileOverlay(Environment environment, IRouter inner) : base(null, null)
        {
            Inner = inner;
            inner.Parent = this;

            DataDirectory = new DirectoryInfo(environment.Data).FullName;
            Overlay = Static.Files(environment.Data).Build();
        }

        #endregion

        #region Functionality

        public override void HandleContext(IEditableRoutingContext current)
        {
            var target = Path.Combine(DataDirectory, current.Request.Path.Substring(1));

            if (File.Exists(target) && target.StartsWith(DataDirectory))
            {
                Overlay.HandleContext(current);
            }
            else
            {
                Inner.HandleContext(current);
            }
        }

        public override IEnumerable<ContentElement> GetContent(IRequest request, string basePath) => Inner.GetContent(request, basePath);

        public override string? Route(string path, int currentDepth)
        {
            return Parent.Route(path, currentDepth);
        }

        #endregion

    }

}
