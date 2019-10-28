using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GenHTTP.Gateway.Configuration
{

    public static class ConfigurationFile
    {

        public static GatewayConfiguration Load(string file)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                        .Build();

            return deserializer.Deserialize<GatewayConfiguration>(File.ReadAllText(file));
        }

    }

}
