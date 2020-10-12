using System;

using GenHTTP.Api.Content;
using GenHTTP.Api.Infrastructure;

using GenHTTP.Modules.IO;
using GenHTTP.Modules.Layouting;

namespace GenHTTP.Gateway.Tests.Domain
{

    public class Upstream : IDisposable
    {

        #region Get-/Setters

        private IServerHost Host { get; }

        public ushort Port { get; }

        #endregion

        #region Initialization

        protected Upstream(ushort port, IHandlerBuilder handler)
        {
            Host = GenHTTP.Engine.Host.Create()
                                      .Port(port)
                                      .Handler(handler)
                                      .Start();

            Port = port;
        }

        public static Upstream Create(string content)
        {
            var router = Layout.Create()
                               .Fallback(Content.From(content));

            return Create(router);
        }

        public static Upstream Create(IHandlerBuilder handler)
        {
            return new Upstream(TestRunner.NextPort(), handler);
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
