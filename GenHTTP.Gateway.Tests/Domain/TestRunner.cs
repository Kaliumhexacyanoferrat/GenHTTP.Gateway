using System;
using System.Net;
using System.Net.Cache;

using GenHTTP.Api.Infrastructure;
using GenHTTP.Gateway.Configuration;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GenHTTP.Gateway.Tests.Domain
{

    public class TestRunner : IDisposable
    {
        private static object _SyncRoot = new object();
        private static ushort _NextPort = 20000;

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
            lock (_SyncRoot)
            {
                return _NextPort++;
            }
        }

        public static TestRunner Run(string configuration)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                        .Build();

            return Run(deserializer.Deserialize<GatewayConfiguration>(configuration));
        }

        public static TestRunner Run(GatewayConfiguration configuration)
        {
            var port = NextPort();

            var environment = TestEnvironment.Create();

            var router = Router.Build(environment, configuration);

            var host = Engine.Setup(environment, configuration, port)
                             .Router(router);

            host.Start();

            return new TestRunner(host, port, environment);
        }

        #endregion

        #region Functionality

        public HttpWebRequest GetRequest(string? uri = null)
        {
            var request = WebRequest.CreateHttp($"http://localhost:{Port}{uri ?? ""}");
            request.Timeout = 3000;
            request.AllowAutoRedirect = false;

            return request;
        }

        public HttpWebResponse GetResponse(string? uri = null)
        {
            return GetRequest(uri).GetSafeResponse();
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
                    Environment.Cleanup();
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
