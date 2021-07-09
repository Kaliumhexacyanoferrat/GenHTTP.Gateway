using System.IO;

using GenHTTP.Gateway.Tests.Domain;
using GenHTTP.Modules.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests
{

    [TestClass]
    public class UpstreamTests
    {

        [TestMethod]
        public void TestDefault()
        {
            using var defaultUpstream = Upstream.Create("default");

            using var routeUpstream = Upstream.Create("route");

            var config = @$"
hosts:
  localhost:
    default:
        destination: http://localhost:{defaultUpstream.Port}
    routes:
        route:
            destination: http://localhost:{routeUpstream.Port}";

            using var runner = TestRunner.Run(config);

            using var defaultResponse = runner.GetResponse();

            Assert.AreEqual("default", defaultResponse.GetContent());

            using var routeResponse = runner.GetResponse("/route/");

            Assert.AreEqual("route", routeResponse.GetContent());
        }

        [TestMethod]
        public void TestWellKnown()
        {
            var handler = InlineHandlerBuilder.Create(async (h, r) =>
            {
                return await (Content.From(Resource.FromString(r.Target.Path.ToString())).Build(h)).HandleAsync(r);
            });

            using var defaultUpstream = Upstream.Create(handler);

            var config = @$"
hosts:
  localhost:
    default:
        destination: http://localhost:{defaultUpstream.Port}";

            using var runner = TestRunner.Run(config);

            Directory.CreateDirectory(Path.Combine(runner.Environment.Data, ".well-known"));

            using var defaultResponse = runner.GetResponse("/.well-known/caldav");

            Assert.AreEqual("/.well-known/caldav", defaultResponse.GetContent());
        }

    }

}
