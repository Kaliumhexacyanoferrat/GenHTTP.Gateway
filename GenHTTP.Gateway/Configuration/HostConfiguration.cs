using System;
using System.Collections.Generic;
using System.Text;

namespace GenHTTP.Gateway.Configuration
{

    public class HostConfiguration
    {

        public Dictionary<string, RouteConfiguration>? Routes { get; set; }

        public SecurityConfiguration? Security { get; set; }

        public RouteConfiguration? Default { get; set; }

    }

}
