using GenHTTP.Gateway.Configuration;

namespace GenHTTP.Gateway;

public static class Program
{

    public static async Task<int> Main(string[] _)
    {
        var env = Environment.Default();

        var config = SetupConfig(env);

        var handler = Handler.Build(env, config);

        return await Engine.Setup(env, config)
                           .Handler(handler)
                           .RunAsync();
    }

    public static GatewayConfiguration SetupConfig(Environment env)
    {
        var configFile = Path.Combine(env.Config, "gateway.yaml");

        if (!File.Exists(configFile))
        {
            if (!Directory.Exists("./config"))
            {
                Directory.CreateDirectory("./config");
            }

            File.Copy("./Resources/Default.yaml", configFile);
        }

        var wellKnown = Path.Combine(env.Data, ".well-known");

        if (!Directory.Exists(wellKnown))
        {
            Directory.CreateDirectory(wellKnown);
        }

        return ConfigurationFile.Load(configFile);
    }

}
