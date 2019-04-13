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

#if DEBUG
                    Console.ReadLine();
#else
                    await Task.Run(() => Thread.Sleep(Timeout.Infinite));
#endif
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
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GenHTTP.Gateway.Resources.Default.yaml"))
                {
                    using (var config = File.OpenWrite(configFile))
                    {
                        stream.CopyTo(config);
                    }
                }
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
