using System;
using System.Collections.Generic;
using System.Text;

namespace GenHTTP.Gateway.Configuration
{

    public class RouteConfiguration
    {

        public Dictionary<string, RouteConfiguration>? Routes { get; set; }

        public string? Destination { get; set; }

    }

}
