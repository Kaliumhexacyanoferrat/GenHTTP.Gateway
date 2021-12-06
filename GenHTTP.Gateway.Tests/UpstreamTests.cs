using System.IO;
using System.Threading.Tasks;

using GenHTTP.Gateway.Tests.Domain;
using GenHTTP.Modules.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests
{

    [TestClass]
    public class UpstreamTests
    {

        [TestMethod]
        public async Task TestDefault()
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

            using var defaultResponse = await runner.GetResponse();

            Assert.AreEqual("default", await defaultResponse.GetContent());

            using var routeResponse = await runner.GetResponse("/route/");

            Assert.AreEqual("route", await routeResponse.GetContent());
        }

        [TestMethod]
        public async Task TestWellKnown()
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

            using var defaultResponse = await runner.GetResponse("/.well-known/caldav");

            Assert.AreEqual("/.well-known/caldav", await defaultResponse.GetContent());
        }

    }

}
