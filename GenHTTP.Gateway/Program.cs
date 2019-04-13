using System;
using System.Collections.Generic;
using System.IO;

using GenHTTP.Gateway.Configuration;

namespace GenHTTP.Gateway
{

    public static class Program
    {

        public static void Main(string[] args)
        {
            var env = Environment.Default();

            var config = ConfigurationFile.Load(Path.Combine(env.Config, "gateway.yaml"));

            var router = Router.Build(env, config);

            var host = Host.Build(env, config)
                           .Router(router);

            using (var server = host.Build())
            {
                Console.WriteLine("Running ...");
                Console.ReadLine();
            }
        }

    }

}
