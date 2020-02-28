using System;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Api.Routing;
using GenHTTP.Modules.Core;

namespace GenHTTP.Gateway.Tests.Domain
{

    public class Upstream : IDisposable
    {

        #region Get-/Setters

        private IServerHost Host { get; }

        public ushort Port { get; }

        #endregion

        #region Initialization

        protected Upstream(ushort port, IRouterBuilder router)
        {
            Host = Core.Host.Create()
                            .Port(port)
                            .Router(router)
                            .Start();

            Port = port;
        }

        public static Upstream Create(string content)
        {
            var router = Layout.Create()
                               .Default(Content.From(content));

            return Create(router);
        }

        public static Upstream Create(IRouterBuilder router)
        {
            return new Upstream(TestRunner.NextPort(), router);
        }

        #endregion

        #region IDisposable Support

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Host.Stop();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}
