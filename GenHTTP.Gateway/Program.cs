using System.IO;

using GenHTTP.Gateway.Configuration;
using GenHTTP.Gateway.Security;

namespace GenHTTP.Gateway
{

    public static class Program
    {

        public static int Main(string[] args)
        {
            var env = Environment.Default();

            var config = SetupConfig(env);

            return new Engine(env, config).Run();
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

}
