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
            var config = ConfigurationFile.Load("gateway.yaml");

            var router = Router.Build(config);

            var host = Host.Build(config)
                           .Router(router);

            using (var server = host.Build())
            {
                Console.WriteLine("Running ...");
                Console.ReadLine();
            }
        }

    }

}
