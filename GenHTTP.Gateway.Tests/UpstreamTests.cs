using GenHTTP.Gateway.Tests.Domain;
using GenHTTP.Modules.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class UpstreamTests
{

    [TestMethod]
    public async Task TestDefault()
    {
            await using var defaultUpstream = await Upstream.CreateAsync("default");

            await using var routeUpstream = await Upstream.CreateAsync("route");

            var config = @$"
hosts:
  localhost:
    default:
        destination: http://localhost:{defaultUpstream.Port}
    routes:
        route:
            destination: http://localhost:{routeUpstream.Port}";

            await using var runner = await TestRunner.RunAsync(config);

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
                return await (Content.From(Resource.FromString(r.Target.Path.ToString())).Build()).HandleAsync(r);
            });

            await using var defaultUpstream = await Upstream.CreateAsync(handler);

            var config = @$"
hosts:
  localhost:
    default:
        destination: http://localhost:{defaultUpstream.Port}";

            await using var runner = await TestRunner.RunAsync(config);

            Directory.CreateDirectory(Path.Combine(runner.Environment.Data, ".well-known"));

            using var defaultResponse = await runner.GetResponse("/.well-known/caldav");

            Assert.AreEqual("/.well-known/caldav", await defaultResponse.GetContent());
        }

}
