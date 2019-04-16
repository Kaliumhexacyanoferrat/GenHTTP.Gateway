using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using GenHTTP.Gateway.Configuration;

namespace GenHTTP.Gateway
{

    public static class Program
    {

        public static async Task<int> Main(string[] args)
        {
            try
            {
                var env = Environment.Default();

                var config = SetupConfig(env);

                var router = Router.Build(env, config);

                var host = Host.Build(env, config)
                               .Router(router);

                using (var server = host.Build())
                {
                    Console.WriteLine("Running ...");

                    if (env.Containerized)
                    {
                        await Task.Run(() => Thread.Sleep(Timeout.Infinite));
                    }
                    else
                    {
                        Console.ReadLine();
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        private static GatewayConfiguration SetupConfig(Environment env)
        {
            var configFile = Path.Combine(env.Config, "gateway.yaml");

            if (!File.Exists(configFile))
            {
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
