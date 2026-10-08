using GenHTTP.Api.Infrastructure;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Tests.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenHTTP.Gateway.Tests;

[TestClass]
public class EngineTests
{

    [TestMethod]
    [DataRow("internal")]
    [DataRow("kestrel")]
    [DataRow("Kestrel")]
    public async Task TestEngineServesContent(string engine)
    {
        var config = @$"
engine: {engine}
hosts:
  localhost:";

        await using var runner = await TestRunner.RunAsync(config);

        File.WriteAllText(Path.Combine(runner.Environment.Data, "file.txt"), "Hello World!");

        using var response = await runner.GetResponse("/file.txt");

        Assert.AreEqual("Hello World!", await response.GetContent());
    }

    [TestMethod]
    public void TestDefaultEngineIsKestrel()
    {
        Assert.AreEqual("GenHTTP.Engine.Kestrel", Engine.CreateHost(null).GetType().Assembly.GetName().Name);
    }

    [TestMethod]
    public void TestIoxideCanBeCreated()
    {
        Assert.AreEqual("GenHTTP.Engine.Ioxide", Engine.CreateHost("ioxide").GetType().Assembly.GetName().Name);
    }

    [TestMethod]
    public void TestUnknownEngine()
    {
        Assert.ThrowsExactly<NotSupportedException>(() => Engine.CreateHost("nginx"));
    }

    [TestMethod]
    public void TestDefaultProtocols()
    {
        Assert.AreEqual(HttpProtocols.Http1AndHttp2, Engine.GetProtocols(new GatewayConfiguration()));
    }

    [TestMethod]
    public void TestConfiguredProtocols()
    {
        var config = new GatewayConfiguration { Protocols = ["http1", "H3"] };

        Assert.AreEqual(HttpProtocols.Http1AndHttp3, Engine.GetProtocols(config));
    }

    [TestMethod]
    public void TestUnknownProtocol()
    {
        Assert.ThrowsExactly<NotSupportedException>(() => Engine.GetProtocols(new GatewayConfiguration { Protocols = ["spdy"] }));
    }

    [TestMethod]
    public void TestNoProtocols()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => Engine.GetProtocols(new GatewayConfiguration { Protocols = [] }));
    }

}
