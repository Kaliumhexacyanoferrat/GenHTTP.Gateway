using Xunit;

using GenHTTP.Gateway.Tests.Domain;
using GenHTTP.Modules.IO;
using System.IO;

namespace GenHTTP.Gateway.Tests
{

    public class UpstreamTests
    {

        [Fact]
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

            Assert.Equal("default", defaultResponse.GetContent());

            using var routeResponse = runner.GetResponse("/route/");

            Assert.Equal("route", routeResponse.GetContent());
        }

        [Fact]
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

            Assert.Equal("/.well-known/caldav", defaultResponse.GetContent());
        }

    }

}
