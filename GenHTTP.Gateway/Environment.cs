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

        #endregion

        #region Initialization

        public static Environment Docker()
        {
            return new Environment("/config/", "/data/", "/certs/");
        }

        public static Environment Local()
        {
            return new Environment("./config/", "./data/", "./certs/");
        }

        public static Environment Default()
        {
#if DEBUG
            return Local();
#else
            return Docker();
#endif
        }

        private Environment(string config, string data, string certs)
        {
            Certificates = certs;
            Data = data;
            Config = config;
        }

#endregion

    }

}
