using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Tests.Domain;
using System.Collections.Generic;

using Xunit;

namespace GenHTTP.Gateway.Tests
{

    public class ReverseProxyTests
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

    }

}
