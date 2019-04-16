using System;
using System.Collections.Generic;
using System.Text;

namespace GenHTTP.Gateway
{

    public class Environment
    {

        #region Get-/Setters

        public string Certificates { get; }

        public string Data { get; }

        public string Config { get; }

        public bool Containerized { get; }

        #endregion

        #region Initialization

        public static Environment DockerLinux()
        {
            return new Environment("/app/config/", "/app/data/", "/app/certs/", true);
        }

        public static Environment DockerWindows()
        {
            return new Environment(@"C:\App\Config\", @"C:\App\Data\", @"C:\App\Certs\", true);
        }

        public static Environment Local()
        {
            return new Environment("./config/", "./data/", "./certs/", false);
        }

        public static Environment Default()
        {
            var flavor = System.Environment.GetEnvironmentVariable("DOCKER_FLAVOR");
            
            if (string.IsNullOrEmpty(flavor))
            {
                return Local();
            }

            switch (flavor)
            {
                case "windows": return DockerWindows();
                case "linux": return DockerLinux();
            }

            throw new NotSupportedException($"Flavor '{flavor}' is not supported");
        }

        private Environment(string config, string data, string certs, bool containerized)
        {
            Containerized = containerized;

            Certificates = certs;
            Data = data;
            Config = config;
        }

#endregion

    }

}
